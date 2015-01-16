using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls;

public partial class CMSModules_OnlineMarketing_Pages_Tools_ABVariant_List : CMSABTestPage
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

        int testID = QueryHelper.GetInteger("abTestId", 0);
        listElem.TestID = testID;
        // Prepare the actions
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = ResHelper.GetString("abtesting.variant.new");
        actions[0, 3] = ResolveUrl("Edit.aspx?abtestID=" + testID);
        actions[0, 5] = GetImageUrl("Objects/CMS_Variant/add.png");

        // Set the actions
        ICMSMasterPage master = CurrentMaster;
        master.HeaderActions.Actions = actions;
    }

    #endregion
}