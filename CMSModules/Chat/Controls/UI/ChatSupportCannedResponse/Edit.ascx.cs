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

public partial class CMSModules_Chat_Controls_UI_ChatSupportCannedResponse_Edit : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// SiteID of a new canned response taken from query string.
    /// 
    /// NULL means global.
    /// </summary>
    public int? SiteID
    {
        get
        {
            int siteID = QueryHelper.GetInteger("siteid", 0);

            // Global
            if (siteID <= 0)
            {
                return null;
            }

            return siteID;
        }
    }

    /// <summary>
    /// Returns edited object info.
    /// </summary>
    public ChatSupportCannedResponseInfo TypedEditedObject
    {
        get
        {
            return (ChatSupportCannedResponseInfo)CMSContext.EditedObject;
        }
    }

    /// <summary>
    /// Indicates if canned response is personal.
    /// </summary>
    public bool Personal { get; set; }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeSave += new EventHandler(UIFormControl_OnBeforeSave);
        EditForm.OnCheckPermissions += new EventHandler(UIFormControl_OnCheckPermissions);

        if (Personal)
        {
            EditForm.RedirectUrlAfterCreate = "~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/EditFromSettings.aspx?responseid={%EditedObject.ID%}&siteid={?siteid?}&saved=1";
        }
        else
        {
            EditForm.RedirectUrlAfterCreate = "~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/Edit.aspx?responseid={%EditedObject.ID%}&siteid={?siteid?}&saved=1";
        }
    }


    void UIFormControl_OnCheckPermissions(object sender, EventArgs e)
    {
        if (Personal)
        {
            CurrentUserInfo currentUser = CMSContext.CurrentUser;

            if (!currentUser.IsAuthorizedPerResource("CMS.Chat", "EnterSupport"))
            {
                RedirectToAccessDenied("CMS.Chat", "EnterSupport");
            }

            if ((TypedEditedObject != null) && (TypedEditedObject.ChatSupportCannedResponseChatUserID.Value != ChatUserHelper.GetChatUserFromCMSUser().ChatUserID))
            {
                RedirectToAccessDenied(GetString("general.modifynotallowed"));
            }
        }
        else
        {
            int? siteIDToCheck;

            // Editing existing canned response
            if (TypedEditedObject != null)
            {
                siteIDToCheck = TypedEditedObject.ChatSupportCannedResponseSiteID;
            }
            // Creating new
            else
            {
                siteIDToCheck = SiteID;
            }

            ((CMSChatPage)Page).CheckModifyPermission(siteIDToCheck);
        }
    }


    void UIFormControl_OnBeforeSave(object sender, EventArgs e)
    {
        // If creating new canned response
        if (TypedEditedObject == null)
        {
            if (QueryHelper.Contains("siteid"))
            {
                EditForm.Data["ChatSupportCannedResponseChatUserID"] = null;

                EditForm.Data["ChatSupportCannedResponseSiteID"] = SiteID;
            }
            else
            {
                EditForm.Data["ChatSupportCannedResponseChatUserID"] = ChatUserHelper.GetChatUserFromCMSUser().ChatUserID;
            }
        }
    }

    #endregion
}

