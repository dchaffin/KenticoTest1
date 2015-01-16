using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.MVTEST, "mvtestid")]
// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "mvtest.list", "List.aspx?nodeid={?nodeid?}", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}")]
// Tabs
[Tabs("content")]
[Tab(0, "general.general", "edit.aspx?mvtestid={%EditedObject.ID%}&nodeid={?nodeid?}", "SetHelpTopic('helpTopic', 'mvtest_general');")]
[Tab(1, "general.reports", "MVTestReport.aspx?mvtestid={%EditedObject.ID%}&nodeid={?nodeid?}", "SetHelpTopic('helpTopic', 'mvtest_reports');")]
// Create empty title for context help
[Title("", "", "mvtest_general")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_Header : CMSMVTestContentPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}