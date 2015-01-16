using System;

using CMS.Forums;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Forums_Groups_ForumGroup_Header : CMSGroupForumPage
{
    protected int groupId;
    protected int siteId;
    private int mCommunityGroupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        string currentForumGroup = string.Empty;

        if (!string.IsNullOrEmpty(Request.QueryString["groupid"]))
        {
            groupId = QueryHelper.GetInteger("groupid", 0);
            ForumGroupInfo group = ForumGroupInfoProvider.GetForumGroupInfo(groupId);
            if (group != null)
            {
                mCommunityGroupId = group.GroupGroupID;
            }
            currentForumGroup = HTMLHelper.HTMLEncode(group.GroupDisplayName);
        }

        InitalizeMasterPage(currentForumGroup);

        if (!RequestHelper.IsPostBack())
        {
            InitalizeMenu();
        }
    }


    /// <summary>
    /// Initializes Master page properties.
    /// </summary>
    protected void InitalizeMasterPage(string currentForumGroup)
    {
        // Initialize title and help
        Title = "Group header";
        CurrentMaster.Title.HelpTopicName = "forum_list2";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Initialize breadcrumbs
        string[,] tabs = new string[2,3];
        tabs[0, 0] = GetString("Group_General.GroupList");
        tabs[0, 1] = "~/CMSModules/Groups/Tools/Forums/Groups/ForumGroups_List.aspx?groupid=" + mCommunityGroupId.ToString();
        tabs[0, 2] = "content";
        tabs[1, 0] = currentForumGroup;
        tabs[1, 1] = "";
        tabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = tabs;
    }


    /// <summary>
    /// Initializes user edit menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        string generalString = GetString("general.general");
        string forumsString = GetString("Group_General.Forums");
        string viewString = GetString("general.view");

        string[,] tabs = new string[3,4];
        tabs[0, 0] = forumsString;
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'forum_list2');";
        tabs[0, 2] = "../Forums/Forums_List.aspx?groupid=" + groupId;
        tabs[1, 0] = generalString;
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'forum_group_general_tab');";
        tabs[1, 2] = "ForumGroup_General.aspx?groupid=" + groupId;
        tabs[2, 0] = viewString;
        tabs[2, 1] = "SetHelpTopic('helpTopic', 'view_tab');";
        tabs[2, 2] = "ForumGroup_View.aspx?groupid=" + groupId;

        CurrentMaster.Tabs.UrlTarget = "groupsContent";
        CurrentMaster.Tabs.Tabs = tabs;
    }
}