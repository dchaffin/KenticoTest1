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

[EditedObject(SiteObjectType.BANNERCATEGORY, "categoryid")]

[Title("Objects/CMS_BannerCategory/object.png", "banner.general.editcategory", "banner_list")]

[Tabs("content")]
[Tab(0, "general.banners", "Category_Edit_Banners.aspx?categoryid={?categoryid?}&siteid={?siteid?}", "SetHelpTopic('helpTopic', 'banner_list');")]
[Tab(1, "general.general", "Category_Edit_General.aspx?categoryid={?categoryid?}&siteid={?siteid?}", "SetHelpTopic('helpTopic', 'banner_category_edit');")]
[Tab(2, "general.report", "~/CMSModules/Reporting/Tools/BannerManagement/Reports.aspx?statcodename=bannercategory&reportcodename=bannercategoryhitsclicks&parameterName=BannerCategoryID&parameterValue={?categoryid?}", "SetHelpTopic('helpTopic', 'banner_category_report');", Resource = "cms.reporting")]

[Breadcrumbs(2)]
[Breadcrumb(0, "banner.bannercategory_new.title", "Category_List.aspx?siteid={?siteid?}", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.BannerCategoryDisplayName%}")]

public partial class CMSModules_BannerManagement_Tools_Category_Category_Edit_Header : CMSBannerManagementEditPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}
