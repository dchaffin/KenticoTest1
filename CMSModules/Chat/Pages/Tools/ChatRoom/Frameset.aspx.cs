using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.Chat;

[EditedObject(PredefinedObjectType.CHATROOM, "roomId")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_Frameset : CMSChatRoomPage
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

    protected void Page_Load(object sender, EventArgs e)
    {
        frm.FrameHeight = TabsBreadFrameHeight;
    }

    #endregion
}