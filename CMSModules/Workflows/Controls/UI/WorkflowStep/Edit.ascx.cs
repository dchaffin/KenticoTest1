using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.WorkflowEngine;
using CMS.SettingsProvider;
using CMS.FormEngine;
using CMS.WorkflowEngine.Definitions;
using CMS.WorkflowEngine.GraphConfig;
using CMS.ExtendedControls;

public partial class CMSModules_Workflows_Controls_UI_WorkflowStep_Edit : CMSAdminEditControl
{
    #region "Private variables"

    private WorkflowStepInfo mCurrentStepInfo = null;
    private WorkflowInfo mCurrentWorkflow = null;
    private bool? mShowTimeout = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current workflow step ID
    /// </summary>
    public int WorkflowStepID
    {
        get;
        set;
    }


    /// <summary>
    /// Current workflow ID
    /// </summary>
    public int WorkflowID
    {
        get;
        set;
    }


    /// <summary>
    /// Redirect URL that is used after new workflow step is created.
    /// </summary>
    public string AfterCreateRedirectURL
    {
        get;
        set;
    }


    private WorkflowStepInfo CurrentStepInfo
    {
        get
        {
            if (mCurrentStepInfo == null)
            {
                if (WorkflowStepID > 0)
                {
                    mCurrentStepInfo = WorkflowStepInfoProvider.GetWorkflowStepInfo(WorkflowStepID);

                    // Set edited object
                    EditedObject = mCurrentStepInfo;
                }
                else
                {
                    // Create new step
                    mCurrentStepInfo = new WorkflowStepInfo();
                    mCurrentStepInfo.StepType = CMS.SettingsProvider.WorkflowStepTypeEnum.Standard;
                    mCurrentStepInfo.StepRolesSecurity = WorkflowStepSecurityEnum.Default;
                    mCurrentStepInfo.StepUsersSecurity = WorkflowStepSecurityEnum.Default;
                }
            }
            return mCurrentStepInfo;
        }
    }


    private WorkflowInfo CurrentWorkflow
    {
        get
        {
            if (mCurrentWorkflow == null)
            {
                int workflowId = (WorkflowID > 0) ? WorkflowID : CurrentStepInfo.StepWorkflowID;
                mCurrentWorkflow = WorkflowInfoProvider.GetWorkflowInfo(workflowId);
            }

            return mCurrentWorkflow;
        }
    }


    private bool ShowTimeout
    {
        get
        {
            if (mShowTimeout == null)
            {
                // All steps except 
                mShowTimeout = CurrentStepInfo.StepAllowTimeout && !CurrentWorkflow.IsBasic;
            }
            return mShowTimeout.Value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register validate event
        ComponentEvents.RequestEvents.RegisterForComponentEvent<SimpleManagerEventArgs>(ComponentName, ComponentEvents.VALIDATE_DATA, null, (s, args) =>
        {
            args.IsValid = ValidateData();
        });

        // Register save data event
        ComponentEvents.RequestEvents.RegisterForComponentEvent(ComponentName, ComponentEvents.SAVE_DATA, (s, args) => { SaveData(false); });

        // Register save event
        ComponentEvents.RequestEvents.RegisterForComponentEvent(ComponentName, ComponentEvents.SAVE, (s, args) => { Save(false); });
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing!
        }
        else
        {
            RequiredFieldValidatorCodeName.ErrorMessage = GetString("Development-Workflow_New.RequiresCodeName");
            RequiredFieldValidatorDisplayName.ErrorMessage = GetString("Development-Workflow_New.RequiresDisplayName");

            pnlTimeout.Visible = ShowTimeout;
            pnlTimeout.GroupingText = GetString("workflowstep.timeoutsettings");
            pnlGeneral.GroupingText = GetString("general.general");
            if ((WorkflowStepID > 0) && (CurrentStepInfo != null))
            {
                LoadData(CurrentStepInfo);
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (CurrentStepInfo != null)
        {
            // Display timeout target source point selector
            plcTimeoutTarget.Visible = ucTimeout.TimeoutEnabled && ucTimeoutTarget.IsVisible();
        }
    }


    /// <summary>
    /// Loads data of edited workflow from DB into TextBoxes.
    /// </summary>
    protected void LoadData(WorkflowStepInfo wsi)
    {
        // Don't display settings for archived, edit and action step
        plcReject.Visible = !wsi.StepIsArchived && !wsi.StepIsEdit && !wsi.StepIsStart && !wsi.StepIsAction;

        // Change reject label
        if (CurrentWorkflow.IsAutomation)
        {
            lblAllowReject.ResourceString = "WorkflowStep.AllowMoveToPrevious";
            lblAllowReject.RefreshText();
        }

        // Display settings for Standard, DocumentEdit and Start step type only
        plcPublish.Visible = !CurrentWorkflow.IsAutomation && ((wsi.StepType == WorkflowStepTypeEnum.Standard) || wsi.StepIsEdit || wsi.StepIsStart || (wsi.StepType == WorkflowStepTypeEnum.Wait));

        // Timeout UI is always enabled for wait step type
        ucTimeout.AllowNoTimeout = (wsi.StepType != WorkflowStepTypeEnum.Wait);

        // Display action parameters form only for action step type
        if (wsi.StepIsAction)
        {
            WorkflowActionInfo action = WorkflowActionInfoProvider.GetWorkflowActionInfo(wsi.StepActionID);
            if (action != null)
            {
                if (!RequestHelper.IsPostBack())
                {
                    pnlContainer.CssClass += " " + action.ActionName.ToLowerCSafe();
                }
                ucActionParameters.FormInfo = new FormInfo(action.ActionParameters);
            }

            ucActionParameters.BasicForm.AllowMacroEditing = true;
            ucActionParameters.BasicForm.ResolverName = WorkflowHelper.GetResolverName(CurrentWorkflow);
            ucActionParameters.Parameters = wsi.StepActionParameters;
            ucActionParameters.ReloadData(!RequestHelper.IsPostBack());
            ucActionParameters.Visible = ucActionParameters.CheckVisibility();
            pnlParameters.GroupingText = string.Format(GetString("workflowstep.parameters"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(action.ActionDisplayName)));

        }

        pnlParameters.Visible = ucActionParameters.Visible;

        if (plcTimeoutTarget.Visible)
        {
            ucTimeoutTarget.WorkflowStepID = WorkflowStepID;
        }

        // Initialize condition edit for certain step types
        ucSourcePointEdit.StopProcessing = true;

        if (!CurrentWorkflow.IsBasic)
        {
            plcStepType.Visible = true;
            WorkflowNode node = WorkflowNode.GetInstance(wsi.StepType);
            lblWorkflowStepTypeValue.Text = node.Name;

            bool conditionStep = (wsi.StepType == WorkflowStepTypeEnum.Condition);
            if (conditionStep || (wsi.StepType == WorkflowStepTypeEnum.Wait) || (!wsi.StepIsStart && !wsi.StepIsAction && !wsi.StepIsFinished && (wsi.StepType != WorkflowStepTypeEnum.MultichoiceFirstWin)))
            {
                // Initialize source point edit control
                ucSourcePointEdit.WorkflowStepId = WorkflowStepID;
                var sourcePoint = CurrentStepInfo.StepDefinition.DefinitionPoint;
                if (sourcePoint != null)
                {
                    pnlCondition.Visible = true;
                    pnlCondition.GroupingText = conditionStep ? GetString("workflowstep.conditionsettings") : GetString("workflowstep.advancedsettings");

                    ucSourcePointEdit.StopProcessing = false;
                    ucSourcePointEdit.SourcePointGuid = sourcePoint.Guid;
                    ucSourcePointEdit.SimpleMode = !conditionStep;
                    ucSourcePointEdit.ShowCondition = (wsi.StepType != WorkflowStepTypeEnum.Userchoice) && (wsi.StepType != WorkflowStepTypeEnum.Multichoice) && (wsi.StepType != WorkflowStepTypeEnum.MultichoiceFirstWin);
                    ucSourcePointEdit.RuleCategoryNames = CurrentWorkflow.IsAutomation ? ModuleEntry.ONLINEMARKETING : WorkflowObjectType.WORKFLOW;
                }
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            if (ShowTimeout)
            {
                ucTimeout.TimeoutEnabled = wsi.StepDefinition.TimeoutEnabled;
                ucTimeout.ScheduleInterval = wsi.StepDefinition.TimeoutInterval;
            }

            txtWorkflowStepCodeName.Text = wsi.StepName;
            txtWorkflowStepDisplayName.Text = wsi.StepDisplayName;
            chkAllowReject.Checked = wsi.StepAllowReject;
            chkAllowPublish.Checked = wsi.StepAllowPublish;
        }
    }


    /// <summary>
    /// Saves data of edited workflow from TextBoxes into edited object.
    /// </summary>
    /// <param name="validateData">Indicates whether form data should be validated prior to save</param>
    public void SaveData(bool validateData)
    {
        if (!validateData || ValidateData())
        {
            if (WorkflowStepID > 0)
            {
                // Edit existing workflow step
                if (CurrentStepInfo.StepDisplayName != txtWorkflowStepDisplayName.Text)
                {
                    // Refresh header
                    ScriptHelper.RefreshTabHeader(Page, GetString("general.general"));
                }

                if (validateData)
                {
                    ucSourcePointEdit.SaveData(false);
                }

                ucActionParameters.SaveData(false);

                SetFormValues(CurrentStepInfo);
            }
            else if (WorkflowID > 0)
            {
                // Create new workflow step
                int workflowStepId = SaveNewWorkflowStep();
                if (workflowStepId > 0)
                {
                    if (String.IsNullOrEmpty(AfterCreateRedirectURL))
                    {
                        ShowChangesSaved();
                    }
                    else
                    {
                        URLHelper.Redirect(URLHelper.AppendQuery(AfterCreateRedirectURL, "?objectid=" + workflowStepId + "&saved=1"));
                    }
                }
            }
        }
    }


    /// <summary>
    /// Validates the data, returns true if succeeded.
    /// </summary>
    public bool ValidateData()
    {
        // Finds whether required fields are not empty
        string result = new Validator().NotEmpty(txtWorkflowStepDisplayName.Text, GetString("Development-Workflow_New.RequiresDisplayName")).NotEmpty(txtWorkflowStepCodeName.Text, GetString("Development-Workflow_New.RequiresCodeName"))
            .IsCodeName(txtWorkflowStepCodeName.Text, GetString("general.invalidcodename"))
            .Result;
        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
            return false;
        }

        // Validate code name uniqueness
        WorkflowStepInfo wsi = WorkflowStepInfoProvider.GetWorkflowStepInfo(txtWorkflowStepCodeName.Text, ((WorkflowID > 0) ? WorkflowID : CurrentStepInfo.StepWorkflowID));
        if ((wsi != null) && (wsi.StepID != CurrentStepInfo.StepID))
        {
            ShowError(GetString("Development-Workflow_Step_New.WorkflowStepExists"));
            return false;
        }

        // Validate source point control
        if (!ucSourcePointEdit.ValidateData())
        {
            return false;
        }

        // Validate action properties control
        if (CurrentStepInfo.StepIsAction && !ucActionParameters.ValidateData())
        {
            return false;
        }

        if ((WorkflowStepID > 0) && (CurrentStepInfo == null))
        {
            ShowError(GetString("Development-Workflow_Step_New.WorkflowExists"));
            return false;
        }


        if (ucTimeout.Visible && String.IsNullOrEmpty(ucTimeout.ScheduleInterval) && ucTimeout.TimeoutEnabled)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Saves edited object to database.
    /// </summary>
    /// <param name="validateData">Indicates whether form data should be validated prior to save</param>
    public void Save(bool validateData)
    {
        if (!validateData || ValidateData())
        {
            if (validateData)
            {
                SaveData(false);
            }

            WorkflowStepInfoProvider.SetWorkflowStepInfo(CurrentStepInfo);
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Saves new workflow step's data into DB.
    /// </summary>
    /// <returns>Returns ID of created workflow step</returns>
    protected int SaveNewWorkflowStep()
    {
        SetFormValues(CurrentStepInfo);

        // Ensure correct order for basic workflow
        if (CurrentWorkflow.IsBasic)
        {
            // Get published step info for the proper position
            WorkflowStepInfo psi = WorkflowStepInfoProvider.GetPublishedStep(WorkflowID);
            if (psi != null)
            {
                CurrentStepInfo.StepOrder = psi.StepOrder;
                // Move the published step down
                psi.StepOrder += 1;
                WorkflowStepInfoProvider.SetWorkflowStepInfo(psi);

                // Move the archived step down
                WorkflowStepInfo asi = WorkflowStepInfoProvider.GetArchivedStep(WorkflowID);
                if (asi != null)
                {
                    asi.StepOrder += 1;
                    WorkflowStepInfoProvider.SetWorkflowStepInfo(asi);
                }
            }
        }

        CurrentStepInfo.StepWorkflowID = WorkflowID;
        WorkflowStepInfoProvider.SetWorkflowStepInfo(CurrentStepInfo);
        return CurrentStepInfo.StepID;
    }


    /// <summary>
    /// Sets values from edit form to edited workflows step info
    /// </summary>
    /// <param name="wsi">Edited workflow step info</param>
    private void SetFormValues(WorkflowStepInfo wsi)
    {
        if (wsi != null)
        {
            wsi.StepDisplayName = txtWorkflowStepDisplayName.Text;
            wsi.StepName = txtWorkflowStepCodeName.Text;
            wsi.StepAllowReject = chkAllowReject.Checked;
            wsi.StepAllowPublish = chkAllowPublish.Checked;

            if (ShowTimeout)
            {
                Step definition = wsi.StepDefinition;
                definition.TimeoutEnabled = ucTimeout.TimeoutEnabled;
                definition.TimeoutInterval = ucTimeout.ScheduleInterval;
                if (ucTimeoutTarget.Visible)
                {
                    Guid timeouTarget = ucTimeoutTarget.SourcePointGuid;
                    // Add timeout source point
                    if (wsi.StepAllowDefaultTimeoutTarget && !definition.SourcePoints.Exists(s => (s is TimeoutSourcePoint)))
                    {
                        TimeoutSourcePoint timeout = new TimeoutSourcePoint();
                        // Timeout source point is selected
                        if (!definition.SourcePoints.Exists(s => (s.Guid == timeouTarget)))
                        {
                            timeout.Guid = timeouTarget;
                        }

                        definition.SourcePoints.Add(timeout);
                    }
                    definition.TimeoutTarget = timeouTarget;
                }
                else
                {
                    // Remove timeout source point
                    var timeoutPoints = definition.SourcePoints.FindAll(s => (s is TimeoutSourcePoint));
                    foreach (var t in timeoutPoints)
                    {
                        string result = wsi.RemoveSourcePoint(t.Guid);
                        if (result != null)
                        {
                            ShowError(result);
                        }
                    }
                }
            }

            if (wsi.StepIsAction)
            {
                wsi.StepActionParameters.LoadData(ucActionParameters.Parameters.GetData());
            }
        }
    }

    #endregion
}