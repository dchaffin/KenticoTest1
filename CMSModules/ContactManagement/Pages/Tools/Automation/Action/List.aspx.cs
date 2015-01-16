using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;

// Actions
[Actions(1)]
[Action(0, "Objects/MA_AutomationAction/add.png", "cms.workflowaction.action.new", "New.aspx?issitemanager={?issitemanager?}")]

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Action_List : CMSContactManagementConfigurationPage
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
        listElem.AllowedObjects = PredefinedObjectType.CONTACT;
        listElem.ObjectType = PredefinedObjectType.AUTOMATIONACTION;
    }
}
