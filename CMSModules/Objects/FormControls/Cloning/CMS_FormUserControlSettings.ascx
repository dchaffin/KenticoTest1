<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_FormUserControlSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_FormUserControlSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblFiles" ResourceString="clonning.settings.formusercontrol.files"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkFiles" Checked="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblFileName" ResourceString="clonning.settings.formusercontrol.filename"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtFileName" CssClass="TextBoxField" />
        </td>
    </tr>
</table>
