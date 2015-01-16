using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

[RegisterTitle("abtesting.abtest.list")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_List : CMSABTestContentPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled;CMSAnalyticsTrackConversions";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("webanalytics.tackconversionsdisabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        listElem.ShowFilter = false;
        UIContext.AnalyticsTab = AnalyticsTabEnum.ABTests;

        // Get the alias path of the current node
        if (Node != null)
        {
            listElem.NodeID = Node.NodeID;
            string aliasPath = Node.NodeAliasPath;
            listElem.AliasPath = aliasPath;

            // Prepare the actions
            string[,] actions = new string[1, 6];
            actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[0, 1] = ResHelper.GetString("abtesting.abtest.new");
            actions[0, 3] = ResolveUrl("new.aspx?nodeID=" + Node.NodeID + "&AliasPath=" + aliasPath);
            actions[0, 5] = GetImageUrl("Objects/OM_AbTest/add.png");

            // Set the actions
            ICMSMasterPage master = CurrentMaster;
            master.HeaderActions.Actions = actions;
        }
        else
        {
            EditedObject = null;
            listElem.StopProcessing = true;
        }
    }

    #endregion
}