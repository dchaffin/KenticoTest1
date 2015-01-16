<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Chat/ChatLogin.ascx.cs" Inherits="CMSWebParts_Chat_ChatLogin" %>

<%-- Panel: whole webpart --%>
<asp:Panel runat="server" CssClass="ChatWebpartContainer ChatWebpartContainerLogin">

<%-- Panel: user is logged in --%>
<asp:Panel runat="server" ID="pnlChatUserLoggedIn" style="display: none;">
    <cms:LocalizedLabel runat="server" ID="lblChatUserLoggedInInfoText" EnableViewState="false" ResourceString="chat.login.loggedinas"></cms:LocalizedLabel>
    <asp:Label runat="server" ID="lblChatUserLoggedInInfoValue" CssClass="ChatUserLoggedInInfoValue"></asp:Label><br />
    <div class="LoginButtons">
    <cms:LocalizedLinkButton runat="server" ID="btnChatUserLoggedInChangeNickname" EnableViewState="false" ResourceString="chat.login.changenickname"></cms:LocalizedLinkButton>
    <cms:LocalizedButton runat="server" ID="btnChatUserLoggedInLogout" CssClass="SubmitButton LongButton" UseSubmitBehavior="true" EnableViewState="false"/>
    </div>
</asp:Panel>

<%-- Panel: form for changing nickname --%>
<asp:Panel runat="server" ID="pnlChatUserChangeNicknameForm" style="display: none;">
    <asp:TextBox runat="server" ID="txtChatUserChangeNicknameInput" MaxLength="50"></asp:TextBox>
    <div class="LoginButtons" >
    <cms:LocalizedButton runat="server" ID="btnChatUserChangeNicknameButton" UseSubmitBehavior="false" CssClass="SubmitButton" EnableViewState="false" ResourceString="general.ok"/>
    <cms:LocalizedButton runat="server" ID="btnChangeNicknameCancel" UseSubmitBehavior="false" CssClass="SubmitButton" EnableViewState="false" ResourceString="general.cancel"/>
    </div> 
</asp:Panel>

<%-- Panel: User has left the chat / not logged --%>
<asp:Panel runat="server" ID="pnlChatUserLoginRelog" style="display: none;">
    <cms:LocalizedLabel runat="server" ID="lblChatUserLoginRelogText" EnableViewState="false" ResourceString="chat.login.logasanonym"></cms:LocalizedLabel>
    <cms:ExtendedTextBox ID="txtChatUserLoginRelogNickname" runat="server" MaxLength="50" />
    <asp:Label runat="server" ID="lblChatUserLoginRelogNickname" CssClass="ChatUserLoggedInInfoValue"></asp:Label>  
    <div class="LoginButtons">
    <cms:LocalizedButton runat="server" ID="btnChatUserLoginRelog" EnableViewState="false" UseSubmitBehavior="true" ResourceString="chat.login.enter"/>
    </div>
</asp:Panel>

<%-- Panel: errors --%>
<asp:Panel runat="server" ID="pnlChatUserLoginError" style="display: none;">
    <asp:Label runat="server" ID="lblChatUserLoginErrorText" CssClass="ChatError"></asp:Label>
</asp:Panel>

</asp:Panel>
