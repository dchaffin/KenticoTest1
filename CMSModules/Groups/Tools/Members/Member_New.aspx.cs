using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Members_Member_New : CMSGroupPage
{
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        memberEditElem.GroupID = QueryHelper.GetInteger("groupid", 0);
        memberEditElem.OnSaved += new EventHandler(memberEditElem_OnSaved);
        memberEditElem.IsLiveSite = false;

        // Initialize breadcrumbs
        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("group.members");
        pageTitleTabs[0, 1] = "~/CMSModules/Groups/Tools/Members/Member_List.aspx";
        pageTitleTabs[0, 1] += "?groupId=" + memberEditElem.GroupID;
        pageTitleTabs[0, 2] = "_self";
        pageTitleTabs[1, 0] = GetString("group.member.newmember");
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;

        CurrentMaster.Title.HelpTopicName = "group_members_new";
        CurrentMaster.Title.HelpName = "helpTopic";
    }


    protected void memberEditElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Member_Edit.aspx?memberId=" + memberEditElem.MemberID);
    }
}