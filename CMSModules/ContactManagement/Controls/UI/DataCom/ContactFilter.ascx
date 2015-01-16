<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContactFilter.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_DataCom_ContactFilter" %>
<table>
    <tbody>
        <tr>
            <td><cms:LocalizedLabel ID="FirstNameLabel" runat="server" AssociatedControlID="FirstNameTextBox" EnableViewState="false" ResourceString="datacom.contact.firstname" DisplayColon="true"></cms:LocalizedLabel></td>
            <td><asp:TextBox ID="FirstNameTextBox" runat="server" EnableViewState="false" CssClass="SelectorTextBox"></asp:TextBox></td>
        </tr>
        <tr>
            <td><cms:LocalizedLabel ID="LastNameLabel" runat="server" AssociatedControlID="LastNameTextBox" EnableViewState="false" ResourceString="datacom.contact.lastname" DisplayColon="true"></cms:LocalizedLabel></td>
            <td><asp:TextBox ID="LastNameTextBox" runat="server" EnableViewState="false" CssClass="SelectorTextBox"></asp:TextBox></td>
        </tr>
        <tr>
            <td><cms:LocalizedLabel ID="EmailLabel" runat="server" AssociatedControlID="EmailTextBox" EnableViewState="false" ResourceString="datacom.contact.email" DisplayColon="true"></cms:LocalizedLabel></td>
            <td><asp:TextBox ID="EmailTextBox" runat="server" EnableViewState="false" CssClass="SelectorTextBox"></asp:TextBox></td>
        </tr>
        <tr>
            <td><cms:LocalizedLabel ID="CompanyNameLabel" runat="server" AssociatedControlID="CompanyNameTextBox" EnableViewState="false" ResourceString="datacom.contact.companyname" DisplayColon="true"></cms:LocalizedLabel></td>
            <td><asp:TextBox ID="CompanyNameTextBox" runat="server" EnableViewState="false" CssClass="SelectorTextBox"></asp:TextBox></td>
        </tr>
        <tr>
            <td></td>
            <td><cms:LocalizedButton ID="SearchButton" runat="server" CssClass="ContentButton" EnableViewState="False" ResourceString="general.search" /></td>
        </tr>
    </tbody>
</table>