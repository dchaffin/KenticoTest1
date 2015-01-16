using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.WorkflowEngine;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_VersionList : VersionHistoryControl
{
    #region "Variables"

    private Label mInfoLabel = null;
    private Label mErrorLabel = null;
    private bool mDisplaySecurityMessage = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets information label.
    /// </summary>
    public Label InfoLabel
    {
        get
        {
            return mInfoLabel ?? (mInfoLabel = MessagesPlaceHolder.InfoLabel);
        }
        set
        {
            mInfoLabel = value;
        }
    }


    /// <summary>
    /// Gets or sets error label.
    /// </summary>
    public Label ErrorLabel
    {
        get
        {
            return mErrorLabel ?? (mErrorLabel = MessagesPlaceHolder.ErrorLabel);
        }
        set
        {
            mErrorLabel = value;
        }
    }


    /// <summary>
    /// Indicates whether to display security message.
    /// </summary>
    public bool DisplaySecurityMessage
    {
        get
        {
            return mDisplaySecurityMessage;
        }
        set
        {
            mDisplaySecurityMessage = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            gridHistory.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override CMS.ExtendedControls.MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Delegates and events"

    public event EventHandler AfterDestroyHistory = null;

    #endregion


    #region "Page events and methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            gridHistory.StopProcessing = true;
        }
        else
        {
            SetupControl();
        }
    }


    public void SetupControl()
    {
        gridHistory.ZeroRowsText = GetString("workflowproperties.documenthasnohistory");
        gridHistory.IsLiveSite = IsLiveSite;
        if (Node != null)
        {
            // Prepare the query parameters
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@DocumentID", Node.DocumentID);

            gridHistory.QueryParameters = parameters;

            ScriptHelper.RegisterStartupScript(this, typeof(string), "confirmDestroyMessage", ScriptHelper.GetScript("var varConfirmDestroy='" + ResHelper.GetString("VersionProperties.ConfirmDestroy") + "'; \n"));

            gridHistory.GridName = "~/CMSModules/Content/Controls/VersionHistory.xml";
            gridHistory.OnExternalDataBound += gridHistory_OnExternalDataBound;
            gridHistory.OnAction += gridHistory_OnAction;

            string viewVersionUrl = null;
            if (IsLiveSite)
            {
                viewVersionUrl = CMSContext.ResolveDialogUrl("~/CMSModules/Content/CMSPages/Versions/ViewVersion.aspx");
            }
            else
            {
                viewVersionUrl = ResolveUrl("~/CMSModules/Content/CMSDesk/Properties/ViewVersion.aspx");
            }

            string viewVersionScript = ScriptHelper.GetScript("function ViewVersion(versionHistoryId) {window.open('" + viewVersionUrl + "?versionHistoryId=' + versionHistoryId)}");
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "viewVersionScript", viewVersionScript);
        }
        else
        {
            gridHistory.GridName = string.Empty;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Node != null)
        {
            btnDestroy.Visible = !gridHistory.IsEmpty;
            btnDestroy.Enabled = (CanDestroy && (!CheckedOutByAnotherUser || CanCheckIn));
            if (DisplaySecurityMessage && !CanModify)
            {
                ShowInformation(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.NodeAliasPath));
            }
            plcLabels.Visible = !(string.IsNullOrEmpty(MessagesPlaceHolder.ErrorLabel.Text) && string.IsNullOrEmpty(MessagesPlaceHolder.InfoLabel.Text));
        }
    }


    /// <summary>
    /// Reloads data in unigrid.
    /// </summary>
    public void ReloadData()
    {
        gridHistory.ReloadData();
    }

    #endregion


    #region "Grid events"

    protected object gridHistory_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();
        switch (sourceName)
        {
            case "rollback":
                ImageButton imgRollback = ((ImageButton)sender);
                if (!WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve) || !CanModify || (CheckedOutByAnotherUser && !CanCheckIn))
                {
                    imgRollback.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/undodisabled.png");
                    imgRollback.Enabled = false;
                }
                break;

            case "allowdestroy":
                ImageButton imgDestroy = ((ImageButton)sender);
                if (!CanDestroy || (CheckedOutByAnotherUser && !CanCheckIn))
                {
                    imgDestroy.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/deletedisabled.png");
                    imgDestroy.Enabled = false;
                }
                break;

            case "tobepublished":
                return ValidationHelper.GetBoolean(parameter, false) ? "<span class=\"StatusEnabled\">" + GetString("general.yes") + "</span>" : string.Empty;

            case "publishfrom":
            case "publishto":
            case "waspublishedfrom":
            case "waspublishedto":

            case "publishfromtooltip":
            case "publishtotooltip":
            case "waspublishedfromtooltip":
            case "waspublishedtotooltip":
                return TimeZoneHelper.ConvertToUserTimeZone(ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME), sourceName.EndsWithCSafe("tooltip"), CurrentUser, CurrentSiteInfo);

            case "modifiedwhenby":
            case "modifiedwhenbytooltip":
                DataRowView data = (DataRowView)parameter;
                DateTime modifiedwhen = ValidationHelper.GetDateTime(data["ModifiedWhen"], DateTimeHelper.ZERO_TIME);
                if (sourceName == "modifiedwhenbytooltip")
                {
                    return TimeZoneHelper.ConvertToUserTimeZone(modifiedwhen, true, CurrentUser, CurrentSiteInfo);
                }
                else
                {
                    string userName = ValidationHelper.GetString(data["UserName"], String.Empty);
                    string fullName = ValidationHelper.GetString(data["FullName"], String.Empty);

                    return TimeZoneHelper.ConvertToUserTimeZone(modifiedwhen, false, CurrentUser, CurrentSiteInfo) + "<br />" + HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, fullName));
                }
        }

        return parameter;
    }


    protected void gridHistory_OnAction(string actionName, object actionArgument)
    {
        int versionHistoryId = ValidationHelper.GetInteger(actionArgument, 0);
        switch (actionName.ToLowerCSafe())
        {
            case "rollback":
                if (versionHistoryId > 0)
                {
                    if (CheckedOutByUserID > 0)
                    {
                        // Document is checked out
                        ShowError(GetString("VersionProperties.CannotRollbackCheckedOut"));
                    }
                    else
                    {
                        // Check permissions
                        if (!WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve) || !CanModify)
                        {
                            ShowError(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.NodeAliasPath));
                        }
                        else
                        {
                            try
                            {
                                VersionManager.RollbackVersion(versionHistoryId);

                                if (!IsLiveSite)
                                {
                                    // Refresh content tree (for the case that document name has been changed)
                                    string refreshTreeScript = ScriptHelper.GetScript("if(window.RefreshTree!=null){RefreshTree(" + Node.NodeParentID + ", " + Node.NodeID + ");}");
                                    ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshTree" + ClientID, refreshTreeScript);
                                }

                                ShowConfirmation(GetString("VersionProperties.RollbackOK"));
                            }
                            catch (Exception ex)
                            {
                                ShowError(ex.Message);
                            }
                        }
                    }
                }
                break;

            case "destroy":
                if (versionHistoryId > 0)
                {
                    if (Node != null)
                    {
                        // Check permissions
                        if (!CanDestroy || (CheckedOutByAnotherUser && !CanCheckIn))
                        {
                            ShowError(GetString("History.ErrorNotAllowedToDestroy"));
                        }
                        else
                        {
                            VersionManager.DestroyDocumentVersion(versionHistoryId);
                            ShowConfirmation(GetString("VersionProperties.DestroyOK"));
                        }
                    }
                }
                break;
        }
    }

    #endregion


    #region "Button handling"

    protected void btnDestroy_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check permissions
            if (!CanDestroy || (CheckedOutByAnotherUser && !CanCheckIn))
            {
                ShowError(GetString("History.ErrorNotAllowedToDestroy"));
                return;
            }
            VersionManager.ClearDocumentHistory(Node.DocumentID);
            ShowConfirmation(GetString("VersionProperties.VersionsCleared"));

            EventLogProvider ev = new EventLogProvider();
            ev.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "Content", "DESTROYHISTORY", TreeProvider.UserInfo.UserID, TreeProvider.UserInfo.UserName, Node.NodeID, Node.GetDocumentName(), HTTPHelper.UserHostAddress, string.Format(ResHelper.GetAPIString("contentedit.documenthistorydestroyed", "Document history of document '{0}' has been destroyed."), HTMLHelper.HTMLEncode(Node.NodeAliasPath)), Node.NodeSiteID, HTTPHelper.GetAbsoluteUri());

            InvalidateNode();
            ReloadData();
            if (AfterDestroyHistory != null)
            {
                AfterDestroyHistory(sender, e);
            }
        }
    }

    #endregion
}