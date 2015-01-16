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

public partial class CMSModules_Groups_Tools_Forums_Groups_ForumGroup_View : CMSGroupForumPage
{
    protected int groupId = 0;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Forum - Group view";

        groupId = ValidationHelper.GetInteger(Request.QueryString["groupid"], 0);
        ForumGroupInfo group = ForumGroupInfoProvider.GetForumGroupInfo(groupId);
        if (group != null)
        {
            Forum1.CommunityGroupID = group.GroupGroupID;
            Forum1.GroupName = group.GroupName;
        }

        Forum1.IsLiveSite = false;
    }
}