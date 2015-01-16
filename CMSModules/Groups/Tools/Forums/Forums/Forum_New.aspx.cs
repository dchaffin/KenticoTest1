using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Forums;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Forums_Forums_Forum_New : CMSGroupForumPage
{
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        groupId = QueryHelper.GetInteger("groupid", 0);
        forumNew.GroupID = groupId;
        forumNew.OnSaved += new EventHandler(forumNew_OnSaved);
        forumNew.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumNew_OnCheckPermissions);
        forumNew.IsLiveSite = false;

        InitializeMasterPage();
    }


    private void forumNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int lGroupId = 0;

        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(forumNew.GroupID);
        if (fgi != null)
        {
            lGroupId = fgi.GroupGroupID;
        }

        CheckPermissions(lGroupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    private void forumNew_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Forum_Frameset.aspx?forumid=" + Convert.ToString(forumNew.ForumID) + "&saved=1");
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        // Initialize help 
        Title = "Forums - New forum";
        CurrentMaster.Title.HelpTopicName = "add_forum";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Initialize page breadcrumbs
        string[,] tabs = new string[2,3];
        tabs[0, 0] = GetString("forum_list.headercaption");
        tabs[0, 1] = "~/CMSModules/Groups/Tools/Forums/Forums/Forums_List.aspx?groupid=" + groupId;
        tabs[0, 2] = "";
        tabs[1, 0] = GetString("Forum_Edit.NewForum");
        tabs[1, 1] = "";
        tabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = tabs;
    }
}