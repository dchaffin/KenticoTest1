using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Controls_UI_Activity_Edit : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Gets or sets value that will generate button Add Stamp.
    /// </summary>
    protected string AddStampValue
    {
        get;
        set;
    }


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
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Shows/hides site selector.
    /// </summary>
    public bool ShowSiteSelector
    {
        get;
        set;
    }


    /// <summary>
    /// Allows to display contacts only for specified site ID. Use 0 for global objects.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Allows to preselect specific contact.
    /// </summary>
    public int ContactID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeSave += new EventHandler(EditForm_OnBeforeSave);
        EditForm.OnBeforeValidate += new EventHandler(EditForm_OnBeforeValidate);

        tsType.ShowManuallyCreatableActivities = true;
        tsType.ShowEnabledActivitiesOnly = true;

        plcSite.Visible = ShowSiteSelector;
        if (!RequestHelper.IsPostBack())
        {
            // Pre-select current site, current date and contact (if set)
            ucSite.SiteName = CMSContext.CurrentSiteName;
            dtCreated.SelectedDateTime = DateTime.Now;
            ucContact.Value = ContactID;
        }

        // Get correct site ID
        int siteId = 0;
        if (!ShowSiteSelector)
        {
            if (SiteID <= 0)
            {
                siteId = CMSContext.CurrentSiteID;
            }
            else
            {
                siteId = SiteID;
            }
        }
        else
        {
            siteId = ucSite.SiteID;
            ucSite.DropDownSingleSelect.SelectedIndexChanged += new EventHandler(DropDownSingleSelect_SelectedIndexChanged);
        }

        // Init contact selector
        ucContact.UniSelector.AllowEmpty = false;
        ucContact.SiteID = siteId;
        ucContact.UniSelector.ResourcePrefix = "om.activityselcontact";

        ucCampaign.WhereCondition = "CampaignSiteID=" + siteId;
        ucCampaign.Reload(true);

        if (ContactID > 0)
        {
            // Change redirect URL when new custom activity is created under edited contact
            EditForm.RedirectUrlAfterCreate = string.Format("~/CMSModules/ContactManagement/Pages/Tools/Contact/Tab_Activities.aspx?saved=1&contactid={0}", ContactID);
        }
        if (QueryHelper.GetBoolean("issitemanager", false))
        {
            EditForm.RedirectUrlAfterCreate += "&issitemanager=1";
        }
    }


    protected override void OnInit(EventArgs e)
    {
        InitHeaderActions();
        InitStampValue();
    }


    /// <summary>
    /// Initializes Add Stamp button value.
    /// </summary>
    private void InitStampValue()
    {
        string stamp = SettingsKeyProvider.GetStringValue(ContactHelper.IsSiteManager ? CMSContext.CurrentSiteName + ".CMSCMStamp" : "CMSCMStamp");
        AddStampValue = CMSContext.CurrentResolver.ResolveMacros(stamp).Replace("'", @"\'");
    }


    /// <summary>
    /// Initializes header actions.
    /// </summary>
    private void InitHeaderActions()
    {
        var actions = new string[1,11];
        ;
        // Initialize SAVE button
        actions[0, 0] = HeaderActions.TYPE_SAVEBUTTON;
        actions[0, 1] = GetString("General.Save");
        actions[0, 5] = GetImageUrl("CMSModules/CMS_Content/EditMenu/save.png");
        actions[0, 6] = "save";
        actions[0, 8] = "true";

        ((CMSPage)Page).CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        ((CMSPage)Page).CurrentMaster.HeaderActions.Actions = actions;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                EditForm.SaveData(null);
                break;
        }
    }


    private void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear selected contact after site has been changed
        ucContact.Value = null;
    }


    protected void EditForm_OnBeforeValidate(object sender, EventArgs e)
    {
        // Check emptyness
        if (String.IsNullOrEmpty(ValidationHelper.GetString(ucContact.Value, null)))
        {
            ShowErrorMessage("om.activity.contactmissing", true);
            return;
        }

        // Check if selected contact exists
        EditForm.ParentObject = ContactInfoProvider.GetContactInfo(ucContact.ContactID);
        if (EditForm.ParentObject == null)
        {
            ShowErrorMessage("om.activity.contactdoesnotexist", true);
            return;
        }

        // Check selected date
        if (dtCreated.SelectedDateTime == DateTimeHelper.ZERO_TIME)
        {
            ShowErrorMessage("om.sctivity.selectdatetime", true);
            return;
        }

        // Check if manual creation of activity of this type is allowed
        string activityType = tsType.SelectedValue;
        if (String.IsNullOrEmpty(activityType) || !ActivityTypeInfoProvider.GetActivityTypeEnabled(activityType))
        {
            ShowErrorMessage("om.activity.manualcreationisnotallowed", true);
            return;
        }
    }


    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Get selected site or use current if selector is hidden
        int siteId = 0;
        if (!ShowSiteSelector)
        {
            if (SiteID <= 0)
            {
                siteId = CMSContext.CurrentSiteID;
            }
            else
            {
                siteId = SiteID;
            }
        }
        else
        {
            siteId = ucSite.SiteID;
        }

        if (siteId <= 0)
        {
            // There is no available site
            ShowErrorMessage("siteselect.nodata", true);
            return;
        }

        int contactID = ucContact.ContactID;
        EditForm.Data["ActivityActiveContactID"] = contactID;
        EditForm.Data["ActivityOriginalContactID"] = contactID;
        EditForm.Data["ActivitySiteID"] = siteId;

        EditForm.Data["ActivityCreated"] = dtCreated.SelectedDateTime;
        EditForm.Data["ActivityItemID"] = 0;
        EditForm.Data["ActivityComment"] = txtComment.Text;

        // Store selected campaign (as string)
        string campaign = ValidationHelper.GetString(ucCampaign.Value, null);
        if (ucCampaign.DropDownSingleSelect.SelectedIndex <= 0)
        {
            EditForm.Data.SetValue("ActivityCampaign", DBNull.Value);
        }
        else
        {
            EditForm.Data.SetValue("ActivityCampaign", TextHelper.LimitLength(campaign, 200));
        }
    }


    /// <summary>
    /// Shows error message and sets new stop processing value.
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="stopProcessingValue">Stop processing value</param>
    private void ShowErrorMessage(string message, bool stopProcessingValue)
    {
        ShowError(GetString(message));
        StopProcessing = stopProcessingValue;
    }

    #endregion
}