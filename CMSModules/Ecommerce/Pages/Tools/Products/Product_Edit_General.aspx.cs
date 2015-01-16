using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[RegisterTitle("com.ui.productsgeneral")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_General : CMSProductPropertiesPage
{
    #region "Properties"

    /// <summary>
    /// Gets the value of the 'categoryId' URL parameter.
    /// </summary>
    public int OptionCategoryID
    {
        get
        {
            return QueryHelper.GetInteger("categoryId", 0);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CheckDocPermissions = true;

        IsProductOption = OptionCategoryID > 0;
        EnableSplitMode = NodeID > 0;
        UIContext.ProductTab = ProductTabEnum.General;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SKUInfo sku = SKUInfoProvider.GetSKUInfo(ProductID);
        if (sku != null)
        {
            CheckEditedObjectSiteID(sku.SKUSiteID);
        }

        if (IsProductOption)
        {
            // Check UI personalization for product option
            if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Ecommerce", "ProductOptions.Options.General"))
            {
                RedirectToCMSDeskUIElementAccessDenied("CMS.Ecommerce", "ProductOptions.Options.General");
            }
        }
        else
        {
            // Check UI personalization for product
            if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Ecommerce", "Products.General"))
            {
                RedirectToCMSDeskUIElementAccessDenied("CMS.Ecommerce", "Products.General");
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterScriptFile(Page, "cmsedit.js");
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "InitializePage", ScriptHelper.GetScript("InitializePage();"));
        ScriptHelper.RegisterProgress(Page);
    }

    #endregion
}
