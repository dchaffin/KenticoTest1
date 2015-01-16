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

[Help("chat_edit_canned_response")]

[EditedObject(PredefinedObjectType.CHATSUPPORTCANNEDRESPONSE, "responseId")]

[Breadcrumbs(2)]
[Breadcrumb(0, "chat.cannedresponse.chatsupportcannedresponse.list", "~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/List.aspx?siteid={?siteid?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.ChatSupportCannedResponseTagName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "chat.cannedresponse.chatsupportcannedresponse.new", NewObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_Edit : CMSChatPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        ChatSupportCannedResponseInfo editedObject = (ChatSupportCannedResponseInfo)CMSContext.EditedObject;

        if (editedObject != null)
        {
            CheckReadPermission(editedObject.ChatSupportCannedResponseSiteID);
        }

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.Personal = false;
    }

    #endregion
}
