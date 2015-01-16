<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Chat/ChatMessageSend.ascx.cs"
    Inherits="CMSWebParts_Chat_ChatMessageSend" %>
<asp:Panel runat="server" ID="pnlWebpartContent" CssClass="ChatWebpartContainer ChatWebpartContainerMessageSend EnabledWebpart">
    <table class="ChatSendAreaButton">
        <tr>
            <td>
                <asp:TextBox runat="server" ID="txtMessage" TextMode="multiline"></asp:TextBox>
                <cms:BBEditor ID="ucBBEditor" runat="server" />
            </td>
            <td>
                <cms:LocalizedButton runat="server" ID="btnSendMessage" CssClass="SubmitButton" UseSubmitBehavior="true"
                    EnableViewState="false" ResourceString="chat.sendmessage" Visible="true" />
            </td>
        </tr>
    </table>
    <cms:LocalizedButton runat="server" ID="btnCannedResponses" CssClass="ContentButton BtnCannedResponses"
                    Style="display: none;" UseSubmitBehavior="true" ResourceString="chat.cannedresponses" />
    <asp:Panel runat="server" ID="pnlRecipientContainer" CssClass="ChatWebpartRecipientConatiner">
        <table><tr>
        <td><asp:CheckBox runat="server" ID="chbWhisper" /></td>
        <td class="ChatDrpRecipient"><asp:DropDownList runat="server" ID="drpRecipient" /></td>
        </tr></table>
    </asp:Panel>

    <%-- Informatino panel --%>
    <asp:Panel runat="server" ID="pnlChatMessageSendInfoDialog" CssClass="ChatDialogBody ChatMessageSendInformDialogBody">
        <div class="ChatDialogHeader">
            <table>
                <tr>
                    <td><asp:Image ID="imgInformationDialog" runat="server" /></td>
                    <td><cms:LocalizedLabel runat="server" ID="lblChatMessageSendInformDialogHeader" SetWindowTitle="false" ResourceString="chat.messages.informationdialog" /></td>
                </tr>
            </table>
        </div>
        <div class="ChatDialogContent ChatMessageSendInformDialogContent">
            <cms:LocalizedLabel runat="server" ID="lblChatMessageSendInformDialog" ResourceString="chat.messages.cannotsendemptymessage" /><br />
        </div>
        <div class="ChatDialogFooter">
            <div class="ChatDialogFooterButtons">
                <cms:LocalizedButton runat="server" ID="btnChatMessageSendInformDialogClose" UseSubmitBehavior="true" 
                    CssClass="SubmitButton" ResourceString="general.close" />
            </div>
        </div>
    </asp:Panel>
</asp:Panel>
