<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Suppliers_Supplier_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Suppliers - Edit"
    CodeFile="Supplier_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <table style="vertical-align: top">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblSupplierDisplayName" EnableViewState="false"
                    ResourceString="general.displayname" DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtSupplierDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" ControlToValidate="txtSupplierDisplayName:textbox"
                    runat="server" Display="Dynamic" ValidationGroup="Suppliers" EnableViewState="false"></cms:CMSRequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblSupplierName" EnableViewState="false"
                    ResourceString="general.codename" DisplayColon="true" />
            </td>
            <td>
                <cms:CodeName ID="txtSupplierName" runat="server" CssClass="TextBoxField" MaxLength="200"
                    EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvCodeName" ControlToValidate="txtSupplierName"
                    runat="server" Display="Dynamic" ValidationGroup="Suppliers" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblSupplierEmail" EnableViewState="false"
                    ResourceString="supplier_Edit.SupplierEmailLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtSupplierEmail" runat="server" CssClass="TextBoxField" MaxLength="200"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblSupplierPhone" EnableViewState="false"
                    ResourceString="supplier_Edit.SupplierPhoneLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtSupplierPhone" runat="server" CssClass="TextBoxField" MaxLength="50"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblSupplierFax" EnableViewState="false" ResourceString="supplier_Edit.SupplierFaxLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtSupplierFax" runat="server" CssClass="TextBoxField" MaxLength="50"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblSupplierEnabled" EnableViewState="false"
                    ResourceString="general.enabled" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkSupplierEnabled" runat="server" Checked="true" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" ValidationGroup="Suppliers" />
            </td>
        </tr>
    </table>
</asp:Content>
