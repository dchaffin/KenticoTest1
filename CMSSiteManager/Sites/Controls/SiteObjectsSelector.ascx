<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteObjectsSelector.ascx.cs"
    Inherits="CMSSiteManager_Sites_Controls_SiteObjectsSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:MessagesPlaceHolder runat="server" ID="plcMsg" />
        <asp:Label ID="lblAvialable" runat="server" CssClass="BoldInfoLabel" />
        <cms:UniSelector ID="usObjects" ShortID="s" runat="server" SelectionMode="Multiple"
            AllowEditTextBox="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
