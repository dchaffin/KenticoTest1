<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_DocumentTypeSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_DocumentTypeSettings" %>
<table>
    <asp:PlaceHolder runat="server" ID="plcTableName">
        <tr>
            <td class="CloneParamTableCol">
                <cms:LocalizedLabel runat="server" ID="lblTableName" ResourceString="clonning.settings.class.tablename"
                    EnableViewState="false" DisplayColon="true" />
            </td>
            <td>
                <cms:CMSTextBox runat="server" ID="txtTableName" CssClass="TextBoxField" MaxLength="100" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblIcons" ResourceString="clonning.settings.documenttype.icons"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkIcons" Checked="true" />
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="plcAlternativeForms">
        <tr>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblCloneAlternativeForms" ResourceString="clonning.settings.class.alternativeform"
                    EnableViewState="false" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox runat="server" ID="chkCloneAlternativeForms" Checked="true" />
            </td>
        </tr>
    </asp:PlaceHolder>
</table>
