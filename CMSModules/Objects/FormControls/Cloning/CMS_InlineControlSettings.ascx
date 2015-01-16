<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_InlineControlSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_InlineControlSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblFiles" ResourceString="clonning.settings.inlinecontrol.files"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkFiles" Checked="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblFileName" ResourceString="clonning.settings.inlinecontrol.filename"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtFileName" CssClass="TextBoxField" />
        </td>
    </tr>
</table>
