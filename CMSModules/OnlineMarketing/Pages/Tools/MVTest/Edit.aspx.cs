using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_Pages_Tools_MVTest_Edit : CMSMVTestPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.OnSaved += new EventHandler(editElem_OnSaved);

        int testId = QueryHelper.GetInteger("mvtestId", 0);
        PageTitle title = CurrentMaster.Title;

        editElem.MVTestID = testId;

        // Prepare the header
        string name = "";
        title.HelpTopicName = "mvtest_general";

        if (editElem.MvtestObj != null)
        {
            name = editElem.MvtestObj.MVTestDisplayName;
        }
        else
        {
            name = GetString("mvtest.new");
        }

        // Prepare the breadcrumbs
        string[,] breadcrumbs = new string[2,3];
        breadcrumbs[0, 0] = GetString("mvtest.list");
        breadcrumbs[0, 1] = URLHelper.ResolveUrl("~/CMSModules/OnlineMarketing/Pages/Tools/MVTest/List.aspx");
        breadcrumbs[1, 0] = name;

        // Set the title
        title.Breadcrumbs = breadcrumbs;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSMVTEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("mvt.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;
    }


    protected void editElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Edit.aspx?saved=1&mvtestId=" + editElem.ItemID);
    }

    #endregion
}