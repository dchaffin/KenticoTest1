<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PageTitle.ascx.cs" Inherits="CMSAdminControls_UI_PageElements_PageTitle" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlBody" CssClass="PageTitleBody" Style="user-select: none;
    -moz-user-select: none; -webkit-user-select: none; -ms-user-select: none;">
    <asp:Panel runat="server" ID="pnlTitle" CssClass="PageTitleHeader" Visible="false">
        <table runat="server" id="titleTable" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td runat="server" id="titleCell">
                    <asp:Image ID="imgTitle" runat="server" Visible="false" CssClass="PageTitleImage"
                        EnableViewState="false" />
                    <asp:Label ID="lblTitle" runat="server" CssClass="PageTitle" EnableViewState="false" />
                    <asp:Label ID="lblTitleInfo" runat="server" CssClass="PageTitleInfo" EnableViewState="false" Visible="false" />
                </td>
                <td class="TextRight">
                    <asp:PlaceHolder runat="server" ID="plcMisc" />
                </td>
                <td style="vertical-align: top;">
                    <cms:help id="helpElem" runat="server" />
                </td>
                <td style="vertical-align: top;" runat="server" id="cellFullScreen">
                    <asp:Panel runat="server" ID="pnlFullScreen" CssClass="FullScreenButton" EnableViewState="false" />
                </td>
                <td style="vertical-align: top;" runat="server" id="cellClose">
                    <asp:Panel runat="server" ID="pnlClose" CssClass="CloseButton" EnableViewState="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlBreadCrumbs" CssClass="PageTitleBreadCrumbs" Visible="false">
        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
            <tr>
                <td style="width: 100%;" class="PageTitleBreadCrumbsPadding">
                    <asp:PlaceHolder ID="plcBreadcrumbs" runat="server" />
                </td>
                <td>
                    <cms:help id="helpBreadcrumbs" runat="server" visible="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
