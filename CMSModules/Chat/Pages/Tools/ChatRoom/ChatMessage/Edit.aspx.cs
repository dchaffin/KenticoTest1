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

[EditedObject(PredefinedObjectType.CHATMESSAGE, "messageid")]

[Help("chat_rooms_edit_message")]

[Breadcrumbs(2)]
[Breadcrumb(0, "chat.messages", "~/CMSModules/Chat/Pages/Tools/ChatRoom/ChatMessage/List.aspx?roomid={?roomid?}", null)]
[Breadcrumb(1, ResourceString = "chat.chatmessage.edit", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "chat.chatmessage.new", NewObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_ChatMessage_Edit : CMSChatRoomPage
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
    }

    #endregion
}
