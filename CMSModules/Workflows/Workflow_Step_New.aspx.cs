using System;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Actions(1)]
[SaveAction(0)]
public partial class CMSModules_Workflows_Workflow_Step_New : CMSWorkflowPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        string workflowSteps = GetString("Development-Workflow_Step_New.Steps");
        string currentWorkflow = GetString("Development-Workflow_Steps.NewStep");

        editElem.WorkflowID = WorkflowId;
        editElem.AfterCreateRedirectURL = "~/CMSModules/Workflows/Workflow_Step_Edit.aspx";

        string workflowStepsUrl = "~/CMSModules/Workflows/Workflow_Steps.aspx?workflowid=" + WorkflowId;

        // Initializes page title
        InitializeMasterPage(workflowSteps, workflowStepsUrl, currentWorkflow);
    }


    /// <summary>
    /// Initializes master page title element.
    /// </summary>
    private void InitializeMasterPage(string workflowSteps, string workflowStepsUrl, string currentWorkflow)
    {
        CurrentMaster.Title.HelpTopicName = "workflow_step";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Set the breadcrumbs element
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = workflowSteps;
        pageTitleTabs[0, 1] = workflowStepsUrl;
        pageTitleTabs[0, 2] = "workflowsContent";
        pageTitleTabs[1, 0] = currentWorkflow;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }

    #endregion
}