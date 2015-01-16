using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "cms.workflowaction.action.list", "~/CMSModules/ContactManagement/Pages/Tools/Automation/Action/List.aspx?issitemanager={?issitemanager?}", null)]
[Breadcrumb(1, "cms.workflowaction.action.new")]

[Help("ma_action_new")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Action_New : CMSContactManagementConfigurationPage
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


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.AllowedObjects = PredefinedObjectType.CONTACT;
        editElem.CurrentAction.ActionWorkflowType = WorkflowTypeEnum.Automation;
    }
}
