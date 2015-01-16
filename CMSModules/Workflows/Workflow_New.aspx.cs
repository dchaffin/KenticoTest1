using System;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.SettingsProvider;
using CMS.Synchronization;

[EditedObject("cms.workflow", "workflowid")]
public partial class CMSModules_Workflows_Workflow_New : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Workflows - New Workflow";

        // Redirect after successful action
        editElem.Form.RedirectUrlAfterCreate = "Workflow_Edit.aspx?workflowid={%EditedObject.ID%}&saved=1";
        editElem.CurrentWorkflow.WorkflowType = (WorkflowTypeEnum)QueryHelper.GetInteger("type", (int)WorkflowTypeEnum.Basic);

        // Initializes labels
        string workflowList = GetString("Development-Workflow_Edit.Workflows");

        string newResString = (WorkflowType == WorkflowTypeEnum.Basic) ? "Development-Workflow_List.NewWorkflow" : "Development-Workflow_List.NewAdvancedWorkflow";
        string currentWorkflow = GetString(newResString);

        const string workflowListUrl = "~/CMSModules/Workflows/Workflow_List.aspx";

        // Initialize master page elements
        InitializeMasterPage(workflowList, workflowListUrl, currentWorkflow);
    }


    /// <summary>
    /// Initializes master page elements.
    /// </summary>
    private void InitializeMasterPage(string workflowList, string workflowListUrl, string currentWorkflow)
    {
        string title = GetString("Development-Workflow_List.NewWorkflow");
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "workflow_new";

        // Initializes page title
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = workflowList;
        pageTitleTabs[0, 1] = workflowListUrl;
        pageTitleTabs[1, 0] = currentWorkflow;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }
}