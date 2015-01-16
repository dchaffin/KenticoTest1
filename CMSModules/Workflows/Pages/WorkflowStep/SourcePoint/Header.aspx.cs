using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Definitions;
using CMS.UIControls;

public partial class CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_Header : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int workflowStepId = QueryHelper.GetInteger("workflowstepid", 0);
        Guid sourcePointGuid = QueryHelper.GetGuid("sourcepointguid", Guid.Empty);

        WorkflowStepInfo step = WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowStepId);
        if ((step != null) && (step.StepDefinition != null))
        {
            SourcePoint sourcePoint = step.StepDefinition.SourcePoints.FirstOrDefault(i => i.Guid == sourcePointGuid);
            if (sourcePoint != null)
            {
                if (!RequestHelper.IsPostBack())
                {
                    InitalizeMenu(sourcePoint, workflowStepId);
                }

                InitializeMasterPage(workflowStepId, sourcePoint.Label);
            }

        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes master page breadcrumbs
    /// </summary>
    /// <param name="workflowStepId">Parent workflow step ID</param>
    /// <param name="sourcePointDisplayName">Display name of current source point</param>
    private void InitializeMasterPage(int workflowStepId, string displayName)
    {
        // Set master page title
        CurrentMaster.Title.HelpTopicName = CurrentWorkflow.IsAutomation ? "process_step_case" : "workflow_step_case";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Set master page breadcrumbs element
        string[,] breadCrumbs = new string[2, 3];
        breadCrumbs[0, 0] = GetString("development-workflow_edit.sourcepoints");
        breadCrumbs[0, 1] = "~/CMSModules/Workflows/Pages/WorkflowStep/SourcePoint/List.aspx?workflowStepId=" + workflowStepId;
        breadCrumbs[0, 2] = "_parent";

        breadCrumbs[1, 0] = displayName;
        breadCrumbs[1, 1] = String.Empty;
        breadCrumbs[1, 2] = String.Empty;

        CurrentMaster.Title.Breadcrumbs = breadCrumbs;
    }


    /// <summary>
    /// Initializes workflow step edit menu.
    /// </summary>
    protected void InitalizeMenu(SourcePoint sourcePoint, int workflowStepId)
    {
        string[,] tabs = new string[2, 4];
        tabs[0, 0] = GetString("general.general");
        tabs[0, 1] = String.Format("SetHelpTopic('helpTopic', '{0}');", CurrentWorkflow.IsAutomation ? "process_step_case" : "workflow_step_case");
        tabs[0, 2] = String.Format("General.aspx?workflowStepId={0}&sourcepointguid={1}", workflowStepId, sourcePoint.Guid);

        if (sourcePoint.Type != SourcePointTypeEnum.Timeout)
        {
            tabs[1, 0] = GetString("general.security");
            tabs[1, 1] = String.Format("SetHelpTopic('helpTopic', '{0}');", CurrentWorkflow.IsAutomation ? "process_step_case_security" : "workflow_step_case_security");
            tabs[1, 2] = String.Format("Security.aspx?workflowStepId={0}&sourcepointguid={1}", workflowStepId, sourcePoint.Guid);
        }
        CurrentMaster.Tabs.UrlTarget = "wfSourcePointContent";
        CurrentMaster.Tabs.Tabs = tabs;
    }

    #endregion
}