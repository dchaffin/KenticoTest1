using System;
using System.Linq;
using System.Web.UI;

using CMS.Controls.Configuration;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Tabs("CMS.Content", "Product", "ProductContent")]
public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_Header : CMSContentProductPage
{
    private string selectedTab = string.Empty;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register script file
        ScriptHelper.RegisterScriptFile(Page, "cmsedit.js");

        selectedTab = DataHelper.GetNotEmpty(QueryHelper.GetString("tab", String.Empty).ToLowerCSafe(), ProductTabCode.ToString(UIContext.ProductTab).ToLowerCSafe());

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.PanelBody.CssClass += " Separator";
        master.Tabs.OnTabCreated += Tabs_OnTabCreated;
        master.SetRTL();
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        string lowerElementName = element.ElementName.ToLowerCSafe();

        // Ensure tab preselection
        if (lowerElementName.StartsWithCSafe("contentproduct.") &&
            (lowerElementName.Substring("contentproduct.".Length) == selectedTab))
        {
            CurrentMaster.Tabs.SelectedTab = tabIndex;
        }

        return tab;
    }
}