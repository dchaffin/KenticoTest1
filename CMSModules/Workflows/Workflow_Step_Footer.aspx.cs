using System;
using System.Text;

using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_Workflows_Workflow_Step_Footer : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int workflowStepId = QueryHelper.GetInteger("workflowStepId", 0);
        string graphName = QueryHelper.GetString("graph", String.Empty);
        if ((workflowStepId > 0) && !String.IsNullOrEmpty(graphName))
        {
            ScriptHelper.RegisterJQuery(Page);
            ScriptHelper.RegisterWOpenerScript(Page);

            StringBuilder actionScript = new StringBuilder();
            actionScript.Append(@"
wopener.RefreshWOpener = function() {
    var graph = wopener.window[", ScriptHelper.GetString(graphName), @"];
    if(graph){
        graph.refreshNode(", workflowStepId, @");
    }
}");

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "WorkflowDesignerRefresh", ScriptHelper.GetScript(actionScript.ToString()));
        }

        btnClose.OnClientClick = "return parent.CloseDialog();";
    }
}