using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;

// Edited object
[EditedObject(SiteObjectType.MACRORULE, "ruleId")]

public partial class CMSModules_Macros_Pages_Tools_MacroRule_Tab_General : CMSToolsPage
{
    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.ResourceName = QueryHelper.GetString("module", "");

        editElem.UIFormControl.OnAfterSave += MacroEdit_OnSaved;
    }


    protected void MacroEdit_OnSaved(Object sender, EventArgs e)
    {
        // Refresh breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, null);
    }

    #endregion
}