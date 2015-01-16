using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.Collections;

using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Chat;
using CMS.PortalControls;
using CMS.ExtendedControls;
using CMS.CMSHelper;

public partial class CMSModules_Chat_Controls_SupportChatHeader : CMSUserControl
{
    #region "Private methods"

    /// <summary>
    /// Create and launch startup script
    /// </summary>
    /// <param name="settingsId">ID of popup window settings</param>
    private void RegisterStartupScript()
    {
        JavaScriptSerializer sr = new JavaScriptSerializer();

        string json = sr.Serialize(
            new
            {
                imgStatus = imgChat.ClientID,
                controlID = pnlSupportChat.ClientID,
                tick = 5000,
                contextMenuID = menuCont.MenuID,
                drpNewMessageWarningID = drpNewMessageWarning.ClientID,
                lblErrorID = lblError.ClientID,
                pnlErrorID = pnlError.ClientID,
                pnlNotificationBubbleID = pnlNotificationBubble.ClientID,
                btnHideBubbleID = imgHideBubble.ClientID,
                lblBubbleTitleID = lblNOtificationBubleTitle.ClientID,
                youAreAvailableText = ResHelper.GetString("chat.support.youreavailable"),
                youAreNotAvailableText = ResHelper.GetString("chat.support.youreunavailable"),
                selectRoomToOpen = ResHelper.GetString("chat.support.selectroom"),
                newMessagesFormat = ResHelper.GetString("chat.support.newmessages"),
                newSupportRequestsSingular = ResHelper.GetString("chat.support.newrequestssingular"),
                newSupportRequestsPlural = ResHelper.GetString("chat.support.newrequestsplural"),
                noNewSupportRequests = ResHelper.GetString("chat.support.nonewrequests"),
                newMessagesTitle = ResHelper.GetString("chat.general.newmessages"),
                imgOnlineSrc = GetImageUrl("CMSModules/CMS_Chat/support_header_on.png"),
                imgOfflineSrc = GetImageUrl("CMSModules/CMS_Chat/support_header_off.png"),
                popupWindowUrl = ChatHelper.GetChatRoomWindowURL() + "?isSupport=1&windowroomid={RoomIDParam}",
                settingsUrl = URLHelper.GetAbsoluteUrl("~/CMSModules/Chat/Pages/ChatSupportSettings.aspx"),
                strCloseError = ResHelper.GetString("chat.support.errorclose"),
                popupWindowError = ResHelper.GetString("chat.settings.popupwindowerrormsg")
            }
        );

        string startupScript = String.Format("jQuery(function(){{ InitChatSupportManager({0}); }});", json);

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SupportChatHeader_" + ClientID, startupScript, true);
    }

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!Visible || !ChatHelper.IsSupportChatPanelEnabled() || CMSPage.SignOutPending)
        {
            Visible = false;

            return;
        }

        // Set context menu for support chat
        imgChat.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/support_header_off.png");
        menuCont.MenuControlPath = "~/CMSModules/Chat/Controls/SupportChatContextMenu.ascx";
        menuCont.MenuID = ClientID + "m_chat_context_menu";
        menuCont.ParentElementClientID = ClientID;
        menuCont.Parameter = "''";
        menuCont.RenderAsTag = HtmlTextWriterTag.A;
        menuCont.MouseButton = MouseButtonEnum.Left;
        menuCont.VerticalPosition = VerticalPositionEnum.Bottom;
        menuCont.HorizontalPosition = HorizontalPositionEnum.Left;

        imgHideBubble.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/hidebubble.png");
        imgHideBubble.AlternateText = ResHelper.GetString("chat.support.hidebubble");
        imgHideBubble.ToolTip = ResHelper.GetString("chat.support.hidebubble");

        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatSupportAJAXProxy(cmsPage);
        }

        // Script references insertion
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/Controls/SupportChatHeader.js");

        // Create and launch startup script.
        RegisterStartupScript();
        CSSHelper.RegisterCSSLink(Page, "~/App_Themes/Design/Chat/ChatSupportHeader.css");
    }

    #endregion
}
