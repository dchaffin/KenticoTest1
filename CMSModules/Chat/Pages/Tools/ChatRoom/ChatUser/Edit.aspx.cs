using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Chat;
using CMS.SettingsProvider;

[ParentObject(PredefinedObjectType.CHATROOM, "roomid")]

[EditedObject(PredefinedObjectType.CHATROOMUSER, "roomUserID")]

[Help("chat_rooms_new_user", NewObject = true)]
[Help("chat_rooms_edit_user", ExistingObject = true)]

[Breadcrumbs(2)]
[Breadcrumb(0, "chat.roomusers", "~/CMSModules/Chat/Pages/Tools/ChatRoom/ChatUser/List.aspx?roomid={?roomid?}", null)]
[Breadcrumb(1, Text = "{%EditedChatUserNickname%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "chat.roomuser.new", ExistingObject = false)]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_ChatUser_Edit : CMSChatRoomPage
{
    #region "Properties"

    protected override BaseInfo EditedChatRoom
    {
        get
        {
            return (BaseInfo)CMSContext.EditedObjectParent;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObjectParent != null)
        {
            editElem.ChatRoomID = ((ChatRoomInfo)EditedObjectParent).ChatRoomID;
        }

        // If editing, add user's nickname to macro resolver, so it can be retrieved when applying breacrumb attribute
        if (EditedObject != null)
        {
            int chatUserID = ((ChatRoomUserInfo)EditedObject).ChatRoomUserChatUserID;

            ChatUserInfo chatUser = ChatUserInfoProvider.GetChatUserInfo(chatUserID);

            CMSContext.CurrentResolver.SetNamedSourceData("EditedChatUserNickname", chatUser.ChatUserNickname);
        }
    }

    #endregion
}
