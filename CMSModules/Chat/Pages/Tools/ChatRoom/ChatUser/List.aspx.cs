using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;
using CMS.UIControls.UniGridConfig;
using CMS.SiteProvider;
using CMS.SettingsProvider;

[ParentObject(PredefinedObjectType.CHATROOM, "roomid")]

[Actions(1)]
[Action(0, "Objects/CMS_ChatUser/add.png", "chat.chatroomuser.new", "Edit.aspx?roomid={%EditedObjectParent.ChatRoomID%}")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_ChatUser_List : CMSChatRoomPage
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


    #region "Private properties"

    private int ChatRoomID
    {
        get
        {
            return QueryHelper.GetInteger("roomid", 0);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        listElem.ChatRoomID = ChatRoomID;

        HeaderActions.Enabled = HasUserModifyPermission();

        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            ShowChangesSaved();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Disable "Add new user" action if room is support room (one to one)
        if (((ChatRoomInfo)CMSContext.EditedObjectParent).IsOneToOneSupport)
        {
            var actions = CurrentMaster.HeaderActions.ActionsList;
            if ((actions != null) && (actions.Count > 0))
            {
                actions[0].Enabled = false;
            }
        }
    }

    #endregion
}
