<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_CustomTableSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_CustomTableSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblTableName" ResourceString="clonning.settings.class.tablename"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtTableName" CssClass="TextBoxField" MaxLength="100" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblCloneItems" ResourceString="clonning.settings.customtable.cloneitems"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCloneItems" Checked="false" />
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
