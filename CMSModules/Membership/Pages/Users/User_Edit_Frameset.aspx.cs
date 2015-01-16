using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;

public partial class CMSModules_Membership_Pages_Users_User_Edit_Frameset : CMSUsersPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UserInfoProvider.CheckLicense();

        // Page is in popup mode (displaying tabs only)
        if (QueryHelper.GetBoolean("popup", false))
        {
            frmsss.FrameHeight = TabsOnlyHeight;
        }
        // Page in UI (breadcrumbs and Tabs)
        else
        {
            frmsss.FrameHeight = TabsBreadFrameHeight;
        }
    }
}