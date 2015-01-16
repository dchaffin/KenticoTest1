using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;
using CMS.SettingsProvider;

[ParentObject(PredefinedObjectType.CHATROOM, "roomid")]

[Actions(1)]
[Action(0, "Objects/CMS_ChatMessage/add.png", "chat.chatmessage.new", "Edit.aspx?roomid={%EditedObjectParent.ChatRoomID%}")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_ChatMessage_List : CMSChatRoomPage
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
        listElem.ChatRoomID = QueryHelper.GetInteger("roomid", 0);

        HeaderActions.Enabled = HasUserModifyPermission();
    }

    #endregion
}
