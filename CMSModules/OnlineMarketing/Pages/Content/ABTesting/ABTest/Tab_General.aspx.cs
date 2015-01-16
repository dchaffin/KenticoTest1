using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_Tab_General : CMSABTestContentPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        // Get the ID from query string
        editElem.ABTestID = QueryHelper.GetInteger("abTestId", 0);
        editElem.ShowAliasPath = false;

        base.OnInit(e);
    }

    #endregion
}