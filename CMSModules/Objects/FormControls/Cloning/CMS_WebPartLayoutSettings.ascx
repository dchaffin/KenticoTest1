<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_WebPartLayoutSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_WebPartLayoutSettings" %>
<table>
     <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblAppThemes" ResourceString="clonning.settings.layouts.appthemesfolder"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkAppThemes" Checked="true" />
        </td>
    </tr>
</table>
