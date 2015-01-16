using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;
using CMS.SettingsProvider;

public partial class CMSModules_Chat_Controls_UI_ChatRoom_Edit : CMSAdminEditControl
{
    #region "Private fields"

    private int? mSiteID;

    #endregion


    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return this.EditForm;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            this.EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }


    /// <summary>
    /// SiteID of a new room.
    /// 
    /// NULL means global.
    /// </summary>
    public int? SiteID
    {
        get
        {
            if (TypedEditedObject != null)
            {
                return TypedEditedObject.ChatRoomSiteID;
            }

            return mSiteID;
        }
        set
        {
            if (value <= 0)
            {
                value = null;
            }
            mSiteID = value;
        }
    }

    public ChatRoomInfo TypedEditedObject
    {
        get
        {
            return (ChatRoomInfo)CMSContext.EditedObject;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        UIFormControl.OnBeforeSave += new EventHandler(UIFormControl_OnBeforeSave);
        UIFormControl.OnAfterSave += new EventHandler(UIFormControl_OnAfterSave);
        UIFormControl.OnCheckPermissions += new EventHandler(UIFormControl_OnCheckPermissions);

        // Allow an empty password
        EditingFormControl passwordEditingControl = UIFormControl.FieldEditingControls["chatroompassword"];
        var passwordStrengthControl = passwordEditingControl.NestedControl as CMSModules_Membership_FormControls_Passwords_PasswordStrength;
        if (passwordStrengthControl != null)
        {
            passwordStrengthControl.AllowEmpty = true;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // If room is one to one support (support room created after user requested support), disable editing some properties.
        if ((TypedEditedObject != null) && TypedEditedObject.IsOneToOneSupport)
        {
            DisableFieldControl("ChatRoomName");
            DisableFieldControl("ChatRoomEnabled");
            DisableFieldControl("ChatRoomPrivate");
            DisableFieldControl("ChatRoomAllowAnonym");
            DisableFieldControl("ChatRoomIsSupport");
        }
    }


    void DisableFieldControl(string fieldName)
    {
        if (UIFormControl.FieldControls != null)
        {
            FormEngineUserControl fieldControl = UIFormControl.FieldControls[fieldName];

            if (fieldControl != null)
            {
                fieldControl.Enabled = false;
            }
        }
    }


    void UIFormControl_OnCheckPermissions(object sender, EventArgs e)
    {
        ((CMSChatPage)Page).CheckModifyPermission(SiteID);
    }


    void UIFormControl_OnBeforeSave(object sender, EventArgs e)
    {
        IDataContainer data = UIFormControl.Data;

        // Set site id and other data if the room is new
        if (CMSContext.EditedObject == null)
        {
            EditingFormControl passwordEditingControl = UIFormControl.FieldEditingControls["chatroompassword"];
            data["ChatRoomCreatedByChatUserID"] = ChatUserHelper.GetChatUserFromCMSUser().ChatUserID;
            data["ChatRoomCreatedWhen"] = DateTime.Now; // GETDATE() will be used on SQL Server side
            data["ChatRoomSiteID"] = SiteID;

            Guid guid = Guid.NewGuid();

            data["ChatRoomGUID"] = guid;

            string password = passwordEditingControl.Value.ToString();
            passwordEditingControl.Value = ChatRoomHelper.GetRoomPasswordHash(password, guid);
        }
        else
        {
            ChatRoomInfo room = CMSContext.EditedObject as ChatRoomInfo;
            if (room != null)
            {
                EditingFormControl enabledControl = UIFormControl.FieldEditingControls["chatroomenabled"];
                bool enabled = (bool)enabledControl.Value;
                if (room.ChatRoomEnabled != enabled)
                {
                    if (enabled)
                    {
                        ChatRoomHelper.EnableChatRoom(room.ChatRoomID); 
                    }
                    else
                    {
                        ChatRoomHelper.DisableChatRoom(room.ChatRoomID);
                    }
                }
            }
        }
    }


    void UIFormControl_OnAfterSave(object sender, EventArgs e)
    {
        // Refresh header with display name in breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, null);
    }

    #endregion
}

