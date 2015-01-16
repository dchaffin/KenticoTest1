using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;

// Edited object
[EditedObject(WorkflowObjectType.AUTOMATIONACTION, "actionId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Action_Tab_General : CMSContactManagementConfigurationPage
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


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        editElem.ObjectType = PredefinedObjectType.AUTOMATIONACTION;
    }
}