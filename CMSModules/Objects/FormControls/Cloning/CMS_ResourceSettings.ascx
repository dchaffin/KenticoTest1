<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_ResourceSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_ResourceSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblClonePermissions" ResourceString="clonning.settings.resource.permissions"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkClonePermissions" Checked="true" />
        </td>
    </tr>
</table>
