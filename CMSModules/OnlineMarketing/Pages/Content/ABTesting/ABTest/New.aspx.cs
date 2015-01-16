using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_New : CMSABTestContentPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        String siteName = CMSContext.CurrentSiteName;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        // Prepare the breadcrumbs
        string[,] breadcrumbs = new string[2,3];
        breadcrumbs[0, 0] = GetString("abtesting.abtest.list");
        breadcrumbs[0, 1] = "~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABTest/List.aspx?nodeid=" + NodeID;
        breadcrumbs[1, 0] = GetString("abtesting.abtest.new");

        // Set the title
        PageTitle title = CurrentMaster.Title;
        title.Breadcrumbs = breadcrumbs;
        title.HelpTopicName = "abtest_general";

        editElem.AliasPath = QueryHelper.GetString("AliasPath", String.Empty);

        editElem.OnSaved += new EventHandler(editElem_OnSaved);
    }


    protected void editElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Frameset.aspx?saved=1&abTestId=" + editElem.ItemID + "&nodeID=" + NodeID);
    }

    #endregion
}