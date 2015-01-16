using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_Edit : CMSMVTestContentPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        String siteName = CMSContext.CurrentSiteName;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSMVTEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("mvt.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        // Get the alias path of the current node
        if (Node != null)
        {
            editElem.AliasPath = Node.NodeAliasPath;
        }
        else
        {
            editElem.StopProcessing = true;
        }

        if (QueryHelper.GetBoolean("saved", false))
        {
            ScriptHelper.RefreshTabHeader(Page, null);        
        }

        editElem.MVTestID = QueryHelper.GetInteger("mvtestId", 0);
        editElem.OnSaved += new EventHandler(editElem_OnSaved);
    }


    protected void editElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Edit.aspx?saved=1&mvtestId=" + editElem.ItemID + "&nodeid=" + NodeID);
    }

    #endregion
}