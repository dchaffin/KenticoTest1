using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MessageBoard;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Groups_Tools_MessageBoards_Messages_Message_List : CMSGroupMessageBoardsPage
{
    #region "Variables"

    private int mBoardId = 0;
    private int mGroupId = 0;
    private BoardInfo boardObj = null;

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mGroupId = QueryHelper.GetInteger("groupId", 0);

        boardObj = BoardInfoProvider.GetBoardInfo(mBoardId);
        if (boardObj != null)
        {
            mGroupId = boardObj.BoardGroupID;

            // Check whether edited board belongs to any group
            if (mGroupId == 0)
            {
                EditedObject = null;
            }
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        messageList.BoardID = mBoardId;
        messageList.GroupID = mGroupId;
        messageList.EditPageUrl = "~/CMSModules/Groups/Tools/MessageBoards/Messages/Message_Edit.aspx";
        messageList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(messageList_OnCheckPermissions);
        messageList.OnAction += new CommandEventHandler(messageList_OnAction);

        if (mBoardId > 0)
        {
            // New message link
            string[,] actions = new string[1,6];
            actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[0, 1] = GetString("Board.MessageList.NewMessage");
            actions[0, 2] = "modalDialog('" + ResolveUrl("~/CMSModules/Groups/Tools/MessageBoards/Messages/Message_Edit.aspx") + "?boardId=" + mBoardId + "', 'MessageEdit', 500, 400); return false;";
            actions[0, 3] = "#";
            actions[0, 4] = null;
            actions[0, 5] = GetImageUrl("CMSModules/CMS_MessageBoards/addmessage.png");
            CurrentMaster.HeaderActions.Actions = actions;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!URLHelper.IsPostback())
        {
            messageList.ReloadData();
        }
    }


    private void messageList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":

                string[] arguments = e.CommandArgument as string[];
                URLHelper.Redirect("Message_Edit.aspx?boardId=" + mBoardId + "&messageId=" + arguments[1].ToString() + arguments[0].ToString() + ((mGroupId > 0) ? "&groupid=" + mGroupId : ""));
                break;

            default:
                break;
        }
    }


    private void messageList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check 'Manage' permission
        CheckPermissions(messageList.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }
}