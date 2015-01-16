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

[RegisterTitle("com.ui.productstaxclasses")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Tax : CMSProductPropertiesPage
{
    #region "Variables"

    private SKUInfo sku = null;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        // Check if product is product option
        if (QueryHelper.GetInteger("categoryid", 0) > 0)
        {
            IsProductOption = true;
        }

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.TaxClasses;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI personalization for product / product option separately
        if (QueryHelper.GetInteger("categoryid", 0) > 0)
        {
            if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Ecommerce", "ProductOptions.Options.TaxClasses"))
            {
                RedirectToCMSDeskUIElementAccessDenied("CMS.Ecommerce", "ProductOptions.Options.TaxClasses");
            }
        }
        else
        {
            if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Ecommerce", "Products.TaxClasses"))
            {
                RedirectToCMSDeskUIElementAccessDenied("CMS.Ecommerce", "Products.TaxClasses");
            }
        }

        if (ProductID > 0)
        {
            sku = SKUInfoProvider.GetSKUInfo(ProductID);
            EditedObject = sku;

            if (sku != null)
            {
                // Check products site id
                CheckEditedObjectSiteID(sku.SKUSiteID);

                taxForm.ProductID = ProductID;
                taxForm.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
            }
        }
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


    /// <summary>
    /// Disables form editing.
    /// </summary>
    private void DisableFormEditing()
    {
        taxForm.Enabled = false;
    }

    #endregion
}