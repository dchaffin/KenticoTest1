using System;

using CMS.GlobalHelper;
using CMS.UIControls;

[Actions(1)]
[SaveAction(0)]
public partial class CMSModules_Workflows_Workflow_Step_General : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int workflowStepId = QueryHelper.GetInteger("workflowStepId", 0);

        if (workflowStepId > 0)
        {
            if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }
            editElem.WorkflowStepID = workflowStepId;
        }
        else
        {
            editElem.StopProcessing = true;
        }
    }
}