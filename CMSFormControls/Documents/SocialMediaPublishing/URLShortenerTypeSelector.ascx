<%@ Control Language="C#" AutoEventWireup="True" CodeFile="URLShortenerTypeSelector.ascx.cs" Inherits="CMSFormControls_Documents_SocialMediaPublishing_URLShortenerTypeSelector" %>

<asp:DropDownList ID="ddlType" runat="server">
    <asp:ListItem Value="disabled" Text="(none)"></asp:ListItem>
    <asp:ListItem Value="bitly" Text="Bit.ly"></asp:ListItem>
    <asp:ListItem Value="tinyurl" Text="TinyURL.com"></asp:ListItem>
    <asp:ListItem Value="googl" Text="Goo.gl"></asp:ListItem>
</asp:DropDownList>
