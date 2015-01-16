<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ScreenLockDialog.ascx.cs" Inherits="CMSAdminControls_UI_ScreenLock_ScreenLockDialog" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>

<div id="screenLockWarningDialog" class="HeaderWarning ScreenLockWarningDialog" style="display: none;">
    <table>
        <tr>
            <td>
                <asp:Image ID="imgWarning" runat="server" EnableViewState="false" Width="16" Height="16" />
            </td>
            <td class="ScreenLockWarningCell">
                <cms:LocalizedLabel ID="lblWarningTextStart" runat="server" ResourceString="screenlock.warningtext" EnableViewState="false" />
                <span id="screenLockTimeDialog">0</span>
                <cms:LocalizedLabel ID="lblWarningTextEnd" runat="server" ResourceString="screenlock.seconds" EnableViewState="false" />
                <asp:LinkButton runat="server" OnClientClick="CancelScreenLockCountdown(); return false;" EnableViewState="false"><%=GetString("general.cancel")%></asp:LinkButton>
            </td>
        </tr>
    </table>
</div>

<div id="screenLockBackground" class="ScreenLockModalBack" style="display: none;"></div>
<div id="screenLockDialog" class="ModalPopupDialog ScreenLockModalPopup" style="display: none;">
    <asp:Panel ID="pnlScreenLock" runat="server" CssClass="DialogPageBody">
        <div style="height: auto; min-height: 0px;">
            <div class="PageHeader">
                <cms:PageTitle ID="screenLockTitle" runat="server" EnableViewState="false" GenerateFullWidth="false"
                    SetWindowTitle="false" />
            </div>
        </div>
        <asp:Panel runat="server" ID="pnlScreenLockContent" class="PageContent" style="width: 350px; height: 150px;"> 
            <cms:LocalizedLabel runat="server" ID="lblInstructions" ResourceString="screenlock.instruction" style="display: block;" />
            
            <div class="ScreenLockTable">
            <table cellpadding="1" cellspacing="0" style="width: 350px;" class="TablePadding">
                <tr>
                    <td class="LoginLabel ScreenLockLogonCell">
                        <cms:LocalizedLabel ID="lblUserName" runat="server" ResourceString="logonform.username" />
                    </td>
                    <td class="LoginTextBox ScreenLockLogonCell">
                        <cms:CMSTextBox ID="txtUserName" runat="server" MaxLength="100" CssClass="LogonTextBox" Enabled="false" />
                    </td>
                </tr>
                <tr>
                    <td class="PasswordLabel ScreenLockLogonCell">
                        <cms:LocalizedLabel ID="lblPassword" runat="server" ResourceString="logonform.password" />
                    </td>
                    <td class="PasswordTextBox ScreenLockLogonCell">
                        <input type="password" id="screenLockDialogPassword" <%=hiddenPassword %> class="LogonTextBox" onkeypress="return ScreenLockEnterHandler(event)" />
                    </td>
                </tr>
                
                <tr>
                    <td colspan="2">
                        <span id="screenLockDialogWarningLogonAttempts" class="Red" style="display: none;"><%=GetString("invalidlogonattempts.unlockaccount.accountlocked") + GetString("screenlock.loggedout")%></span>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <span id="screenLockDialogWarning" class="Red" style="display: none;"><%=GetString("screenlock.wrongpassword")%></span>
                    </td>
                </tr>
            </table>   
            </div>
        </asp:Panel>
        <div class="PageFooterLine">
            <div class="Buttons">
                <input type="button" class="SubmitButton" id="screenLockSignInButton" value="<%=GetString("screenlock.signin")%>"
                    onclick="ScreenLockRedirectToLogon('<%=CMS.SettingsProvider.SettingsKeyProvider.GetStringValue(CMS.CMSHelper.CMSContext.CurrentSiteName + ".CMSSecuredAreasLogonPage")%>'); return false;" style="display: none;" />
                <input type="button" class="SubmitButton" id="screenLockUnlockButton" value="<%=GetString("screenlock.unlock")%>"
                    onclick="ScreenLockValidateUser(); return false;" />
                <input type="button" class="SubmitButton" id="screenLockSignOutButton" value="<%=GetString("signoutbutton.signout")%>"
                    onclick="ScreenLockLogoutUser(); return false;" />
            </div>
            <div class="ClearBoth">
                &nbsp;</div>
        
        </div>
    </asp:Panel>
</div>