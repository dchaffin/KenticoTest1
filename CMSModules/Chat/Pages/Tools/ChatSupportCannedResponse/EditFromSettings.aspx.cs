using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Chat;
using CMS.SettingsProvider;

[EditedObject(PredefinedObjectType.CHATSUPPORTCANNEDRESPONSE, "responseId")]

[Title("CMSModules/CMS_Chat/support_chat_settings24.png", "chat.support.settingstitle", null)]

[Breadcrumbs(2)]
[Breadcrumb(0, "chat.support.settingstitle", "~/CMSModules/Chat/Pages/ChatSupportSettings.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.ChatSupportCannedResponseTagName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "chat.cannedresponse.chatsupportcannedresponse.new", NewObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_EditFromSettings : CMSModalPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Chat", "EnterSupport"))
        {
            RedirectToAccessDenied("CMS.Chat", "EnterSupport");
        }

        ChatSupportCannedResponseInfo editedObject = (ChatSupportCannedResponseInfo)CMSContext.EditedObject;

        if ((editedObject != null) && (!editedObject.ChatSupportCannedResponseChatUserID.HasValue ||  (editedObject.ChatSupportCannedResponseChatUserID.Value != ChatUserHelper.GetChatUserFromCMSUser().ChatUserID)))
        {
            RedirectToAccessDenied(GetString("chat.error.cannedresponsenotyours"));
        }

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnClose.Text = GetString("general.close");

        CurrentMaster.DisplayActionsPanel = true;

        editElem.Personal = true;
    }

    #endregion
}
