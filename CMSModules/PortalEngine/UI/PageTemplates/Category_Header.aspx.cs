using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_PageTemplates_Category_Header : SiteManagerPage
{
    #region "Variables"

    protected int categoryId;
    protected string currentPageTemplateCategory = "";

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.Contains("categoryid"))
        {
            categoryId = QueryHelper.GetInteger("categoryid", 0);
            PageTemplateCategoryInfo categoryInfo = PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo(categoryId);
            if (categoryInfo != null)
            {
                currentPageTemplateCategory = categoryInfo.DisplayName;
            }
            else
            {
                // Set root category
                PageTemplateCategoryInfo rootCategory = PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfoByCodeName("/");
                if (rootCategory != null)
                {
                    currentPageTemplateCategory = rootCategory.DisplayName;
                }
            }
        }

        // If query string contains "saved" (means: new category was created) then show second tab (general)
        if (QueryHelper.Contains("saved"))
        {
            CurrentMaster.Tabs.SelectedTab = 1;
        }

        if (!RequestHelper.IsPostBack())
        {
            InitalizeMenu();
        }

        InitializeMasterPage();
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes master page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "pagetemplate_list";

        // Initializes page title control		
        string[,] tabs = new string[2,4];

        tabs[0, 0] = GetString("development.pagetemplates");
        tabs[0, 1] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/Category_Frameset.aspx");
        tabs[0, 2] = "_parent";
        tabs[0, 3] = "if (parent.parent.frames['pt_tree']) { parent.parent.frames['pt_tree'].location.href = '" + URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Tree.aspx") + "'; }";

        tabs[1, 0] = HTMLHelper.HTMLEncode(currentPageTemplateCategory);
        tabs[1, 1] = "";
        tabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = tabs;
        CurrentMaster.Title.TitleText = "";
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_PageTemplate/object.png");
    }


    /// <summary>
    /// Initializes user edit menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        string generalString = GetString("general.general");
        string templatesString = GetString("development.pagetemplates");

        string[,] tabs = new string[2,4];

        tabs[0, 0] = templatesString;
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'pagetemplate_list');";
        tabs[0, 2] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_List.aspx?categoryid=" + categoryId);

        tabs[1, 0] = generalString;
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'page_template_category_general');";
        tabs[1, 2] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Category.aspx?categoryid=" + categoryId);

        CurrentMaster.Tabs.UrlTarget = "categoryContent";
        CurrentMaster.Tabs.Tabs = tabs;
    }

    #endregion
}