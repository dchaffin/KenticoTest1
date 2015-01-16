using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;

// Edited object
[EditedObject("cms.workflowaction", "actionid")]

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "cms.workflowaction.action.list", "~/CMSModules/Workflows/Pages/WorkflowAction/List.aspx", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}")]

// Help
[Help("workflow_action_general", "helpTopic")]

// Tabs
[Tabs("content")]
[Tab(0, "general.general", "Tab_General.aspx?actionid={%EditedObject.ID%}", "SetHelpTopic('helpTopic', 'workflow_action_general');")]
[Tab(1, "general.parameters", "Tab_Parameters.aspx?actionid={%EditedObject.ID%}", "SetHelpTopic('helpTopic', 'workflow_action_parameters');")]

public partial class CMSModules_Workflows_Pages_WorkflowAction_Header : CMSWorkflowPage
{
}
