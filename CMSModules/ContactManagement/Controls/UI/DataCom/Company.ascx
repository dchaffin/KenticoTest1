<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Company.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_DataCom_Company" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>
<cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" />
<table>
    <tbody>
        <asp:Repeater ID="AttributeRepeater" runat="server">
            <ItemTemplate>
                <tr>
                    <td><%# HTMLHelper.HTMLEncode(((RepeaterDataItem)Container.DataItem).AttributeName) %>:</td>
                    <td><%# HTMLHelper.HTMLEncode(((RepeaterDataItem)Container.DataItem).AttributeValue) %></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
