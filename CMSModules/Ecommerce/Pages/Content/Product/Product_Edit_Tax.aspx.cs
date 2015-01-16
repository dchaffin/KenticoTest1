using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Content", UIElements = "ContentProduct.TaxClasses")]
public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_Tax : CMSContentProductPage
{
    #region "Variables"

    protected SKUInfo sku = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set help topic
        CurrentMaster.HeaderActions.HelpTopicName = "CMS_Ecommerce_Products_TaxClasses";

        if (Node != null)
        {
            sku = SKUInfoProvider.GetSKUInfo(Node.NodeSKUID);

            EditedObject = sku;

            if (sku != null)
            {
                // Check site ID
                CheckProductSiteID(sku.SKUSiteID);

                taxForm.ProductID = sku.SKUID;
                taxForm.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
            }
        }

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.TaxClasses;
    }

    #endregion


    #region "Methods"

    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        if (CheckProductPermissions(sku))
        {
            // Save chages
            taxForm.SaveItems();
        }
    }

    #endregion
}