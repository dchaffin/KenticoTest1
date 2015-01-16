using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[RegisterTitle("com.ui.productsdocuments")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Products.Documents")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Documents : CMSProductsAdvancedPage
{
    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.Documents;

        if (ProductID > 0)
        {
            productDocuments.ProductID = ProductID;
            SKUInfo sku = SKUInfoProvider.GetSKUInfo(ProductID);
            EditedObject = sku;
        }

        // Set master title
        if (DisplayTitle)
        {
            CurrentMaster.Title.TitleText = GetString("com.ui.productsdocuments");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Document/object.png");
            CurrentMaster.Title.HelpTopicName = "cms_ecommerce_products_documents";
        }
    }

    #endregion
}