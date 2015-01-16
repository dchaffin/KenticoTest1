<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_PageTemplateSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_PageTemplateSettings" %>
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
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblTemplateScope" ResourceString="clonning.settings.pagetemplate.templatescope"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkTemplateScope" Checked="true" />
        </td>
    </tr>
</table>
