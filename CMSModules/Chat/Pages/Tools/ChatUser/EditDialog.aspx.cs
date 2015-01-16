using System;

using CMS.UIControls;
using CMS.Chat;
using CMS.SettingsProvider;

[EditedObject(PredefinedObjectType.CHATUSER, "userid")]

[Title(ImageUrl = "Objects/CMS_User/object.png", Text = "{%EditedObject.ChatUserNickname%}")]

public partial class CMSModules_Chat_Pages_Tools_ChatUser_EditDialog : CMSChatPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Localize buttons
        btnClose.Text = GetString("general.close");
    }
}
