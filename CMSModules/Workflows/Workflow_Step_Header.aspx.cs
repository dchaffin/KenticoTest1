using System;
using System.Collections.Generic;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.Controls.Configuration;

public partial class CMSModules_Workflows_Workflow_Step_Header : CMSWorkflowPage
{
    #region "Variables"

    protected int mWorkflowStepId = 0;
    protected WorkflowStepInfo mCurrentWorkflowStep = null;
    private bool isDialog = false;
    private int mSelectedTab = -1;

    #endregion


    #region "Properties"

    private int WorkflowStepID
    {
        get
        {
            if (mWorkflowStepId == 0)
            {
                mWorkflowStepId = QueryHelper.GetInteger("workflowstepid", 0);
            }
            return mWorkflowStepId;
        }
    }


    private int SelectedTab
    {
        get
        {
            if (mSelectedTab == -1)
            {
                mSelectedTab = QueryHelper.GetInteger("selectedtab", 0);
            }
            return mSelectedTab;
        }
    }


    private WorkflowStepInfo CurrentWorkflowStep
    {
        get
        {
            if (mCurrentWorkflowStep == null)
            {
                mCurrentWorkflowStep = WorkflowStepInfoProvider.GetWorkflowStepInfo(WorkflowStepID);
            }
            return mCurrentWorkflowStep;
        }
    }


    private bool ShowSecurityTab
    {
        get
        {
            return !CurrentWorkflowStep.StepIsAction && !CurrentWorkflowStep.StepIsFinished;
        }
    }


    private bool ShowEmailsTab
    {
        get
        {
            return !CurrentWorkflowStep.StepIsAction && !CurrentWorkflow.IsAutomation;
        }
    }

    #endregion


    /// <summary>
    /// PreInit event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        isDialog = QueryHelper.GetBoolean("dialog", false);

        // Change master page for dialog
        if (isDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string stepDisplayName = String.Empty;
        int workflowId = 0;

        ScriptHelper.RegisterWOpenerScript(Page);

        // Set edited object
        EditedObject = CurrentWorkflowStep;

        if (CurrentWorkflowStep != null)
        {
            stepDisplayName = ResHelper.LocalizeString(CurrentWorkflowStep.StepDisplayName);
            workflowId = CurrentWorkflowStep.StepWorkflowID;
        }

        InitalizeMenu();

        string workflowStepsUrl = "~/CMSModules/Workflows/Workflow_Steps.aspx?workflowid=" + workflowId + "&showtab=steps";
        InitializeMasterPage(GetString("Publish.WorkflowSteps"), workflowStepsUrl, stepDisplayName);
    }


    #region "Methods"

    /// <summary>
    /// Initialize page title.
    /// </summary>
    private void InitializeMasterPage(string workflowSteps, string workflowStepsUrl, string currentWorkflowStep)
    {
        string[,] breadcrumbs = null;
        if (isDialog)
        {
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_WorkflowStep/object.png");
            CurrentMaster.Title.TitleText = ResHelper.GetString(CurrentWorkflow.IsAutomation ? "ma.step.properties" : "workflowstep.properties");

            breadcrumbs = new string[2, 3];
            breadcrumbs[0, 0] = GetString("Development-Workflow_Edit.Steps");
            breadcrumbs[0, 1] = "";
            breadcrumbs[0, 2] = "";

            breadcrumbs[1, 0] = HTMLHelper.HTMLEncode(currentWorkflowStep);
            breadcrumbs[1, 1] = "";
            breadcrumbs[1, 2] = "";
        }
        else
        {
            breadcrumbs = new string[2, 3];
            breadcrumbs[0, 0] = workflowSteps;
            breadcrumbs[0, 1] = workflowStepsUrl;
            breadcrumbs[0, 2] = "_parent";

            breadcrumbs[1, 0] = HTMLHelper.HTMLEncode(currentWorkflowStep);
            breadcrumbs[1, 1] = string.Empty;
            breadcrumbs[1, 2] = string.Empty;
        }
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;

        // Set master page title
        CurrentMaster.Title.HelpTopicName = CurrentWorkflow.IsAutomation ? "process_step" : "workflow_step";
        CurrentMaster.Title.HelpName = "helpTopic";
    }


    /// <summary>
    /// Initializes workflow step edit menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        UITabs tabs = CurrentMaster.Tabs;
        if (CurrentWorkflowStep != null)
        {
            TabItem general = new TabItem()
            {
                Text = GetString("general.general"),
                OnClientClick = string.Format("SetHelpTopic('helpTopic', '{0}');", CurrentWorkflow.IsAutomation ? "process_step" : "workflow_step"),
                RedirectUrl = "Workflow_Step_General.aspx?workflowStepId=" + WorkflowStepID
            };
            tabs.TabItems.Add(general);

            if (!CurrentWorkflowStep.StepIsDefault)
            {
                if (CurrentWorkflowStep.StepAllowBranch)
                {
                    TabItem cases = new TabItem()
                    {
                        Text = GetString("development-workflow_edit.sourcepoints"),
                        OnClientClick = string.Format("SetHelpTopic('helpTopic', '{0}');", CurrentWorkflow.IsAutomation ? "process_step_cases" : "workflow_step_cases"),
                        RedirectUrl = "Pages/WorkflowStep/SourcePoint/List.aspx?workflowStepId=" + WorkflowStepID
                    };
                    tabs.TabItems.Add(cases);
                }

                tabs.SelectedTab = SelectedTab;

                if (ShowSecurityTab)
                {
                    TabItem security = new TabItem()
                    {
                        Text = GetString("general.security"),
                        OnClientClick = string.Format("SetHelpTopic('helpTopic', '{0}');", CurrentWorkflow.IsAutomation ? "process_step_security" : "workflow_step_security"),
                        RedirectUrl = "Workflow_Step_Security.aspx?workflowStepId=" + WorkflowStepID
                    };
                    tabs.TabItems.Add(security);
                }

                if (ShowEmailsTab)
                {
                    TabItem emails = new TabItem()
                    {
                        Text = GetString("Development-Workflow_Edit.Emails"),
                        OnClientClick = "SetHelpTopic('helpTopic', 'workflow_step_emails');",
                        RedirectUrl = "Workflow_Step_Emails.aspx?workflowStepId=" + WorkflowStepID
                    };
                    tabs.TabItems.Add(emails);
                }
            }

            tabs.UrlTarget = "wfStepContent";
        }
    }

    #endregion
}