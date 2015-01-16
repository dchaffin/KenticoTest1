using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_Category_Header : SiteManagerPage
{
    #region "Variables"

    protected int categoryId;
    protected string currentWebPartCategory = "";

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
            WebPartCategoryInfo categoryInfo = WebPartCategoryInfoProvider.GetWebPartCategoryInfoById(categoryId);
            if (categoryInfo != null)
            {
                currentWebPartCategory = HTMLHelper.HTMLEncode(categoryInfo.CategoryDisplayName);
            }
            else
            {
                // Set root category
                WebPartCategoryInfo rootCategory = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName("/");
                if (rootCategory != null)
                {
                    currentWebPartCategory = rootCategory.CategoryDisplayName;
                }
            }
        }

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
        CurrentMaster.Title.HelpTopicName = "webpart_list";

        // initializes page title control		
        string[,] tabs = new string[2,4];

        WebPartCategoryInfo rootCategory = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName("/");

        tabs[0, 0] = GetString("development-webpart_header.webparttitle");
        tabs[0, 1] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/Development/Category_Frameset.aspx");
        tabs[0, 2] = "_parent";
        tabs[0, 3] = "if (parent.parent.frames['webparttree']) { parent.parent.frames['webparttree'].location.href = '" + URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/Development/WebPart_Tree.aspx") + "'; }";

        tabs[1, 0] = HTMLHelper.HTMLEncode(currentWebPartCategory);
        tabs[1, 1] = "";
        tabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = tabs;
        CurrentMaster.Title.TitleText = "";
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_WebPart/object.png");
    }


    /// <summary>
    /// Initializes user edit menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        string generalString = GetString("general.general");
        string webPartString = GetString("development-webpart_header.webparttitle");

        string[,] tabs = new string[2,4];

        tabs[0, 0] = webPartString;
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'webpart_list');";
        tabs[0, 2] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/Development/WebPart_List.aspx?categoryid=" + categoryId);

        tabs[1, 0] = generalString;
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'web_part_category_general');";
        tabs[1, 2] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/Development/WebPart_Category.aspx?categoryid=" + categoryId);

        CurrentMaster.Tabs.UrlTarget = "categoryContent";
        CurrentMaster.Tabs.Tabs = tabs;
    }

    #endregion
}