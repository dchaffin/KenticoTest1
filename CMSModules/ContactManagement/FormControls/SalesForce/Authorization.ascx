<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Authorization.ascx.cs" Inherits="CMSModules_ContactManagement_FormControls_SalesForce_Authorization" %>
<cms:CMSUpdatePanel ID="MainUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="CredentialsHiddenField" runat="server" EnableViewState="false" />
        <p><asp:Label ID="MessageLabel" runat="server" EnableViewState="false"></asp:Label></p>
        <asp:Button ID="SetupButton" runat="server" EnableViewState="false" CssClass="ContentButton" Text="Authorize" />
        <asp:Button ID="ClearButton" runat="server" EnableViewState="false" CssClass="ContentButton" Text="Clear" />
    </ContentTemplate>
</cms:CMSUpdatePanel>