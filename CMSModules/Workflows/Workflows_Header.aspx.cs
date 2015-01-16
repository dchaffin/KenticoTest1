using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

// Title
[Title("Objects/CMS_Workflow/object.png", "Workflow.Title", "workflow_list")]

// Tabs
[Tabs("workflowsContent")]
[Tab(0, "Development-Workflow_List.Title", "Workflow_List.aspx", "SetHelpTopic('helpTopic', 'workflow_list');")]

public partial class CMSModules_Workflows_Workflows_Header : SiteManagerPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (WorkflowInfoProvider.IsAdvancedWorkflowAllowed())
        {
            SetTab(1, GetString("workflow.action.title"), "Pages/WorkflowAction/List.aspx", "SetHelpTopic('helpTopic', 'workflow_action_list');");
            SetTab(2, GetString("macros.macrorules"), "Pages/MacroRule/List.aspx?displaytitle=false", "SetHelpTopic('helpTopic', 'macros_macrorule_list');");
        }
    }
}