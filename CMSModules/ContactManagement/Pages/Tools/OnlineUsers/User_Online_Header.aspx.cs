using System;

using CMS.CMSHelper;
using CMS.UIControls;
using CMS.OnlineMarketing;

[Breadcrumbs(1)]
[Breadcrumb(0, "administration.users_header.onlineusers")]
public partial class CMSModules_ContactManagement_Pages_Tools_OnlineUsers_User_Online_Header : CMSContactManagementPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        if (SessionManager.StoreOnlineUsersInDatabase)
        {
            string[,] tabs = new string[2, 4];
            tabs[0, 0] = GetString("administration.users.onlineusers");
            tabs[0, 2] = ResolveUrl("~/CMSModules/Membership/Pages/Users/General/User_Online.aspx?guest=1&isonlinemarketing=1&siteid=" + CMSContext.CurrentSiteID);

            tabs[1, 0] = GetString("administration.users_header.kickedusers");
            tabs[1, 2] = ResolveUrl("~/CMSModules/Membership/Pages/Users/General/User_Kicked.aspx?isonlinemarketing=1&siteid=" + CMSContext.CurrentSiteID);

            CurrentMaster.Tabs.Tabs = tabs;
            CurrentMaster.Tabs.UrlTarget = "content";
        }
        else
        {
            MasterPageFile = "~/CMSMasterPages/UI/SimplePage.master";
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        var user = CMSContext.CurrentUser;
        if (!user.IsAuthorizedPerUIElement("CMS.Desk", "OnlineMarketing"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Desk", "OnlineMarketing");
        }
        if (!user.IsAuthorizedPerUIElement("CMS.OnlineMarketing", new string[] { "OMCMGroup", "ContactsFrameset", "On-line_users" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.OnlineMarketing", "OMCMGroup;ContactsFrameset;On-line_users");
        }
    }
}
