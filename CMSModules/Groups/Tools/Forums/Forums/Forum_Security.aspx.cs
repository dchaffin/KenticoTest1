using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Forums;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Forums_Forums_Forum_Security : CMSGroupForumPage
{
    protected int forumId;
    protected ForumInfo forum;


    protected void Page_Load(object sender, EventArgs e)
    {
        forumId = ValidationHelper.GetInteger(Request.QueryString["forumid"], 0);
        if (forumId == 0)
        {
            return;
        }

        forumSecurity.ForumID = forumId;
        forumSecurity.IsGroupForum = true;
        forumSecurity.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumSecurity_OnCheckPermissions);
        forumSecurity.IsLiveSite = false;

        InitializeMasterPage();
    }


    private void forumSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        ForumInfo fi = ForumInfoProvider.GetForumInfo(ValidationHelper.GetInteger(Request.QueryString["forumid"], 0));
        if (fi != null)
        {
            ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
            if (fgi != null)
            {
                groupId = fgi.GroupGroupID;
            }
        }

        // Check permissions
        if (!CMSContext.CurrentUser.IsGroupAdministrator(groupId))
        {
            // Check permissions
            if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Groups", permissionType))
            {
                forumSecurity.StopProcessing = true;

                // Redirect only if permission READ is check
                if (permissionType == CMSAdminControl.PERMISSION_READ)
                {
                    RedirectToCMSDeskAccessDenied("CMS.Groups", permissionType);
                }
            }
        }
    }


    /// <summary>
    /// Initializes master page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        Title = "Forums - Forum security";
    }
}