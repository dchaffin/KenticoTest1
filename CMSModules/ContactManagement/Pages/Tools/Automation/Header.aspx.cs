using System;

using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.WorkflowEngine;

// Title
[Title("Objects/MA_AutomationProcess/object.png", "ma.automationprocess.list", "ma_process_list")]

// Tabs
[Tabs("processesContent")]

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Header : CMSAutomationPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Add processes tab
        SetTab(0, GetString("ma.process.list"), AddSiteQuery("List.aspx", null), "SetHelpTopic('helpTopic', 'ma_process_list');");

        // Show actions tab only for global administrator
        if (CurrentUser.IsGlobalAdministrator)
        {
            SetTab(1, GetString("workflow.action.title"), AddSiteQuery("Action/List.aspx", null), "SetHelpTopic('helpTopic', 'ma_action_list');");

            if (QueryHelper.GetString("showtab", String.Empty).ToLowerCSafe() == "actions")
            {
                CurrentMaster.Tabs.SelectedTab = 1;
            }
        }
    }
}