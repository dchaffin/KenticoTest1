using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_Controls_Workflow : CMSUserControl
{
    #region "Private variables"

    // Current Node
    private TreeNode mNode = null;
    private TreeProvider mTree = null;
    private WorkflowManager mWorkflowManager = null;
    private int mNodeId = -1;
    private int currentStepId = 0;
    private WorkflowInfo mWorkflowInfo = null;

    private UserInfo currentUserInfo = null;
    private SiteInfo currentSiteInfo = null;
    private readonly bool displaySplitMode = CMSContext.DisplaySplitMode;

    #endregion


    #region "Properties"

    /// <summary>
    /// Tree provider
    /// </summary>
    public TreeProvider Tree
    {
        get
        {
            return mTree ?? (mTree = new TreeProvider(CMSContext.CurrentUser));
        }
        set
        {
            mTree = value;
        }
    }


    /// <summary>
    /// Workflow manager
    /// </summary>
    private WorkflowManager WorkflowManager
    {
        get
        {
            return mWorkflowManager ?? (mWorkflowManager = WorkflowManager.GetInstance(Tree));
        }
    }


    /// <summary>
    /// Tree node.
    /// </summary>
    public TreeNode Node
    {
        get
        {
            return mNode ?? (mNode = DocumentHelper.GetDocument(NodeID, CMSContext.PreferredCultureCode, Tree));
        }
        set
        {
            mNode = value;

            mNodeId = -1;
            if (value != null)
            {
                mNodeId = value.NodeID;
            }
        }
    }


    /// <summary>
    /// Determines whether document is checked out
    /// </summary>
    private bool DocumentIsCheckedOut
    {
        get
        {
            return (Node != null) && Node.IsCheckedOut;
        }
    }


    /// <summary>
    /// Identifier of current node
    /// </summary>
    private int NodeID
    {
        get
        {
            return mNodeId;
        }
        set
        {
            mNodeId = value;

            mNode = null;
        }
    }


    /// <summary>
    /// Workflow info object
    /// </summary>
    private WorkflowInfo WorkflowInfo
    {
        get
        {
            if (mWorkflowInfo == null)
            {
                if (Node != null)
                {
                    mWorkflowInfo = Node.GetWorkflow();
                }
            }

            return mWorkflowInfo;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        gridHistory.GridName = "~/CMSModules/Content/CMSDesk/Properties/WorkflowHistory.xml";
        gridSteps.GridName = "~/CMSModules/Content/CMSDesk/Properties/WorkflowSteps.xml";

        gridSteps.WhereCondition = "StepWorkflowID = @StepWorkflowID";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Turn sorting off
        gridSteps.GridView.AllowSorting = false;
        ReloadData();

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


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// External step binding.
    /// </summary>
    protected object gridSteps_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "currentstepinfo":
                DataRowView data = (DataRowView)parameter;
                if (currentStepId <= 0)
                {
                    WorkflowStepTypeEnum stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(data["StepType"], 1);
                    if (stepType == WorkflowStepTypeEnum.DocumentEdit)
                    {
                        return "<img src=\"" + GetImageUrl("CMSModules/CMS_Content/Properties/currentstep.png") + "\" alt=\"\" />";
                    }
                }
                else
                {
                    // Check if version history exists and node is published
                    if (Node.IsPublished && (Node.DocumentCheckedOutVersionHistoryID <= 0))
                    {
                        WorkflowStepTypeEnum stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(data["StepType"], 1);
                        if (stepType == WorkflowStepTypeEnum.DocumentPublished)
                        {
                            return "<img src=\"" + GetImageUrl("CMSModules/CMS_Content/Properties/currentstep.png") + "\" alt=\"\" />";
                        }
                    }
                    else
                    {
                        int stepId = ValidationHelper.GetInteger(data["StepID"], 0);
                        if (stepId == currentStepId)
                        {
                            return "<img src=\"" + GetImageUrl("CMSModules/CMS_Content/Properties/currentstep.png") + "\" alt=\"\" />";
                        }
                    }
                }
                return string.Empty;

            case "steporder":
                if (sender != null)
                {
                    // Get grid row
                    GridViewRow row = (GridViewRow)((DataControlFieldCell)sender).Parent;
                    int pageOffset = (gridSteps.Pager.CurrentPage - 1) * gridSteps.Pager.CurrentPageSize;
                    // Return row index
                    return (pageOffset + row.RowIndex + 1).ToString();
                }
                return string.Empty;
        }
        return parameter;
    }


    /// <summary>
    /// External history binding.
    /// </summary>
    protected object gridHistory_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv = null;
        switch (sourceName.ToLowerCSafe())
        {
            case "action":
                drv = (DataRowView)parameter;
                bool wasrejected = ValidationHelper.GetBoolean(drv["WasRejected"], false);

                // Get type of the steps
                WorkflowStepTypeEnum stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "StepType"), 0);
                WorkflowStepTypeEnum targetStepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "TargetStepType"), (int)stepType);
                WorkflowTransitionTypeEnum transitionType = (WorkflowTransitionTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "HistoryTransitionType"), 0);

                // Get name of steps
                string stepName = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "StepName"), String.Empty);
                string targetStepName = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "TargetStepName"), stepName);
                if (!wasrejected)
                {
                    // If step type defined, use it for identification
                    if (targetStepType != WorkflowStepTypeEnum.Undefined)
                    {
                        bool isAutomatic = (transitionType == WorkflowTransitionTypeEnum.Automatic);
                        string actionString = isAutomatic ? GetString("WorfklowProperties.Automatic") + " ({0})" : "{0}";
                        // Return correct step title
                        switch (targetStepType)
                        {
                            case WorkflowStepTypeEnum.DocumentArchived:
                                actionString = string.Format(actionString, GetString("WorfklowProperties.Archived"));
                                break;

                            case WorkflowStepTypeEnum.DocumentPublished:
                                actionString = string.Format(actionString, GetString("WorfklowProperties.Published"));
                                break;

                            case WorkflowStepTypeEnum.DocumentEdit:
                                actionString = GetString("WorfklowProperties.NewVersion");
                                break;

                            default:
                                if(stepType == WorkflowStepTypeEnum.DocumentEdit)
                                {
                                    actionString = GetString("WorfklowProperties.NewVersion");
                                }
                                else
                                {
                                    actionString = isAutomatic ? GetString("WorfklowProperties.Automatic") : GetString("WorfklowProperties.Approved");
                                }
                                break;
                        }

                        return actionString;
                    }
                    // Backward compatibility
                    else
                    {
                        // Return correct step title
                        switch (targetStepName.ToLowerCSafe())
                        {
                            case "archived":
                                return GetString("WorfklowProperties.Archived");

                            case "published":
                                return GetString("WorfklowProperties.Published");

                            case "edit":
                                return GetString("WorfklowProperties.NewVersion");

                            default:
                                if (CMSString.Equals(stepName, "edit", true))
                                {
                                    return GetString("WorfklowProperties.NewVersion");
                                }
                                return GetString("WorfklowProperties.Approved");
                        }
                    }
                }
                else
                {
                    return GetString("WorfklowProperties.Rejected");
                }

            // Get approved time
            case "approvedwhen":
            case "approvedwhentooltip":
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    return string.Empty;
                }
                else
                {
                    if (currentUserInfo == null)
                    {
                        currentUserInfo = CMSContext.CurrentUser;
                    }

                    if (currentSiteInfo == null)
                    {
                        currentSiteInfo = CMSContext.CurrentSite;
                    }

                    // Apply time zone information
                    bool displayGMT = (sourceName == "approvedwhentooltip");
                    DateTime time = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                    return TimeZoneHelper.ConvertToUserTimeZone(time, displayGMT, currentUserInfo, currentSiteInfo);
                }

            case "stepname":
                drv = (DataRowView)parameter;
                string step = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "StepDisplayName"), String.Empty);
                string targetStep = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "TargetStepDisplayName"), String.Empty);
                if (!string.IsNullOrEmpty(targetStep))
                {
                    step += " -> " + targetStep;
                }
                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(step));
        }
        return parameter;
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Reloads the page data.
    /// </summary>
    protected void ReloadData()
    {
        string where = string.Format("StepType IN ({0}, {1}, {2})", (int)WorkflowStepTypeEnum.DocumentEdit, (int)WorkflowStepTypeEnum.DocumentPublished, (int)WorkflowStepTypeEnum.DocumentArchived);
        // Hide custom steps if license doesn't allow them or check automatically publish changes
        if (!WorkflowInfoProvider.IsCustomStepAllowed())
        {
            gridSteps.WhereCondition = SqlHelperClass.AddWhereCondition(gridSteps.WhereCondition, where);
        }
        // Hide custom steps (without actual step) if functionality 'Automatically publish changes' is allowed
        else if ((WorkflowInfo != null) && WorkflowInfo.WorkflowAutoPublishChanges)
        {
            gridSteps.WhereCondition = SqlHelperClass.AddWhereCondition(gridSteps.WhereCondition, where);
            // Get current step info
            WorkflowStepInfo currentStep = WorkflowManager.GetStepInfo(Node);

            if (currentStep != null)
            {
                if (!currentStep.StepIsDefault)
                {
                    gridSteps.WhereCondition = SqlHelperClass.AddWhereCondition(gridSteps.WhereCondition, "(StepName = '" + SqlHelperClass.GetSafeQueryString(currentStep.StepName, false) + "')", "OR");
                }
            }
        }

        // Do not display steps without order - advanced workflow steps
        gridSteps.WhereCondition = SqlHelperClass.AddWhereCondition(gridSteps.WhereCondition, "StepOrder IS NOT NULL");


        // Prepare the query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@DocumentID", 0);

        // Prepare the steps query parameters
        QueryDataParameters stepsParameters = new QueryDataParameters();
        stepsParameters.Add("@StepWorkflowID", 0);

        if (Node != null)
        {
            // Check read permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), Node.NodeAliasPath));
            }

            // Prepare parameters
            parameters[0].Value = Node.DocumentID;
            currentStepId = ValidationHelper.GetInteger(Node.GetValue("DocumentWorkflowStepID"), 0);

            if (WorkflowInfo != null)
            {
                plcBasic.Visible = WorkflowInfo.IsBasic;
                plcAdvanced.Visible = !plcBasic.Visible;
                if (plcAdvanced.Visible)
                {
                    ucDesigner.WorkflowID = WorkflowInfo.WorkflowID;
                    ucDesigner.SelectedStepID = currentStepId;
                    ucDesigner.WorkflowType = WorkflowTypeEnum.Approval;
                }
                else
                {
                    stepsParameters[0].Value = WorkflowInfo.WorkflowID;
                }

                // Initialize grids
                gridHistory.OnExternalDataBound += gridHistory_OnExternalDataBound;
                gridSteps.OnExternalDataBound += gridSteps_OnExternalDataBound;
                gridHistory.ZeroRowsText = GetString("workflowproperties.nohistoryyet");
            }
        }
        else
        {
            pnlWorkflow.Visible = false;
        }

        // Initialize query parameters of grids
        gridSteps.QueryParameters = stepsParameters;
        gridHistory.QueryParameters = parameters;

        SetupForm();
        gridHistory.ReloadData();
        if (plcBasic.Visible)
        {
            gridSteps.ReloadData();
        }
    }


    private void ShowInfo(string message, bool persistent)
    {
        if (IsLiveSite)
        {
            ShowInformation(message, null, persistent);
        }
        else
        {
            DocumentManager.DocumentInfo = message;
        }
    }


    /// <summary>
    /// Reloads the form status.
    /// </summary>
    protected void SetupForm()
    {
        // Check modify permissions
        if ((CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied) &&
            !CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Content", "ManageWorkflow"))
        {
            ShowInfo(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.NodeAliasPath), true);
        }
        else
        {
            bool autoPublishChanges = false;
            // Check if auto publish changes is allowed
            if (WorkflowInfo != null)
            {
                autoPublishChanges = WorkflowInfo.WorkflowAutoPublishChanges;
            }

            if ((WorkflowInfo != null) || (currentStepId > 0))
            {
                // Setup the form
                WorkflowStepInfo stepInfo = null;
                if (Node.IsPublished && (Node.DocumentCheckedOutVersionHistoryID <= 0))
                {
                    stepInfo = WorkflowStepInfoProvider.GetPublishedStep(WorkflowInfo.WorkflowID);
                }
                else
                {
                    stepInfo = WorkflowStepInfoProvider.GetWorkflowStepInfo(currentStepId) ?? WorkflowStepInfoProvider.GetFirstStep(WorkflowInfo.WorkflowID);
                }

                if (stepInfo != null)
                {
                    currentStepId = stepInfo.StepID;
                    if (plcAdvanced.Visible)
                    {
                        ucDesigner.SelectedStepID = currentStepId;
                    }
                }
            }
        }

        // Check if document isn't checked out
        if (DocumentIsCheckedOut)
        {
            ShowInfo(GetString("WorfklowProperties.DocumentIsCheckedOut"), true);
        }

        // If no workflow scope set for node, hide the data  
        if (WorkflowInfo == null)
        {
            lblWorkflow.Text = GetString("properties.scopenotset");
            lblWorkflow.Visible = true;
            pnlWorkflow.Visible = false;
        }
    }

    #endregion
}
