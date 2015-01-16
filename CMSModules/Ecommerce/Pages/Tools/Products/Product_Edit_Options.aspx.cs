using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[RegisterTitle("optioncategory_edit.itemlistlink")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Products.Options")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Options : CMSProductsAdvancedPage
{
    #region "Variables"

    private SKUInfo sku = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.Options;

        if (ProductID > 0)
        {
            sku = SKUInfoProvider.GetSKUInfo(ProductID);

            EditedObject = sku;

            if (sku != null)
            {
                // Check site ID
                CheckEditedObjectSiteID(sku.SKUSiteID);

                ucOptions.ProductID = ProductID;
                ucOptions.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
            }
        }

        // Set master title
        if (DisplayTitle)
        {
            CurrentMaster.Title.TitleText = GetString("optioncategory_edit.itemlistlink");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Ecommerce_OptionCategory/object.png");
            CurrentMaster.Title.HelpTopicName = "cms_ecommerce_products_options";
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Disables form editing.
    /// </summary>
    private void DisableFormEditing()
    {
        ucOptions.Enabled = false;
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