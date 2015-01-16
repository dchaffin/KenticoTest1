using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

[Actions(1)]
[Action(0, "Objects/CMS_ChatUser/add.png", "chat.chatuser.new", "Edit.aspx")]

public partial class CMSModules_Chat_Pages_Tools_ChatUser_List : CMSChatPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        HeaderActions.Enabled = AnyModifyPermissionAllowed;
    }

    #endregion
}
