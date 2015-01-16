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

// Edited object
[EditedObject(SiteObjectType.BANNER, "id")]

// Parent object
[ParentObject(SiteObjectType.BANNERCATEGORY, "categoryid")]
public partial class CMSModules_BannerManagement_Tools_Banner_Banner_Edit_General : CMSBannerManagementEditPage
{
}
