using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[EditedObject(SiteObjectType.BANNER, "id")]

[Help("banner_edit", "helpTopic2")]

[Tabs("content")]
[Tab(0, "general.general", "Banner_Edit_General.aspx?id={?id?}&categoryid={?categoryid?}&siteid={?siteid?}", "SetHelpTopic('helpTopic2', 'banner_edit');")]
[Tab(1, "general.report", "~/CMSModules/Reporting/Tools/BannerManagement/Reports.aspx?statcodename=banner&reportcodename=bannerhitsclicks.yearreport;bannerhitsclicks.monthreport;bannerhitsclicks.weekreport;bannerhitsclicks.dayreport;bannerhitsclicks.hourreport&parameterName=BannerId&parameterValue={?id?}", "SetHelpTopic('helpTopic2', 'banner_report');", Resource = "cms.reporting")]

[Breadcrumbs(2)]
[Breadcrumb(0, "general.banners", "~/CMSModules/BannerManagement/Tools/Category/Category_Edit_Banners.aspx?siteid={?siteid?}&categoryid={?categoryid?}", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.BannerDisplayName%}")]

public partial class CMSModules_BannerManagement_Tools_Banner_Banner_Edit_Header : CMSBannerManagementEditPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}
