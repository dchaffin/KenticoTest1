<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Manufacturers_Manufacturer_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Manufacturers - Edit"
    CodeFile="Manufacturer_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagPrefix="cms"
    TagName="LocalizableTextBox" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagPrefix="cms"
    TagName="File" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <table style="vertical-align: top">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblManufacturerDisplayName" EnableViewState="false"
                    ResourceString="general.displayname" DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtManufacturerDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" ControlToValidate="txtManufacturerDisplayName:textbox"
                    runat="server" Display="Dynamic" ValidationGroup="Manufacturers" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblManufacturerName" EnableViewState="false"
                    ResourceString="general.codename" DisplayColon="true" />
            </td>
            <td>
                <cms:CodeName ID="txtManufacturerName" runat="server" CssClass="TextBoxField" MaxLength="200"
                    EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvCodeName" ControlToValidate="txtManufacturerName"
                    runat="server" Display="Dynamic" ValidationGroup="Manufacturers" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblDescription" EnableViewState="false" ResourceString="general.description"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtDescription" runat="server" CssClass="TextAreaField"
                    TextMode="MultiLine" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblManufactureHomepage" EnableViewState="false"
                    ResourceString="Manufacturer_Edit.ManufactureHomepageLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtManufactureHomepage" runat="server" CssClass="TextBoxField"
                    MaxLength="400" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblImage" EnableViewState="false" ResourceString="com.manufacturer.thumbnail"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:File ID="file" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblIsImportant" EnableViewState="false" ResourceString="com.manufacturer.isimportant"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkIsImportant" runat="server" CssClass="CheckBoxMovedLeft" EnableViewState="false"
                    Checked="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblManufacturerEnabled" EnableViewState="false"
                    ResourceString="general.enabled" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkManufacturerEnabled" runat="server" CssClass="CheckBoxMovedLeft"
                    EnableViewState="false" Checked="true" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" ValidationGroup="Manufacturers" />
            </td>
        </tr>
    </table>
</asp:Content>
