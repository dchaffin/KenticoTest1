using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls.Configuration;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[RegisterTitle("com.ui.productsadvanced")]
[Tabs("CMS.Ecommerce", "Products.Advanced", "advancedEdit")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Advanced_Header : CMSProductsAdvancedPage
{
    private string selectedTab = null;
    protected SKUInfo sku = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get product info
        if (ProductID > 0)
        {
            sku = SKUInfoProvider.GetSKUInfo(ProductID);
        }

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Tabs.OnTabCreated += Tabs_OnTabCreated;

        selectedTab = DataHelper.GetNotEmpty(QueryHelper.GetString("tab", String.Empty).ToLowerCSafe(), ProductTabCode.ToString(UIContext.ProductTab).ToLowerCSafe());

        master.PanelBody.CssClass += " Separator";
        master.SetRTL();
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        int siteId = SiteID;
        bool splitViewSupported = false;
        string lowerElementName = element.ElementName.ToLowerCSafe();

        // Skip some elements if editing just SKU without document binding
        switch (lowerElementName)
        {
            case "products.relatedproducts":
                if (NodeID <= 0)
                {
                    return null;
                }

                break;

            case "products.documents":
                if ((NodeID <= 0) && DisplayTreeInProducts)
                {
                    if (!CurrentUser.IsGlobalAdministrator || (sku == null) || !sku.IsGlobal)
                    {
                        return null;
                    }
                }

                break;

            case "products.workflow":
            case "products.versions":
                splitViewSupported = true;
                if ((NodeID <= 0) || (DocumentManager.Workflow == null))
                {
                    return null;
                }

                break;
        }

        // Ensure tab preselection
        if (lowerElementName.StartsWithCSafe("products.") && (lowerElementName.Substring("products.".Length) == selectedTab))
        {
            CurrentMaster.Tabs.SelectedTab = tabIndex;
        }

        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl) && (siteId != CMSContext.CurrentSiteID))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "siteId", siteId.ToString());
        }

        // Ensure split view mode
        if (splitViewSupported && CMSContext.DisplaySplitMode)
        {
            tab.RedirectUrl = GetSplitViewUrl(tab.RedirectUrl);
        }

        return tab;
    }
}
