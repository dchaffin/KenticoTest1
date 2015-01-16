using System;
using System.Text;

using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Controls.Configuration;
using CMS.ExtendedControls;

[Title("Objects/CMS_ScheduledTask/object.png", "Task_List.HeaderCaption", "tasks_list")]

[Tabs("tasksContent")]
public partial class CMSModules_Scheduler_Pages_Header : CMSScheduledTasksPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InitalizeMenu();

        if (!RequestHelper.IsPostBack())
        {
            if (SelectedSiteID == 0)
            {
                siteSelector.SiteID = CMSContext.CurrentSiteID;
            }

            if (QueryHelper.GetString("showtab", String.Empty).ToLowerCSafe() == "system")
            {
                CurrentMaster.Tabs.SelectedTab = 1;
                SetHelp("systemtasks_list", "helpTopic");
            }
        }

        // Show site selector only in Site Manager
        if (SiteID <= 0)
        {
            // Register selected site ID for tab links
            ScriptHelper.RegisterStartupScript(this, typeof(string), "selectedSiteId", ScriptHelper.GetScript(string.Format("var selectedSiteId = {0};", siteSelector.SiteID)));

            CurrentMaster.DisplaySiteSelectorPanel = true;
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.UniSelector.OnSelectionChanged += siteSelector_OnSelectionChanged;
        }
    }


    /// <summary>
    /// Initializes scheduled tasks tabs.
    /// </summary>
    private void InitalizeMenu()
    {
        string siteParam = SiteID > 0 ? "?siteid=" + SiteID : "?selectedsiteid=' + selectedSiteId + '";

        // Tasks tab
        TabItem tasks = new TabItem()
        {
            Text = GetString("scheduler.tasks"),
            RedirectUrl = "Tab_Tasks.aspx" + siteParam,
            OnClientClick = "SetHelpTopic('helpTopic', 'tasks_list');"
        };
        SetTab(tasks);

        // System tasks tab
        TabItem systemTasks = new TabItem()
        {
            Text = GetString("scheduler.systemtasks"),
            RedirectUrl = "Tab_SystemTasks.aspx" + siteParam,
            OnClientClick = "SetHelpTopic('helpTopic', 'systemtasks_list');"
        };
        SetTab(systemTasks);
    }


    private void siteSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Create script to refresh current tab with updated selected site ID
        StringBuilder script = new StringBuilder();
        script.Append(@"
(function() {
    selectedTab = document.getElementById('TabControlSelItemNo');
    if (selectedTab != null) {
        parent.frames['tasksContent'].location = (selectedTab.value == 0 ? 'Tab_Tasks.aspx?' : 'Tab_SystemTasks.aspx?') + 'selectedsiteid=", siteSelector.SiteID, @"';
    }
})();");
        ScriptHelper.RegisterStartupScript(this, GetType(), "OnSelectedIndex", ScriptHelper.GetScript(script.ToString()));
    }
}