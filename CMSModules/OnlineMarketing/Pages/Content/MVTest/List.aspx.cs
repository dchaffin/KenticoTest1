using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.ExtendedControls;

[RegisterTitle("mvtest.list")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_List : CMSMVTestContentPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        listElem.EditPage = "frameset.aspx";

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSMVTEnabled;CMSAnalyticsTrackConversions";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("mvt.disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("webanalytics.tackconversionsdisabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;
        
        // Get the alias path of the current node
        if (Node != null)
        {
            // Set NodeID in order to check the access to the document
            listElem.NodeID = Node.NodeID;
            listElem.AliasPath = Node.NodeAliasPath;

            // Prepare the actions
            string[,] actions = new string[1, 6];
            actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[0, 1] = GetString("mvtest.new");
            actions[0, 3] = ResolveUrl("new.aspx?nodeid=" + Node.NodeID);
            actions[0, 5] = GetImageUrl("Objects/OM_MVTest/add.png");

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