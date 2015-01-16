using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;
using CMS.Chat;
using CMS.GlobalHelper;
using System.Web.Script.Serialization; 


public partial class CMSModules_Chat_CMSPages_ChatRoomWindow : CMSLiveModalPage
{
    #region "Private fields"

    // "cache" for queryhelper.
    private bool? mIsSupport = null;

    // indicates if window was opened by supporter
    private bool IsSupport
    {
        get
        {
            if (!mIsSupport.HasValue)
            {
                mIsSupport = QueryHelper.GetInteger("isSupport", 0) == 1;
            }
            return mIsSupport.Value;
        }
    }

    private int PopupSettingsId
    {
        get
        {
            return QueryHelper.GetInteger("popupSettingsId", 0);
        }
    }

    
    #endregion


    #region "Private methods"

    private void SetWebpartsUp()
    {
        // Load parent webpart settings stored in DB.
        ChatPopupWindowSettingsInfo settings = ChatPopupWindowSettingsHelper.GetPopupWindowSettings(PopupSettingsId);

        // If no settings are stored use default values.
        if (settings == null)
        {
            settings = new ChatPopupWindowSettingsInfo();
            settings.MessageTransformationName = ChatHelper.TransformationRoomMessages;
            settings.UserTransformationName = ChatHelper.TransformationRoomUsers;
            settings.ErrorTransformationName = ChatHelper.TransformationErrors;
            settings.ErrorClearTransformationName = ChatHelper.TransformationErrorsDeleteAll;
        }
        else
        {
            if (string.IsNullOrEmpty(settings.MessageTransformationName))
            {
                settings.MessageTransformationName = ChatHelper.TransformationRoomMessages;
            }
            if (string.IsNullOrEmpty(settings.UserTransformationName))
            {
                settings.UserTransformationName = ChatHelper.TransformationRoomUsers;
            }
            if (string.IsNullOrEmpty(settings.ErrorTransformationName))
            {
                settings.ErrorTransformationName = ChatHelper.TransformationErrors;
            }
            if (string.IsNullOrEmpty(settings.ErrorClearTransformationName))
            {
                settings.ErrorClearTransformationName = ChatHelper.TransformationErrorsDeleteAll;
            }
        }

        // Set errors webpart up.
        ChatErrorsElem.ErrorTransformationName = settings.ErrorTransformationName;
        ChatErrorsElem.ButtonDeleteAllTransformationName = settings.ErrorClearTransformationName;
        ChatErrorsElem.ShowDeleteAllBtn = true;

        // Set messages webpart up.
        ChatRoomMessagesElem.ChatMessageTransformationName = settings.MessageTransformationName;
        ChatRoomMessagesElem.Direction = ChatRoomMessagesDirectionEnum.Down;
        ChatRoomMessagesElem.Count = ChatHelper.FirstLoadMessagesCountSetting;

        // Set users webpart up.
        ChatRoomUsersElem.ChatUserTransformationName = settings.UserTransformationName;
        ChatRoomUsersElem.EnablePaging = true;
        ChatRoomUsersElem.PagingItems = 5;
        ChatRoomUsersElem.EnableFiltering = true;
        ChatRoomUsersElem.ShowFilterItems = 6;
        ChatRoomUsersElem.SortByStatus = true;

        // Set send webpart up.
        ChatMessageSendElem.IsSupport = IsSupport;
        ChatRoomUsersElem.IsSupport = IsSupport;
        ChatRoomMessagesElem.IsSupport = IsSupport;
        ChatErrorsElem.IsSupport = IsSupport;    
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = CurrentMaster.Title;
        title.TitleImage = GetImageUrl("CMSModules/CMS_Chat/list.png");

        passwordPromptElem.TitleImage = GetImageUrl("CMSModules/CMS_Chat/password.png");
        passwordPromptElem.TitleText = ResHelper.GetString("chat.password");

        // Script references insertion
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/jquery/jquery-resize.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ChatRoomWindow.js");

        // Get information about chat room this window has been opened for.
        int roomID = QueryHelper.GetInteger("windowroomid", 0);

        ChatRoomInfo room = ChatRoomInfoProvider.GetChatRoomInfo(roomID);
        if (room == null)
        {
            lblError.Text = ResHelper.GetString("chat.error.window.badroomid");
            lblError.Visible = true;
            return;
        }

        string pnlChatRoomWindowCssClass = " ChatPopupWindow";

        // Disable irrelevant controls
        if (room.ChatRoomIsOneToOne)
        {
            ChatMessageSendElem.HideUserPicker();
            pnlChatRoomWindowCssClass += " IsOneToOne";

            // Set appropriate window title
            string pageTitle = ResHelper.GetString(room.ChatRoomIsSupport ? "chat.title.support" : "chat.title.privateconversation");
            if (room.ChatRoomIsSupport && IsSupport)
            {
                pageTitle = room.ChatRoomDisplayName + " – " + pageTitle;
            }
            Page.Header.Title = title.TitleText = pageTitle;
        }
        else
        {
            Page.Header.Title = title.TitleText = room.ChatRoomDisplayName;
        }

        SetWebpartsUp();

        // True if this window is open by a customer in need of support (not support engineer)
        bool isCustomerSupport = room.ChatRoomIsSupport && !IsSupport;
        if (room.ChatRoomIsSupport)
        {
            pnlChatRoomWindowCssClass += " IsSupport";
        }

        if (isCustomerSupport && ChatHelper.IsSupportMailEnabledAndValid)
        {
            pnlSupportSendMail.Visible = true;
            hplSupportSendMail.NavigateUrl = ChatHelper.SupportMailDialogURL + "?roomid=" + roomID;
            hplSupportSendMail.Target = "_blank";
        }

        pnlChatRoomWindow.CssClass += pnlChatRoomWindowCssClass;
        JavaScriptSerializer sr = new JavaScriptSerializer();

        string json = sr.Serialize(
            new
            {
                roomID = roomID,
                password = room.HasPassword,
                pnlChatRoomPasswordPrompt = '#' + pnlChatRoomPasswordPrompt.ClientID,
                txtChatRoomPasswordPromptInput = '#' + txtChatRoomPasswordPromptInput.ClientID,
                btnChatRoomPasswordPromptSubmit = '#' + btnChatRoomPasswordPromptSubmit.ClientID,
                isOneToOne = room.IsWhisperRoom,
                isCustomerSupport = isCustomerSupport,
                hplSupportSendMailClientID = isCustomerSupport ? '#' + hplSupportSendMail.ClientID : "",
                pnlPasswordPromptError = '#' + pnlChatRoomsPromptPasswordError.ClientID,
                pnlChatRoomWindow = '#' + pnlChatRoomWindow.ClientID,
                ChatRoomMessagesClientID = ChatRoomMessagesElem.ClientID,
                btnClose = "#" + btnCloseWindow.ClientID,
                isSupport = IsSupport
            }
        );


        string startupScript = String.Format("ChatRoomWindow({0});", json);

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatRoomWindow_" + ClientID, startupScript, true);
    }

    #endregion
}
