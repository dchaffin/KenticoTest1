<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_ClassSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_ClassSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblTableName" ResourceString="clonning.settings.class.tablename"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtTableName" CssClass="TextBoxField" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblCloneAlternativeForms" ResourceString="clonning.settings.class.alternativeform"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCloneAlternativeForms" Checked="true" />
        </td>
    </tr>
</table>
