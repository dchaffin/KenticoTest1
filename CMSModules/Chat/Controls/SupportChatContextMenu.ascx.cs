using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;
using CMS.PortalControls;
using CMS.ExtendedControls;

public partial class CMSModules_Chat_Controls_SupportChatContextMenu : CMSContextMenuControl
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        imgSettings.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/support_chat_settings.png");
        imgSettings.AlternateText = ResHelper.GetString("chat.support.settings");

        imgShowBubble.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/showbubble.png");
        imgShowBubble.AlternateText = ResHelper.GetString("chat.support.showbubble");
        
        imgLogin.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/support_chat_on.png");
        imgLogin.AlternateText = ResHelper.GetString("chat.support.login");

        imgLogout.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/support_chat_off.png");
        imgLogout.AlternateText = ResHelper.GetString("chat.support.logout");

        imgHelp.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/help.png");
        imgHelp.AlternateText = ResHelper.GetString("helpicon.help");
        linkHelp.Target = "_blank";
        linkHelp.NavigateUrl = CMS.GlobalHelper.UIHelper.GetContextHelpUrl("chat_support_chat");

    }

    #endregion
}
