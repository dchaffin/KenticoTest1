using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.ExtendedControls;

public partial class CMSModules_Groups_Tools_Polls_Polls_List : CMSGroupPollsPage
{
    protected int groupID = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get GroupID from query string
        groupID = QueryHelper.GetInteger("groupID", 0);

        CheckGroupPermissions(groupID, CMSAdminControl.PERMISSION_READ);

        if (CheckGroupPermissions(groupID, CMSAdminControl.PERMISSION_MANAGE, false))
        {
            // New item link
            string[,] actions = new string[1, 6];
            actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[0, 1] = GetString("group.polls.newpoll");
            actions[0, 2] = null;
            actions[0, 3] = ResolveUrl("Polls_New.aspx") + "?groupid=" + groupID;
            actions[0, 4] = null;
            actions[0, 5] = GetImageUrl("Objects/Polls_Poll/add.png");
            CurrentMaster.HeaderActions.Actions = actions;

            pollsList.DeleteEnabled = true;
        }

        pollsList.OnEdit += new EventHandler(pollsList_OnEdit);
        pollsList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(pollsList_OnCheckPermissions);
        pollsList.GroupId = groupID;
        pollsList.IsLiveSite = false;
    }


    private void pollsList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckPermissions(groupID, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// Edit poll click handler.
    /// </summary>
    private void pollsList_OnEdit(object sender, EventArgs e)
    {
        URLHelper.Redirect(ResolveUrl("Polls_Edit.aspx?pollId=" + pollsList.SelectedItemID + "&GroupId=" + groupID));
    }
}