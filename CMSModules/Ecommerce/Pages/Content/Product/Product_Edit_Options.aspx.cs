using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Content", UIElements = "ContentProduct.Options")]
public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_Options : CMSContentProductPage
{
    #region "Variables"

    protected SKUInfo sku = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set help topic
        CurrentMaster.HeaderActions.HelpTopicName = "cms_ecommerce_products_options";

        if (Node != null)
        {
            sku = SKUInfoProvider.GetSKUInfo(Node.NodeSKUID);

            EditedObject = sku;

            if (sku != null)
            {
                // Check site ID
                CheckProductSiteID(sku.SKUSiteID);

                ucOptions.ProductID = sku.SKUID;
                ucOptions.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
            }
        }

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.Options;
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Check permissions
        if (CheckProductPermissions(sku))
        {
            ucOptions.SaveItems();
        }
    }

    #endregion
}