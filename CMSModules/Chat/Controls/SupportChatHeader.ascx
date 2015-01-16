<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Chat_Controls_SupportChatHeader" CodeFile="SupportChatHeader.ascx.cs" %>
<asp:Panel ID="pnlSupportChat" runat="server" CssClass="ChatSupportNotificationControl">
    <div class="UIHeaderSelector">
        <cms:ContextMenuContainer runat="server" ID="menuCont">
            <asp:ImageButton runat="server" ID="imgChat" />
        </cms:ContextMenuContainer>
    </div>
    <asp:Panel ID="pnlError" runat="server" CssClass="ChatSupportError Btn" Style="display:none">
        <cms:LocalizedLabel ID="lblErrorText" runat="server" ResourceString="chat.support.error"></cms:LocalizedLabel>
        <asp:Label ID="lblError" runat="server" />
    </asp:Panel>
    
    <asp:Panel runat="server" ID="pnlNotificationBubble" CssClass="SupportNotificationBubble" Style="display:none">
        <div class="FloatLeft">
            <div class="BubbleTitleDiv">
                <asp:Label id="lblNOtificationBubleTitle" runat="server" Text="Title" CssClass="FloatLeft"></asp:Label>
            </div>
            <div class="BubbleNewMessagesDiv">
                <cms:ExtendedDropDownList ID="drpNewMessageWarning" runat="server" Visible="true" CssClass="NewMessageSelect" >
                </cms:ExtendedDropDownList>
            </div>
            <asp:Image runat="server" ID="imgHideBubble" EnableViewState="false" CssClass="ChatHideBubble Btn" />
        </div>
    </asp:Panel>
</asp:Panel>
