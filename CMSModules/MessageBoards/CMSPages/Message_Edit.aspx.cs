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
using CMS.SettingsProvider;

public partial class CMSModules_MessageBoards_CMSPages_Message_Edit : CMSLiveModalPage
{
    private int mBoardId = 0;
    private int mMessageId = 0;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("messageboardid", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        messageEditElem.AdvancedMode = true;
        messageEditElem.CheckFloodProtection = true;
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

        if (!URLHelper.IsPostback())
        {
            messageEditElem.ReloadData();
        }
    }


    private void messageEditElem_OnAfterMessageSaved(BoardMessageInfo message)
    {
        int queryMarkIndex = Request.RawUrl.IndexOfCSafe('?');
        string filterParams = Request.RawUrl.Substring(queryMarkIndex);

        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBoardList('" + filterParams + "'); CloseDialog();");
    }


    private void messageEditElem_OnBeforeMessageSaved()
    {
        bool isOwner = false;

        BoardInfo board = BoardInfoProvider.GetBoardInfo(messageEditElem.MessageBoardID);
        if (board != null)
        {
            // Check if the current user is allowed to modify the message
            isOwner = BoardInfoProvider.IsUserAuthorizedToManageMessages(board);
        }

        if (!isOwner && !CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.MessageBoards", "Modify"))
        {
            RedirectToAccessDenied(GetString("board.messageedit.notallowed"));
        }
    }
}