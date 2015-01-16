using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.GlobalHelper;

// Edited object
[EditedObject(AnalyticsObjectType.CONVERSION, "conversionID")]
// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "abtesting.conversions", "~/CMSModules/WebAnalytics/Pages/Tools/Conversion/List.aspx?displayreport={?displayreport?}", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}")]

// Empty title for creating context help
[Title("", "", "conversion_general")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Conversion_Header : CMSCampaignAndConversionPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int conversionID = QueryHelper.GetInteger("conversionID", 0);
        string[,] tabs = new string[3, 4];
        tabs[0, 0] = GetString("general.general");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'conversion_general');";
        tabs[0, 2] = "edit.aspx?conversionid=" + conversionID;

        tabs[1, 0] = GetString("analytics_codename.campaign");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'conversions_campaign');";
        tabs[1, 2] = "Tab_Campaigns.aspx?campaignid=" + conversionID;

        if (QueryHelper.GetBoolean("DisplayReport", false))
        {
            tabs[2, 0] = GetString("conversion.detail");
            tabs[2, 1] = "SetHelpTopic('helpTopic', 'conversions_detail');";
            tabs[2, 2] = ResolveUrl("~/CMSModules/WebAnalytics/Pages/Tools/Campaign/CampaignReport.aspx?statCodeName=conversions_detail&dataCodeName=conversiondetail&reportCodeName=conversiondetail&displayTitle=0&conversionid=" + conversionID);
        }

        CurrentMaster.Tabs.Tabs = tabs;
        CurrentMaster.Tabs.UrlTarget = "content";
    }
}