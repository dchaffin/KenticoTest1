<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu.ascx.cs" Inherits="CMSModules_Content_Controls_Menu" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniMenu.ascx" TagName="UniMenu" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlBody" CssClass="ContentMenu">
    <asp:Panel runat="server" ID="pnlLeft" CssClass="ContentMenuLeft">
        <cms:unimenu id="contentMenu" shortid="m" runat="server" />
    </asp:Panel>
    <cms:frameresizer id="frmResizer" runat="server" vertical="True" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlHelp" CssClass="ContentMenuHelp" EnableViewState="false">
    <cms:help id="helpElem" runat="server" topicname="mode_selection" />
</asp:Panel>
