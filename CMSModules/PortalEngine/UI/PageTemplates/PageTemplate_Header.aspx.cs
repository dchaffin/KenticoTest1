using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.IO;

// Set edited object
[EditedObject("cms.pagetemplate", "templateid")]
// Set tabs number and ensure additional tab
[Tabs("pt_edit_content")]
public partial class CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_Header : CMSEditTemplatePage
{
    protected bool isDialog = false;

    #region "Page events"

    /// <summary>
    /// PreInit event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        isDialog = QueryHelper.GetBoolean("dialog", false);

        // Change master page for dialog
        if (isDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (PageTemplate != null)
        {
            // Initialize page title
            string title = GetString("Administration-PageTemplate_Header.TemplateProperties");

            // Check if the object exists - if not->redirect and inform the user
            EditedObject = PageTemplate;

            PageTemplateCategoryInfo categoryInfo = PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo(PageTemplate.CategoryID);
            string currentPageTemplate = HTMLHelper.HTMLEncode(PageTemplate.DisplayName);
            string[,] breadcrumbs;

            if (!isDialog)
            {
                breadcrumbs = new string[3, 4];

                breadcrumbs[0, 0] = GetString("development.pagetemplates");
                breadcrumbs[0, 1] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/Category_Frameset.aspx");
                breadcrumbs[0, 2] = "_parent";
                breadcrumbs[0, 3] = "if (parent.parent.frames['pt_tree']) { parent.parent.frames['pt_tree'].location.href = '" + URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Tree.aspx") + "'; }";

                if (categoryInfo != null)
                {
                    breadcrumbs[1, 0] = HTMLHelper.HTMLEncode(categoryInfo.DisplayName);
                    breadcrumbs[1, 1] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/Category_Frameset.aspx?categoryid=" + PageTemplate.CategoryID);
                    breadcrumbs[1, 2] = "_parent";
                    breadcrumbs[1, 3] = "if (parent.parent.frames['pt_tree']) { parent.parent.frames['pt_tree'].location.href = '" + URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Tree.aspx?categoryid=" + PageTemplate.CategoryID) + "'; }";

                    breadcrumbs[2, 0] = HTMLHelper.HTMLEncode(currentPageTemplate);
                    breadcrumbs[2, 1] = "";
                    breadcrumbs[2, 2] = "";
                }
            }
            else
            {
                breadcrumbs = new string[2, 3];

                breadcrumbs[0, 0] = GetString("development.pagetemplates");
                breadcrumbs[0, 1] = "";
                breadcrumbs[0, 2] = "";

                breadcrumbs[1, 0] = HTMLHelper.HTMLEncode(currentPageTemplate);
                breadcrumbs[1, 1] = "";
                breadcrumbs[1, 2] = "";

                if (PageTemplate.IsReusable)
                {
                    CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_PageTemplates/pagetemplate.png");
                }
                else
                {
                    CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_PageTemplates/adhoc.png");
                }
                CurrentMaster.Title.TitleText = title;
            }

            CurrentMaster.Title.Breadcrumbs = breadcrumbs;
            CurrentMaster.Title.HelpTopicName = "general_tab12";
            CurrentMaster.Title.HelpName = "helpTopic";

            // Initialize menu
            int i = 0;
            string defaultDeviceProfileQuery = "&devicename=default&ObjectLifeTime=request";

            string dialog = String.Empty;
            if (isDialog)
            {
                dialog = "&dialog=1";
            }

            // General tab
            SetTab(i, GetString("general.general"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_General.aspx?templateid=" + PageTemplateID + dialog), "SetHelpTopic('helpTopic', 'general_tab12');");
            i++;

            bool showDesign = ((PageTemplate.PageTemplateType == PageTemplateTypeEnum.Portal) || (PageTemplate.PageTemplateType == PageTemplateTypeEnum.Dashboard));

            // Design tab
            if (!isDialog && showDesign)
            {
                SetTab(i, GetString("edittabs.design"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Design.aspx?templateid=" + PageTemplateID + defaultDeviceProfileQuery), "SetHelpTopic('helpTopic', 'design_tab2');");
                i++;
            }

            if (showDesign)
            {
                string aliasPath = QueryHelper.GetString("aliaspath", string.Empty);
                string query = string.IsNullOrEmpty(aliasPath) ? "" : "&aliasPath=" + aliasPath;
                query += defaultDeviceProfileQuery;

                // Layouts tab
                SetTab(i, GetString("Administration-PageTemplate_Header.Layouts"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_Edit.aspx?allowswitch=1&templateid=" + PageTemplateID + dialog + query), "SetHelpTopic('helpTopic', 'layout');");
                i++;

                // Show the Device layouts tab
                bool deviceProfilesEnabled = DeviceProfileInfoProvider.IsDeviceProfilesEnabled(CMSContext.CurrentSiteName);
                if (deviceProfilesEnabled && (PageTemplate.PageTemplateType == PageTemplateTypeEnum.Portal))
                {
                    // Device layouts tab
                    SetTab(i, "Device layouts", URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_DeviceLayouts.aspx?templateid=" + PageTemplateID + dialog + query), "SetHelpTopic('helpTopic', 'device_layouts');");
                    i++;
                }
            }

            CurrentUserInfo user = CMSContext.CurrentUser;
            if (!isDialog && (PageTemplate.IsReusable) && user.UserSiteManagerAdmin)
            {
                // Sites tab
                SetTab(i, GetString("general.sites"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Sites.aspx?templateid=" + PageTemplateID), "SetHelpTopic('helpTopic', 'sites_tab2');");
                i++;

                if (PageTemplate.PageTemplateType != PageTemplateTypeEnum.Dashboard)
                {
                    // Scopes tab
                    SetTab(i, GetString("pagetemplate.edit.scopes"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/Scopes/PageTemplateScopes_List.aspx?templateid=" + PageTemplateID), "SetHelpTopic('helpTopic', 'page_templates_scopes');");
                    i++;
                }
            }

            if (showDesign && PageTemplate.IsReusable && !StorageHelper.IsExternalStorage(PageTemplate.GetThemePath()) && user.UserSiteManagerAdmin)
            {
                // Theme tab
                SetTab(i, GetString("Stylesheet.Theme"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Theme.aspx?templateid=" + PageTemplateID + dialog), "SetHelpTopic('helpTopic', 'page_templates_theme');");
                i++;
            }

            // Show web part tab
            if (showDesign || (PageTemplate.PageTemplateType == PageTemplateTypeEnum.AspxPortal))
            {
                // Web parts tab
                if ((!isDialog || SettingsKeyProvider.DevelopmentMode) && CurrentUser.IsGlobalAdministrator)
                {
                    SetTab(i, GetString("Administration-PageTemplate_Header.WebParts"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_WebParts.aspx?templateid=" + PageTemplateID + dialog), "SetHelpTopic('helpTopic', 'web_parts');");
                    i++;

                    if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSShowTemplateASPXTab"], false))
                    {
                        // ASPX tab
                        SetTab(i, GetString("Administration-PageTemplate_Header.ASPX"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_ASPX.aspx?templateid=" + PageTemplateID), "SetHelpTopic('helpTopic', 'page_templates_aspx_code');");
                        i++;
                    }
                }
            }

            if (PageTemplate.PageTemplateType != PageTemplateTypeEnum.Dashboard)
            {
                // Header tab
                SetTab(i, GetString("Administration-PageTemplate_Header.Header"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_HeaderTab.aspx?templateid=" + PageTemplateID + dialog), "SetHelpTopic('helpTopic', 'header');");
                i++;

                // Documents tab
                SetTab(i, GetString("general.documents"), URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Documents.aspx?templateid=" + PageTemplateID + dialog), "SetHelpTopic('helpTopic', 'page_templates_documents');");
                i++;
            }
        }
    }

    #endregion
}