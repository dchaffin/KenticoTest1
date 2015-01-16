using System;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Templates : CMSModalPage
{
    bool dialogMode = false;


    protected override void OnPreInit(EventArgs e)
    {
        dialogMode = QueryHelper.GetBoolean("editonlycode", false);

        if (dialogMode)
        {
            QueryHelper.ValidateHash("hash");
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }
        else
        {
            CheckAccessToSiteManager();
        }

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Filter templates for current layout
        gridTemplates.WhereCondition = "ObjectID IN (SELECT PageTemplateID FROM CMS_PageTemplate WHERE PageTemplateLayoutID = " + QueryHelper.GetInteger("layoutid", 0) + ") OR ObjectID IN (SELECT PageTemplateID FROM CMS_TemplateDeviceLayout WHERE LayoutID = " + QueryHelper.GetInteger("layoutid", 0) + ")";
        gridTemplates.ZeroRowsText = GetString("layout.notemplates");
        gridTemplates.FilteredZeroRowsText = GetString("layout.notemplates");

        // Register edit script 
        string script = @"
        function EditPageTemplate(id){
        modalDialog('" + ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Edit.aspx") + @"?templateid='+id+'&nobreadcrumbs=1&dialog=1', 'TemplateSelection', 1024, 768, false);
        }
        ";

        ScriptHelper.RegisterDialogScript(this.Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditPageTemplate", ScriptHelper.GetScript(script));

        pnlContentWrap.CssClass = dialogMode ? "PageContent" : null;
    }
}
