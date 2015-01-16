using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls.Configuration;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Tabs("CMS.Ecommerce", "Configuration.Settings", "storesettingsContent")]
[Title("CMSModules/CMS_Ecommerce/storesettings.png", "Store_Settings.HeaderCaption", "genral_tab")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_Header : CMSEcommerceStoreSettingsPage
{
    private UserInfo currentUser = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = CMSContext.CurrentUser;

        // Initialize tab handler
        CurrentMaster.Tabs.OnTabCreated += Tabs_OnTabCreated;
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Global objects tab will be displayed only to global admin
        if (!currentUser.IsGlobalAdministrator && (element.ElementName.ToLowerCSafe() == "configuration.settings.globalobjects"))
        {
            return null;
        }

        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "siteId", SiteID.ToString());
        }

        return tab;
    }
}