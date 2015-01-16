using System;

using CMS.Ecommerce;
using CMS.UIControls;

[Title("Objects/CMS_VersionHistory/object.png", "com.ui.productsversions", "cms_ecommerce_products_versions")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Products.Versions")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Versions : CMSProductsAdvancedPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.Versions;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        DocumentManager.LocalDocumentPanel = pnlDocInfo;
    }
}
