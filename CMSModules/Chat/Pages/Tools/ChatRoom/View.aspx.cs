using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.Chat;
using CMS.SettingsProvider;
using CMS.CMSHelper;

[EditedObject(PredefinedObjectType.CHATROOM, "roomid")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_View : CMSChatRoomPage
{
    #region "Properties"

    protected override BaseInfo EditedChatRoom
    {
        get
        {
            return (BaseInfo)CMSContext.EditedObject;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ChatHelper.RegisterStylesheet(Page);
    }      


    protected void Page_Load(object sender, EventArgs e)
    {
        int roomID = QueryHelper.GetInteger("roomid", 0);

        ChatRoomMessages.RoomID = roomID;

        ChatRoomInfo room = ChatRoomInfoProvider.GetChatRoomInfo(roomID);   
        if (room != null)
        {
            ChatOnlineUserHelper.LogInCurrentCMSUser();
            ChatRoomUserHelper.JoinUserToRoom(room.ChatRoomID, ChatUserHelper.GetChatUserFromCMSUser() , room.ChatRoomPassword, false);
        }
        
        ChatRoomUsers.ChatUserTransformationName = "Chat.Transformations.CMSChatRoomUser";
        ChatRoomUsers.EnableFiltering = true;
        ChatRoomUsers.ShowFilterItems = 20;
        ChatRoomUsers.EnablePaging = true;
        ChatRoomUsers.PagingItems = 30;
        ChatRoomUsers.ChatErrorDeleteAllButtonTransformationName = "Chat.Transformations.CMSChatErrorDeleteAllButton";
        ChatRoomUsers.ChatErrorTransformationName = "Chat.Transformations.CMSChatError";
        ChatRoomUsers.ChatMessageTransformationName = "Chat.Transformations.CMSChatMessage";
        ChatRoomUsers.ChatRoomUserTransformationName = "Chat.Transformations.CMSChatRoomUser";
        
        RoomName.ChatRoomNameTransformationName = "Chat.Transformations.CMSRoomName";
        RoomName.DisplayInitialTitle = false;

        ChatNotification.NotificationTransformation = "Chat.Transformations.CMSChatNotification";
        ChatNotification.ChatErrorDeleteAllButtonTransformationName = "Chat.Transformations.CMSChatErrorDeleteAllButton";
        ChatNotification.ChatErrorTransformationName = "Chat.Transformations.CMSChatError";
        ChatNotification.ChatMessageTransformationName = "Chat.Transformations.CMSChatMessage";
        ChatNotification.ChatRoomUserTransformationName = "Chat.Transformations.CMSChatRoomUser";
        ChatNotification.EnableNotificationBubble = false;

        ChatErrors.ErrorTransformationName = "Chat.Transformations.CMSChatError";
        ChatErrors.ShowDeleteAllBtn = true;
        ChatErrors.ButtonDeleteAllTransformationName = "Chat.Transformations.CMSChatErrorDeleteAllButton";

        ChatRoomMessages.ChatMessageTransformationName = "Chat.Transformations.CMSChatMessage";
        ChatRoomMessages.Count = 100;
        ChatRoomMessages.Direction = ChatRoomMessagesDirectionEnum.Down;
        

        // Registration to chat webservice
        AbstractCMSPage cmsPage = this.Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatSupportAJAXProxy(cmsPage);
        }
    }

    #endregion
}

