using System;
using System.Data;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls.Configuration;

using CMS.Ecommerce;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Tabs("CMS.Ecommerce", "Customers", "CustomerContent")]
[Title("Objects/Ecommerce_Customer/object.png", "com.customer.edit", "general_tab14")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Header : CMSCustomersPage
{
    private bool hideBreadcrumbs = false;
    private CustomerInfo customerInfoObj = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        string currentCustomer = "";
        int customerId = QueryHelper.GetInteger("customerid", 0);
        if (customerId > 0)
        {
            customerInfoObj = CustomerInfoProvider.GetCustomerInfo(customerId);

            // Prepare customer name string
            if (customerInfoObj != null)
            {
                if (ValidationHelper.GetString(customerInfoObj.CustomerCompany, "") != "")
                {
                    currentCustomer = customerInfoObj.CustomerCompany;
                }
                else
                {
                    currentCustomer = customerInfoObj.CustomerLastName + " " + customerInfoObj.CustomerFirstName;
                }
            }
        }

        // Initialize breadcrumbs when visible
        if (!hideBreadcrumbs)
        {
            InitializeBreadcrumbs(currentCustomer);
        }

        // Ensure page with changes saved message is loaded initially if required
        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            CurrentMaster.Tabs.StartPageURL = "Customer_Edit_General.aspx" + URLHelper.Url.Query;
        }

        InitializeMasterPage();

        AddMenuButtonSelectScript("Customers", "");
    }


    /// <summary>
    /// Initializes master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        CMSMasterPage master = (CMSMasterPage)CurrentMaster;

        master.Tabs.OnTabCreated += Tabs_OnTabCreated;
        master.Tabs.OpenTabContentAfterLoad = !QueryHelper.GetBoolean("onlyRefresh", false);
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        switch (element.ElementName.ToLowerCSafe())
        {
            case "customers.customfields":

                // Check if customer has any custom fields
                FormInfo formInfo = FormHelper.GetFormInfo("ecommerce.customer", false);
                if (!formInfo.GetFormElements(true, false, true).Any())
                {
                    return null;
                }
                break;

            case "customers.newsletters":
                if (!ModuleEntry.IsModuleLoaded(ModuleEntry.NEWSLETTER))
                {
                    return null;
                }
                break;

            case "customers.credit":
                // Hide Credit tab for anonymous customer
                if ((customerInfoObj == null) || !customerInfoObj.CustomerIsRegistered)
                {
                    return null;
                }
                break;
        }

        return tab;
    }


    /// <summary>
    /// Initializes the breadcrumb mastre page element.
    /// </summary>
    private void InitializeBreadcrumbs(string currentCustomer)
    {
        // initializes page title control		
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("Customer_Edit.ItemListLink");
        pageTitleTabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Customers/Customer_List.aspx";
        pageTitleTabs[0, 2] = "ecommerceContent";
        pageTitleTabs[1, 0] = currentCustomer;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }
}