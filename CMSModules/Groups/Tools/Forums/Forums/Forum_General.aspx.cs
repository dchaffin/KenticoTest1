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

public partial class CMSModules_Groups_Tools_Forums_Forums_Forum_General : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        forumEdit.ForumID = QueryHelper.GetInteger("forumid", 0);
        forumEdit.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumEdit_OnCheckPermissions);
        forumEdit.IsLiveSite = false;
    }


    private void forumEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        ForumInfo fi = ForumInfoProvider.GetForumInfo(forumEdit.ForumID);
        if (fi != null)
        {
            ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
            if (fgi != null)
            {
                groupId = fgi.GroupGroupID;
            }
        }
        // Check permissions
        CheckPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }
}