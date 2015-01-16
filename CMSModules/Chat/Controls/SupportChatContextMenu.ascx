<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Chat_Controls_SupportChatContextMenu" CodeFile="SupportChatContextMenu.ascx.cs" %>
<asp:Panel runat="server" ID="pnlUniGridMenu" CssClass="PortalContextMenu WebPartContextMenu"
    EnableViewState="false">
    <asp:Panel runat="server" ID="pnlChatSupportMenu" CssClass="ItemLast">
        <asp:Panel runat="server" ID="pnlLogin" CssClass="ItemPadding ChatPnlLogin" Visible="true">
            <asp:Image runat="server" ID="imgLogin" CssClass="Icon" EnableViewState="false"/>
            <cms:LocalizedLabel runat="server" ID="lblLogin" CssClass="Name" EnableViewState="false"
                ResourceString="chat.support.login" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlLogout" CssClass="ItemPadding ChatPnlLogout" Visible="true">
            <asp:Image runat="server" ID="imgLogout" CssClass="Icon" EnableViewState="false"/>
            <cms:LocalizedLabel runat="server" ID="lblLogout" CssClass="Name" EnableViewState="false"
                ResourceString="chat.support.logout" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSettings" CssClass="ItemPadding ChatPnlSettings" Visible="true">
            <asp:Image runat="server" ID="imgSettings" CssClass="Icon" EnableViewState="false"/>
            <cms:LocalizedLabel runat="server" ID="lblSettings" CssClass="Name" EnableViewState="false"
                ResourceString="chat.support.settings" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlShowBubble" CssClass="ItemPadding ChatPnlShowBubble" Visible="true">
            <asp:Image runat="server" ID="imgShowBubble" CssClass="Icon" EnableViewState="false"/>
            <cms:LocalizedLabel runat="server" ID="lblShowBubble" CssClass="Name" EnableViewState="false"
                ResourceString="chat.support.showbubble" />
        </asp:Panel>
        <asp:HyperLink runat="server" ID="linkHelp" CssClass="ItemPadding" Visible="true" style="text-decoration:none">
            <asp:Image runat="server" ID="imgHelp" CssClass="Icon" EnableViewState="false"/>
            <cms:LocalizedLabel runat="server" ID="lblHelp" CssClass="Name" EnableViewState="false"
                ResourceString="helpicon.help" style="padding-left:6px;" />
        </asp:Hyperlink>
    </asp:Panel>
</asp:Panel>
