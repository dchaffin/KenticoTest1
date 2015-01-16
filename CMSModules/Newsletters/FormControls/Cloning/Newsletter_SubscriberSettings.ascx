<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Newsletter_SubscriberSettings.ascx.cs"
    Inherits="CMSModules_Newsletters_FormControls_Cloning_Newsletter_SubscriberSettings" %>
<table>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblEmail" ResourceString="general.email" EnableViewState="false"
                DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtEmail" CssClass="TextBoxField" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblFirstName" ResourceString="general.firstname"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtFirstName" CssClass="TextBoxField" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblLastName" ResourceString="general.lastname"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtLastName" CssClass="TextBoxField" />
        </td>
    </tr>
</table>
