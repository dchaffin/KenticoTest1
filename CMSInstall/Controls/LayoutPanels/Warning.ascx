<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Warning.ascx.cs" Inherits="CMSInstall_Controls_LayoutPanels_Warning" %>
<asp:PlaceHolder ID="plcWarning" runat="server" EnableViewState="False" >
    <div class="InstallerContent">
        <div style="text-align: left; padding: 0px 0px 10px 20px;">
            <asp:Label ID="lblWarning" runat="server" CssClass="ErrorLabel" />
        </div>
    </div>
</asp:PlaceHolder>
