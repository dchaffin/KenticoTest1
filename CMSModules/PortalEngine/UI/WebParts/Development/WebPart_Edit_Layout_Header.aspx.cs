using System;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.UIControls;

[EditedObject("cms.webpartlayout", "layoutid")]
[Help("newedit_webpart_layout", "helpTopic")]
[Tabs("Content")]
public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Layout_Header : CMSModalPage
{
    bool isDialog = false;


    protected override void OnPreInit(EventArgs e)
    {
        isDialog = QueryHelper.GetBoolean("editonlycode", false);

        if (isDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";

            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_WebPartLayout/object.png");
            CurrentMaster.Title.TitleText = GetString("layout_edit.headercaption");
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

        WebPartLayoutInfo wpli = EditedObject as WebPartLayoutInfo;
        if ((wpli != null) && (!isDialog))
        {
            InitBreadcrumbs(2);
            SetBreadcrumb(0, GetString("WebParts.Layout"), ResolveUrl("~/CMSModules/PortalEngine/UI/Webparts/Development/WebPart_Edit_Layout.aspx") + URLHelper.Url.Query, "_parent", null);
            SetBreadcrumb(1, wpli.WebPartLayoutDisplayName, null, null, null);
        }

        if ((wpli == null) || (!StorageHelper.IsExternalStorage(wpli.GetThemePath())))
        {
            SetTab(0, GetString("general.general"), "WebPart_Edit_Layout_Edit.aspx" + URLHelper.Url.Query, "SetHelpTopic('helpTopic', 'newedit_webpart_layout');");
            SetTab(1, GetString("stylesheet.theme"), "WebPart_Edit_Layout_Theme.aspx" + URLHelper.Url.Query, "SetHelpTopic('helpTopic', 'webpart_layout_theme');");
        }

    }
}
