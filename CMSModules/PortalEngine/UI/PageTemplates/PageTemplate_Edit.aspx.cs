using System;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_Edit : CMSEditTemplatePage
{
    private bool dialog = false;

    protected string mHeight = null;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        dialog = QueryHelper.GetBoolean("dialog", false);

        if (dialog)
        {
            mHeight = TabsBreadHeadFrameHeight + ", *, 43";
            frameFooter.Visible = true;
        }
        else
        {
            mHeight = TabsBreadFrameHeight + ", *";
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string pageTemplateRefreshScript = ScriptHelper.GetScript(@"
function Refresh() {
    if ((parent != null) && (parent.frames['pt_tree'] != null)) parent.frames['pt_tree'].ReloadPage(" + PageTemplateID + @");
    if (window.frames['pt_edit_menu'] != null) window.frames['pt_edit_menu'].location.href = 'PageTemplate_Header.aspx?templateid=" + PageTemplateID + ((ValidationHelper.GetInteger(Request.QueryString["dialog"], 0) > 0) ? "&dialog=1" : "") + @"';
}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "pageTemplateRefreshScript", pageTemplateRefreshScript);
    }
}
