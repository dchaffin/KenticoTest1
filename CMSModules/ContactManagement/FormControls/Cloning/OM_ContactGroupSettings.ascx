<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OM_ContactGroupSettings.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_Cloning_OM_ContactGroupSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblCloneContacts" ResourceString="clonning.settings.contactgroup.contacts"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCloneContacts" Checked="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblCloneAccounts" ResourceString="clonning.settings.contactgroup.accounts"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCloneAccounts" Checked="true" />
        </td>
    </tr>
</table>
