using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

[Guid("8EF3FF11-429C-4438-B922-A0884CED307F")]
public partial class CMSModules_ContactManagement_Controls_UI_Contact_Edit : CMSAdminEditControl
{
    #region "Variables and constants"

    private int mSiteID = 0;
    private bool mergedIntoSite = false;
    private bool mergedIntoGlobal = false;
    private ContactInfo parentContact = null;
    private bool isNew = false;
    private HeaderAction btnSave;
    private HeaderAction btnSplit;

    /// <summary>
    /// URL of modal dialog window for contact editing.
    /// </summary>
    private const string CONTACT_DETAIL_DIALOG = "~/CMSModules/ContactManagement/Pages/Tools/Account/Contact_Detail.aspx";

    #endregion


    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return EditForm;
        }
    }


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
            EditForm.StopProcessing = value;
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
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
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

            if (userSelector != null)
            {
                userSelector.ShowSiteFilter = true;
                userSelector.SiteID = mSiteID;
            }
            if (contactStatusSelector != null)
            {
                contactStatusSelector.SiteID = mSiteID;
            }
            if (campaignSelector != null)
            {
                campaignSelector.SiteID = mSiteID;
            }
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        EditForm.OnAfterDataLoad += new EventHandler(EditForm_OnAfterDataLoad);
        EditForm.OnBeforeSave += new EventHandler(EditForm_OnBeforeSave);
        EditForm.OnAfterSave += new EventHandler(EditForm_OnAfterSave);

        InitHeaderActions();
    }


    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        // Refresh breadcrumbs after data are saved
        RaiseOnSaved();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        fCampaignSelection.Visible = CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.WebAnalytics", "read");
        RegisterScripts();
        SetButtonsVisibility();

        // Initialize properties
        pnlGeneral.GroupingText = GetString("general.general");
        pnlPersonal.GroupingText = GetString("om.contact.personal");
        pnlSettings.GroupingText = GetString("om.contact.settings");
        pnlAddress.GroupingText = GetString("general.address");
        pnlNotes.GroupingText = GetString("om.contact.notes");
        btnStamp.OnClientClick = "AddStamp('" + htmlNotes.CurrentEditor.ClientID + "'); return false;";

        // Initialize redirection URL
        string url = "Frameset.aspx?contactid={%EditedObject.ID%}&saved=1";
        url = URLHelper.AddParameterToUrl(url, "siteid", SiteID.ToString());
        if (ContactHelper.IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "issitemanager", "1");
        }
        EditForm.RedirectUrlAfterCreate = url;

        if (userSelector.SiteID <= 0)
        {
            userSelector.WhereCondition = "UserName NOT LIKE N'public'";
            userSelector.ShowSiteFilter = true;
        }
        userSelector.ReloadData();

        htmlNotes.SetValue("width", "950");
    }

    #endregion


    #region "UIform events"

    /// <summary>
    /// OnAfterDataLoad event handler.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            countrySelector.CountryID = ValidationHelper.GetInteger(EditForm.Data["ContactCountryID"], 0);
            countrySelector.StateID = ValidationHelper.GetInteger(EditForm.Data["ContactStateID"], 0);
        }
        if ((EditForm.EditedObject != null) && (ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("ContactID"), 0) != 0))
        {
            SiteID = ValidationHelper.GetInteger(EditForm.Data["ContactSiteID"], 0);
        }
        else
        {
            userSelector.SiteID = SiteID;
            contactStatusSelector.SiteID = SiteID;
            campaignSelector.SiteID = SiteID;
            chkMonitored.Value = 1;
            isNew = true;
        }
        if (userSelector.SiteID <= 0)
        {
            userSelector.WhereCondition = "UserName NOT LIKE N'public'";
        }

        userSelector.ReloadData();
        contactStatusSelector.ReloadData();
        campaignSelector.ReloadData();

        // Set value of date and time labels
        cCreated.Text = DateTimeHelper.DateTimeToString(EditForm.EditedObject.GetDateTimeValue("ContactCreated", DateTimeHelper.ZERO_TIME));
        cLastLogon.Text = DateTimeHelper.DateTimeToString(EditForm.EditedObject.GetDateTimeValue("ContactLastLogon", DateTimeHelper.ZERO_TIME));
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        if (countrySelector.CountryID == 0)
        {
            EditForm.Data["ContactCountryID"] = DBNull.Value;
        }
        else
        {
            EditForm.Data["ContactCountryID"] = countrySelector.CountryID;
        }
        if (countrySelector.StateID == 0)
        {
            EditForm.Data["ContactStateID"] = DBNull.Value;
        }
        else
        {
            EditForm.Data["ContactStateID"] = countrySelector.StateID;
        }
        if (SiteID > 0)
        {
            EditForm.Data["ContactSiteID"] = SiteID;
        }
        else
        {
            EditForm.Data["ContactSiteID"] = DBNull.Value;
        }

        if (isNew)
        {
            // Set ContactIsAnonymous default value
            EditForm.Data["ContactIsAnonymous"] = true;
        }
    }

    #endregion


    #region "Methods"

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

function EditContact(contactID)
{
    modalDialog('" + ResolveUrl(CONTACT_DETAIL_DIALOG) + @"?contactid=' + contactID + '&isSiteManager=" + ContactHelper.IsSiteManager + @"', 'ContactParent', '1024px', '700px');
}

"));
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        // Initialize SAVE button
        btnSave = btnSave ?? new SaveAction(Page);

        // Initialize SPLIT button
        btnSplit = btnSplit ?? new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("om.contact.splitfromparent"),
            ImageUrl = GetImageUrl("CMSModules/CMS_ContactManagement/split.png"),
            CommandName = "split",
            CommandArgument = "false",
        };

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        HeaderActions.AddAction(btnSave);
        HeaderActions.AddAction(btnSplit);
    }

    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check permission
        ContactHelper.AuthorizedModifyContact(SiteID, true);

        switch (e.CommandName.ToLowerCSafe())
        {
            // Save contact
            case "save":
                if (ValidateForm())
                {
                    EditForm.SaveData(null);
                    SetButtonsVisibility();
                }
                break;

            // Split from parent contact
            case "split":
                List<ContactInfo> mergedContact = new List<ContactInfo>(1) { (ContactInfo)CMSPage.EditedObject };
                ContactHelper.Split(parentContact, mergedContact, false, false, false, false);
                SetButtonsVisibility();
                ShowConfirmation(GetString("om.contact.splitted"));
                break;
        }
    }


    /// <summary>
    /// Sets visibility of buttons that are connected to merged contact - split button and link to his parent.
    /// </summary>
    private void SetButtonsVisibility()
    {
        // Find out if current contact is merged into another site or global contact
        mergedIntoSite = ValidationHelper.GetInteger(EditForm.Data["ContactMergedWithContactID"], 0) != 0;
        mergedIntoGlobal = ValidationHelper.GetInteger(EditForm.Data["ContactGlobalContactID"], 0) != 0;
        mergedIntoGlobal &= ContactHelper.AuthorizedModifyContact(UniSelector.US_GLOBAL_RECORD, false);

        if (!ContactHelper.IsSiteManager)
        {
            mergedIntoGlobal &= SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSCMGlobalContacts");
        }

        btnSplit.Visible = mergedIntoGlobal || mergedIntoSite;

        // Get parent contact
        if (mergedIntoSite)
        {
            parentContact = ContactInfoProvider.GetContactInfo(ValidationHelper.GetInteger(EditForm.Data["ContactMergedWithContactID"], 0));
            lblMergedInto.ResourceString = "om.contact.mergedintosite";
            lblMergedInto.Visible = true;
        }
        else if (mergedIntoGlobal)
        {
            parentContact = ContactInfoProvider.GetContactInfo(ValidationHelper.GetInteger(EditForm.Data["ContactGlobalContactID"], 0));
            lblMergedInto.ResourceString = "om.contact.mergedintoglobal";
            lblMergedInto.Visible = true;
        }
        else
        {
            lblMergedInto.Visible = false;
        }

        if (mergedIntoSite || mergedIntoGlobal)
        {
            string contactFullName = parentContact.ContactFirstName + " " + parentContact.ContactMiddleName;
            contactFullName = contactFullName.Trim() + " " + parentContact.ContactLastName;
            ltlButton.Text = string.Format(@" {0} <img class='UnigridActionButton' style='cursor:pointer;' onclick='EditContact({1});return false;' src='{2}' title='{3}'>",
                                           HTMLHelper.HTMLEncode(contactFullName.Trim()),
                                           parentContact.ContactID,
                                           GetImageUrl("Design/Controls/UniGrid/Actions/contactdetail.png"),
                                           GetString("om.contact.viewdetail"));
            ltlButton.Visible = true;
        }
        else
        {
            ltlButton.Visible = false;
        }
    }


    /// <summary>
    /// Performs custom validation and displays error in top of the page.
    /// </summary>
    /// <returns>Returns true if validation is successful.</returns>
    protected bool ValidateForm()
    {
        // Validate name
        string errorMessage = new Validator().NotEmpty(txtLastName.Text.Trim(), GetString("om.contact.enterlastname")).Result;
        if (!String.IsNullOrEmpty(errorMessage.ToString()))
        {
            ShowError(errorMessage);
            return false;
        }

        // Validates birthday
        if (!calBirthday.IsValid())
        {
            ShowError(calBirthday.ValidationError);
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

    #endregion
}