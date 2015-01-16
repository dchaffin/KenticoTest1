using System;

using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_Workflows_Workflow_Edit : CMSWorkflowPage
{
    protected string headerTargetUrl = "";
    protected string contentTargetUrl = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        string selectedTab = QueryHelper.GetString("showtab", null);
        if (!string.IsNullOrEmpty(selectedTab))
        {
            switch (selectedTab.ToLowerCSafe())
            {
                case "scopes":
                    contentTargetUrl = "Workflow_Scopes.aspx";
                    break;

                case "steps":
                    contentTargetUrl = "Workflow_Steps.aspx";
                    break;

                default:
                    contentTargetUrl = "Workflow_General.aspx";
                    break;
            }

            headerTargetUrl = URLHelper.AddParameterToUrl("Workflow_Header.aspx", "showtab", selectedTab);
        }
        else
        {
            contentTargetUrl = "Workflow_General.aspx";
            headerTargetUrl = "Workflow_Header.aspx";
        }

        bool saved = QueryHelper.GetBoolean("saved", false);
        bool converted = QueryHelper.GetBoolean("converted", false);
        if (converted)
        {
            contentTargetUrl = URLHelper.AddParameterToUrl(contentTargetUrl, "converted", "1");
        }
        else if (saved)
        {
            contentTargetUrl = URLHelper.AddParameterToUrl(contentTargetUrl, "saved", "1");
        }

        if (WorkflowId != 0)
        {
            contentTargetUrl = URLHelper.AddParameterToUrl(contentTargetUrl, "workflowid", WorkflowId.ToString());
            headerTargetUrl = URLHelper.AddParameterToUrl(headerTargetUrl, "workflowid", WorkflowId.ToString());
        }
    }
}