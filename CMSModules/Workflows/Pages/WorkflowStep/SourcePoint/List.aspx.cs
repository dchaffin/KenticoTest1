using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.WorkflowEngine;
using CMS.SettingsProvider;
using CMS.ExtendedControls.ActionsConfig;
using CMS.GlobalHelper.UniGraphConfig;
using CMS.WorkflowEngine.GraphConfig;

public partial class CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_List : CMSWorkflowPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        int workflowStepId = QueryHelper.GetInteger("workflowStepId", 0);
        if (workflowStepId > 0)
        {
            listElem.WorkflowStepID = workflowStepId;
            listElem.IsLiveSite = false;
        }
        else
        {
            listElem.StopProcessing = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        InitializeMasterPage(listElem.WorkflowStepID);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    /// <param name="workflowStepId">Workflow step ID</param>
    private void InitializeMasterPage(int workflowStepId)
    {
        WorkflowStepInfo step = WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowStepId);

        // Set edited object
        EditedObject = step;

        if (step != null)
        {
            if (step.StepAllowBranch)
            {
                // Condition step type can't have additional switch cases
                if (step.StepType != WorkflowStepTypeEnum.Condition)
                {
                    // Set actions
                    HeaderAction action = new HeaderAction()
                    {
                        ControlType = HeaderActionTypeEnum.Hyperlink,
                        Text = GetString("Development-Workflow_Step_SourcePoints.New"),
                        RedirectUrl = "~/CMSModules/Workflows/Pages/WorkflowStep/SourcePoint/General.aspx?workflowStepId=" + step.StepID,
                        ImageUrl = GetImageUrl("Objects/CMS_WorkflowStep/SourcePoint/add.png"),
                    };
                    if (!CanAddSourcePoint(step))
                    {
                        action.Enabled = false;
                        action.Tooltip = GetString("workflowstep.toomanysourcepoints");
                    }

                    CurrentMaster.HeaderActions.AddAction(action);
                }
            }
            else
            {
                ShowInformation(GetString("workflowstep.cannothavecustomsourcepoints"));
                listElem.StopProcessing = true;
            }
        }
    }


    private bool CanAddSourcePoint(WorkflowStepInfo step)
    {
        if (step != null)
        {
            Node node = WorkflowNode.GetInstance(step);
            if (NodeSourcePointsLimits.Max[node.Type] <= step.StepDefinition.SourcePoints.Count)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    #endregion
}