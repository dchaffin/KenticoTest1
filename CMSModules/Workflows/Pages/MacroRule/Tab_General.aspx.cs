using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.WorkflowEngine;

// Edited object
[EditedObject(SiteObjectType.MACRORULE, "ruleId")]

public partial class CMSModules_Workflows_Pages_MacroRule_Tab_General : SiteManagerPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.ResourceName = WorkflowObjectType.WORKFLOW;

        editElem.UIFormControl.OnAfterSave += MacroEdit_OnSaved;
    }


    protected void MacroEdit_OnSaved(Object sender, EventArgs e)
    {
        // Refresh breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, null);
    }

    #endregion
}