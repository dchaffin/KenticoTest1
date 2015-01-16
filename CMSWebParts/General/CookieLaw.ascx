<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_General_CookieLaw"
    CodeFile="~/CMSWebParts/General/CookieLaw.ascx.cs" %>
<div class="CookieConsent">
    <asp:Label runat="server" ID="lblText" EnableViewState="false" CssClass="ConsentText" />
    <span class="ConsentButtons">
        <asp:Button runat="server" ID="btnDenyAll" EnableViewState="false" CssClass="ConsentButton" OnClick="btnDenyAll_Click" />
        <asp:Button runat="server" ID="btnAllowSpecific" EnableViewState="false" CssClass="ConsentButton" OnClick="btnAllowSpecific_Click" />
        <asp:Button runat="server" ID="btnAllowAll" EnableViewState="false" CssClass="ConsentButton" OnClick="btnAllowAll_Click" />
    </span>
</div>
