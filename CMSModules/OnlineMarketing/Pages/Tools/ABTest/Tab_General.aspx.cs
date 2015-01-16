using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_OnlineMarketing_Pages_Tools_AbTest_Tab_General : CMSABTestPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        String siteName = CMSContext.CurrentSiteName;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        // Get the ID from query string
        editElem.ABTestID = QueryHelper.GetInteger("abTestId", 0);

        base.OnInit(e);
    }

    #endregion
}