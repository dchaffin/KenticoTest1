using System;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_UI_CustomerNew : CMSUserControl
{
    /// <summary>
    /// Indicates whether the control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return pnlGeneral.Enabled;
        }
        set
        {
            pnlGeneral.Enabled = value;
            pnlUser.Enabled = value;
        }
    }


    /// <summary>
    /// Gets placeholder for info messages.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        pnlGeneral.GroupingText = GetString("com.customeredit.general");
        pnlContacts.GroupingText = GetString("com.customeredit.contacts");
        pnlUser.GroupingText = GetString("com.customeredit.userinfo");

        rqvUserName.ErrorMessage = GetString("customer_edit_login_edit.rqvusername");
        rqvPassword2.ErrorMessage = GetString("customer_edit_login_edit.rqvpassword2");

        // Set changes saved message
        if (QueryHelper.GetString("saved", "") == "1")
        {
            ShowChangesSaved();
        }

        if (!RequestHelper.IsPostBack())
        {
            drpCountry.CountryID = UniSelector.US_NONE_RECORD;
        }

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        plcUserFields.Visible = chkCreateLogin.Checked;

        base.OnPreRender(e);
    }


    public bool IsValid()
    {
        string firstName = txtCustomerFirstName.Text.Trim();
        string lastName = txtCustomerLastName.Text.Trim();
        string company = txtCustomerCompany.Text.Trim();

        // Check if name or company information are filled
        if ((string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)) && string.IsNullOrEmpty(company))
        {
            ShowError(GetString("customers_edit.errorinsert"));
            return false;
        }
        // Check company information if required
        else if (ECommerceSettings.RequireCompanyInfo(CMSContext.CurrentSite.SiteName)
            && (!string.IsNullOrEmpty(txtCustomerCompany.Text.Trim()) || !string.IsNullOrEmpty(txtOrganizationID.Text.Trim()) || !string.IsNullOrEmpty(txtTaxRegistrationID.Text.Trim())))
        {
            string error = new Validator()
                .NotEmpty(txtCustomerCompany.Text.Trim(), GetString("customers_edit.errorcompany"))
                .NotEmpty(txtOrganizationID.Text.Trim(), GetString("customers_edit.errororganizationid"))
                .NotEmpty(txtTaxRegistrationID.Text.Trim(), GetString("customers_edit.errortaxregid"))
                .Result;

            if (!string.IsNullOrEmpty(error))
            {
                ShowError(error);
                return false;
            }
        }

        // Check email
        if (!string.IsNullOrEmpty(txtCustomerEmail.Text.Trim()) && !ValidationHelper.IsEmail(txtCustomerEmail.Text))
        {
            ShowError(GetString("customers_edit.erroremail"));
            return false;
        }

        // Validate user fields
        if (chkCreateLogin.Checked)
        {
            // Check validators
            string error = new Validator()
                .NotEmpty(txtUserName.Text.Trim(), GetString("customer_edit_login_edit.rqvusername"))
                .NotEmpty(passStrength.Text.Trim(), GetString("customer_edit_login_edit.rqvpassword1"))
                .NotEmpty(txtPassword2.Text.Trim(), GetString("customer_edit_login_edit.rqvpassword2"))
                .Result;

            if (!string.IsNullOrEmpty(error))
            {
                // Disaply error message
                ShowError(error);
                return false;
            }

            // Check policy
            if (!passStrength.IsValid())
            {
                ShowError(AuthenticationHelper.GetPolicyViolationMessage(CMSContext.CurrentSiteName));
                return false;
            }

            // Compare passwords
            if (passStrength.Text != txtPassword2.Text)
            {
                ShowError(GetString("customer_edit_login_edit.differentpasswords"));
                return false;
            }

            // Check if user already exists
            UserInfo existingUser = UserInfoProvider.GetUserInfo(txtUserName.Text.Trim());
            if (existingUser != null)
            {
                ShowError(GetString("customer_edit_login_edit.userexist"));
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Saves customer data and returns customer ID.
    /// </summary>
    public int Save()
    {
        if (!IsValid())
        {
            return 0;
        }

        // Create customer
        CustomerInfo customer = new CustomerInfo()
        {
            CustomerSiteID = CMSContext.CurrentSiteID,
            CustomerFirstName = txtCustomerFirstName.Text.Trim().Truncate(100),
            CustomerLastName = txtCustomerLastName.Text.Trim().Truncate(100),
            CustomerCompany = txtCustomerCompany.Text.Trim(),
            CustomerOrganizationID = txtOrganizationID.Text.Trim(),
            CustomerTaxRegistrationID = txtTaxRegistrationID.Text.Trim(),
            CustomerCountryID = drpCountry.CountryID,
            CustomerStateID = drpCountry.StateID,
            CustomerEmail = txtCustomerEmail.Text.Trim().Truncate(100),
            CustomerPhone = txtCustomerPhone.Text.Trim(),
            CustomerFax = txtCustomerFax.Text.Trim(),
            CustomerEnabled = true
        };

        // Save data in transaction
        using (CMSTransactionScope tr = new CMSTransactionScope())
        {
            CustomerInfoProvider.SetCustomerInfo(customer);

            if (chkCreateLogin.Checked)
            {
                // Create user
                UserInfo user = new UserInfo()
                {
                    UserName = txtUserName.Text.Trim(),
                    FullName = customer.CustomerFirstName + " " + customer.CustomerLastName,
                    Email = customer.CustomerEmail,
                    IsGlobalAdministrator = false
                };

                user.SetValue("UserEnabled", true);

                UserInfoProvider.SetPassword(user, passStrength.Text);
                UserInfoProvider.AddUserToSite(user.UserName, CMSContext.CurrentSiteName);

                user.UserSettings.SetValue("UserPhone", customer.CustomerPhone);
                UserInfoProvider.SetUserInfo(user);

                customer.CustomerUserID = user.UserID;
                CustomerInfoProvider.SetCustomerInfo(customer);
            }

            tr.Commit();
        }

        return customer.CustomerID;
    }
}

