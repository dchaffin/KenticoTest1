<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_FormSettings.ascx.cs"
    Inherits="CMSModules_BizForms_FormControls_Cloning_CMS_FormSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblTableName" ResourceString="clonning.settings.class.tablename"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtTableName" CssClass="TextBoxField" MaxLength="100"/>
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblCloneItems" ResourceString="clonning.settings.form.cloneitems"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCloneItems" Checked="true" />
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
