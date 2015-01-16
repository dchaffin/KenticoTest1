<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ecommerce_AddressSettings.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_AddressSettings" %>
<table>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblPersonalName" ResourceString="customer_edit_address_edit.lblpersonalname" EnableViewState="false"
                DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtPersonalName" CssClass="TextBoxField" MaxLength="100" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblAddress1" ResourceString="customer_edit_address_edit.addressline1label" EnableViewState="false"
                DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtAddress1" CssClass="TextBoxField" MaxLength="100" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtAddress2" CssClass="TextBoxField" MaxLength="100" />
        </td>
    </tr>
</table>
