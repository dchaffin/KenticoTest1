using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Forums_Groups_ForumGroup_New : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get info on the current community group ID
        int groupId = QueryHelper.GetInteger("groupid", 0);
        if (groupId > 0)
        {
            forumGroup.CommunityGroupID = groupId;
        }

        InitializeMasterPage();

        forumGroup.OnSaved += new EventHandler(forumGroup_OnSaved);
        forumGroup.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumGroup_OnCheckPermissions);
        forumGroup.IsLiveSite = false;
    }


    private void forumGroup_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check if user is allowed to manage module
        CheckPermissions(forumGroup.CommunityGroupID, CMSAdminControl.PERMISSION_MANAGE);
    }


    protected void forumGroup_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Group_Frameset.aspx?groupId=" + Convert.ToString(forumGroup.GroupID) + "&saved=1");
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        // Set title and help
        Title = "Forum - New group";
        CurrentMaster.Title.HelpTopicName = "new_forum_group";
        CurrentMaster.Title.HelpName = "helpTopic";

        int groupId = QueryHelper.GetInteger("groupid", 0);

        // Initializes page title control		
        string[,] tabs = new string[2,3];
        tabs[0, 0] = GetString("Group_General.GroupList");
        tabs[0, 1] = "~/CMSModules/Groups/Tools/Forums/Groups/ForumGroups_List.aspx" + ((groupId > 0) ? "?groupid=" + groupId.ToString() : "");
        tabs[0, 2] = "";
        tabs[1, 0] = GetString("Group_General.NewGroup");
        tabs[1, 1] = "";
        tabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = tabs;
    }
}