using System;

using CMS.Controls.Configuration;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[RegisterTitle("Site Manager")]
public partial class CMSSiteManager_Default : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        paneFooter.Visible = SettingsKeyProvider.DevelopmentMode;
        OnTabCreated += new UITabs.TabCreatedEventHandler(tabControl_OnTabCreated);
    }


    public TabItem tabControl_OnTabCreated(UIElementInfo uiElement, TabItem tab, int tabIndex)
    {
        switch (uiElement.ElementName.ToLowerCSafe())
        {
            case "tools":
                // Hide "Tools" tab if none of modules is available (E-commerce or Contact management)
                bool toolsAvailable = LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Ecommerce, ModuleEntry.ECOMMERCE) || LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.ContactManagement, ModuleEntry.ONLINEMARKETING);
                if (!toolsAvailable)
                {
                    return null;
                }
                break;

            case "buy":
                if (!LicenseKeyInfoProvider.OnlyTrialLicenseKeys) 
                {
                    return null;
                }
                break;

            case "dashboard":
                // Add hash to url
                tab.RedirectUrl = URLHelper.ResolveUrl(URLHelper.EnsureHashToQueryParameters(uiElement.ElementTargetURL));
                break;
        }

        return tab;
    }
}