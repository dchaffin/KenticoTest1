using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_ContactManagement_Controls_UI_Account_Edit : CMSAdminEditControl
{
    #region "Variables"

    private AccountInfo ai = null;
    private int mSiteID = 0;
    private bool mergedIntoSite = false;
    private bool mergedIntoGlobal = false;
    private AccountInfo parentAI = null;

    /// <summary>
    /// URL of modal dialog window for account editing.
    /// </summary>
    private const string ACCOUNT_DETAIL_DIALOG = "~/CMSModules/ContactManagement/Pages/Tools/Contact/Account_Detail.aspx";

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            base.IsLiveSite = value;
            parentAccount.IsLiveSite = value;
            accountStatus.IsLiveSite = value;
            accountOwner.IsLiveSite = value;
            countrySelector.IsLiveSite = value;
            primaryContact.IsLiveSite = value;
            secondaryContact.IsLiveSite = value;
            contactRolePrimary.IsLiveSite = value;
            contactRoleSecondary.IsLiveSite = value;
            htmlNotes.IsLiveSite = value;
        }
    }


    /// <summary>
    /// SiteID of current account.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;

            if ((mSiteID > 0) && !CMSContext.CurrentUser.UserSiteManagerAdmin)
            {
                mSiteID = CMSContext.CurrentSiteID;
            }
            parentAccount.SiteID = mSiteID;
            accountStatus.SiteID = mSiteID;
            primaryContact.SiteID = mSiteID;
            secondaryContact.SiteID = mSiteID;
            contactRolePrimary.SiteID = mSiteID;
            contactRoleSecondary.SiteID = mSiteID;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        primaryContact.UniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        secondaryContact.UniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;

        // Get edited object
        if (CMSContext.EditedObject != null)
        {
            ai = (AccountInfo)CMSContext.EditedObject;
        }

        InitHeaderActions();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            // Display info label that account is merged into site account
            if ((ai != null) && (mergedIntoSite))
            {
                parentAI = AccountInfoProvider.GetAccountInfo(ai.AccountMergedWithAccountID);
                lblMergedInto.ResourceString = "om.account.mergedintosite";
            }
            // Display info that account is merged into global account
            else if ((ai != null) && (mergedIntoGlobal))
            {
                parentAI = AccountInfoProvider.GetAccountInfo(ai.AccountGlobalAccountID);
                lblMergedInto.ResourceString = "om.account.mergedintoglobal";
            }
            // Don't display any info
            else
            {
                lblMergedInto.Visible = false;
            }

            // Set basic properties
            pnlGeneral.GroupingText = GetString("general.general");
            pnlAddress.GroupingText = GetString("contentmenu.address");
            pnlContacts.GroupingText = GetString("om.contact.list");
            pnlNotes.GroupingText = GetString("om.account.notes");
            btnStamp.OnClientClick = "AddStamp('" + htmlNotes.ClientID + "'); return false;";
            Reload();
            RegisterScripts();

            primaryContact.OnSelectionChanged += (s, ea) => SetContactRoleID((FormEngineUserControl)s, contactRolePrimary);
            secondaryContact.OnSelectionChanged += (s, ea) => SetContactRoleID((FormEngineUserControl)s, contactRoleSecondary);

            // Display 'changes saved' or 'split' label
            if (!RequestHelper.IsPostBack())
            {
                if (QueryHelper.GetBoolean("saved", false))
                {
                    ShowChangesSaved();
                }
                else if (QueryHelper.GetBoolean("split", false))
                {
                    ShowConfirmation(GetString("om.account.splitted"));
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display panel with primary/secondary contacts
        if ((CMSContext.EditedObject != null) && primaryContact.UniSelector.HasData)
        {
            pnlContacts.Visible = true;
        }
        else
        {
            pnlContacts.Visible = false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload()
    {
        // Load controls with data
        if (ai != null)
        {
            SiteID = ai.AccountSiteID;

            // Load values from DB
            if (!RequestHelper.IsPostBack())
            {
                txtName.Text = ai.AccountName;
                accountStatus.Value = ai.AccountStatusID;
                parentAccount.Value = ai.AccountSubsidiaryOfID;
                accountOwner.Value = ai.AccountOwnerUserID;
                txtAddress1.Text = ai.AccountAddress1;
                txtAddress2.Text = ai.AccountAddress2;
                txtCity.Text = ai.AccountCity;
                txtZip.Text = ai.AccountZIP;
                countrySelector.CountryID = ai.AccountCountryID;
                countrySelector.StateID = ai.AccountStateID;
                txtPhone.Text = ai.AccountPhone;
                txtFax.Text = ai.AccountFax;
                emailInput.Value = ai.AccountEmail;
                txtURL.Text = ai.AccountWebSite;
                htmlNotes.Value = ai.AccountNotes;
                primaryContact.Value = ai.AccountPrimaryContactID;
                secondaryContact.Value = ai.AccountSecondaryContactID;

                // Get primary contact role
                AccountContactInfo aci = null;
                if (ai.AccountPrimaryContactID > 0)
                {
                    aci = AccountContactInfoProvider.GetAccountContactInfo(ai.AccountID, ai.AccountPrimaryContactID);
                    if (aci != null)
                    {
                        contactRolePrimary.Value = aci.ContactRoleID;
                    }
                }
                // Get secondary contact role
                if (ai.AccountSecondaryContactID > 0)
                {
                    aci = AccountContactInfoProvider.GetAccountContactInfo(ai.AccountID, ai.AccountSecondaryContactID);
                    if (aci != null)
                    {
                        contactRoleSecondary.Value = aci.ContactRoleID;
                    }
                }
            }

            // Setup selectors
            parentAccount.WhereCondition = "(AccountID NOT IN (SELECT * FROM Func_OM_Account_GetSubsidiaries(" + ai.AccountID + ", 1)) AND AccountMergedWithAccountID IS NULL)";
            primaryContact.WhereCondition = "(ContactID IN (SELECT ContactID FROM OM_AccountContact WHERE AccountID = " + ai.AccountID + "))";
            secondaryContact.WhereCondition = "(ContactID IN (SELECT ContactID FROM OM_AccountContact WHERE AccountID = " + ai.AccountID + "))";

            // Reload controls
            parentAccount.ReloadData();
            primaryContact.ReloadData();
            secondaryContact.ReloadData();
            accountOwner.ReloadData();
            contactRolePrimary.ReloadData();
            contactRoleSecondary.ReloadData();
            accountStatus.ReloadData();
        }
    }


    /// <summary>
    /// Sets contact role ID to given role selector control based on contact selector value and edited account.
    /// </summary>
    private void SetContactRoleID(FormEngineUserControl contactSelector, FormEngineUserControl roleSelector)
    {
        int contactID = ValidationHelper.GetInteger(contactSelector.Value, 0);
        int accountID = ai.AccountID;
        var accountContactInfo = AccountContactInfoProvider.GetAccountContactInfo(accountID, contactID);
        if (accountContactInfo != null)
        {
            roleSelector.Value = accountContactInfo.ContactRoleID;
        }
        else
        {
            roleSelector.Value = UniSelector.US_NONE_RECORD;
        }
    }


    /// <summary>
    /// Registers JavaScripts on page.
    /// </summary>
    private void RegisterScripts()
    {
        string stamp = null;
        if (ContactHelper.IsSiteManager)
        {
            stamp = SettingsKeyProvider.GetStringValue("CMSCMStamp");
        }
        else
        {
            stamp = SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSCMStamp");
        }
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "AddStamp", ScriptHelper.GetScript(
            @"function InsertHTML(htmlString, ckClientID)
{
    // Get the editor instance that we want to interact with.
    var oEditor = oEditor = window.CKEDITOR.instances[ckClientID];
    // Check the active editing mode.
    if (oEditor != null) {
        // Check the active editing mode.
        if (oEditor.mode == 'wysiwyg') {
            // Insert the desired HTML.
            oEditor.focus();
            oEditor.insertHtml(htmlString);        
        }
    }    
    return false;
}   

function AddStamp(ckClientID)
{
InsertHTML('<div>" + CMSContext.CurrentResolver.ResolveMacros(stamp).Replace("'", @"\'") + @"</div>', ckClientID);
}

function EditAccount(accountID)
{
    modalDialog('" + ResolveUrl(ACCOUNT_DETAIL_DIALOG) + @"?accountid=' + accountID + '&isSiteManager=" + ContactHelper.IsSiteManager + @"', 'AccountParent', '1024', '700');
}

"));

        if (mergedIntoSite || mergedIntoGlobal)
        {
            ltlButton.Text = string.Format(@" {0} <img class='UnigridActionButton' style='cursor:pointer;' onclick='EditAccount({1});return false;' src='{2}' title='{3}'>",
                                           HTMLHelper.HTMLEncode(parentAI.AccountName.Trim()),
                                           parentAI.AccountID,
                                           GetImageUrl("Design/Controls/UniGrid/Actions/accountdetail.png"),
                                           GetString("om.account.viewdetail"));
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        // Find out if current account is merged into another site or global account
        if (ai != null)
        {
            mergedIntoSite = (ai.AccountMergedWithAccountID != 0);
            mergedIntoGlobal = (ai.AccountGlobalAccountID != 0);
            mergedIntoGlobal &= AccountHelper.AuthorizedReadAccount(UniSelector.US_GLOBAL_RECORD, false);

            if (!ContactHelper.IsSiteManager)
            {
                mergedIntoGlobal &= SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSCMGlobalAccounts");
            }
        }
        
        HeaderActions.AddAction(new SaveAction(Page));

        if (mergedIntoSite || mergedIntoGlobal)
        {
            HeaderActions.AddAction(new HeaderAction
            {
                ControlType = HeaderActionTypeEnum.Hyperlink,
                Text = GetString("om.contact.splitfromparent"),
                ImageUrl = GetImageUrl("CMSModules/CMS_ContactManagement/split.png"),
                CommandName = "split"
            });
        }

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        AccountHelper.AuthorizedModifyAccount(SiteID, true);

        // Split from parent account
        if (e.CommandName == "split")
        {
            List<AccountInfo> mergedAccount = new List<AccountInfo>();
            mergedAccount.Add(ai);
            AccountHelper.Split(parentAI, mergedAccount, false, false, false);
            ScriptHelper.RefreshTabHeader(Page, null);

            string url = URLHelper.AddParameterToUrl(URLHelper.CurrentURL, "split", "1");
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "RefreshPage", ScriptHelper.GetScript("window.location.replace('" + url + "');"));
        }
        // Save contact
        else
        {
            Save();
        }
    }


    /// <summary>
    /// Validates fields.
    /// </summary>
    private bool IsValid()
    {
        // Validates account name
        string errorMessage = new Validator().NotEmpty(txtName.Text.Trim(), GetString("om.account.entername")).Result;
        if (!String.IsNullOrEmpty(errorMessage.ToString()))
        {
            ShowError(errorMessage);
            return false;
        }

        // Validates email
        if (!emailInput.IsValid())
        {
            ShowError(emailInput.ValidationError);
            return false;
        }

        return true;
    }


    /// <summary>
    /// Save data.
    /// </summary>
    protected void Save()
    {
        if (IsValid())
        {
            bool redirect = false;

            // Create new account 
            if (ai == null)
            {
                ai = new AccountInfo();
                EditedObject = ai;
                redirect = true;
            }

            // Set values
            ai.AccountName = txtName.Text.Trim();
            ai.AccountSubsidiaryOfID = parentAccount.AccountID;
            ai.AccountOwnerUserID = ValidationHelper.GetInteger(accountOwner.Value, 0);
            ai.AccountStatusID = accountStatus.AccountStatusID;
            ai.AccountAddress1 = txtAddress1.Text.Trim();
            ai.AccountAddress2 = txtAddress2.Text.Trim();
            ai.AccountCity = txtCity.Text.Trim();
            ai.AccountZIP = txtZip.Text.Trim();
            ai.AccountCountryID = countrySelector.CountryID;
            ai.AccountStateID = countrySelector.StateID;
            ai.AccountPhone = txtPhone.Text.Trim();
            ai.AccountFax = txtFax.Text.Trim();
            ai.AccountEmail = ValidationHelper.GetString(emailInput.Value, null);
            ai.AccountWebSite = txtURL.Text.Trim();
            ai.AccountPrimaryContactID = primaryContact.ContactID;
            ai.AccountSecondaryContactID = secondaryContact.ContactID;
            ai.AccountNotes = htmlNotes.Value;
            ai.AccountSiteID = SiteID;

            try
            {
                // Save account changes
                AccountInfoProvider.SetAccountInfo(ai);
                if (AssignContacts())
                {
                    ShowChangesSaved();
                }

                RaiseOnSaved();

                // Redirect page after newly created item is saved
                if (redirect)
                {
                    string url = "Frameset.aspx?accountId=" + ai.AccountID + "&saved=1";
                    url = URLHelper.AddParameterToUrl(url, "siteid", SiteID.ToString());
                    if (ContactHelper.IsSiteManager)
                    {
                        url = URLHelper.AddParameterToUrl(url, "issitemanager", "1");
                    }

                    URLHelper.Redirect(url);
                }
            }
            catch (Exception e)
            {
                ShowError(GetString("general.saveerror"), e.Message, null);
            }
        }
    }


    /// <summary>
    /// Sets primary and secondary contacts.
    /// </summary>
    private bool AssignContacts()
    {
        ContactInfo contact = null;
        AccountContactInfo accountContact = null;
        // Assign primary contact to account and/or assign role
        if (primaryContact.ContactID > 0)
        {
            contact = ContactInfoProvider.GetContactInfo(primaryContact.ContactID);
            if (contact != null)
            {
                // Check if contact <-> account relation is already created
                accountContact = AccountContactInfoProvider.GetAccountContactInfo(ai.AccountID, primaryContact.ContactID);

                // Update relation
                if (accountContact != null)
                {
                    accountContact.ContactRoleID = contactRolePrimary.ContactRoleID;
                }
                AccountContactInfoProvider.SetAccountContactInfo(accountContact);
            }
            // Selected contact doesn't exist
            else
            {
                ShowError(GetString("om.contact.primarynotexists"));
                return false;
            }
        }

        // Assign secondary contact to account and/or assign role
        if (secondaryContact.ContactID > 0)
        {
            contact = ContactInfoProvider.GetContactInfo(secondaryContact.ContactID);
            if (contact != null)
            {
                // Check if contact <-> account relation is already created
                accountContact = AccountContactInfoProvider.GetAccountContactInfo(ai.AccountID, secondaryContact.ContactID);

                // Update relation
                if (accountContact != null)
                {
                    accountContact.ContactRoleID = contactRoleSecondary.ContactRoleID;
                }
                AccountContactInfoProvider.SetAccountContactInfo(accountContact);
            }
            else
            {
                ShowError(GetString("om.contact.secondarynotexists"));
                return false;
            }
        }

        return true;
    }

    #endregion
}