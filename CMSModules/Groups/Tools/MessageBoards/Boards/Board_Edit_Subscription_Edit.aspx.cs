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

public partial class CMSModules_Groups_Tools_MessageBoards_Boards_Board_Edit_Subscription_Edit : CMSGroupMessageBoardsPage
{
    #region "Variables"

    private int mSubscriptionId = 0;
    private int boardId = 0;
    private int groupId = 0;
    private BoardSubscriptionInfo mCurrentSubscription = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the controls
        SetupControl();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControl()
    {
        // Get current subscription ID
        mSubscriptionId = QueryHelper.GetInteger("subscriptionid", 0);
        mCurrentSubscription = BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(mSubscriptionId);

        // Get current board and group ID
        boardId = QueryHelper.GetInteger("boardid", 0);
        groupId = QueryHelper.GetInteger("groupid", 0);

        BoardInfo boardObj = BoardInfoProvider.GetBoardInfo(boardId);
        if (boardObj != null)
        {
            // Check whether edited board belongs to group
            if ((boardObj.BoardGroupID == 0) || (groupId != boardObj.BoardGroupID))
            {
                EditedObject = null;
            }
        }

        boardSubscription.IsLiveSite = false;
        boardSubscription.BoardID = boardId;
        boardSubscription.GroupID = groupId;
        boardSubscription.SubscriptionID = mSubscriptionId;
        boardSubscription.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(boardSubscription_OnCheckPermissions);
        boardSubscription.OnSaved += new EventHandler(boardSubscription_OnSaved);

        InitializeBreadcrumbs();
    }


    private void boardSubscription_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("~/CMSModules/Groups/Tools/MessageBoards/Boards/Board_Edit_Subscription_Edit.aspx?subscriptionid=" + boardSubscription.SubscriptionID + "&boardid=" + boardId + "&saved=1&groupid=" + groupId);
    }


    private void boardSubscription_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// Initializes the breadcrumbs on the page.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        string[,] breadcrumbs = new string[2,3];

        breadcrumbs[0, 0] = GetString("board.subscription.subscriptions");
        breadcrumbs[0, 1] = "~/CMSModules/Groups/Tools/MessageBoards/Boards/Board_Edit_Subscriptions.aspx?boardid=" + boardId + "&groupid=" + groupId;
        breadcrumbs[0, 2] = "_self";

        // Display current subscription e-mail
        if (mCurrentSubscription != null)
        {
            breadcrumbs[1, 0] = HTMLHelper.HTMLEncode(mCurrentSubscription.SubscriptionEmail);
        }
        else
        {
            breadcrumbs[1, 0] = GetString("board.subscriptions.newitem");
        }
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = breadcrumbs;
    }

    #endregion
}