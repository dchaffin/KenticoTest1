
using CMS.UIControls;
using CMS.Chat;
using CMS.CMSHelper;
using CMS.SettingsProvider;

[EditedObject(PredefinedObjectType.CHATROOM, "roomId")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_Edit : CMSChatRoomPage
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
}