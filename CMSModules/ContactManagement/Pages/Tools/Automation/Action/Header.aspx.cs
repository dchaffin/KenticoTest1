using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.OnlineMarketing;

// Edited object
[EditedObject(PredefinedObjectType.AUTOMATIONACTION, "actionid")]

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "cms.workflowaction.action.list", "~/CMSModules/ContactManagement/Pages/Tools/Automation/Action/List.aspx", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}")]

// Tabs
[Tabs("content")]
[Tab(0, "general.general", "Tab_General.aspx?actionid={%EditedObject.ID%}", "SetHelpTopic('helpTopic', 'ma_action_general');")]
[Tab(1, "general.parameters", "Tab_Parameters.aspx?actionid={%EditedObject.ID%}", "SetHelpTopic('helpTopic', 'ma_action_parameters');")]

[Help("ma_action_general", "helpTopic")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Action_Header : CMSContactManagementConfigurationPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Only global administrator can access automation process actions
        if (!CurrentUser.IsGlobalAdministrator)
        {
            RedirectToAccessDenied(GetString("security.accesspage.onlyglobaladmin"));
        }
    }
}
