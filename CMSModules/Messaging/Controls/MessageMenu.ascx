<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Messaging_Controls_MessageMenu"
    CodeFile="MessageMenu.ascx.cs" %>
<asp:Panel runat="server" ID="pnlMessageMenu" CssClass="PortalContextMenu WebPartContextMenu">
    <asp:Panel runat="server" ID="pnlReply" CssClass="Item">
        <asp:Panel runat="server" ID="pnlReplyPadding" CssClass="ItemPadding">
            <asp:Image runat="server" ID="imgReply" CssClass="Icon" EnableViewState="false" />&nbsp;
            <cms:LocalizedLabel runat="server" ID="lblReply" CssClass="Name" EnableViewState="false"
                ResourceString="messaging.reply" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlForward" CssClass="Item">
        <asp:Panel runat="server" ID="pnlForwardPadding" CssClass="ItemPadding">
            <asp:Image runat="server" ID="imgForward" CssClass="Icon" EnableViewState="false" />&nbsp;
            <cms:LocalizedLabel runat="server" ID="lblForward" CssClass="Name" EnableViewState="false"
                ResourceString="messaging.forward" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlMarkRead" CssClass="Item">
        <asp:Panel runat="server" ID="pnlMarkReadPadding" CssClass="ItemPadding">
            <asp:Image runat="server" ID="imgMarkRead" CssClass="Icon" EnableViewState="false" />&nbsp;
            <cms:LocalizedLabel runat="server" ID="lblMarkRead" CssClass="Name" EnableViewState="false"
                ResourceString="Messaging.Action.MarkAsRead" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlMarkUnread" CssClass="ItemLast">
        <asp:Panel runat="server" ID="pnlMarkUnreadPadding" CssClass="ItemPadding">
            <asp:Image runat="server" ID="imgMarkUnread" CssClass="Icon" EnableViewState="false" />&nbsp;
            <cms:LocalizedLabel runat="server" ID="lblMarkUnread" CssClass="Name" EnableViewState="false"
                ResourceString="Messaging.Action.MarkAsUnread" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>