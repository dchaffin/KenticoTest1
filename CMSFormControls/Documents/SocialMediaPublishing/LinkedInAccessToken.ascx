<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Documents_SocialMediaPublishing_LinkedInAccessToken" CodeFile="LinkedInAccessToken.ascx.cs" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:CMSTextBox ID="txtToken" runat="server" CssClass="SelectorTextBox" />
        <cms:LocalizedButton ID="btnSelect" runat="server" CssClass="ContentButton" ResourceString="socialnetworking.get"/>
    </ContentTemplate>
</cms:CMSUpdatePanel>
