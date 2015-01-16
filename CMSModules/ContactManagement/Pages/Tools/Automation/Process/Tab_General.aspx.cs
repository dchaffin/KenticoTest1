using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;

// Edited object
[EditedObject(PredefinedObjectType.AUTOMATIONPROCESS, "processid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_General : CMSAutomationPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check permissions
        editElem.Form.SecurityCheck.Resource = ModuleEntry.ONLINEMARKETING;
        editElem.Form.SecurityCheck.Permission = "ManageProcesses";
    }
}