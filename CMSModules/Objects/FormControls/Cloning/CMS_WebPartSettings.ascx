<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_WebPartSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_WebPartSettings" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblWebPartCategory" ResourceString="clonning.settings.webpart.category"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:DropDownList ID="drpWebPartCategories" runat="server" CssClass="DropDownField" />
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="plcFiles">
        <tr>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblFiles" ResourceString="clonning.settings.webpart.files"
                    EnableViewState="false" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox runat="server" ID="chkFiles" Checked="true" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblFileName" ResourceString="clonning.settings.webpart.filename"
                    EnableViewState="false" DisplayColon="true" />
            </td>
            <td>
                <cms:CMSTextBox runat="server" ID="txtFileName" CssClass="TextBoxField" />
            </td>
        </tr>
    </asp:PlaceHolder>
     <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblAppThemes" ResourceString="clonning.settings.layouts.appthemesfolder"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkAppThemes" Checked="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblWebpartLayouts" ResourceString="clonning.settings.webpart.layouts"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkWebpartLayouts" Checked="true" />
        </td>
    </tr>
</table>
