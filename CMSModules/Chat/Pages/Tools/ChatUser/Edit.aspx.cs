using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;
using CMS.SettingsProvider;

[EditedObject(PredefinedObjectType.CHATUSER, "userid")]

[Help("chat_new_user", NewObject = true)]
[Help("chat_edit_user", ExistingObject = true)]

[Breadcrumbs(2)]
[Breadcrumb(0, "chat.chatuser.list", "~/CMSModules/Chat/Pages/Tools/ChatUser/List.aspx", null)]
[Breadcrumb(1,  "chat.chatuser.new", NewObject = true)]
[Breadcrumb(1, Text = "{%EditedObject.ChatUserNickname%}", ExistingObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatUser_Edit : CMSChatPage
{
}
