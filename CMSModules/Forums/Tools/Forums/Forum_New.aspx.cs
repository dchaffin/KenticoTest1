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

public partial class CMSModules_Forums_Tools_Forums_Forum_New : CMSForumsPage
{
    private int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        groupId = QueryHelper.GetInteger("groupid", 0);

        ForumContext.CheckSite(groupId, 0, 0);

        forumNew.GroupID = groupId;
        forumNew.OnSaved += new EventHandler(forumNew_OnSaved);
        forumNew.IsLiveSite = false;

        InitializeMasterPage();
    }


    protected void forumNew_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Forum_Frameset.aspx?forumid=" + forumNew.ForumID);
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
        tabs[0, 1] = "~/CMSModules/Forums/Tools/Forums/Forum_List.aspx?groupid=" + groupId;
        tabs[0, 2] = "";
        tabs[1, 0] = GetString("Forum_Edit.NewForum");
        tabs[1, 1] = "";
        tabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = tabs;
    }
}