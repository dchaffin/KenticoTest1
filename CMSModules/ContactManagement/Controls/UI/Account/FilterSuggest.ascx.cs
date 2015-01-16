using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_Account_FilterSuggest : CMSUserControl
{
    #region "Variables"

    private AccountInfo ai;
    private int mSelectedSiteID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current account.
    /// </summary>
    private AccountInfo CurrentAccount
    {
        get
        {
            if (ai == null)
            {
                ai = (AccountInfo)CMSContext.EditedObject;
            }
            return ai;
        }
    }


    /// <summary>
    /// Gets value indicating if contacts checkbox is selected.
    /// </summary>
    public bool ContactsChecked
    {
        get
        {
            return chkContacts.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if post address checkbox is selected.
    /// </summary>
    public bool AddressChecked
    {
        get
        {
            return chkAddress.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if email is selected.
    /// </summary>
    public bool EmailChecked
    {
        get
        {
            return chkEmail.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if URL checkbox is selected.
    /// </summary>
    public bool URLChecked
    {
        get
        {
            return chkURL.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if phone&fax checkbox is selected.
    /// </summary>
    public bool PhonesChecked
    {
        get
        {
            return chkPhone.Checked;
        }
    }

    /// <summary>
    /// Gets selected site ID.
    /// </summary>
    public int SelectedSiteID
    {
        get
        {
            return mSelectedSiteID;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        chkContacts.Enabled = (CurrentAccount.AccountPrimaryContactID != 0) || (CurrentAccount.AccountSecondaryContactID != 0);
        chkAddress.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountAddress1) || !String.IsNullOrEmpty(CurrentAccount.AccountAddress2) || !String.IsNullOrEmpty(CurrentAccount.AccountCity) || !String.IsNullOrEmpty(CurrentAccount.AccountZIP);
        chkEmail.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountEmail);
        chkURL.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountWebSite);
        chkPhone.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountPhone) || !String.IsNullOrEmpty(CurrentAccount.AccountFax);

        // Current account is global object
        if (ai.AccountSiteID == 0)
        {
            plcSite.Visible = true;
            plcContact.Visible = false;
            plcEmpty.Visible = false;
            // Display site selector in site manager
            if (ContactHelper.IsSiteManager)
            {
                siteOrGlobalSelector.Visible = false;
            }
            // Display 'site or global' selector in CMS desk for global objects
            else if (AccountHelper.AuthorizedReadAccount(CMSContext.CurrentSiteID, false) && AccountHelper.AuthorizedModifyAccount(CMSContext.CurrentSiteID, false))
            {
                siteSelector.Visible = false;
            }
            else
            {
                plcSite.Visible = false;
            }
        }
    }


    /// <summary>
    /// Returns SQL WHERE condition depending on selected checkboxes.
    /// </summary>
    /// <returns>Returns SQL WHERE condition</returns>
    public string GetWhereCondition()
    {
        string where = null;

        // Contacts checked
        if (chkContacts.Checked)
        {
            string contactWhere = null;
            ContactInfo contact;

            // Get primary contact WHERE condition
            if (CurrentAccount.AccountPrimaryContactID != 0)
            {
                contact = ContactInfoProvider.GetContactInfo(CurrentAccount.AccountPrimaryContactID);
                if (contact != null)
                {
                    if (!String.IsNullOrEmpty(contact.ContactFirstName))
                    {
                        contactWhere = SqlHelperClass.AddWhereCondition(contactWhere, "PrimaryContactFirstName LIKE N'%" + SqlHelperClass.GetSafeQueryString(contact.ContactFirstName, false) + "%'", "OR");
                    }
                    if (!String.IsNullOrEmpty(contact.ContactMiddleName))
                    {
                        contactWhere = SqlHelperClass.AddWhereCondition(contactWhere, "PrimaryContactMiddleName LIKE N'%" + SqlHelperClass.GetSafeQueryString(contact.ContactMiddleName, false) + "%'", "OR");
                    }
                    if (!String.IsNullOrEmpty(contact.ContactLastName))
                    {
                        contactWhere = SqlHelperClass.AddWhereCondition(contactWhere, "PrimaryContactLastName LIKE N'%" + SqlHelperClass.GetSafeQueryString(contact.ContactLastName, false) + "%'", "OR");
                    }
                }
            }

            // Get secondary contact WHERE condition
            if (CurrentAccount.AccountSecondaryContactID != 0)
            {
                contact = ContactInfoProvider.GetContactInfo(CurrentAccount.AccountSecondaryContactID);
                if (contact != null)
                {
                    if (!String.IsNullOrEmpty(contact.ContactFirstName))
                    {
                        contactWhere = SqlHelperClass.AddWhereCondition(contactWhere, "SecondaryContactFirstName LIKE N'%" + SqlHelperClass.GetSafeQueryString(contact.ContactFirstName, false) + "%'", "OR");
                    }
                    if (!String.IsNullOrEmpty(contact.ContactMiddleName))
                    {
                        contactWhere = SqlHelperClass.AddWhereCondition(contactWhere, "SecondaryContactMiddleName LIKE N'%" + SqlHelperClass.GetSafeQueryString(contact.ContactMiddleName, false) + "%'", "OR");
                    }
                    if (!String.IsNullOrEmpty(contact.ContactLastName))
                    {
                        contactWhere = SqlHelperClass.AddWhereCondition(contactWhere, "SecondaryContactLastName LIKE N'%" + SqlHelperClass.GetSafeQueryString(contact.ContactLastName, false) + "%'", "OR");
                    }
                }
            }

            if (!String.IsNullOrEmpty(contactWhere))
            {
                where = SqlHelperClass.AddWhereCondition(where, contactWhere);
            }
        }

        // Address checked
        if (chkAddress.Checked)
        {
            string addressWhere = null;
            if (!String.IsNullOrEmpty(CurrentAccount.AccountAddress1))
            {
                addressWhere = SqlHelperClass.AddWhereCondition(addressWhere, "AccountAddress1 LIKE N'%" + SqlHelperClass.GetSafeQueryString(CurrentAccount.AccountAddress1, false) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentAccount.AccountAddress2))
            {
                addressWhere = SqlHelperClass.AddWhereCondition(addressWhere, "AccountAddress2 LIKE N'%" + SqlHelperClass.GetSafeQueryString(CurrentAccount.AccountAddress2, false) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentAccount.AccountCity))
            {
                addressWhere = SqlHelperClass.AddWhereCondition(addressWhere, "AccountCity LIKE N'%" + SqlHelperClass.GetSafeQueryString(CurrentAccount.AccountCity, false) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentAccount.AccountZIP))
            {
                addressWhere = SqlHelperClass.AddWhereCondition(addressWhere, "AccountZIP LIKE N'%" + SqlHelperClass.GetSafeQueryString(CurrentAccount.AccountZIP, false) + "%'", "OR");
            }

            if (!String.IsNullOrEmpty(addressWhere))
            {
                where = SqlHelperClass.AddWhereCondition(where, "(" + addressWhere + ")");
            }
        }

        // Email address checked
        if (chkEmail.Checked)
        {
            string domain = ContactHelper.GetEmailDomain(CurrentAccount.AccountEmail);
            if (!String.IsNullOrEmpty(domain))
            {
                string emailWhere = "AccountEmail LIKE N'%@" + SqlHelperClass.GetSafeQueryString(domain) + "'";
                where = SqlHelperClass.AddWhereCondition(where, emailWhere);
            }
        }

        // URL checked
        if (chkURL.Checked && !String.IsNullOrEmpty(CurrentAccount.AccountWebSite))
        {
            string urlWhere = "(AccountWebSite LIKE N'%" + SqlHelperClass.GetSafeQueryString(URLHelper.CorrectDomainName(CurrentAccount.AccountWebSite)) + "%')";
            where = SqlHelperClass.AddWhereCondition(where, urlWhere);
        }

        // Phone & fax checked
        if (chkPhone.Checked && (!String.IsNullOrEmpty(CurrentAccount.AccountPhone) || !String.IsNullOrEmpty(CurrentAccount.AccountFax)))
        {
            string phoneWhere = null;
            if (!String.IsNullOrEmpty(CurrentAccount.AccountPhone))
            {
                phoneWhere = "AccountPhone LIKE N'%" + SqlHelperClass.GetSafeQueryString(CurrentAccount.AccountPhone, false) + "%'";
            }
            if (!String.IsNullOrEmpty(CurrentAccount.AccountFax))
            {
                phoneWhere = SqlHelperClass.AddWhereCondition(phoneWhere, "AccountFax LIKE N'%" + SqlHelperClass.GetSafeQueryString(CurrentAccount.AccountFax, false) + "%'", "OR");
            }

            if (!String.IsNullOrEmpty(phoneWhere))
            {
                where = SqlHelperClass.AddWhereCondition(where, "(" + phoneWhere + ")");
            }
        }

        if ((!chkContacts.Checked && !chkAddress.Checked && !chkEmail.Checked && !chkURL.Checked && !chkPhone.Checked) || (String.IsNullOrEmpty(where)))
        {
            return "(1 = 0)";
        }

        // Filter out current account
        where = SqlHelperClass.AddWhereCondition(where, "AccountID <> " + CurrentAccount.AccountID);
        // Filter out merged records
        where = SqlHelperClass.AddWhereCondition(where, "(AccountMergedWithAccountID IS NULL AND AccountGlobalAccountID IS NULL AND AccountSiteID > 0) OR (AccountGlobalAccountID IS NULL AND AccountSiteID IS NULL)");

        // For global object use siteselector's value
        if (plcSite.Visible)
        {
            mSelectedSiteID = UniSelector.US_ALL_RECORDS;
            if (siteSelector.Visible)
            {
                mSelectedSiteID = siteSelector.SiteID;
            }
            else if (siteOrGlobalSelector.Visible)
            {
                mSelectedSiteID = siteOrGlobalSelector.SiteID;
            }

            // Only global objects
            if (mSelectedSiteID == UniSelector.US_GLOBAL_RECORD)
            {
                where = SqlHelperClass.AddWhereCondition(where, "AccountSiteID IS NULL");
            }
            // Global and site objects
            else if (mSelectedSiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD)
            {
                where = SqlHelperClass.AddWhereCondition(where, "AccountSiteID IS NULL OR AccountSiteID = " + CMSContext.CurrentSiteID);
            }
            // Site objects
            else if (mSelectedSiteID != UniSelector.US_ALL_RECORDS)
            {
                where = SqlHelperClass.AddWhereCondition(where, "AccountSiteID = " + mSelectedSiteID);
            }
        }
        // Filter out accounts from different sites
        else
        {
            // Site accounts only
            if (CurrentAccount.AccountSiteID > 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "AccountSiteID = " + CurrentAccount.AccountSiteID);
            }
            // Global accounts only
            else
            {
                where = SqlHelperClass.AddWhereCondition(where, "AccountSiteID IS NULL");
            }
        }

        return where;
    }

    #endregion
}