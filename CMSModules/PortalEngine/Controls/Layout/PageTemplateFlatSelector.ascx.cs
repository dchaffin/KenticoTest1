using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_PortalEngine_Controls_Layout_PageTemplateFlatSelector : CMSAdminControl
{
    #region "Variables"

    private PageTemplateCategoryInfo mSelectedCategory = null;
    private string mTreeSelectedItem = null;
    private bool mShowOnlyReusable = true;
    private int mSiteId = 0;
    private int mDocumentID = 0;
    private bool mIsNewPage = false;

    #endregion


    #region "Page template properties"

    /// <summary>
    /// Idticates whether only reusable templates (not in AdHoc cateogory) should be displayed.
    /// </summary>
    public bool ShowOnlyReusable
    {
        get
        {
            return mShowOnlyReusable;
        }
        set
        {
            mShowOnlyReusable = value;
        }
    }


    /// <summary>
    /// Indicates whether only templates for product section should be displayed.
    /// </summary>
    public bool ShowOnlyProductSectionTemplates
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the site id. Only templates of this site will be diplayed if set.
    /// </summary>
    public int SiteId
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets document id.
    /// </summary>
    public int DocumentID
    {
        get
        {
            return mDocumentID;
        }
        set
        {
            mDocumentID = value;
        }
    }


    /// <summary>
    /// Whether selecting new page.
    /// </summary>
    public bool IsNewPage
    {
        get
        {
            return mIsNewPage;
        }
        set
        {
            mIsNewPage = value;
        }
    }


    /// <summary>
    /// Extra where condition on templates
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Node GUID, if set, also templates from the given NodeGUID are included
    /// </summary>
    public Guid NodeGUID
    {
        get;
        set;
    }

    #endregion


    #region "Flat selector properties"

    /// <summary>
    /// Retruns inner instance of UniFlatSelector control.
    /// </summary>
    public UniFlatSelector UniFlatSelector
    {
        get
        {
            return flatElem;
        }
    }

    /// <summary>
    /// Gets or sets selected item in flat selector.
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return flatElem.SelectedItem;
        }
        set
        {
            flatElem.SelectedItem = value;
        }
    }


    /// <summary>
    /// Gets or sets the current page template category.
    /// </summary>
    public PageTemplateCategoryInfo SelectedCategory
    {
        get
        {
            // If not loaded yet
            if (mSelectedCategory == null)
            {
                int categoryId = ValidationHelper.GetInteger(TreeSelectedItem, 0);
                if (categoryId > 0)
                {
                    mSelectedCategory = PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo(categoryId);
                }
            }

            return mSelectedCategory;
        }
        set
        {
            mSelectedCategory = value;
            // Update ID
            if (mSelectedCategory != null)
            {
                mTreeSelectedItem = mSelectedCategory.CategoryId.ToString();
            }
        }
    }


    /// <summary>
    /// Gets or sets the selected item in tree, ususaly the category id.
    /// </summary>
    public string TreeSelectedItem
    {
        get
        {
            return mTreeSelectedItem;
        }
        set
        {
            // Clear loaded category if change
            if (value != mTreeSelectedItem)
            {
                mSelectedCategory = null;
            }
            mTreeSelectedItem = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            flatElem.StopProcessing = value;
            EnableViewState = !value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Register jquery
        ScriptHelper.RegisterJQuery(Page);

        // Setup flat selector
        flatElem.QueryName = "cms.pagetemplate.selectall";
        flatElem.ValueColumn = "PageTemplateID";
        flatElem.SearchLabelResourceString = "pagetemplates.templatename";
        flatElem.SearchColumn = "PageTemplateDisplayName";
        flatElem.SelectedColumns = "PageTemplateCodeName, PageTemplateThumbnailGUID, PageTemplateDisplayName, PageTemplateID";
        flatElem.OrderBy = "PageTemplateDisplayName";
        flatElem.NotAvailableImageUrl = GetImageUrl("Objects/CMS_PageTemplate/notavailable.png");
        flatElem.NoRecordsMessage = "pagetemplates.norecordsincategory";
        flatElem.NoRecordsSearchMessage = "pagetemplates.norecords";

        flatElem.OnItemSelected += new UniFlatSelector.ItemSelectedEventHandler(flatElem_OnItemSelected);
    }


    /// <summary>
    /// On PreRender.
    /// </summary>    
    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        string where = flatElem.WhereCondition;

        // Show only reusable templates
        if (ShowOnlyReusable)
        {
            where = SqlHelperClass.AddWhereCondition(where, "PageTemplateIsReusable = 1");
        }

        if (ShowOnlyProductSectionTemplates)
        {
            where = SqlHelperClass.AddWhereCondition(where, "PageTemplateIsAllowedForProductSection = 1");
        }

        // Show only templates of this site
        if (SiteId > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "PageTemplateID IN (SELECT PageTemplateID FROM CMS_PageTemplateSite WHERE SiteID = " + SiteId + ")");
        }

        // Node GUID condition
        if (NodeGUID != Guid.Empty)
        {
            where = SqlHelperClass.AddWhereCondition(where, "PageTemplateNodeGUID = '" + NodeGUID + "' AND PageTemplateSiteID = " + SiteId, "OR");
        }

        // Do not display dashboard items
        where = SqlHelperClass.AddWhereCondition(where, "PageTemplateType IS NULL OR PageTemplateType <> N'" + PageTemplateInfoProvider.GetPageTemplateTypeCode(PageTemplateTypeEnum.Dashboard) + "'");

        // Restrict to items in selected category (if not root)
        if ((SelectedCategory != null) && (SelectedCategory.ParentId > 0))
        {
            where = SqlHelperClass.AddWhereCondition(where, "PageTemplateCategoryID = " + SelectedCategory.CategoryId + " OR PageTemplateCategoryID IN (SELECT CategoryID FROM CMS_PageTemplateCategory WHERE CategoryPath LIKE N'" + SqlHelperClass.GetSafeQueryString(SelectedCategory.CategoryPath, false) + "/%')");
        }

        // Add extra where condition
        where = SqlHelperClass.AddWhereCondition(where, WhereCondition);

        TreeProvider tp = new TreeProvider(CMSContext.CurrentUser);
        TreeNode node = DocumentHelper.GetDocument(DocumentID, tp);

        string culture = CMSContext.PreferredCultureCode;

        if (node != null)
        {
            int level = node.NodeLevel;
            string path = node.NodeAliasPath;

            if (IsNewPage)
            {
                level++;
                path = path + "/%";
            }
            else
            {
                culture = node.DocumentCulture;
            }

            string className = node.NodeClassName;

            // Check if class id is in query string - then use it's value instead of document class name 
            int classID = QueryHelper.GetInteger("classid", 0);
            if (classID != 0)
            {
                DataClassInfo dci = DataClassInfoProvider.GetDataClass(classID);
                if (dci != null)
                {
                    className = dci.ClassName;
                }
            }

            // Add scopes condition
            string scopeWhere = PageTemplateScopeInfoProvider.GetScopeWhereCondition(path, culture, className, level, CMSContext.CurrentSiteName, "View_CMS_PageTemplateMetafile_Joined", "PageTemplateID");
            where = SqlHelperClass.AddWhereCondition(where, scopeWhere);
        }

        flatElem.WhereCondition = where;

        // Description area
        litCategory.Text = ShowInDescriptionArea(SelectedItem);

        base.OnPreRender(e);
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Updates description after item is selected in flat selector.
    /// </summary>
    protected string flatElem_OnItemSelected(string selectedValue)
    {
        return ShowInDescriptionArea(selectedValue);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        flatElem.ReloadData();
        flatElem.ResetToDefault();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Generates HTML text to be used in description area.
    /// </summary>
    ///<param name="selectedValue">Selected item for which generate description</param>
    private string ShowInDescriptionArea(string selectedValue)
    {
        string name = String.Empty;
        string description = String.Empty;
        string portal = String.Empty;
        string reusable = String.Empty;

        if (!String.IsNullOrEmpty(selectedValue))
        {
            int templateId = ValidationHelper.GetInteger(selectedValue, 0);

            // Get the template data
            DataSet ds = PageTemplateInfoProvider.GetTemplates("PageTemplateID = " + templateId, null, 0, "PageTemplateDisplayName, PageTemplateDescription, PageTemplateIsPortal, PageTemplateIsReusable");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                DataRow dr = ds.Tables[0].Rows[0];

                name = ResHelper.LocalizeString(ValidationHelper.GetString(dr["PageTemplateDisplayName"], ""));
                description = ResHelper.LocalizeString(ValidationHelper.GetString(dr["PageTemplateDescription"], ""));
                portal = ValidationHelper.GetBoolean(dr["PageTemplateIsPortal"], false).ToString().ToLowerCSafe();
                reusable = ValidationHelper.GetBoolean(dr["PageTemplateIsReusable"], false).ToString().ToLowerCSafe();
            }
        }
        // No selection show selected category
        else if (SelectedCategory != null)
        {
            name = SelectedCategory.DisplayName;
        }

        string text = "<div class=\"ItemName\">" + HTMLHelper.HTMLEncode(name) + "</div>";
        if (description != null)
        {
            text += "<div class=\"Description\">" + HTMLHelper.HTMLEncode(description) + "</div>";
        }

        text += "<input type=\"hidden\" id=\"selectedTemplateIsPortal\" value=\"" + portal + "\" />";
        text += "<input type=\"hidden\" id=\"selectedTemplateIsReusable\" value=\"" + reusable + "\" />";

        return text;
    }


    /// <summary>
    /// Add a reload script to the page which will update the page size (items count) according to the window size.
    /// </summary>
    /// <param name="forceResize">Indicates whether to invoke resizing of the page before calculating the items count</param>
    public void RegisterRefreshPageSizeScript(bool forceResize)
    {
        flatElem.RegisterRefreshPageSizeScript(forceResize);
    }

    #endregion
}