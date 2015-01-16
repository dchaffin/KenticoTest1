using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.PortalControls;
using CMS.UIControls;
using CMS.Chat;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.Controls;
using CMS.DataEngine;
using CMS.SettingsProvider;
using System.Web.Script.Serialization;

public partial class CMSWebParts_Chat_ChatSearchOnlineUsers : CMSAbstractWebPart 
{

    #region "Variables"

    bool mInviteMode = false;
    string mGroupName = "";
    bool mIsSupport = false;

    #endregion

    
    #region Properties

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
    /// Gets or sets ChatErrorTransformationName property.
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
    /// Gets or sets ChatErrorDeleteAllButtonTransformationName property.
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


    /// <summary>
    /// Gets or sets ChatRoomUserTransformationName property.
    /// </summary>
    public string ChatRoomUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatRoomUserTransformationName"), ChatHelper.TransformationRoomUsers);
        }
        set
        {
            this.SetValue("ChatRoomUserTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets PagingEnabled property.
    /// </summary>
    public bool PagingEnabled 
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("PagingEnabled"), false);
        }
        set
        {
            this.SetValue("PagingEnabled", value);
        }
    }


    /// <summary>
    /// Gets or sets PagingItems property.
    /// </summary>
    public int PagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("PagingItems"), -1);
        }
        set
        {
            this.SetValue("PagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets ResponseMaxUsers property.
    /// </summary>
    public int ResponseMaxUsers
    {
        get
        {
            int maxUsers = ValidationHelper.GetInteger(this.GetValue("ResponseMaxUsers"), -1);
            if (maxUsers < 0)
            {
                maxUsers = ChatHelper.WPSearchModeMaxUsers;
            }
            return maxUsers;
        }
        set
        {
            this.SetValue("ResponseMaxUsers", value);
        }
    }


    /// <summary>
    /// Indicates if webpart is in invite mode
    /// That means that clicking users in list will invite them to current room.
    /// </summary>
    public bool InviteMode
    {
        get
        {
            return mInviteMode;
        }
        set
        {
            mInviteMode = value;
        }
    }


    /// <summary>
    /// Gets or sets GroupName property (only used when invite mode is set). 
    /// </summary>
    public string GroupName
    {
        get
        {
            return mGroupName;
        }
        set
        {
            mGroupName = value;
        }
    }


    /// <summary>
    /// Indicates if this webpart is in support chat window.
    /// </summary>
    public bool IsSupport
    {
        get
        {
            return mIsSupport;
        }
        set
        {
            mIsSupport = value;
        }
    }


    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }


    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        
    } 


    protected void Page_Prerender(object sender, EventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }
        ChatHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeOnlineUsers", this, InnerContainerTitle, InnerContainerName);
        if (IsSupport)
        {
            ChatHelper.RegisterStylesheet(Page, true);
        }
        else
        {
            ChatHelper.RegisterStylesheet(Page);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsVisible)
        {
            pnlChatSearchOnlineUsersWP.Visible = false;
            return;
        }

        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion
        ChatHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterScriptFile(Page, "jquery/jquery-tmpl.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatSearchOnlineUsers_files/ChatSearchOnlineUsers.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ListPaging.js");        

        if (!PagingEnabled)
        {
            pnlChatSearchOnlineUsersPaging.Visible = false;
        }
        if (InviteMode)
        {
            pnlChatSearchOnlineUsersInvite.Visible = true;
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SearchOnlineUsers_" + ClientID, BuildStartupScript(), true);
    }


    #region "Methods"

    /// <summary>
    /// Builds startup script.
    /// </summary>
    private string BuildStartupScript()
    {
        int id = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                onlineUserTemplate = ChatHelper.GetWebpartTransformation(OnlineUserTransformationName, "chat.error.transformation.users.onlineuser"),
                chatRoomWindowUrl = ChatHelper.GetChatRoomWindowURL(),
                clientID = ClientID,
                GUID = id,
                contentClientID = pnlChatSearchOnlineUsers.ClientID,
                textbox = txtChatSearchOnlineUsers.ClientID,
                button = btnChatSearchOnlineUsers.ClientID,
                pnlPaging = pnlChatSearchOnlineUsersPaging.ClientID,
                pagingEnabled = PagingEnabled,
                pagingItems = PagingItems > 0 ? PagingItems : ChatHelper.WPPagingItems,
                maxUsers = ResponseMaxUsers,
                inviteMode = InviteMode,
                loadingDiv = ChatHelper.GetWebpartLoadingDiv("ChatSearchOnlineUsersWPLoading", "chat.wploading.searchonlineusers"),
                resStrMoreFound = (ResponseMaxUsers > 0) ? String.Format(ResHelper.GetString("chat.searchonlineusers.morefound"), ResponseMaxUsers) : "",
                resStrNotFound = ResHelper.GetString("chat.searchonlineusers.notfound"),
                pnlInfo = pnlChatSearchOnlineUsersInfo.ClientID,
                resStrFound = ResHelper.GetString("chat.searchonlineusers.results"),
                envelopeID = "envelope_" + ClientID,
                groupID = GroupName,
                invitePanel = pnlChatSearchOnlineUsersInvite.ClientID
            }
        );
        return String.Format("InitChatSearchOnlineUsersWebpart({0});", json);
    }

    #endregion

}
