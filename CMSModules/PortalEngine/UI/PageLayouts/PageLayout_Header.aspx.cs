using System;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.UIControls;

[Tabs("pl_edit_content")]
public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Header : CMSModalPage
{
    bool dialogMode = false;

    int layoutId = 0;
    int templateId = 0;

    string displayName = null;
    string themePath = null;


    protected override void OnPreInit(EventArgs e)
    {
        dialogMode = QueryHelper.GetBoolean("editonlycode", false);
        layoutId = QueryHelper.GetInteger("layoutid", 0);
        templateId = QueryHelper.GetInteger("templateid", 0);

        if (dialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }
        else
        {
            CheckAccessToSiteManager();
        }

        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "CMS_Design_page_layout";

        if (templateId > 0)
        {
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Layout/object.png");
            CurrentMaster.Title.TitleText = GetString("pagetemplate.layoutproperties");
        }
        else
        {
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Layout/object.png");
            CurrentMaster.Title.TitleText = GetString("administration-pagelayout_new.editlayout");
        }

        if (templateId > 0)
        {
            var template = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);

            if (template.LayoutID > 0)
            {
                layoutId = template.LayoutID;
            }
            else
            {
                EditedObject = template;
                displayName = template.DisplayName;
                themePath = template.GetThemePath();
            }
        }

        if (layoutId > 0)
        {
            var layout = LayoutInfoProvider.GetLayoutInfo(layoutId);
            EditedObject = layout;
            displayName = layout.LayoutDisplayName;
            themePath = layout.GetThemePath();
        }

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!dialogMode)
        {
            InitBreadcrumbs(2);
            SetBreadcrumb(1, displayName, null, null, null);
            SetBreadcrumb(0, GetString("administration-pagelayout_new.newlayout"), ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_List.aspx"), "_parent", null);
        }

        SetTab(0, GetString("general.general"), ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_Edit.aspx") + URLHelper.Url.Query, "SetHelpTopic('helpTopic', 'newedit_page_layout');");

        if (templateId == 0)
        {
            SetTab(1, GetString("layout.templates"), ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_Templates.aspx") + URLHelper.Url.Query, "SetHelpTopic('helpTopic', 'pagelayout_template_tab');");

            if (!StorageHelper.IsExternalStorage(themePath))
            {
                SetTab(2, GetString("stylesheet.theme"), ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_Edit_Theme.aspx") + URLHelper.Url.Query, "SetHelpTopic('helpTopic', 'pagelayout_theme_tab');");
            }
        }
    }
}
