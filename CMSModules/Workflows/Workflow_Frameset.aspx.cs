using System;

using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_Workflows_Workflow_Frameset : SiteManagerPage
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
                case "actions":
                    contentTargetUrl = "Pages/WorkflowAction/List.aspx";
                    break;

                default:
                    contentTargetUrl = "Workflow_List.aspx";
                    break;
            }

            headerTargetUrl = URLHelper.AddParameterToUrl("Workflows_Header.aspx", "showtab", selectedTab);
        }
        else
        {
            contentTargetUrl = "Workflow_List.aspx";
            headerTargetUrl = "Workflows_Header.aspx";
        }
    }
}