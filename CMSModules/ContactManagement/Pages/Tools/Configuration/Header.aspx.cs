using System;

using CMS.Controls.Configuration;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.SiteProvider;
using CMS.SettingsProvider;

// Tabs
[Tabs("CMS.OnlineMarketing", "Configuration", "configurationContent")]
public partial class CMSModules_ContactManagement_Pages_Tools_Configuration_Header : CMSContactManagementConfigurationPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ((CMSMasterPage)CurrentMaster).PanelBody.AddCssClass("Separator");

        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Configuration", null, "menu");
        OnTabCreated += Page_OnTabCreated;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Join URL queries if needed
        string query = URLHelper.Url.Query;
        if (!String.IsNullOrEmpty(query))
        {
            UITabs tabs = ((CMSMasterPage)CurrentMaster).Tabs;
            foreach (TabItem t in tabs.TabItems)
            {
                if (!String.IsNullOrEmpty(t.RedirectUrl))
                {
                    t.RedirectUrl = URLHelper.AppendQuery(t.RedirectUrl, query);
                }
            }
        }
    }

    // TODO JaroslavK: Po presunuti prijde odstranit
    protected TabItem Page_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Hide actions tab
        if (element.ElementName.EqualsCSafe("Automation_actions", true) && !CurrentUser.IsGlobalAdministrator)
        {
            return null;
        }
        return tab;
    }

    #endregion
}