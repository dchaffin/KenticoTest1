using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_TaggingCategories_CategoryMenu : CMSAbstractWebPart
{
    #region "Variables and constants"

    protected string[] mCSSPrefixes = null;
    private CategoryInfo mCategory = null;
    private CategoryInfo mStartingCategoryObj = null;
    private bool? mAllowGlobalCategories = null;

    private const int CATEGORIES_ROOT_PARENT_ID = -1;
    private const int PERSONAL_CATEGORIES_ROOT_PARENT_ID = -2;

    #endregion


    #region "Properties"

    /// <summary>
    /// Display site categories.
    /// </summary>
    public bool DisplaySiteCategories
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplaySiteCategories"), true);
        }
        set
        {
            SetValue("DisplaySiteCategories", value);
        }
    }


    /// <summary>
    /// DisplayGlobalCategories.
    /// </summary>
    public bool DisplayGlobalCategories
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayGlobalCategories"), true);
        }
        set
        {
            SetValue("DisplayGlobalCategories", value);
        }
    }


    /// <summary>
    /// Display personal categories.
    /// </summary>
    public bool DisplayPersonalCategories
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPersonalCategories"), false);
        }
        set
        {
            SetValue("DisplayPersonalCategories", value);
        }
    }


    /// <summary>
    /// Categories root.
    /// </summary>
    public string CategoriesRoot
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoriesRoot"), "");
        }
        set
        {
            SetValue("CategoriesRoot", value);
        }
    }


    /// <summary>
    /// Personal categories root.
    /// </summary>
    public string PersonalCategoriesRoot
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PersonalCategoriesRoot"), "");
        }
        set
        {
            SetValue("PersonalCategoriesRoot", value);
        }
    }


    /// <summary>
    /// Starting category.
    /// </summary>
    public string StartingCategory
    {
        get
        {
            return ValidationHelper.GetString(GetValue("StartingCategory"), "");
        }
        set
        {
            SetValue("StartingCategory", value);
        }
    }


    /// <summary>
    /// Categories page path.
    /// </summary>
    public string CategoriesPagePath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoriesPagePath"), "");
        }
        set
        {
            SetValue("CategoriesPagePath", value);
        }
    }


    /// <summary>
    /// Indicates if category code name will be used in query parameter. Category ID is used by default.
    /// </summary>
    public bool UseCodeNameInQuery
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseCodeNameInQuery"), false);
        }
        set
        {
            SetValue("UseCodeNameInQuery", value);
        }
    }


    /// <summary>
    /// Max relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), -1);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
        }
    }


    /// <summary>
    /// Current category info object.
    /// </summary>
    private CategoryInfo Category
    {
        get
        {
            if (mCategory == null)
            {
                mCategory = SiteContext.CurrentCategory;
            }

            return mCategory;
        }
    }


    /// <summary>
    /// Starting category info object.
    /// </summary>
    private CategoryInfo StartingCategoryObj
    {
        get
        {
            if ((mStartingCategoryObj == null) && !string.IsNullOrEmpty(StartingCategory))
            {
                mStartingCategoryObj = CategoryInfoProvider.GetCategoryInfo(StartingCategory, CMSContext.CurrentSiteName);
                if (mStartingCategoryObj != null)
                {
                    if (mStartingCategoryObj.CategoryIsPersonal ||
                        (!mStartingCategoryObj.CategoryIsGlobal && !DisplaySiteCategories) ||
                        (mStartingCategoryObj.CategoryIsGlobal && !DisplayGlobalCategories))
                    {
                        mStartingCategoryObj = null;
                        StartingCategory = "";
                    }
                }
            }

            return mStartingCategoryObj;
        }
    }


    /// <summary>
    /// Indicates whether global categories are allowed for selected site.
    /// </summary>
    private bool AllowGlobalCategories
    {
        get
        {
            if (!mAllowGlobalCategories.HasValue)
            {
                mAllowGlobalCategories = SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSAllowGlobalCategories");
            }

            return mAllowGlobalCategories ?? false;
        }
    }


    /// <summary>
    /// Selected item CSS.
    /// </summary>
    public string SelectedItemCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemCSS"), "");
        }
        set
        {
            SetValue("SelectedItemCSS", value);
        }
    }


    /// <summary>
    /// CSS prefix.
    /// </summary>
    public string CSSPrefix
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CSSPrefix"), "");
        }
        set
        {
            SetValue("CSSPrefix", value);
        }
    }


    /// <summary>
    /// Expand all
    /// </summary>
    public bool ExpandAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExpandAll"), false);
        }
        set
        {
            SetValue("ExpandAll", value);
        }
    }


    /// <summary>
    /// Categories page target.
    /// </summary>
    public string CategoriesPageTarget
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoriesPageTarget"), "");
        }
        set
        {
            SetValue("CategoriesPageTarget", value);
        }
    }


    /// <summary>
    /// Render link title
    /// </summary>
    public bool RenderLinkTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderLinkTitle"), false);
        }
        set
        {
            SetValue("RenderLinkTitle", value);
        }
    }


    /// <summary>
    /// Render sub items
    /// </summary>
    public bool RenderSubItems
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderSubItems"), false);
        }
        set
        {
            SetValue("RenderSubItems", value);
        }
    }


    /// <summary>
    /// Category content before
    /// </summary>
    public string CategoryContentBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoryContentBefore"), "");
        }
        set
        {
            SetValue("CategoryContentBefore", value);
        }
    }


    /// <summary>
    /// Category content after
    /// </summary>
    public string CategoryContentAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoryContentAfter"), "");
        }
        set
        {
            SetValue("CategoryContentAfter", value);
        }
    }


    /// <summary>
    /// Categories root image url
    /// </summary>
    public string CategoriesRootImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoriesRootImageUrl"), "");
        }
        set
        {
            SetValue("CategoriesRootImageUrl", value);
        }
    }


    /// <summary>
    /// Personal categories root image url
    /// </summary>
    public string PersonalCategoriesRootImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PersonalCategoriesRootImageUrl"), "");
        }
        set
        {
            SetValue("PersonalCategoriesRootImageUrl", value);
        }
    }


    /// <summary>
    /// Categories image url
    /// </summary>
    public string CategoriesImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoriesImageUrl"), "");
        }
        set
        {
            SetValue("CategoriesImageUrl", value);
        }
    }


    /// <summary>
    /// Personal categories image url
    /// </summary>
    public string PersonalCategoriesImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PersonalCategoriesImageUrl"), "");
        }
        set
        {
            SetValue("PersonalCategoriesImageUrl", value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init trees handlers
        treeElemG.OnGetImage += treeElem_OnGetImage;
        treeElemP.OnGetImage += treeElem_OnGetImage;
        treeElemG.OnNodeCreated += treeElem_OnNodeCreated;
        treeElemP.OnNodeCreated += treeElem_OnNodeCreated;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string script = @"
function SelectNode(prefix, elementName) {
    // Set selected item in tree
    $j('span[id^=""'+prefix+'node_""]').each(function() {
        var jThis = $j(this);
        jThis.removeClass('" + SelectedItemCSS + @"');
        if (!jThis.hasClass('CategoryTreeItem')) {
            jThis.addClass('CategoryTreeItem');
        }
        if (this.id == prefix+'node_' + elementName) {
            jThis.addClass('" + SelectedItemCSS + @"');
        }
    });
}";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), ClientID + "CategoryMenuScript", ScriptHelper.GetScript(script));

        if (Category != null)
        {
            ScriptHelper.RegisterStartupScript(Page, typeof(string), ClientID + "CategorySelectionScript", ScriptHelper.GetScript("SelectNode('" + ClientID + "', " + ScriptHelper.GetString(Category.CategoryName) + ");"));
        }

        // Reload trees
        treeElemG.ReloadData();
        treeElemP.ReloadData();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Register JQuery
            ScriptHelper.RegisterJQuery(Page);

            treeElemG.StopProcessing = !DisplayGlobalCategories && !DisplaySiteCategories;
            treeElemP.StopProcessing = !DisplayPersonalCategories;

            // Use images according to culture
            if (CultureHelper.IsPreferredCultureRTL())
            {
                treeElemG.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree", false, false);
                treeElemP.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree", false, false);
            }
            else
            {
                treeElemG.LineImagesFolder = GetImageUrl("Design/Controls/Tree", false, false);
                treeElemP.LineImagesFolder = GetImageUrl("Design/Controls/Tree", false, false);
            }

            // Prepare node templates
            treeElemP.SelectedNodeTemplate = treeElemG.SelectedNodeTemplate = "<span id=\"" + ClientID + "node_##NODECODENAME##\" class=\"CategoryTreeItem " + SelectedItemCSS + "\">##BEFORENAME####ICON##<span class=\"Name\">##NODECUSTOMNAME##</span>##AFTERNAME##</span>";
            treeElemP.NodeTemplate = treeElemG.NodeTemplate = "<span id=\"" + ClientID + "node_##NODECODENAME##\" class=\"CategoryTreeItem\">##BEFORENAME####ICON##<span class=\"Name\">##NODECUSTOMNAME##</span>##AFTERNAME##</span>";

            // Init tree provider objects
            treeElemG.ProviderObject = CreateTreeProvider(CMSContext.CurrentSiteID, 0);
            if (!treeElemP.StopProcessing && (CMSContext.CurrentUser != null))
            {
                treeElemP.ProviderObject = CreateTreeProvider(0, CMSContext.CurrentUser.UserID);
            }

            // Expand first level by default
            treeElemP.ExpandPath = treeElemG.ExpandPath = "/";

            // Create root node for global and site categories
            string rootIcon = "";
            string rootCatName = CategoriesRoot;
            string rootId = "NULL";
            string before = "";
            string after = "";

            if (StartingCategoryObj != null)
            {
                rootId = StartingCategoryObj.CategoryID.ToString();
                rootCatName = HTMLHelper.HTMLEncode(StartingCategoryObj.CategoryDisplayName);

                before = string.Format("<a href=\"{0}\">", URLHelper.EncodeQueryString(GetUrl(StartingCategoryObj)));
                after = "</a>";
            }

            string rootName = "<span class=\"TreeRoot\">" + ResHelper.LocalizeString(rootCatName) + "</span>";
            string rootText = treeElemG.ReplaceMacros(treeElemG.NodeTemplate, 0, 6, rootName, rootIcon, 0, null, null);

            // Replace macros
            rootText = rootText.Replace("##NODECUSTOMNAME##", rootName);
            rootText = rootText.Replace("##NODECODENAME##", "CategoriesRoot");
            rootText = rootText.Replace("##PARENTID##", CATEGORIES_ROOT_PARENT_ID.ToString());
            rootText = rootText.Replace("##BEFORENAME##", before);
            rootText = rootText.Replace("##AFTERNAME##", after);

            string itemImg = CategoriesRootImageUrl;
            if (!string.IsNullOrEmpty(itemImg) && itemImg.StartsWithCSafe("~/"))
            {
                itemImg = ResolveUrl(itemImg);
            }

            if (string.IsNullOrEmpty(itemImg))
            {
                itemImg = null;
            }

            treeElemG.SetRoot(rootText, rootId, itemImg, null, null);

            rootName = "<span class=\"TreeRoot\">" + ResHelper.LocalizeString(PersonalCategoriesRoot) + "</span>";
            rootText = treeElemP.ReplaceMacros(treeElemP.NodeTemplate, 0, 6, rootName, rootIcon, 0, null, null);

            // Replace macros
            rootText = rootText.Replace("##NODECUSTOMNAME##", rootName);
            rootText = rootText.Replace("##NODECODENAME##", "PersonalCategoriesRoot");
            rootText = rootText.Replace("##PARENTID##", PERSONAL_CATEGORIES_ROOT_PARENT_ID.ToString());
            rootText = rootText.Replace("##BEFORENAME##", "");
            rootText = rootText.Replace("##AFTERNAME##", "");

            itemImg = PersonalCategoriesRootImageUrl;
            if (!string.IsNullOrEmpty(itemImg) && itemImg.StartsWithCSafe("~/"))
            {
                itemImg = ResolveUrl(itemImg);
            }

            if (string.IsNullOrEmpty(itemImg))
            {
                itemImg = null;
            }

            treeElemP.SetRoot(rootText, "NULL", itemImg, null, null);
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    /// <summary>
    /// Invoked when new treenode is created.
    /// </summary>
    /// <param name="itemData">Category data.</param>
    /// <param name="defaultNode">Default node.</param>
    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        defaultNode.Selected = false;
        defaultNode.SelectAction = TreeNodeSelectAction.None;
        defaultNode.NavigateUrl = "";

        if (itemData != null)
        {
            CategoryInfo category = new CategoryInfo(itemData);

            // Ensure name
            string catName = ValidationHelper.GetString(itemData["CategoryName"], "");
            string caption = ValidationHelper.GetString(itemData["CategoryDisplayName"], "");
            int catParentId = ValidationHelper.GetInteger(itemData["CategoryParentID"], 0);
            int catId = ValidationHelper.GetInteger(itemData["CategoryID"], 0);
            int catLevel = ValidationHelper.GetInteger(itemData["CategoryLevel"], 0);
            string catIDPath = ValidationHelper.GetString(itemData["CategoryIDPath"], "");

            if ((StartingCategoryObj != null) && (catIDPath.StartsWithCSafe(StartingCategoryObj.CategoryIDPath)))
            {
                catLevel = catLevel - StartingCategoryObj.CategoryLevel - 1;
            }

            string cssClass = GetCssClass(catLevel);

            if (String.IsNullOrEmpty(caption))
            {
                caption = catName;
            }

            // Get target url
            string url = GetUrl(category);
            caption = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(caption));

            StringBuilder attrs = new StringBuilder();

            // Append target sttribute
            if (!string.IsNullOrEmpty(CategoriesPageTarget))
            {
                attrs.Append(" target=\"").Append(CategoriesPageTarget).Append("\"");
            }

            // Append title attribute
            if (RenderLinkTitle)
            {
                attrs.Append(" title=\"").Append(caption).Append("\"");
            }

            // Append CSS class
            if (!string.IsNullOrEmpty(cssClass))
            {
                attrs.Append(" class=\"" + cssClass + "\"");
            }

            // Append before/after texts
            caption = (CategoryContentBefore ?? "") + caption;
            caption += CategoryContentAfter ?? "";

            // Set caption
            defaultNode.Text = defaultNode.Text.Replace("##NODECUSTOMNAME##", caption);
            defaultNode.Text = defaultNode.Text.Replace("##NODECODENAME##", HTMLHelper.HTMLEncode(catName));
            defaultNode.Text = defaultNode.Text.Replace("##PARENTID##", catParentId.ToString());
            defaultNode.Text = defaultNode.Text.Replace("##ID##", catId.ToString());
            defaultNode.Text = defaultNode.Text.Replace("##BEFORENAME##", string.Format("<a href=\"{0}\" {1}>", URLHelper.EncodeQueryString(url), attrs.ToString()));
            defaultNode.Text = defaultNode.Text.Replace("##AFTERNAME##", "</a>");

            // Expand node if all nodes are to be expanded
            if (ExpandAll)
            {
                defaultNode.Expand();
            }
            else
            {
                // Check if selected category exists
                if (Category != null)
                {
                    if ((Category.CategoryID != catId) || RenderSubItems)
                    {
                        // Expand whole path to selected category
                        string strId = catId.ToString().PadLeft(CategoryInfoProvider.CategoryIDLength, '0');
                        if (Category.CategoryIDPath.Contains(strId))
                        {
                            defaultNode.Expand();
                        }
                    }
                }
            }

            return defaultNode;
        }

        return null;
    }


    /// <summary>
    /// Method for obtaining image url for given tree node (category).
    /// </summary>
    /// <param name="node">Node (category)</param>
    /// <returns>Image URL</returns>
    protected string treeElem_OnGetImage(UniTreeNode node)
    {
        DataRow dr = node.ItemData as DataRow;

        // Use default icon
        string imgUrl = "";

        if (dr != null)
        {
            bool personal = ValidationHelper.GetInteger(dr["CategoryUserID"], 0) > 0;

            imgUrl = personal ? PersonalCategoriesImageUrl : CategoriesImageUrl;
        }

        if (string.IsNullOrEmpty(imgUrl))
        {
            return null;
        }

        return URLHelper.ResolveUrl(UIHelper.GetImagePath(Page, imgUrl, true, false));
    }


    /// <summary>
    /// Creates tree provider.
    /// </summary>
    /// <param name="siteId">ID of the site to create provider for.</param>
    /// <param name="userId">ID of the user to create provider for.</param>
    private UniTreeProvider CreateTreeProvider(int siteId, int userId)
    {
        int rootOffset = -1;

        if (userId == 0)
        {
            rootOffset = (StartingCategoryObj != null) ? StartingCategoryObj.CategoryLevel : -1;
        }

        // Prepare where condition for child counting restriction
        string whereMaxLevel = "";
        if (MaxRelativeLevel > 0)
        {
            whereMaxLevel = string.Format("(C.CategoryLevel <= {0}) AND", rootOffset + MaxRelativeLevel);
        }

        // Create and set category provider
        UniTreeProvider provider = new UniTreeProvider();
        provider.UseCustomRoots = true;
        provider.RootLevelOffset = rootOffset;
        provider.ObjectType = "cms.category";
        provider.DisplayNameColumn = "CategoryDisplayName";
        provider.IDColumn = "CategoryID";
        provider.LevelColumn = "CategoryLevel";
        provider.OrderColumn = "CategoryOrder";
        provider.ParentIDColumn = "CategoryParentID";
        provider.PathColumn = "CategoryIDPath";
        provider.ValueColumn = "CategoryID";
        provider.ChildCountColumn = "CategoryChildCount";
        provider.MaxRelativeLevel = MaxRelativeLevel;

        // Prepare the parameters
        provider.Parameters = new QueryDataParameters();
        provider.Parameters.Add("SiteID", siteId);
        provider.Parameters.Add("UserID", userId);

        // Subquery to obtain count of enabled child categories for specified user, site and 'use global categories' setting
        string countSiteWhere = GetSiteWhere("C.CategorySiteID", siteId, userId);
        string ChildCountColumn = "(SELECT COUNT(C.CategoryID) FROM CMS_Category AS C WHERE " + whereMaxLevel + " (C.CategoryEnabled = 1) AND (C.CategoryParentID = CMS_Category.CategoryID) AND " + countSiteWhere + " AND (ISNULL(C.CategoryUserID, 0) = @UserID)) AS CategoryChildCount";

        // Prepare columns
        provider.Columns = string.Format("CategoryID, CategoryName, CategoryDisplayName, CategoryLevel, CategoryOrder, CategoryParentID, CategoryIDPath, CategoryUserID, CategorySiteID, {0}", ChildCountColumn);
        provider.OrderBy = "CategoryUserID, CategorySiteID, CategoryOrder";

        string mainSiteWhere = GetSiteWhere("CategorySiteID", siteId, userId);
        provider.WhereCondition = "ISNULL(CategoryUserID, 0) = " + userId + " AND (CategoryEnabled = 1)";
        provider.WhereCondition = SqlHelperClass.AddWhereCondition(provider.WhereCondition, mainSiteWhere);

        return provider;
    }


    /// <summary>
    /// Creates site where condition for UniTreeProvider
    /// </summary>
    /// <param name="siteIdColumn">Expession used as ID column name.</param>
    /// <param name="siteId">ID of the site.</param>
    /// <param name="userId">Users ID</param>
    private string GetSiteWhere(string siteIdColumn, int siteId, int userId)
    {
        // Prepare site where condition
        string siteWhere = "";
        if (userId == 0)
        {
            // Filter global categories
            if (DisplayGlobalCategories && AllowGlobalCategories)
            {
                siteWhere = string.Format("({0} IS NULL)", siteIdColumn);
            }

            // Append site categories where
            if (DisplaySiteCategories && (siteId > 0))
            {
                siteWhere = SqlHelperClass.AddWhereCondition(siteWhere, string.Format("ISNULL({0}, 0) = {1}", siteIdColumn, siteId), "OR");
            }
        }
        else
        {
            // Personal categories are global
            siteWhere = string.Format("({0} IS NULL)", siteIdColumn);
        }

        return "(" + siteWhere + ")";
    }


    /// <summary>
    /// Returns url to categories page for given category ID.
    /// </summary>
    /// <param name="categoryId">ID of the category to create url for.</param>
    private string GetUrl(CategoryInfo category)
    {
        // Get target url
        string url = (String.IsNullOrEmpty(CategoriesPagePath) ? URLHelper.CurrentURL : CMSContext.GetUrl(CategoriesPagePath));

        // Append category parameter
        if (UseCodeNameInQuery)
        {
            url = URLHelper.AddParameterToUrl(url, "categoryname", category.CategoryName);
        }
        else
        {
            url = URLHelper.AddParameterToUrl(url, "categoryId", category.CategoryID.ToString());
        }
        
        return URLHelper.GetAbsoluteUrl(url);
    }


    private string GetCssClass(int level)
    {
        // returns CSS prefix for specified level of menu
        if (CSSPrefix.IndexOfCSafe(';') >= 0)
        {
            if (mCSSPrefixes == null)
            {
                mCSSPrefixes = CSSPrefix.Split(';');
            }
            if (mCSSPrefixes.GetUpperBound(0) >= level)
            {
                return mCSSPrefixes[level];
            }
            else
            {
                return mCSSPrefixes[mCSSPrefixes.GetUpperBound(0)];
            }
        }
        else
        {
            return CSSPrefix;
        }
    }

    #endregion
}