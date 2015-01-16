using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Forums;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Forums_Forums_Forums_List : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupId = QueryHelper.GetInteger("groupid", 0);
        forumList.GroupID = groupId;
        forumList.OnAction += new CommandEventHandler(forumList_OnAction);
        forumList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumList_OnCheckPermissions);
        forumList.IsLiveSite = false;

        InitializeMasterPage(groupId);
    }


    private void forumList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int lGroupId = 0;

        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(forumList.GroupID);
        if (fgi != null)
        {
            lGroupId = fgi.GroupGroupID;
        }

        CheckPermissions(lGroupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    private void forumList_OnAction(object sender, CommandEventArgs e)
    {
        int lGroupId = 0;

        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(forumList.GroupID);
        if (fgi != null)
        {
            lGroupId = fgi.GroupGroupID;
        }

        CheckPermissions(lGroupId, CMSAdminControl.PERMISSION_READ);

        if (e.CommandName.ToLowerCSafe() == "edit")
        {
            URLHelper.Redirect("Forum_Frameset.aspx?forumid=" + Convert.ToString(e.CommandArgument));
        }
        else
        {
            forumList.ReloadData();
        }
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage(int groupId)
    {
        // Set title
        Title = "Forum List";
        // Set actions
        string[,] actions = new string[1,8];
        actions[0, 0] = "HyperLink";
        actions[0, 1] = GetString("Forum_List.NewItemCaption");
        actions[0, 3] = ResolveUrl("Forum_New.aspx?groupId=" + groupId.ToString());
        actions[0, 5] = GetImageUrl("Objects/Forums_Forum/add.png");

        CurrentMaster.HeaderActions.Actions = actions;
    }
}