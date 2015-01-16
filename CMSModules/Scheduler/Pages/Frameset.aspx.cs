using System;

using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_Scheduler_Pages_Frameset : CMSScheduledTasksPage
{
    protected string headerTargetUrl = "";
    protected string contentTargetUrl = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        string selectedTab = QueryHelper.GetString("showtab", null);
        if (!String.IsNullOrEmpty(selectedTab))
        {
            switch (selectedTab.ToLowerCSafe())
            {
                case "system":
                    contentTargetUrl = "Tab_SystemTasks.aspx";
                    break;

                default:
                    contentTargetUrl = "Tab_Tasks.aspx";
                    break;
            }

            headerTargetUrl = URLHelper.AddParameterToUrl("Header.aspx", "showtab", selectedTab);
        }
        else
        {
            switch (QueryHelper.GetString("action", String.Empty).ToLowerCSafe())
            {
                case "newtask":
                    contentTargetUrl = "Task_Edit.aspx";
                    break;

                default:
                    contentTargetUrl = "Tab_Tasks.aspx";
                    break;
            }
            headerTargetUrl = "Header.aspx";
        }

        if (QueryHelper.GetBoolean("saved", false))
        {
            contentTargetUrl = URLHelper.AddParameterToUrl(contentTargetUrl, "saved", "1");
        }

        contentTargetUrl = URLHelper.AppendQuery(contentTargetUrl, URLHelper.Url.Query);
        headerTargetUrl = URLHelper.AppendQuery(headerTargetUrl, URLHelper.Url.Query);
    }
}