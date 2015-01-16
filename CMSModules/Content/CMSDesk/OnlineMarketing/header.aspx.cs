using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

using CMS.Controls.Configuration;

public partial class CMSModules_Content_CMSDesk_OnlineMarketing_header : CMSAnalyticsContentPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Tabs.OnTabCreated += tabElem_OnTabCreated;
        CurrentMaster.Tabs.ModuleName = "CMS.Content";
        CurrentMaster.Tabs.ElementName = "Analytics";
        CurrentMaster.Tabs.UrlTarget = "edit";

        // Initializes breadcrumbs 		
        string[,] pageTitleTabs = new string[1, 3];
        pageTitleTabs[0, 0] = GetString("content.ui.analytics");
        pageTitleTabs[0, 1] = string.Empty;
        pageTitleTabs[0, 2] = string.Empty;

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
        CurrentMaster.SetRTL();

        CurrentMaster.Title.HelpName = "helpTopic";

        // Must be set be to help icon created
        CurrentMaster.Title.HelpTopicName = "Analytics";

        CMSContext.ViewMode = ViewModeEnum.Analytics;
    }


    protected TabItem tabElem_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        string elementName = element.ElementName.ToLowerCSafe();
        string siteName = CMSContext.CurrentSiteName;
        if ((elementName == "onlinemarketing.languages") && (!CultureInfoProvider.IsSiteMultilignual(siteName) || !CultureInfoProvider.LicenseVersionCheck()))
        {
            return null;
        }

        if (elementName.StartsWithCSafe("onlinemarketing.") && (!ModuleEntry.IsModuleLoaded("cms.onlinemarketing")))
        {
            return null;
        }

        return tab;
    }
}