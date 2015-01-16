using System;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_UI_Macros_Dialogs_Header : CMSModalPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        InitTabs("insertContent");
        SetTab(0, GetString("insertmacro.insertmacrotree"), (URLHelper.ResolveUrl("~/CMSAdminControls/UI/Macros/Dialogs/Tab_InsertMacroTree.aspx") + URLHelper.Url.Query), "");
        SetTab(1, GetString("insertmacro.insertmacrocode"), URLHelper.ResolveUrl("~/CMSAdminControls/UI/Macros/Dialogs/Tab_InsertMacroCode.aspx"), "");

        CurrentMaster.Tabs.UrlTarget = "insertContent";

        CurrentMaster.Title.TitleText = GetString("insertmacro.dialogtitle");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Snippets/module.png");
    }
}