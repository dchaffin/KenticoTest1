using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MessageBoard;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_MessageBoards_Tools_Boards_Board_Edit_Subscriptions : CMSMessageBoardBoardsPage
{
    // Current board ID
    private int mBoardId = 0;
    private int mGroupId = 0;
    private bool changeMaster = false;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current board and group ID
        mBoardId = QueryHelper.GetInteger("boardid", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);

        // Get board info and chceck whether it belongs to current site
        BoardInfo board = BoardInfoProvider.GetBoardInfo(mBoardId);
        if (board != null)
        {
            CheckMessageBoardSiteID(board.BoardSiteID);
        }

        boardSubscriptions.GroupID = mGroupId;
        boardSubscriptions.Board = board;
        boardSubscriptions.ChangeMaster = changeMaster;
        boardSubscriptions.OnAction += new CommandEventHandler(boardSubscriptions_OnAction);

        // Initialize the master page
        InitializeMasterPage();
    }


    protected void boardSubscriptions_OnAction(object sender, CommandEventArgs e)
    {
        if (e.CommandName.ToLowerCSafe() == "edit")
        {
            // Redirect to edit page with subscription ID specified
            URLHelper.Redirect("Board_Edit_Subscription_Edit.aspx?subscriptionid=" + e.CommandArgument.ToString() + "&boardid=" + mBoardId.ToString() + "&changemaster=" + changeMaster
                               + ((mGroupId > 0) ? "&groupid=" + mGroupId : ""));
        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        // Setup master page action element
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("board.subscriptions.newitem");
        actions[0, 3] = "~/CMSModules/MessageBoards/Tools/Boards/Board_Edit_Subscription_Edit.aspx?boardid=" + mBoardId.ToString() + "&changemaster=" + changeMaster;
        actions[0, 5] = GetImageUrl("CMSModules/CMS_MessageBoards/newsubscription.png");

        CurrentMaster.HeaderActions.Actions = actions;
    }

    #endregion
}