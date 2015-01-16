using System;

using CMS.Controls.Configuration;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Tabs("CMS.Ecommerce", "Configuration", "configEdit")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Configuration_Header : CMSEcommerceSharedConfigurationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Tabs.OnTabCreated += Tabs_OnTabCreated;
        master.PanelBody.CssClass += " Separator";
        master.SetRTL();

        AddMenuButtonSelectScript("Configuration", "");

        ScriptHelper.RegisterTitleScript(this, GetString("ecommerce.configuration"));
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        int siteId = SiteID;

        // Skip some elements if editing global configuration
        if (siteId == 0)
        {
            switch (element.ElementName.ToLowerCSafe())
            {
                case "configuration.departments":
                case "configuration.shippingoptions":
                case "configuration.paymentmethods":
                    return null;
            }
        }

        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl) && (siteId != CMSContext.CurrentSiteID))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "SiteId", siteId.ToString());
        }

        return tab;
    }
}