using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;


public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignHeader : CMSCampaignAndConversionPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query parameters
        string tabName = GetString(QueryHelper.GetText("displayTab", String.Empty));
        string statName = QueryHelper.GetString("statCodeName", String.Empty);
        string titleText = GetString("analytics_codename.campaign");

        // Create topic based on statname, for statname 'campaigns' use default topic name
        string helpTopicName = Server.UrlEncode(QueryHelper.GetString("statCodeName", String.Empty)).Replace(".", "_");
        if (string.IsNullOrEmpty(helpTopicName) || (statName == "campaigns"))
        {
            helpTopicName = "campaign_overview";
        }

        // Create title based on stat name, use default for 'campaigns'
        if (statName != "campaigns")
        {
            titleText += " - " + GetString(statName);
        }

        bool displayReport = QueryHelper.GetBoolean("displayReport", false);

        string[,] tabs = new string[2, 4];
        tabs[0, 0] = (tabName != String.Empty) ? tabName : GetString("general.report");
        tabs[0, 1] = "SetHelpTopic('helpTopic', '" + helpTopicName + "');";
        tabs[0, 2] = ResolveUrl("CampaignReport.aspx" + URLHelper.Url.Query + "&displayTitle=0");

        tabs[1, 0] = GetString("analytics_codename.campaign");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'campaign_list');";
        tabs[1, 2] = "list.aspx?displayReport=" + displayReport;

        CurrentMaster.Tabs.Tabs = tabs;
        CurrentMaster.Tabs.UrlTarget = "content";

        PageTitle title = CurrentMaster.Title;
        title.TitleText = titleText;

        // Set icon
        string iconName = QueryHelper.GetString("icon", String.Empty);
        string imageUrl = GetImageUrl("CMSModules/CMS_WebAnalytics/Details/" + iconName + ".png");

        if (!FileHelper.FileExists(imageUrl))
        {
            imageUrl = GetImageUrl("Objects/Reporting_ReportCategory/object.png");
        }

        title.TitleImage = imageUrl;
        title.HelpTopicName = helpTopicName;
        title.HelpName = "helpTopic";

        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Campaigns", null, "menu");
    }
}