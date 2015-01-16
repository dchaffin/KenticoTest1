using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSSiteManager_Sites_Site_Edit : SiteManagerPage
{
    protected int siteId = 0;
    protected string currentWebSite = string.Empty;


    protected void Page_PreInit(object sender, EventArgs e)
    {
        CurrentMaster.Tabs.SelectedTab = QueryHelper.GetInteger("selectedtab", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("Site_Edit.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Site/object.png");
        CurrentMaster.Tabs.UseClientScript = true;
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "general_tab6";


        siteId = QueryHelper.GetInteger("siteid", 0);
        SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
        if (si != null)
        {
            currentWebSite = si.DisplayName;
        }

        if (!RequestHelper.IsPostBack())
        {
            InitalizeMenu();
        }

        CurrentMaster.SetRTL();

        InitializeBreadCrumbs();
    }


    /// <summary>
    /// Initializes site edit menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        if (siteId > 0)
        {
            string[,] tabs = new string[5, 4];
            tabs[0, 0] = GetString("general.general");
            tabs[0, 1] = "SetHelpTopic('helpTopic', 'general_tab6');";
            tabs[0, 2] = "Site_Edit_General.aspx?siteid=" + siteId;
            tabs[1, 0] = GetString("Administration-Site_Edit.DomainAliases");
            tabs[1, 1] = "SetHelpTopic('helpTopic', 'domain_aliases_tab');";
            tabs[1, 2] = "Site_Edit_DomainAliases.aspx?siteid=" + siteId;

            int index = 2;
            bool showCulturesTab = LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Multilingual);
            if (showCulturesTab)
            {
                tabs[index, 0] = GetString("Administration-Site_Edit.Cultures");
                tabs[index, 1] = "SetHelpTopic('helpTopic', 'cultures_tab');";
                tabs[index, 2] = "Site_Edit_Cultures.aspx?siteid=" + siteId;
                index++;
            }

            tabs[index, 0] = GetString("Administration-Site_Edit.OfflineMode");
            tabs[index, 1] = "SetHelpTopic('helpTopic', 'offline_mode_tab');";
            tabs[index, 2] = "Site_Edit_OfflineMode.aspx?siteid=" + siteId;

            index++;

            tabs[index, 0] = GetString("Administration-Site_Edit.SiteObjects");
            tabs[index, 1] = "SetHelpTopic('helpTopic', 'site_objects_tab');";
            tabs[index, 2] = URLHelper.ResolveUrl("Site_SiteObjects_Frameset.aspx?siteid=" + siteId);

            CurrentMaster.Tabs.Tabs = tabs;
            CurrentMaster.Tabs.UrlTarget = "content";
        }
    }


    /// <summary>
    /// Initializes BreadCrumbs.
    /// </summary>
    protected void InitializeBreadCrumbs()
    {
        // Initialize PageTitle
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("general.sites");
        pageTitleTabs[0, 1] = "~/CMSSiteManager/Sites/site_list.aspx";
        pageTitleTabs[0, 2] = "cmsdesktop";
        pageTitleTabs[1, 0] = currentWebSite;
        pageTitleTabs[1, 1] = string.Empty;
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }
}