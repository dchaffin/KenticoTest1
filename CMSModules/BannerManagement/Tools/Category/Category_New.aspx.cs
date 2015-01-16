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
using CMS.UIControls;

[Title("Objects/CMS_BannerCategory/new.png", "banner.bannercategory_list.newcategory", "banner_category_new")]

[Breadcrumbs(2)]
[Breadcrumb(0, ResourceString = "banner.bannercategory_new.title", TargetUrl = "~/CMSModules/BannerManagement/Tools/Category/Category_List.aspx?siteid={?siteid?}")]
[Breadcrumb(1, "banner.bannercategory_new.new")]

public partial class CMSModules_BannerManagement_Tools_Category_Category_New : CMSBannerManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}
