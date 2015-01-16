using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WorkflowEngine;

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "cms.workflowaction.action.list", "~/CMSModules/Workflows/Pages/WorkflowAction/List.aspx", null)]
[Breadcrumb(1, "cms.workflowaction.action.new")]

[Help("workflow_action_new")]

public partial class CMSModules_Workflows_Pages_WorkflowAction_New : CMSWorkflowPage
{
}
