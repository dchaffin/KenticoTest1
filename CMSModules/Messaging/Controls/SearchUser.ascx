<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Messaging_Controls_SearchUser" CodeFile="SearchUser.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="ListPanel">
    <asp:Panel ID="pnlFilter" runat="server" EnableViewState="false">
        <cms:CMSTextBox ID="txtSearch" runat="server" EnableViewState="false" CssClass="TextBoxField" /><cms:LocalizedButton
            ID="btnSearch" runat="server" OnClick="btnSearch_Click" ResourceString="general.search"
            CssClass="ContentButton" />
        <br />
        <br />
    </asp:Panel>
    <br />
    <cms:UniGrid ID="gridUsers" runat="server" GridName="~/CMSModules/Messaging/Controls/SearchUser.xml"
        OrderBy="UserName" />
</div>
