using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

[Title("Objects/CMS_ChatRoom/object.png", "chat.chat", "chat_rooms")]

[Tabs("content")]
[Tab(0, "chat.chatrooms.list", "ChatRoom/List.aspx", "SetHelpTopic('helpTopic', 'chat_rooms');")]
[Tab(1, "chat.chatuser.list", "ChatUser/List.aspx", "SetHelpTopic('helpTopic', 'chat_users');")]
[Tab(2, "chat.cannedresponses", "ChatSupportCannedResponse/List.aspx", "SetHelpTopic('helpTopic', 'chat_canned_responses');")]

public partial class CMSModules_Chat_Pages_Tools_Header : CMSChatPage
{
}
