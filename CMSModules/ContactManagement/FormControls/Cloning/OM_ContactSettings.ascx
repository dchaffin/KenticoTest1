<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OM_ContactSettings.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_Cloning_OM_ContactSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblFirstName" ResourceString="om.contact.firstname"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtFirstName" CssClass="TextBoxField" MaxLength="100" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblLastName" ResourceString="om.contact.lastname"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtLastName" CssClass="TextBoxField" MaxLength="100" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblMerged" ResourceString="clonning.settings.contact.merged"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkMerged" Checked="false" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblAddressesAgents" ResourceString="clonning.settings.contact.ipaddressesuseragents"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkAddressesAgents" Checked="false" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblActivity" ResourceString="clonning.settings.contact.activity"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkActivity" Checked="false" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblContactGroup" ResourceString="clonning.settings.contact.contactgroup"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkContactGroup" Checked="false" />
        </td>
    </tr>
</table>
