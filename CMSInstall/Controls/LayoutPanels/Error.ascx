<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Error.ascx.cs" Inherits="CMSInstall_Controls_LayoutPanels_Error" %>
<%@ Register TagPrefix="cms" TagName="Help" Src="~/CMSAdminControls/UI/PageElements/Help.ascx" %>
<asp:PlaceHolder ID="plcError" runat="server" EnableViewState="False">
    <div class="InstallerError">
        <div style="text-align: left; padding: 0px 20px 10px 20px;">
            <div style="padding-bottom: 8px">
                <cms:Help ID="hlpTroubleshoot" runat="Server" Visible="false" />
            </div>
            <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" />
        </div>
    </div>
</asp:PlaceHolder>
