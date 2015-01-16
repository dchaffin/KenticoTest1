using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.MessageBoard;
using CMS.UIControls;

public partial class CMSModules_MessageBoards_Tools_Messages_Message_Edit : CMSModalPage
{
    private int mBoardId = 0;
    private int mMessageId = 0;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Check permissions for CMS Desk -> Tools -> MessageBoards
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Tools", "MessageBoards"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Tools", "MessageBoards");
        }

        // Check permissions for MessageBoards -> Messages
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.MessageBoards", "Messages"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.MessageBoards", "Messages");
        }

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'Read' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.messageboards", CMSAdminControl.PERMISSION_READ))
        {
            RedirectToAccessDenied("cms.messageboards", CMSAdminControl.PERMISSION_READ);
        }

        messageEditElem.IsLiveSite = false;
        messageEditElem.AdvancedMode = true;
        messageEditElem.MessageID = mMessageId;
        messageEditElem.MessageBoardID = mBoardId;
        messageEditElem.OnBeforeMessageSaved += new OnBeforeMessageSavedEventHandler(messageEditElem_OnBeforeMessageSaved);
        messageEditElem.OnAfterMessageSaved += new OnAfterMessageSavedEventHandler(messageEditElem_OnAfterMessageSaved);

        // initializes page title control		
        if (mMessageId > 0)
        {
            CurrentMaster.Title.TitleText = GetString("Board.MessageEdit.title");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Board_Message/object.png");
        }
        else
        {
            CurrentMaster.Title.TitleText = GetString("Board.MessageNew.title");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Board_Message/new.png");
        }

        CurrentMaster.Title.HelpTopicName = "messages_edit";
        CurrentMaster.Title.HelpName = "helpTopic";
    }


    private void messageEditElem_OnAfterMessageSaved(BoardMessageInfo message)
    {
        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBoardList(); CloseDialog();");
    }


    private void messageEditElem_OnBeforeMessageSaved()
    {
        bool isOwner = false;

        BoardInfo board = BoardInfoProvider.GetBoardInfo(mBoardId);
        if (board != null)
        {
            // Check if the current user is allowed to modify the message
            isOwner = BoardInfoProvider.IsUserAuthorizedToManageMessages(board);
        }

        if (!isOwner && !CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.MessageBoards", CMSAdminControl.PERMISSION_MODIFY))
        {
            RedirectToAccessDenied("cms.messageboards", CMSAdminControl.PERMISSION_MODIFY);
        }
    }
}