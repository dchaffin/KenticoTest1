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
using CMS.SiteProvider;
using CMS.CMSHelper;

[Breadcrumbs(2)]
[Breadcrumb(0, "general.banners", TargetUrl = "~/CMSModules/BannerManagement/Tools/Category/Category_Edit_Banners.aspx?siteid={?siteid?}&categoryid={?categoryid?}")]
[Breadcrumb(1, "banner.bannercategory_edit_banner.newbanner")]

[Help("banner_new")]

// Parent object
[ParentObject(SiteObjectType.BANNERCATEGORY, "categoryid")]
public partial class CMSModules_BannerManagement_Tools_Banner_Banner_New : CMSBannerManagementEditPage
{
    #region "Properties"

    protected override int? EditedSiteID
    {
        get
        {
            return ((BannerCategoryInfo)CMSContext.EditedObjectParent).BannerCategorySiteID;
        }
    }

    #endregion
}
