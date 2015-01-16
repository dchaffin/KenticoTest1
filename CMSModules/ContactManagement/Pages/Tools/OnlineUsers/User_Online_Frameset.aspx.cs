using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_OnlineUsers_User_Online_Frameset : CMSContactManagementPage
{
    #region "Properties"

    protected int FrameHeight
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

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


    protected override void OnPreInit(EventArgs e)
    {
        LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.OnlineUsers);
        base.OnPreInit(e);
        FrameHeight = SessionManager.StoreOnlineUsersInDatabase ? TabsBreadFrameHeight : 0;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        frmHeader.Attributes["src"] = "User_Online_Header.aspx" + URLHelper.Url.Query;
        frmContent.Attributes["src"] = ResolveUrl(URLHelper.AppendQuery("~/CMSModules/Membership/Pages/Users/General/User_Online.aspx" + "?guest=1&siteid=" + CMSContext.CurrentSiteID, URLHelper.Url.Query));
    }

    #endregion
}
