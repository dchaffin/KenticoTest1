<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_ScreenLock_ScreenLock" CodeFile="ScreenLock.ascx.cs" %>

<div id="screenLockWarning" class="HeaderWarning" style="display: none;">
    <table>
        <tr>
            <td>
                <asp:Image ID="imgWarning" runat="server" EnableViewState="false" Width="16" Height="16" />
            </td>
            <td class="ScreenLockWarningCell">
                <cms:LocalizedLabel ID="lblWarningTextStart" runat="server" ResourceString="screenlock.warningtext" EnableViewState="false" />
                <span id="screenLockTime">0</span>
                <cms:LocalizedLabel ID="lblWarningTextEnd" runat="server" ResourceString="screenlock.seconds" EnableViewState="false" />
                <asp:LinkButton runat="server" OnClientClick="CancelScreenLockCountdown(); return false;" EnableViewState="false"><% =ResHelper.GetString("general.cancel") %></asp:LinkButton>
            </td>
        </tr>
    </table>
</div>