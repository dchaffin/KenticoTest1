<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_AttachmentSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_AttachmentSettings" %>
<table>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblFileName" ResourceString="clonning.settings.attachment.filename"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtFileName" CssClass="TextBoxField" MaxLength="240" />
        </td>
    </tr>
</table>
