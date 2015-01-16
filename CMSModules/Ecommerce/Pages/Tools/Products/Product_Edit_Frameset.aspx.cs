using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.DocumentEngine;

public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Frameset : CMSProductPropertiesPage
{
    #region "Page methods"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not redirect when document does not exist
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnPreInit(e);

        int categoryId = QueryHelper.GetInteger("categoryid", 0);
        bool dialogMode = QueryHelper.GetBoolean("dialogmode", false);

        if (categoryId > 0)
        {
            IsProductOption = true;
        }

        if (dialogMode)
        {
            rowsFrameset.Attributes["rows"] = "29,*";
        }
        else
        {
            // Set different height of the header in product edit and product categories > product edit
            rowsFrameset.Attributes["rows"] = (categoryId > 0) ? "64,*" : "102,*";
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        bool isSplitMode = CMSContext.DisplaySplitMode;

        EnsureSplitModeCrossoverScript();

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "refreshtree", ScriptHelper.GetScript("function RefreshTree(expandNodeId, selectNodeId) { parent.RefreshTree(expandNodeId, selectNodeId); } function SelectNode(selectNodeId) { parent.SelectNode(selectNodeId); parent.SetMode('page',false); }"));

        bool combineWithDefaultCulture = !isSplitMode && SiteInfoProvider.CombineWithDefaultCulture(CMSContext.CurrentSiteName);

        string name = null;

        // When node ID specified
        if (NodeID > 0)
        {
            // Check if product node exists
            TreeNode node = Tree.SelectSingleNode(NodeID, CultureCode, combineWithDefaultCulture);
            if (node == null)
            {
                // Product does not exist -> redirect to new culture version creation dialog
                RedirectToNewCultureVersionPage();
            }
            else
            {
                // Get the name for automatic title
                name = (node.NodeAliasPath == "/" ? CMSContext.CurrentSite.DisplayName : node.GetDocumentName());
            }
        }
        else if (ProductID > 0)
        {
            SKUInfo sku = SKUInfoProvider.GetSKUInfo(ProductID);
            name = (sku != null) ? sku.SKUName : null;
        }

        if (!String.IsNullOrEmpty(name))
        {
            ScriptHelper.RegisterTitleScript(this, ResHelper.LocalizeString(name));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register wopener script
        ScriptHelper.RegisterWOpenerScript(Page);
    }

    #endregion
}