<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GooglePlusAccessToken.ascx.cs" Inherits="CMSFormControls_Documents_SocialMediaPublishing_GooglePlusAccessToken" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:CMSTextBox ID="txtToken" runat="server" CssClass="SelectorTextBox" />
        <cms:CMSTextBox ID="txtTokenSecret" runat="server" style="display: none;" />
        <cms:LocalizedButton ID="btnSelect" runat="server" CssClass="ContentButton" ResourceString="socialnetworking.get"/>
    </ContentTemplate>
</cms:CMSUpdatePanel>