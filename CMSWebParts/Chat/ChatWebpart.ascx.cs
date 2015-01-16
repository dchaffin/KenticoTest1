using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;

public partial class CMSWebParts_Chat_ChatWebpart : CMSAbstractWebPart
{
    #region "Properties"
    

    /// <summary>
    /// Gets or sets GroupID property.
    /// </summary>
    public string GroupID
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("GroupID"), "DefaultGroup");
        }
        set
        {
            this.SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Gets or sets EnableBBCode property.
    /// </summary>
    public bool EnableBBCode
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("EnableBBCode"), false);
        }
        set
        {
            this.SetValue("EnableBBCode", value);
        }
    }

    
    /// <summary>
    /// Gets or sets DisplayInline property.
    /// </summary>
    public bool DisplayInline
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("DisplayInline"), false);
        }
        set
        {
            this.SetValue("DisplayInline", value);
        }
    }


    /// <summary>
    /// Gets or sets Direction property.
    /// </summary>
    public ChatRoomMessagesDirectionEnum Direction
    {
        get
        {
            return (ChatRoomMessagesDirectionEnum)ValidationHelper.GetInteger(this.GetValue("Direction"), (int)ChatRoomMessagesDirectionEnum.Up);
        }
        set
        {
            this.SetValue("Direction", (int)value);
        }
    }


    /// <summary>
    /// Gets or sets Count property.
    /// </summary>
    public int Count
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("Count"), -1);
        }
        set
        {
            this.SetValue("Count", value);
        }
    }


    /// <summary>
    /// Gets or sets EnableNotificationBubble property.
    /// </summary>
    public bool EnableNotificationBubble
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("EnableNotificationBubble"), true);
        }
        set
        {
            this.SetValue("EnableNotificationBubble", value);
        }
    }


    /// <summary>
    /// Gets or sets DisplayInitialTitle property.
    /// </summary>
    public bool DisplayInitialTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("DisplayInitialTitle"), false);
        }
        set
        {
            this.SetValue("DisplayInitialTitle", value);
        }
    }


    /// <summary>
    /// Initial title text
    /// </summary>
    public string InitialTitle
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("InitialTitle"), ResHelper.GetString("chat.roomname.initialtitle"));
        }
        set
        {
            this.SetValue("InitialTitle", value);
        }
    }

    
    /// <summary>
    /// Gets or sets ErrorShowDeleteAllBtn property.
    /// </summary>
    public bool ErrorShowDeleteAllBtn
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ErrorShowDeleteAllBtn"), false);
        }
        set
        {
            this.SetValue("ErrorShowDeleteAllBtn", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomsEnablePaging property.
    /// </summary>
    public bool RoomsEnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("RoomsEnablePaging"), false);
        }
        set
        {
            this.SetValue("RoomsEnablePaging", value);
        }
    }

    
    /// <summary>
    /// Gets or sets RoomsPagingItems property.
    /// </summary>
    public int RoomsPagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("RoomsPagingItems"), -1);
        }
        set
        {
            this.SetValue("RoomsPagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomsEnableFiltering property.
    /// </summary>
    public bool RoomsEnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("RoomsEnableFiltering"), false);
        }
        set
        {
            this.SetValue("RoomsEnableFiltering", value);
        }
    }

    
    /// <summary>
    /// Gets or sets RoomsShowFilterItems property.
    /// </summary>
    public int RoomsShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("RoomsShowFilterItems"), -1);
        }
        set
        {
            this.SetValue("RoomsShowFilterItems", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomUsersEnablePaging property.
    /// </summary>
    public bool RoomUsersEnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("RoomUsersEnablePaging"), false);
        }
        set
        {
            this.SetValue("RoomUsersEnablePaging", value);
        }
    }

    
    /// <summary>
    /// Gets or sets RoomUsersPagingItems property.
    /// </summary>
    public int RoomUsersPagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("RoomUsersPagingItems"), -1);
        }
        set
        {
            this.SetValue("RoomUsersPagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomUsersEnableFiltering property.
    /// </summary>
    public bool RoomUsersEnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("RoomUsersEnableFiltering"), false);
        }
        set
        {
            this.SetValue("RoomUsersEnableFiltering", value);
        }
    }

    
    /// <summary>
    /// Gets or sets RoomUsersShowFilterItems property.
    /// </summary>
    public int RoomUsersShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("RoomUsersShowFilterItems"), -1);
        }
        set
        {
            this.SetValue("RoomUsersShowFilterItems", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomUsersSortByStatus property.
    /// </summary>
    public bool RoomUsersSortByStatus
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("RoomUsersSortByStatus"), false);
        }
        set
        {
            this.SetValue("RoomUsersSortByStatus", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomsEnablePopup property.
    /// </summary>
    public bool RoomsEnablePopup
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("RoomsEnablePopup"), false);
        }
        set
        {
            this.SetValue("RoomsEnablePopup", value);
        }
    }


    /// <summary>
    /// Gets or sets SearchMode property.
    /// </summary>
    public bool SearchMode
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("SearchMode"), false);
        }
        set
        {
            this.SetValue("SearchMode", value);
        }
    }


    /// <summary>
    /// Gets or sets OnlineUsersEnablePaging property.
    /// </summary>
    public bool OnlineUsersEnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("OnlineUsersEnablePaging"), false);
        }
        set
        {
            this.SetValue("OnlineUsersEnablePaging", value);
        }
    }

    
    /// <summary>
    /// Gets or sets OnlineUsersPagingItems property.
    /// </summary>
    public int OnlineUsersPagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("OnlineUsersPagingItems"), -1);
        }
        set
        {
            this.SetValue("OnlineUsersPagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets OnlineUsersEnableFiltering property.
    /// </summary>
    public bool OnlineUsersEnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("OnlineUsersEnableFiltering"), false);
        }
        set
        {
            this.SetValue("OnlineUsersEnableFiltering", value);
        }
    }

    
    /// <summary>
    /// Gets or sets OnlineUsersShowFilterItems property.
    /// </summary>
    public int OnlineUsersShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("OnlineUsersShowFilterItems"), -1);
        }
        set
        {
            this.SetValue("OnlineUsersShowFilterItems", value);
        }
    }


    /// <summary>
    /// Gets or sets ResponseMaxUsers property.
    /// </summary>
    public int ResponseMaxUsers
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("ResponseMaxUsers"), -1);
        }
        set
        {
            this.SetValue("ResponseMaxUsers", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatRoomsTransformation property.
    /// </summary>
    public string ChatRoomsTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatRoomsTransformation"), ChatHelper.TransformationRooms);
        }
        set
        {
            this.SetValue("ChatRoomsTransformation", value);
        }
    }

    
    /// <summary>
    /// Gets or sets ChatRoomNameTransformationName property.
    /// </summary>
    public string ChatRoomNameTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatRoomNameTransformationName"), ChatHelper.TransformationRoomName);
        }
        set
        {
            this.SetValue("ChatRoomNameTransformationName", value);
        }
    }

    
    /// <summary>
    /// Gets or sets NotificationTransformation property.
    /// </summary>
    public string NotificationTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("NotificationTransformation"), ChatHelper.TransformationNotifications);
        }
        set
        {
            this.SetValue("NotificationTransformation", value);
        }
    }

    
    /// <summary>
    /// Gets or sets ChatMessageTransformationName property.
    /// </summary>
    public string ChatMessageTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatMessageTransformationName"), ChatHelper.TransformationRoomMessages);
        }
        set
        {
            this.SetValue("ChatMessageTransformationName", value);
        }
    }

    
    /// <summary>
    /// Gets or sets ChatUserTransformationName property.
    /// </summary>
    public string ChatUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatUserTransformationName"), ChatHelper.TransformationRoomUsers);
        }
        set
        {
            this.SetValue("ChatUserTransformationName", value);
        }
    }

    
    /// <summary>
    /// Gets or sets OnlineUserTransformationName property.
    /// </summary>
    public string OnlineUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("OnlineUserTransformationName"), ChatHelper.TransformationOnlineUsers);
        }
        set
        {
            this.SetValue("OnlineUserTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string ChatErrorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatErrorTransformationName"), ChatHelper.TransformationErrors);
        }
        set
        {
            this.SetValue("ChatErrorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ButtonTemplate property.
    /// </summary>
    public string ChatErrorDeleteAllButtonTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatErrorDeleteAllButtonTransformationName"), ChatHelper.TransformationErrorsDeleteAll);
        }
        set
        {
            this.SetValue("ChatErrorDeleteAllButtonTransformationName", value);
        }
    }
    
   
    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ChatHelper.RegisterStylesheet(Page);
    } 


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Insert cript references
        ChatHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatWebpart_files/ChatWebpart.js");
        
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatWebpart_" + ClientID, string.Format("InitChatWebpart('{0}');", pnlChatWebpartHeader.ClientID), true);
        
        SetUpWebparts();
    }


    private void SetUpWebparts()
    {
        RoomLeave.GroupID = GroupID;

        Errors.ErrorTransformationName = ChatErrorTransformationName;
        Errors.ButtonDeleteAllTransformationName = ChatErrorDeleteAllButtonTransformationName;
        Errors.ShowDeleteAllBtn = ErrorShowDeleteAllBtn;

        Notification.NotificationTransformation = NotificationTransformation;
        Notification.ChatMessageTransformationName = ChatMessageTransformationName;
        Notification.ChatRoomUserTransformationName = ChatUserTransformationName;
        Notification.ChatErrorTransformationName = ChatErrorTransformationName;
        Notification.ChatErrorDeleteAllButtonTransformationName = ChatErrorDeleteAllButtonTransformationName;
        Notification.EnableNotificationBubble = EnableNotificationBubble;

        OnlineUsers.OnlineUserTransformationName = OnlineUserTransformationName;
        OnlineUsers.EnablePaging = OnlineUsersEnablePaging;
        OnlineUsers.PagingItems = OnlineUsersPagingItems;
        OnlineUsers.EnableFiltering = OnlineUsersEnableFiltering;
        OnlineUsers.ShowFilterItems = OnlineUsersShowFilterItems;
        OnlineUsers.ChatMessageTransformationName = ChatMessageTransformationName;
        OnlineUsers.ChatRoomUserTransformationName = ChatUserTransformationName;
        OnlineUsers.ChatErrorTransformationName = ChatErrorTransformationName;
        OnlineUsers.ChatErrorDeleteAllButtonTransformationName = ChatErrorDeleteAllButtonTransformationName;

        SearchOnlineUsers.OnlineUserTransformationName = OnlineUserTransformationName;
        SearchOnlineUsers.PagingEnabled = OnlineUsersEnablePaging;
        SearchOnlineUsers.PagingItems = OnlineUsersPagingItems;
        SearchOnlineUsers.ResponseMaxUsers = ResponseMaxUsers;
        SearchOnlineUsers.ChatMessageTransformationName = ChatMessageTransformationName;
        SearchOnlineUsers.ChatRoomUserTransformationName = ChatUserTransformationName;
        SearchOnlineUsers.ChatErrorTransformationName = ChatErrorTransformationName;
        SearchOnlineUsers.ChatErrorDeleteAllButtonTransformationName = ChatErrorDeleteAllButtonTransformationName;

        Rooms.GroupID = GroupID;
        Rooms.ListItemTransformation = ChatRoomsTransformation;
        Rooms.EnablePaging = RoomsEnablePaging;
        Rooms.PagingItems = RoomsPagingItems;
        Rooms.EnableFiltering = RoomsEnableFiltering;
        Rooms.ShowFilterItems = RoomsShowFilterItems;
        Rooms.EnablePopup = RoomsEnablePopup;

        RoomMessages.GroupID = GroupID;
        RoomMessages.Count = Count;
        RoomMessages.EnableBBCode = EnableBBCode;
        RoomMessages.DisplayInline = DisplayInline;
        RoomMessages.Direction = Direction;
        RoomMessages.ChatMessageTransformationName = ChatMessageTransformationName;

        RoomName.GroupID = GroupID;
        RoomName.DisplayInitialTitle = DisplayInitialTitle;
        RoomName.InitialTitle = InitialTitle;
        RoomName.ChatRoomNameTransformationName = ChatRoomNameTransformationName;

        RoomUsers.GroupID = GroupID;
        RoomUsers.EnablePaging = RoomUsersEnablePaging;
        RoomUsers.PagingItems = RoomUsersPagingItems;
        RoomUsers.EnableFiltering = RoomUsersEnableFiltering;
        RoomUsers.ShowFilterItems = RoomUsersShowFilterItems;
        RoomUsers.ChatUserTransformationName = ChatUserTransformationName;
        RoomUsers.ChatRoomUserTransformationName = ChatUserTransformationName;
        RoomUsers.ChatMessageTransformationName = ChatMessageTransformationName;
        RoomUsers.ChatErrorTransformationName = ChatErrorTransformationName;
        RoomUsers.ChatErrorDeleteAllButtonTransformationName = ChatErrorDeleteAllButtonTransformationName;
        RoomUsers.SortByStatus = RoomUsersSortByStatus;
        RoomUsers.InviteSearchMode = SearchMode;
        RoomUsers.InviteSearchModeMaxUsers = ResponseMaxUsers;

        RoomMessageSend.GroupID = GroupID;
        RoomMessageSend.EnableBBCode = EnableBBCode;

        if (SearchMode)
        {
            SearchOnlineUsers.Enabled = true;
            SearchOnlineUsers.Visible = true;
        }
        else
        {
            OnlineUsers.Enabled = true;
            OnlineUsers.Visible = true;
        }
    }
}