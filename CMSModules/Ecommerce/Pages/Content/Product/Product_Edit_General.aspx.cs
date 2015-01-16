using System;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

[RegisterTitle("com.ui.productsgeneral")]
[Security(Resource = "CMS.Content", UIElements = "EditForm")]
public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_General : CMSContentProductPage
{
    #region "Lifecycle"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        EnableSplitMode = true;

        // If not EditLive view mode, switch to form mode to keep editing the form
        if (CMSContext.ViewMode != ViewModeEnum.EditLive)
        {
            CMSContext.ViewMode = ViewModeEnum.EditForm;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SKUInfo sku = null;
        if (Node != null)
        {
            sku = SKUInfoProvider.GetSKUInfo(Node.NodeSKUID);
        }

        if ((sku != null) && (sku.SKUSiteID != CMSContext.CurrentSiteID) && ((sku.SKUSiteID != 0) || !ECommerceSettings.AllowGlobalProducts(CMSContext.CurrentSiteName)))
        {
            EditedObject = null;
        }

        productEditElem.ProductSaved += productEditElem_ProductSaved;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterScriptFile(Page, "cmsedit.js");
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "InitializePage", ScriptHelper.GetScript("InitializePage();"));
        ScriptHelper.RegisterProgress(Page);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the ProductEdit.ProductSaved event.
    /// </summary>
    protected void productEditElem_ProductSaved(object sender, EventArgs e)
    {
        if ((Action == "newculture") || (Action == "bindsku") || productEditElem.ProductSavedSkuBindingRemoved)
        {
            var node = (TreeNode)productEditElem.Product;

            ScriptHelper.RefreshTree(this, node.NodeID, node.NodeID);

            var selectNodeScript = ScriptHelper.GetScript(string.Format("SelectNode({0});", node.NodeID));
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "Refresh", selectNodeScript);
        }
    }

    #endregion
}