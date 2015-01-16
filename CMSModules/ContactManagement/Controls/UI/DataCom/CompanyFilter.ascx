<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CompanyFilter.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_DataCom_CompanyFilter" %>
<table>
    <tbody>
        <tr>
            <td><cms:LocalizedLabel ID="NameLabel" runat="server" AssociatedControlID="NameTextBox" EnableViewState="false" ResourceString="datacom.company.name" DisplayColon="true"></cms:LocalizedLabel></td>
            <td><asp:TextBox ID="NameTextBox" runat="server" EnableViewState="false" CssClass="SelectorTextBox"></asp:TextBox></td>
        </tr>
        <tr>
            <td></td>
            <td><cms:LocalizedButton ID="SearchButton" runat="server" CssClass="ContentButton" EnableViewState="False" ResourceString="general.search" /></td>
        </tr>
    </tbody>
</table>