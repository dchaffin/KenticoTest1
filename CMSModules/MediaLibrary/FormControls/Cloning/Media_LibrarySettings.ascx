<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Media_LibrarySettings.ascx.cs"
    Inherits="CMSModules_MediaLibrary_FormControls_Cloning_Media_LibrarySettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblFolderName" ResourceString="clonning.settings.medialibrary.foldername"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtFolderName" CssClass="TextBoxField" />
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="plcFiles">
        <tr>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblFiles" ResourceString="clonning.settings.medialibrary.files"
                    EnableViewState="false" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox runat="server" ID="chkFiles" Checked="true" />
            </td>
        </tr>
    </asp:PlaceHolder>
</table>
