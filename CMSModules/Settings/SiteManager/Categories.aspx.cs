using System;
using System.Data;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Settings_SiteManager_Categories : SiteManagerPage
{
    protected int mSiteId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        // URL for tree selection
        string script = "var categoryURL = '" + ResolveUrl("keys.aspx") + "';\n";
        script += "var doNotReloadContent = false;\n";

        // Get selected site id
        mSiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        TreeViewCategories.SiteID = mSiteId;
        TreeViewCategories.RootIsClickable = true;

        int categoryId = 0;
        if (Request.Params["selectedCategoryId"] != null)
        {
            // Selected category
            categoryId = ValidationHelper.GetInteger(Request.Params["selectedCategoryId"], 0);
        }
        else
        {
            // First request to Settings
            categoryId = SettingsCategoryInfoProvider.GetRootSettingsCategoryInfo().CategoryID;
        }


        bool reload = QueryHelper.GetBoolean("reload", true);

        // Select category
        SettingsCategoryInfo sci = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(categoryId);
        if (sci != null)
        {
            // Stop reloading of right frame, if explicitly set
            if (!reload)
            {
                script += "doNotReloadContent = true;";
            }
            script += SelectAtferLoad(sci.CategoryIDPath, sci.CategoryName, sci.CategoryID, sci.CategoryParentID);
        }
        
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SelectCat", ScriptHelper.GetScript(script));

        // Style site selector 
        siteSelector.DropDownSingleSelect.CssClass = "";
        siteSelector.DropDownSingleSelect.Attributes.Add("style", "width: 100%");
        lblSite.Text = GetString("general.site") + ResHelper.Colon;

        // Set site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.AllowAll = false;
        siteSelector.UniSelector.SpecialFields = new string[1, 2] { { GetString("general.global"), "0" } };
        siteSelector.OnlyRunningSites = false;
    }


    /// <summary>
    /// Reloads tree content.
    /// </summary>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        // Reload tree after selected site has changed.
        if (RequestHelper.IsPostBack())
        {
            TreeViewCategories.ReloadData();
        }
    }


    /// <summary>
    /// Expands tree at specified path and selects tree item by javascript.
    /// </summary>
    /// <param name="path">Selected path</param>
    /// <param name="itemName">Codename of selected tree item</param>
    /// <param name="itemId">Item identifier</param>
    /// <param name="parentId">ID of parent</param>    
    private string SelectAtferLoad(string path, string itemName, int itemId, int parentId)
    {
        TreeViewCategories.SelectPath = path;
        TreeViewCategories.SelectedItem = itemName;
        string script = String.Format("SelectNode('{0}'); NodeSelected('', {2}, {3}, {1});", itemName, parentId, itemId, mSiteId);
        return script;
    }
}