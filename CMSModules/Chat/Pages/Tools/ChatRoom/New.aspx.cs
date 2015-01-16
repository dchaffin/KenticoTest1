using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;
using CMS.SettingsProvider;

[Help("new_chat_room")]

[Breadcrumbs(2)]
[Breadcrumb(0, "chat.chatroom.list", "~/CMSModules/Chat/Pages/Tools/ChatRoom/List.aspx?siteid={?siteid?}", null)]
[Breadcrumb(1, ResourceString = "chat.chatroom.new")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_New : CMSChatPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        int siteID = QueryHelper.GetInteger("siteid", CMSContext.CurrentSiteID);

        CheckReadPermission(siteID);

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int siteID = QueryHelper.GetInteger("siteid", CMSContext.CurrentSiteID);

        editElem.SiteID = siteID;
    }

    #endregion
}
