using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.Chat;

[Help("chat_rooms_general_tab")]

[EditedObject(PredefinedObjectType.CHATROOM, "roomid")]

[Breadcrumbs(2)]
[Breadcrumb(0, "chat.chatroom.list", "~/CMSModules/Chat/Pages/Tools/ChatRoom/List.aspx?siteid={?siteid?}", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.ChatRoomDisplayName%}", ExistingObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_Header : CMSChatRoomPage
{
    #region "Properties"

    private int currentTabID = 0;

    private int GetNextTabID()
    {
        return currentTabID++;
    }


    protected override BaseInfo EditedChatRoom
    {
        get 
        {
            return (BaseInfo)CMSContext.EditedObject;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ChatRoomInfo editedRoom = (ChatRoomInfo)EditedObject;

        InitTabs("content");

        SetTabPrivate("general.general", "Edit.aspx?roomid={?roomid?}", "chat_rooms_general_tab");

        if ((editedRoom != null) && !editedRoom.IsOneToOneSupport)
        {
            SetTabPrivate("general.password", "EditPassword.aspx?roomid={?roomid?}", "chat_rooms_password_tab");
        }
        
        SetTabPrivate("chat.users", "ChatUser/List.aspx?roomid={?roomid?}", "chat_rooms_users_tab");
        SetTabPrivate("chat.messages", "ChatMessage/List.aspx?roomid={?roomid?}", "chat_room_essages_tab");

        // Show 'View' tab only if room is not disabled and is not one to one support room
        if ((editedRoom != null) && editedRoom.ChatRoomEnabled && !editedRoom.IsOneToOneSupport)
        {
            SetTabPrivate("general.view", "View.aspx?roomid={?roomid?}", "chat_rooms_view_tab");
        }
    }

    #endregion


    #region "Private methods"

    private void SetTabPrivate(string resourceString, string url, string helpTopic)
    {
        // Resolve macros in url
        url = CMSContext.ResolveMacros(url);

        SetTab(GetNextTabID(), ResHelper.GetString(resourceString), ResolveUrl(url), string.Format("SetHelpTopic('helpBreadcrumbs', '{0}');", helpTopic));
    }

    #endregion
}
