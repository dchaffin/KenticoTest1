using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

[Security(Resource = "CMS.MessageBoards", UIElements = "Messages")]
public partial class CMSModules_MessageBoards_Tools_Messages_Message_List : CMSMessageBoardPage
{
    private int mBoardId = 0;
    private int mGroupId = 0;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        messageList.IsLiveSite = false;
        messageList.BoardID = mBoardId;
        messageList.GroupID = mGroupId;
        messageList.OnAction += messageList_OnAction;

        if (mBoardId > 0)
        {
            // New message link
            string[,] actions = new string[1, 6];
            actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[0, 1] = GetString("Board.MessageList.NewMessage");
            actions[0, 2] = "modalDialog('" + CMSContext.ResolveDialogUrl("~/CMSModules/MessageBoards/Tools/Messages/Message_Edit.aspx") + "?boardId=" + mBoardId + "&changemaster=" + QueryHelper.GetBoolean("changemaster", false) + "', 'MessageEdit', 500, 400); return false;";
            actions[0, 3] = "#";
            actions[0, 4] = null;
            actions[0, 5] = GetImageUrl("CMSModules/CMS_MessageBoards/addmessage.png");
            CurrentMaster.HeaderActions.Actions = actions;
        }
    }


    private void messageList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":

                string[] arguments = e.CommandArgument as string[];
                URLHelper.Redirect("Message_Edit.aspx?boardId=" + mBoardId + "&messageId=" + arguments[1] + arguments[0] + ((mGroupId > 0) ? "&groupid=" + mGroupId : ""));
                break;
        }
    }
}