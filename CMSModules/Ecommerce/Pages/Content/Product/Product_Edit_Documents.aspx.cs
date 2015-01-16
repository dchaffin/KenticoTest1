using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;

[Security(Resource = "CMS.Content", UIElements = "ContentProduct.Documents")]
public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_Documents : CMSContentProductPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set help topic
        CurrentMaster.HeaderActions.HelpTopicName = "cms_ecommerce_products_documents";

        if (Node != null)
        {
            SKUInfo sku = SKUInfoProvider.GetSKUInfo(Node.NodeSKUID);
            EditedObject = sku;

            if (sku != null)
            {
                // Check site ID
                CheckProductSiteID(sku.SKUSiteID);

                productDocuments.ProductID = sku.SKUID;
            }
        }

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.Documents;
    }
}