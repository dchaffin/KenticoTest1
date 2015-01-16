<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Controls_ReportTable"
    CodeFile="ReportTable.ascx.cs" %>
<cms:contextmenucontainer runat="server" id="menuCont" MenuID="">
    <asp:PlaceHolder runat="server" ID="plcGrid" />
    <asp:Label runat="server" ID="lblError" EnableViewState="false" CssClass="InlineControlError"
        Visible="false" />
    <asp:PlaceHolder runat="server" ID="plcInfo" Visible="false" EnableViewState="false">
        <table>
            <tr>
                <td>
                    <asp:Image runat="server" ID="imgInfo" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblInfo" />
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>
</cms:contextmenucontainer>
<asp:Literal runat="server" ID="ltlEmail" Visible="false" EnableViewState="false" />