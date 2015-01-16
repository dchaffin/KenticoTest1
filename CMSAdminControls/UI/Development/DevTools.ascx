<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DevTools.ascx.cs" Inherits="CMSAdminControls_UI_Development_DevTools" %>
<div class="DevTools">
    <div class="DevContent">
        <asp:HyperLink runat="server" ID="lnkDebug" EnableViewState="false" CssClass="ContextMenuButton">
            <asp:Image runat="server" ID="imgDebug" EnableViewState="false" />
        </asp:HyperLink>
        <cms:ContextMenuButton runat="server" ID="btnLocalize" />
    </div>
</div>