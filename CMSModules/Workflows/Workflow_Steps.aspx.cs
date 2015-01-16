using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.GraphConfig;

public partial class CMSModules_Workflows_Workflow_Steps : CMSWorkflowPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        stepsGrid.OnAction += stepsGrid_OnAction;
        stepsGrid.DataBinding += stepsGrid_DataBinding;

        int workflowId = QueryHelper.GetInteger("workflowid", 0);

        // Control initialization
        InitializeMasterPage(workflowId);

        // Prepare the steps query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@StepWorkflowID", workflowId);
        stepsGrid.WhereCondition = "StepWorkflowID = @StepWorkflowID";
        stepsGrid.QueryParameters = parameters;

        stepsGrid.OnExternalDataBound += stepsGrid_OnExternalDataBound;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage(int workflowId)
    {
        // Get workflow
        WorkflowInfo wi = WorkflowInfoProvider.GetWorkflowInfo(workflowId);
        // Set edited object
        EditedObject = wi;

        if (wi != null)
        {
            // Check if 'automatically publish changes' is allowed
            if (wi.WorkflowAutoPublishChanges)
            {
                ShowInformation(GetString("Development-Workflow_Steps.CustomStepsCanNotBeCreated"));
            }
            else
            {
                // Set actions
                string[,] actions = new string[1, 8];
                actions[0, 0] = "HyperLink";
                actions[0, 1] = GetString("Development-Workflow_Steps.NewStep");
                actions[0, 3] = "~/CMSModules/Workflows/Workflow_Step_New.aspx?workflowid=" + workflowId;
                actions[0, 5] = GetImageUrl("Objects/CMS_WorkflowStep/add.png");

                CurrentMaster.HeaderActions.Actions = actions;
            }
        }
    }

    #endregion


    #region "Grid events"

    protected static object stepsGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        WorkflowStepTypeEnum stepType = WorkflowStepTypeEnum.Undefined;
        GridViewRow container = null;

        switch (sourceName.ToLowerCSafe())
        {
            case "allowaction":
                container = (GridViewRow)parameter;
                stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue((DataRowView)container.DataItem, "StepType"), 3);
                switch (stepType)
                {
                    case WorkflowStepTypeEnum.DocumentEdit:
                    case WorkflowStepTypeEnum.DocumentPublished:
                    case WorkflowStepTypeEnum.DocumentArchived:
                        ((Control)sender).Visible = false;
                        break;
                }
                break;

            case "steptype":
                stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(parameter, 3);
                WorkflowNode node = WorkflowNode.GetInstance(stepType);
                return HTMLHelper.HTMLEncode(node.Name);

            case "#objectmenu":
                container = (GridViewRow)parameter;
                WorkflowStepInfo step = new WorkflowStepInfo(((DataRowView)container.DataItem).Row);
                if (step.StepIsDefault)
                {
                    ImageButton button = ((ImageButton)sender);
                    button.Visible = false;
                }
                break;
        }
        return parameter;
    }


    protected void stepsGrid_DataBinding(object sender, EventArgs e)
    {
        stepsGrid.GridView.Sort("StepOrder", SortDirection.Ascending);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void stepsGrid_OnAction(string actionName, object actionArgument)
    {
        int workflowstepid = Convert.ToInt32(actionArgument);
        if (actionName == "edit")
        {
            URLHelper.Redirect("Workflow_Step_Edit.aspx?objectid=" + workflowstepid);
        }

        else if (actionName == "delete")
        {
            // Check if documents use the workflow
            WorkflowStepInfo si = WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowstepid);
            if (si != null)
            {
                List<string> documentNames = new List<string>();
                if (WorkflowStepInfoProvider.CheckDependencies(workflowstepid, ref documentNames))
                {
                    // Encode and localize names
                    StringBuilder sb = new StringBuilder();
                    documentNames.ForEach(item => sb.Append("<br />", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(item))));
                    ShowError(GetString("Workflow.CannotDeleteStepUsed"), GetString("workflow.documentlist") + sb.ToString(), null);
                    return;
                }
                else
                {
                    // Delete the workflow step
                    WorkflowStepInfoProvider.DeleteWorkflowStepInfo(workflowstepid);
                }
            }
        }
        else if (actionName == "moveup")
        {
            WorkflowStepInfoProvider.MoveStepUp(WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowstepid));
        }
        else if (actionName == "movedown")
        {
            WorkflowStepInfoProvider.MoveStepDown(WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowstepid));
        }
    }

    #endregion
}