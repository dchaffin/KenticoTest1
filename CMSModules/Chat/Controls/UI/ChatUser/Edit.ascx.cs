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
using CMS.SiteProvider;
using CMS.ExtendedControls;

public partial class CMSModules_Chat_Controls_UI_ChatUser_Edit : CMSAdminEditControl
{
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
            UIFormControl.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets ChatUser which is edited now
    /// </summary>
    private ChatUserInfo TypedEditedObject
    {
        get
        {
            return (ChatUserInfo)CMSContext.EditedObject;
        }
    }


    /// <summary>
    /// If true, error messages will be displayed on top of the controls.
    /// </summary>
    public bool IsOnModalDialog { get; set; }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        UIFormControl.OnCheckPermissions += new EventHandler(UIFormControl_OnCheckPermissions);
        UIFormControl.OnBeforeValidate += new EventHandler(UIFormOnOnBeforeValidate);

        ScriptHelper.RegisterDialogScript(Page);

        ChatUserInfo user = TypedEditedObject;

        // Setting attributes of controls
        if (user != null)
        {
            if (user.IsAnonymous)
            {
                RedirectToInformation(ResHelper.GetString("chat.user.cannoteditanonymoususer"));
                return;
            }

            UserInfo cmsUser = UserInfoProvider.GetUserInfo(user.ChatUserUserID.Value);

            pnlStaticUser.Visible = true;
            fUserSelector.Visible = false;
            fUserSelector.Value = user.ChatUserUserID;

            txtUserNameStaticValue.Text = HTMLHelper.HTMLEncode(cmsUser.FullName);
            btnEditUser.OnClientClick = "modalDialog('" + CMSContext.ResolveDialogUrl("~/CMSModules/Membership/Pages/Users/User_Edit_Dialog.aspx") + "?userid=" + user.ChatUserUserID + "', 'UserEdit', 950, 800); return false;";
            btnEditUser.Text = GetString("general.edit");
        }
        else
        {
            // New object
            pnlStaticUser.Visible = false;
            fUserSelector.Visible = true;
        }

    }


    void UIFormControl_OnCheckPermissions(object sender, EventArgs e)
    {
        // User can edit chat users if he has at least one of modify global and modify permissions
        ((CMSChatPage)Page).CheckAnyModifyPermission();
    }

    #endregion


    #region "Methods"

    public void Save()
    {
        UIFormControl.SaveData(null);
    }

    protected void UIFormOnOnBeforeValidate(object sender, EventArgs e)
    {
        int selectedCMSUserId = ValidationHelper.GetInteger(fUserSelector.Value, 0);

        UserInfo user = UserInfoProvider.GetUserInfo(selectedCMSUserId);

        // Validate form - both fields must be filled
        if ((user == null) || (fNickname.EditingControl.Text == ""))
        {
            ShowErrorAndStopProcessing("chat.user.erroridnickname");
            return;
        }

        if (user.IsPublic())
        {
            ShowErrorAndStopProcessing("chat.cantassociatechatusertopublic");
            return;
        }


        // Editing
        if (TypedEditedObject != null)
        {
            if (!TypedEditedObject.ChatUserUserID.HasValue)
            {
                ShowErrorAndStopProcessing("chat.user.cannoteditanonymoususer");
                return;
            }
        }
        else
        {
            // New object

            // Check if userID is unique in ChatUser table if adding a new user
            if (ChatUserInfoProvider.GetChatUserByUserID(selectedCMSUserId) != null)
            {
                ShowErrorAndStopProcessing("chat.user.erroridunique");
                return;
            }
        }

        string newNickname = fNickname.EditingControl.Text;

        // Check nickname only if text has been changed
        if ((TypedEditedObject == null) || (newNickname != TypedEditedObject.ChatUserNickname))
        {
            try
            {
                ChatUserHelper.VerifyNicknameIsValid(ref newNickname);
            }
            catch (ChatServiceException ex)
            {
                ShowErrorAndStopProcessing(ex.StatusMessage);
                return;
            }


            // Check if Nickname is unique in registered users
            if (!ChatUserHelper.IsNicknameAvailable(newNickname))
            {
                ShowErrorAndStopProcessing("chat.user.errornickunique");
                return;
            }
        }

        fNickname.EditingControl.Text = newNickname;
    }


    /// <summary>
    /// Shows error and sets StopProcessing flag of UIFormControl to true.
    /// 
    /// Error is passed as resource string, which is resolved before displaying.
    /// </summary>
    /// <param name="resourceString">Error resource string</param>
    private void ShowErrorAndStopProcessing(string resourceString)
    {
        if (IsOnModalDialog)
        {
            ShowMessage(MessageTypeEnum.Error, GetString(resourceString), null, null, false);
        }
        else
        {
            ShowError(GetString(resourceString));
        }

        UIFormControl.StopProcessing = true;
    }

    #endregion
}

