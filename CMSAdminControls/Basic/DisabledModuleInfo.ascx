<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DisabledModuleInfo.ascx.cs"
    Inherits="CMSAdminControls_Basic_DisabledModuleInfo" %>
<div class="SettingDisabledDiv">
    <table width="100%">
        <tr>
            <td rowspan="2">
                <asp:Image runat="server" ID="imgWarning" />
            </td>
            <td rowspan="2">
                <cms:LocalizedLabel runat="server" ID="lblText" EnableViewState="false" />
            </td>
            <td>
                <div class="FloatRight">
                    <asp:Panel runat="server" ID="pnlGlobalButton">
                        <asp:Button runat="server" ID="btnGlobal" CssClass="XLongButton" OnClick="btnGlobal_clicked" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlSiteButton">
                        <asp:Button runat="server" ID="btnSite" CssClass="XLongButton" OnClick="btnSiteOnly_clicked" />
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>
</div>
<asp:Panel runat="server" ID="pnlDelimiter">
    <br />
</asp:Panel>
