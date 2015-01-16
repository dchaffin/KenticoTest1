<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_General_ViewModeMenu"
    CodeFile="ViewModeMenu.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/MenuButton.ascx"
    TagName="MenuButton" TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlModes" CssClass="DialogMenuRight">
    <cms:MenuButton ID="menuBtnList" runat="server" CssClass="MenuItemRight" CssHoverClass="MenuItemRightOver"
        CssActiveClass="MenuItemRightOver" ActiveGroup="MenuRight" />
    <cms:MenuButton ID="menuBtnTiles" runat="server" CssClass="MenuItemRight" CssHoverClass="MenuItemRightOver"
        CssActiveClass="MenuItemRightOver" ActiveGroup="MenuRight" />
    <cms:MenuButton ID="menuBtnThumbs" runat="server" CssClass="MenuItemRight" CssHoverClass="MenuItemRightOver"
        CssActiveClass="MenuItemRightOver" ActiveGroup="MenuRight" />
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false"></asp:Literal>
<asp:HiddenField ID="hdnLastSelectedTab" runat="server" />

