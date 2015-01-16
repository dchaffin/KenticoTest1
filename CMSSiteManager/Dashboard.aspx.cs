using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSSiteManager_Dashboard : DashboardPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Get current user info
        CurrentUserInfo currentUser = CMSContext.CurrentUser;

        // Check whether user is global admin
        if (currentUser.UserSiteManagerAdmin)
        {
            ucDashboard.PortalPageInstance = this as PortalPage;
            ucDashboard.TagsLiteral = ltlTags;

            ucDashboard.SetupDashboard();
        }
        // For non-global admin redirect to access denied
        else
        {
            URLHelper.Redirect(UIHelper.GetAccessDeniedUrl("accessdeniedtopage.sitemanagerdenied"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }
}