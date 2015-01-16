<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HtmlBarGraph.ascx.cs"
    Inherits="CMSModules_Reporting_Controls_HtmlBarGraph" %>
<asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
    Visible="false" />
<cms:ContextMenuContainer runat="server" ID="menuCont" MenuID="">
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
    <div>
        <asp:Literal runat="server" ID="ltlGraph" EnableViewState="false" />
    </div>
</cms:ContextMenuContainer>
<asp:Literal runat="server" ID="ltlEmail" EnableViewState="false" Visible="false" />