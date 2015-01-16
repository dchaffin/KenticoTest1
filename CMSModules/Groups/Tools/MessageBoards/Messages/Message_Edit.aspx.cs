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

public partial class CMSModules_Groups_Tools_MessageBoards_Messages_Message_Edit : CMSModalPage
{
    private int mBoardId = 0;
    private int mMessageId = 0;
    private int mGroupId = 0;
    private CurrentUserInfo cu = null;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Check permissions for CMS Desk -> Tools -> Groups
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Tools", "Groups"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Tools", "Groups");
        }

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'Read' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_READ))
        {
            RedirectToCMSDeskAccessDenied("cms.groups", CMSAdminControl.PERMISSION_READ);
        }

        cu = CMSContext.CurrentUser;

        messageEditElem.IsLiveSite = false;
        messageEditElem.AdvancedMode = true;
        messageEditElem.MessageID = mMessageId;
        messageEditElem.MessageBoardID = mBoardId;

        messageEditElem.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(messageEditElem_OnCheckPermissions);

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


    private void messageEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckLocalPermissions();
    }


    protected void CheckLocalPermissions()
    {
        int groupId = 0;
        int boardId = mBoardId;

        BoardMessageInfo bmi = BoardMessageInfoProvider.GetBoardMessageInfo(mMessageId);
        if (bmi != null)
        {
            boardId = bmi.MessageBoardID;
        }

        BoardInfo bi = BoardInfoProvider.GetBoardInfo(boardId);
        if (bi != null)
        {
            groupId = bi.BoardGroupID;
        }


        // Check 'Manage' permission
        if (!CMSContext.CurrentUser.IsGroupAdministrator(groupId))
        {
            if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
            {
                RedirectToCMSDeskAccessDenied("cms.groups", CMSAdminControl.PERMISSION_MANAGE);
            }
        }
    }


    private void messageEditElem_OnAfterMessageSaved(BoardMessageInfo message)
    {
        int queryMarkIndex = Request.RawUrl.IndexOfCSafe('?');
        string filterParams = Request.RawUrl.Substring(queryMarkIndex);

        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBoardList('" + filterParams + "');CloseDialog();");
    }


    private void messageEditElem_OnBeforeMessageSaved()
    {
        CheckLocalPermissions();
    }
}