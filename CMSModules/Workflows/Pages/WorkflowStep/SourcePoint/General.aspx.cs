using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;

[Actions(1)]
[SaveAction(0)]
public partial class CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_General : CMSWorkflowPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register save event
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, (s, args) => { editElem.Save(false); });
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int workflowStepId = QueryHelper.GetInteger("workflowstepid", 0);
        Guid sourcePointGuid = QueryHelper.GetGuid("sourcepointguid", Guid.Empty);

        if (workflowStepId > 0)
        {
            editElem.WorkflowStepId = workflowStepId;
            editElem.SourcePointGuid = sourcePointGuid;
            editElem.RuleCategoryNames = CurrentWorkflow.IsAutomation ? ModuleEntry.ONLINEMARKETING : WorkflowObjectType.WORKFLOW;
            if (sourcePointGuid == Guid.Empty)
            {
                // Set master page title
                editElem.AfterCreateRedirectURL = "~/CMSModules/Workflows/Pages/WorkflowStep/SourcePoint/Frameset.aspx";
                CurrentMaster.Title.HelpTopicName = CurrentWorkflow.IsAutomation ? "process_step_case" : "workflow_step_case";
                CurrentMaster.Title.HelpName = "helpTopic";

                // Initialize master page breadcrumbs for new source point
                string[,] breadcrumbs = new string[2, 3];
                breadcrumbs[0, 0] = GetString("development-workflow_edit.sourcepoints");
                breadcrumbs[0, 1] = "~/CMSModules/Workflows/Pages/WorkflowStep/SourcePoint/List.aspx?workflowstepid=" + workflowStepId;
                breadcrumbs[0, 2] = "_self";
                breadcrumbs[1, 0] = GetString("Development-Workflow_Step_SourcePoints.New");
                breadcrumbs[1, 1] = "";
                breadcrumbs[1, 2] = "";
                CurrentMaster.Title.Breadcrumbs = breadcrumbs;
            }
            else if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }
        }
        else
        {
            editElem.StopProcessing = true;
        }
    }
}