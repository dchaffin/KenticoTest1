<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_TransformationSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_TransformationSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblFiles" ResourceString="clonning.settings.layouts.appthemesfolder"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkFiles" Checked="true" />
        </td>
    </tr>
</table>
