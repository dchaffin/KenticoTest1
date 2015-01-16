<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Log.ascx.cs" Inherits="CMSInstall_Controls_LayoutPanels_Log" %>
<asp:PlaceHolder ID="plcLog" runat="server" Visible="False" EnableViewState="False">
    <div class="InstallerContent">
        <div style="text-align: left; padding: 0px 25px 10px 25px;">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblLog" AssociatedControlID="txtLog" runat="server" CssClass="InstallGroupTitle" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Panel runat="server" ID="pnlGroupLog">
                            <cms:CMSTextBox ID="txtLog" runat="server" CssClass="InstallLog" TextMode="MultiLine"
                                ReadOnly="True" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:PlaceHolder>
