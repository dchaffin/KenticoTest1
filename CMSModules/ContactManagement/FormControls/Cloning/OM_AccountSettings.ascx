<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OM_AccountSettings.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_Cloning_OM_AccountSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblMerged" ResourceString="clonning.settings.account.merged"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkMerged" Checked="false" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblSubsidiaries" ResourceString="clonning.settings.account.subsidiaries"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkSubsidiaries" Checked="false" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblContactGroup" ResourceString="clonning.settings.account.contactgroup"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkContactGroup" Checked="false" />
        </td>
    </tr>
</table>
