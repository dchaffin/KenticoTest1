using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Desk", UIElements = "Ecommerce")]
[Security(Resource = "CMS.Ecommerce", UIElements = "OrdersGroup;Customers;Customers.Newsletters")]
public partial class CMSModules_Newsletters_Tools_Customers_Customer_Edit_Newsletters : CMSDeskPage
{
    private int siteId = 0;
    private int customerSiteId = -1;
    private string currentValues = string.Empty;
    private string email = null;
    private string firstName = null;
    private string lastName = null;
    private int customerUserId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check the license
        if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.Newsletters);
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Newsletter", CMSContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Newsletter");
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Ecommerce", CMSContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Ecommerce");
        }

        siteSelector.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        if (!URLHelper.IsPostback())
        {
            siteSelector.SiteID = CMSContext.CurrentSiteID;
        }

        CurrentUserInfo user = CMSContext.CurrentUser;
    
        // Check e-commerce permision
        if (!user.IsAuthorizedPerResource("CMS.Ecommerce", "ReadCustomers") && !user.IsAuthorizedPerResource("CMS.Ecommerce", "EcommerceRead"))
        {
            RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "ReadCustomers OR EcommerceRead");
        }
        // Check 'NewsletterRead' permission
        if (!user.IsAuthorizedPerResource("CMS.Newsletter", "Read"))
        {
            RedirectToCMSDeskAccessDenied("CMS.Newsletter", "Read");
        }
        lblTitle.Text = GetString("Customer_Edit_Newsletters.Title");

        // Load customer data
        GeneralizedInfo customerObj = ModuleCommands.ECommerceGetCustomerInfo(QueryHelper.GetInteger("customerId", 0));
        if (customerObj != null)
        {
            email = Convert.ToString(customerObj.GetValue("CustomerEmail"));
            firstName = Convert.ToString(customerObj.GetValue("CustomerFirstName"));
            lastName = Convert.ToString(customerObj.GetValue("CustomerLastName"));
            customerUserId = ValidationHelper.GetInteger(customerObj.GetValue("CustomerUserID"), -1);

            object customerSiteIdObj = customerObj.GetValue("CustomerSiteID");
            customerSiteId = ValidationHelper.GetInteger((customerSiteIdObj == null) ? 0 : customerSiteIdObj, CMSContext.CurrentSiteID);
        }

        if ((email == null) || (email.Trim() == string.Empty) || (!ValidationHelper.IsEmail(email)))
        {
            lblTitle.Visible = false;
            ShowError(GetString("ecommerce.customer.invalidemail"));
            usNewsletters.Visible = false;
        }

        usNewsletters.ButtonRemoveSelected.CssClass = "XLongButton";
        usNewsletters.ButtonAddItems.CssClass = "XLongButton";
        usNewsletters.OnSelectionChanged += new EventHandler(usNewsletters_OnSelectionChanged);

        SetWhereCondition();

        LoadSelection(false);
    }


    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SetWhereCondition();

        LoadSelection(true);
        usNewsletters.Reload(true);
    }


    private void usNewsletters_OnSelectionChanged(object sender, EventArgs e)
    {
        // Check 'EcommerceModify' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Ecommerce", "EcommerceModify"))
        {
            // Check 'ModifyCustomers' permission if don't have general ecommerce modify permission
            if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Ecommerce", "ModifyCustomers"))
            {
                RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
            }
        }

        // Check if a customer has a valid email 
        if ((email != null) && (email.Trim() != string.Empty) && (ValidationHelper.IsEmail(email)))
        {
            // Check whether subscriber already exist
            SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(email, siteId);
            if (sb == null)
            {
                // Create new subscriber
                sb = new SubscriberInfo();
                sb.SubscriberEmail = email;
                sb.SubscriberFirstName = firstName;
                sb.SubscriberLastName = lastName;
                sb.SubscriberFullName = (firstName + " " + lastName).Trim();
                sb.SubscriberSiteID = siteId;
                sb.SubscriberGUID = Guid.NewGuid();
                SubscriberInfoProvider.SetSubscriberInfo(sb);
            }

            // Remove old items
            string newValues = ValidationHelper.GetString(usNewsletters.Value, null);
            string items = DataHelper.GetNewItemsInList(newValues, currentValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (newItems != null)
                {
                    foreach (string item in newItems)
                    {
                        int newsletterId = ValidationHelper.GetInteger(item, 0);

                        // If subscriber is subscribed, unsubscribe him
                        if (SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId))
                        {
                            SubscriberInfoProvider.Unsubscribe(sb.SubscriberID, newsletterId);
                        }
                    }
                }
            }

            // Add new items
            items = DataHelper.GetNewItemsInList(currentValues, newValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (newItems != null)
                {
                    foreach (string item in newItems)
                    {
                        int newsletterId = ValidationHelper.GetInteger(item, 0);

                        // If subscriber is not subscribed, subscribe him
                        if (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId))
                        {
                            SubscriberInfoProvider.Subscribe(sb.SubscriberID, newsletterId, DateTime.Now);
                        }
                    }
                }
            }

            // Display information about successful (un)subscription
            ShowChangesSaved();
        }
    }


    private void LoadSelection(bool force)
    {
        currentValues = string.Empty;

        SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(email, siteId);
        if (sb != null)
        {
            // Get selected newsletters
            DataSet ds = SubscriberNewsletterInfoProvider.GetSubscriberNewsletters(sb.SubscriberID, null, -1, "NewsletterID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(ds.Tables[0], "NewsletterID"));
            }

            if (!RequestHelper.IsPostBack() || force)
            {
                // Load selected newsletters
                usNewsletters.Value = currentValues;
            }
        }
    }


    private void SetWhereCondition()
    {
        // Working with registered customer
        if (customerUserId > 0)
        {
            // Show site selector
            CurrentMaster.DisplaySiteSelectorPanel = true;

            // Show site selector for registered customer
            pnlSiteSelector.Visible = true;
            siteSelector.UserId = customerUserId;
            siteId = siteSelector.SiteID;
            // Show only selected site newsletters for registered customer
            if (siteId > 0)
            {
                usNewsletters.WhereCondition = "NewsletterSiteID = " + siteId;
            }
            // When "all sites" selected
            else
            {
                usNewsletters.WhereCondition = "NewsletterSiteID IN (SELECT SiteID FROM CMS_UserSite WHERE UserID = " + customerUserId + ")";
            }
        }
        else
        {
            siteId = CMSContext.CurrentSiteID;
            usNewsletters.WhereCondition = "NewsletterSiteID = " + customerSiteId;
        }

        usNewsletters.Enabled = siteId > 0;
    }
}