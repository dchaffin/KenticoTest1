<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_FormControls_Documents_SelectSinglePath"
    CodeFile="SelectSinglePath.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniControls/UniButton.ascx" TagName="UniButton"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
        <cms:CMSTextBox ID="txtPath" runat="server" MaxLength="200" CssClass="SelectorTextBox" /><cms:UniButton
            ID="btnSelectPath" runat="server" EnableViewState="false" />
        <cms:UniButton ID="btnSetPermissions" runat="server" EnableViewState="false" />
        <cms:CMSTextBox ID="txtNodeId" runat="server" CssClass="Hidden" AutoPostBack="true" /><cms:LocalizedLabel
            ID="lblNodeId" runat="server" EnableViewState="false" Display="false" ResourceString="generalproperties.nodeid"
            AssociatedControlID="txtNodeId" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
