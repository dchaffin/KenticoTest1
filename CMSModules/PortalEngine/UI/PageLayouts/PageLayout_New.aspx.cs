using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Breadcrumbs(2)]
[Breadcrumb(0, "administration-pagelayout_list.title", "PageLayout_List.aspx", null)]
[Breadcrumb(1, "administration-pagelayout_new.title")]
public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_New : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("administration-pagelayout_new.title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Layout/new.png");
        CurrentMaster.Title.HelpTopicName = "CMS_Design_page_layout";
        CurrentMaster.Title.HelpName = "helpTopic";
    }
}