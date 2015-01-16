using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.Chat;

[Title("CMSModules/CMS_Chat/support_chat_settings24.png", "chat.support.settingstitle", null)]

[Actions(1)]
[Action(0, "Objects/CMS_ChatSupportCannedResponse/add.png", "chat.cannedresponse.chatsupportcannedresponse.new", "~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/EditFromSettings.aspx")]

public partial class CMSModules_Chat_Pages_ChatSupportSettings : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        listElem.FromSettings = true;

        btnClose.Text = GetString("general.close");
    }
}