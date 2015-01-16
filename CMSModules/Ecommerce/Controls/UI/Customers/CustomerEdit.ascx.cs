using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_UI_Customers_CustomerEdit : CMSAdminEditControl
{
    #region "Variables"

    private int mCustomerId = 0;
    private CustomerInfo mCustomer = null;
    private bool mAllowEditUser = true;

    private bool allowGlobalDiscountLevels = false;
    private int currentSiteId = -1;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of edited customer.
    /// </summary>
    public int CustomerID
    {
        get
        {
            if (mCustomer != null)
            {
                return mCustomer.CustomerID;
            }

            return mCustomerId;
        }
        set
        {
            mCustomerId = value;

            // Invalidate info object
            mCustomer = null;
        }
    }


    /// <summary>
    /// Edited customer object.
    /// </summary>
    public CustomerInfo Customer
    {
        get
        {
            return mCustomer ?? (mCustomer = CustomerInfoProvider.GetCustomerInfo(CustomerID));
        }
        set
        {
            mCustomer = value;

            // Invalidate ID
            mCustomerId = 0;
        }
    }


    /// <summary>
    /// Indicates whether 'edit user' button will be visible in case of registered customer. True by default.
    /// </summary>
    public bool AllowEditUser
    {
        get
        {
            return mAllowEditUser;
        }
        set
        {
            mAllowEditUser = value;
        }
    }


    /// <summary>
    /// Indicates whether 'OK' button is to be hidden. Default value is false.
    /// </summary>
    public bool HideOKButton
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init current site id
        currentSiteId = CMSContext.CurrentSiteID;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Figure out if site allows using global discount levels
        allowGlobalDiscountLevels = ECommerceSettings.AllowGlobalDiscountLevels(CMSContext.CurrentSiteName);

        pnlGeneral.GroupingText = GetString("com.customeredit.general");
        pnlContacts.GroupingText = GetString("com.customeredit.contacts");
        pnlUserInfo.GroupingText = GetString("com.customeredit.userinfo");

        chkHasLogin.Text = GetString("Customer_Edit_Login_Edit.lblHasLogin");
        rqvUserName.ErrorMessage = GetString("Customer_Edit_Login_Edit.rqvUserName");
        rqvPassword2.ErrorMessage = GetString("Customer_Edit_Login_Edit.rqvPassword2");

        // Init buttons
        btnEditUser.Text = GetString("general.edit");

        if (Customer != null)
        {
            InitForm();

            LoadForm();
        }

        chkHasLogin.Attributes["onclick"] = "ShowHideLoginControls()";

        // Register script to show / hide user controls
        string script = @"
function ShowHideLoginControls() {{ 
   checkbox = document.getElementById('" + chkHasLogin.ClientID + @"');
   line1 = document.getElementById('loginLine1');
   line2 = document.getElementById('loginLine2');
   line3 = document.getElementById('loginLine3');
   if ((checkbox != null) && (checkbox.checked)) {{
       line1.style.display = '';
       line2.style.display = '';
       line3.style.display = '';
   }} else {{
       line1.style.display = 'none';
       line2.style.display = 'none';
       line3.style.display = 'none';
   }}
}}";

        ltlScript.Text = ScriptHelper.GetScript(script);

        if (pnlEdit.Visible)
        {
            ltlScript.Text += ScriptHelper.GetScript("ShowHideLoginControls()");
        }

        // Hide edit user button when requested
        plcButton.Visible = !HideOKButton;
        btnOk.Visible = !HideOKButton;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        Save();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Hides/displays parts of the form according edited customer.
    /// </summary>
    private void InitForm()
    {
        // Display site discount level only for registered customers
        plcSiteDiscount.Visible = Customer.CustomerIsRegistered;
        if (Customer.CustomerIsRegistered)
        {
            UserInfo ui = UserInfoProvider.GetUserInfo(Customer.CustomerUserID);
            if (ui != null)
            {
                pnlEdit.Visible = false;
                pnlStatic.Visible = true;
                lblUserNameStaticValue.Text = HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(ui.UserName));
                if (AllowEditUser)
                {
                    btnEditUser.OnClientClick = "modalDialog('" + CMSContext.ResolveDialogUrl("~/CMSModules/Membership/Pages/Users/User_Edit_Dialog.aspx") + "?userid=" + Customer.CustomerUserID + "', 'UserEdit', 950, 800); return false;";
                    btnEditUser.Visible = true;
                }
            }

            // Hide global discount level selector when global levels not allowed
            plcGlobalDiscount.Visible = allowGlobalDiscountLevels;
        }
        else
        {
            // Hide fields requiring registered customer
            plcDiscounts.Visible = false;
            plcPreferences.Visible = false;
            pnlEdit.Visible = true;
            pnlStatic.Visible = false;
        }

        // Set up discount level selector
        string emptyRecordText = GetString(allowGlobalDiscountLevels ? "general.UseGlobal" : "general.empty");
        drpDiscountLevel.SpecialFields = new string[,] { { emptyRecordText, "0" } };
        drpGlobalDiscountLevel.SiteID = 0;
    }


    /// <summary>
    /// Loads data.
    /// </summary>
    private void LoadForm()
    {
        if (!URLHelper.IsPostback())
        {
            string currentSiteName = CMSContext.CurrentSiteName;

            // Fill text fields
            txtCustomerCompany.Text = Customer.CustomerCompany;
            txtCustomerEmail.Text = Customer.CustomerEmail;
            txtCustomerFax.Text = Customer.CustomerFax;
            txtCustomerFirstName.Text = Customer.CustomerFirstName;
            txtCustomerLastName.Text = Customer.CustomerLastName;
            txtCustomerPhone.Text = Customer.CustomerPhone;
            txtOraganizationID.Text = Customer.CustomerOrganizationID;
            txtTaxRegistrationID.Text = Customer.CustomerTaxRegistrationID;
            chkCustomerEnabled.Checked = Customer.CustomerEnabled;

            if (Customer.CustomerCountryID > 0)
            {
                drpCountry.CountryID = Customer.CustomerCountryID;
            }
            if (Customer.CustomerStateID > 0)
            {
                drpCountry.StateID = Customer.CustomerStateID;
            }

            // Fill preferred currency
            int currencyId = (Customer.CustomerUser != null) ? Customer.CustomerUser.GetUserPreferredCurrencyID(currentSiteName) : 0;
            currencyId = (currencyId > 0) ? currencyId : Customer.CustomerPreferredCurrencyID;
            if (currencyId > 0)
            {
                drpCurrency.CurrencyID = currencyId;
            }

            // Fill preferred payment option
            int paymentId = (Customer.CustomerUser != null) ? Customer.CustomerUser.GetUserPreferredPaymentOptionID(currentSiteName) : 0;
            paymentId = (paymentId > 0) ? paymentId : Customer.CustomerPreferredPaymentOptionID;
            if (paymentId > 0)
            {
                drpPayment.PaymentID = paymentId;
            }

            // Fill preferred shipping option
            int shippingId = (Customer.CustomerUser != null) ? Customer.CustomerUser.GetUserPreferredShippingOptionID(currentSiteName) : 0;
            shippingId = (shippingId > 0) ? shippingId : Customer.CustomerPreferredShippingOptionID;
            if (shippingId > 0)
            {
                drpShipping.ShippingID = shippingId;
            }

            // Fill customers discount level
            if (Customer.CustomerDiscountLevelID > 0)
            {
                drpGlobalDiscountLevel.DiscountLevel = Customer.CustomerDiscountLevelID;
            }
            int siteDiscountId = (Customer.CustomerUser != null) ? Customer.CustomerUser.GetUserDiscountLevelID(currentSiteName) : 0;
            if (siteDiscountId > 0)
            {
                drpDiscountLevel.DiscountLevel = siteDiscountId;
            }
        }
    }


    /// <summary>
    /// Saves edited customer.
    /// </summary>
    public void Save()
    {
        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
        }

        // Validate form and get error message
        string errorMessage = Validate();

        if (string.IsNullOrEmpty(errorMessage))
        {
            // Save when no error
            SaveData();

            // Show message
            ShowChangesSaved();
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Validates entered values and returns error message.
    /// </summary>
    private string Validate()
    {
        string errorMessage = "";

        if ((txtCustomerCompany.Text.Trim() == "") &&
            ((txtCustomerFirstName.Text.Trim() == "") || (txtCustomerLastName.Text.Trim() == "")))
        {
            errorMessage = GetString("Customers_Edit.errorInsert");
        }
        else if (ECommerceSettings.RequireCompanyInfo(CMSContext.CurrentSite.SiteName) && (txtCustomerCompany.Text.Trim() != "" || txtOraganizationID.Text.Trim() != "" || txtTaxRegistrationID.Text.Trim() != ""))
        {
            errorMessage = new Validator().NotEmpty(txtCustomerCompany.Text.Trim(), GetString("customers_edit.errorcompany"))
                .NotEmpty(txtOraganizationID.Text.Trim(), GetString("customers_edit.errororganizationid"))
                .NotEmpty(txtTaxRegistrationID.Text.Trim(), GetString("customers_edit.errortaxregid")).Result;
        }
        else if ((txtCustomerEmail.Text.Trim() != "") && !ValidationHelper.IsEmail(txtCustomerEmail.Text))
        {
            errorMessage = GetString("Customers_Edit.errorEmail");
        }

        if (chkHasLogin.Checked)
        {
            // Check that user name and passwords fields are not empty
            if (errorMessage == "")
            {
                errorMessage = new Validator().NotEmpty(txtUserName.Text.Trim(), GetString("Customer_Edit_Login_Edit.rqvUserName"))
                    .NotEmpty(passStrength.Text, GetString("Customer_Edit_Login_Edit.rqvPassword1"))
                    .NotEmpty(txtPassword2.Text, GetString("Customer_Edit_Login_Edit.rqvPassword2")).Result;
            }

            // Check if passwords are the same
            if ((errorMessage == "") && (passStrength.Text != txtPassword2.Text))
            {
                errorMessage = GetString("Customer_Edit_Login_Edit.DifferentPasswords");
            }

            // Check policy
            if ((errorMessage == "") && !passStrength.IsValid())
            {
                errorMessage = AuthenticationHelper.GetPolicyViolationMessage(CMSContext.CurrentSiteName);
            }

            // Check if user name is unique
            if (errorMessage == "")
            {
                UserInfo existingUser = UserInfoProvider.GetUserInfo(txtUserName.Text.Trim());
                if (existingUser != null)
                {
                    errorMessage = GetString("Customer_Edit_Login_Edit.UserExist");
                }
            }
        }

        return errorMessage;
    }


    /// <summary>
    /// Saves data to DB without validation and permissions check.
    /// </summary>
    private void SaveData()
    {
        // If customer does not already exist, create new one
        if (Customer == null)
        {
            Customer = new CustomerInfo();
            Customer.CustomerSiteID = currentSiteId;
            Customer.CustomerEnabled = true;
        }

        Customer.CustomerEmail = txtCustomerEmail.Text.Trim().Truncate(100);
        Customer.CustomerFax = txtCustomerFax.Text.Trim();
        Customer.CustomerLastName = txtCustomerLastName.Text.Trim();
        Customer.CustomerPhone = txtCustomerPhone.Text.Trim();
        Customer.CustomerFirstName = txtCustomerFirstName.Text.Trim();
        Customer.CustomerCompany = txtCustomerCompany.Text.Trim();
        Customer.CustomerCountryID = drpCountry.CountryID;
        Customer.CustomerStateID = drpCountry.StateID;
        Customer.CustomerOrganizationID = txtOraganizationID.Text.Trim();
        Customer.CustomerTaxRegistrationID = txtTaxRegistrationID.Text.Trim();

        // Set customer's preferences
        Customer.CustomerPreferredCurrencyID = drpCurrency.CurrencyID;
        Customer.CustomerPreferredPaymentOptionID = drpPayment.PaymentID;
        Customer.CustomerPreferredShippingOptionID = drpShipping.ShippingID;

        if (plcDiscounts.Visible && plcGlobalDiscount.Visible)
        {
            Customer.CustomerDiscountLevelID = drpGlobalDiscountLevel.DiscountLevel;
        }

        // Only registered customer can be enabled/disabled
        if (Customer.CustomerIsRegistered)
        {
            Customer.CustomerEnabled = chkCustomerEnabled.Checked;
        }

        using (CMSTransactionScope tr = new CMSTransactionScope())
        {
            bool newUserCreated = false;

            // Create user for customer
            if (chkHasLogin.Checked)
            {
                UserInfo ui = new UserInfo();
                ui.UserName = txtUserName.Text.Trim();
                ui.FullName = Customer.CustomerFirstName + " " + Customer.CustomerLastName;
                ui.Email = Customer.CustomerEmail.Truncate(100);
                ui.IsGlobalAdministrator = false;
                ui.UserEnabled = true;

                UserInfoProvider.SetPassword(ui, passStrength.Text);
                UserInfoProvider.AddUserToSite(ui.UserName, CMSContext.CurrentSiteName);

                ui.UserSettings.SetValue("UserPhone", Customer.CustomerPhone);
                UserInfoProvider.SetUserInfo(ui);

                Customer.CustomerEnabled = true;
                Customer.CustomerUserID = ui.UserID;

                chkCustomerEnabled.Checked = Customer.CustomerEnabled;

                newUserCreated = true;

                // Show fields requiring registered customer
                pnlEdit.Visible = false;
                pnlStatic.Visible = true;
                plcDiscounts.Visible = true;
                plcPreferences.Visible = true;
                chkHasLogin.Checked = false;
                plcSiteDiscount.Visible = true;
                lblUserNameStaticValue.Text = HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(ui.UserName));
                if (AllowEditUser)
                {
                    btnEditUser.OnClientClick = "modalDialog('" + CMSContext.ResolveDialogUrl("~/CMSModules/Membership/Pages/Users/User_Edit_Frameset.aspx") + "?userid=" + Customer.CustomerUserID + "', 'UserEdit', 950, 700); return false;";
                    btnEditUser.Visible = true;
                }

                // Hide global discount level selector when global levels not allowed
                plcGlobalDiscount.Visible = allowGlobalDiscountLevels;
            }

            // Save customer
            CustomerInfoProvider.SetCustomerInfo(Customer);

            // Enable/disable corresponding registered user
            if (Customer.CustomerIsRegistered && !newUserCreated)
            {
                UserInfo ui = UserInfoProvider.GetUserInfo(Customer.CustomerUserID);

                // If the customer already has the record in the CMS_User table, update email
                if (ui != null)
                {
                    ui.Email = Customer.CustomerEmail;
                    UserInfoProvider.SetUserInfo(ui);
                }

                // Save site specific values
                UserSiteInfo userSite = UserSiteInfoProvider.GetUserSiteInfo(Customer.CustomerUserID, currentSiteId);
                if (userSite != null)
                {
                    userSite.UserPreferredCurrencyID = drpCurrency.CurrencyID;
                    userSite.UserPreferredPaymentOptionID = drpPayment.PaymentID;
                    userSite.UserPreferredShippingOptionID = drpShipping.ShippingID;
                    userSite.UserDiscountLevelID = drpDiscountLevel.DiscountLevel;

                    UserSiteInfoProvider.SetUserSiteInfo(userSite);
                }
            }

            // Commit transaction
            tr.Commit();

            // Raise OnSaved event
            RaiseOnSaved();
        }
    }

    #endregion
}
