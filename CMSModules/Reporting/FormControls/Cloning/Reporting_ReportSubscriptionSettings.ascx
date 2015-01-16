<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Reporting_ReportSubscriptionSettings.ascx.cs"
    Inherits="CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSubscriptionSettings" %>
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
</table>