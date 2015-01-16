<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_Currencies_Currency_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Currency - Edit"
    CodeFile="Currency_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <table style="vertical-align: top">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCurrencyDisplayName" EnableViewState="false"
                    ResourceString="general.displayname" DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtCurrencyDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" Display="Dynamic"
                    ValidationGroup="Currency" ControlToValidate="txtCurrencyDisplayName:textbox"
                    EnableViewState="false" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCurrencyName" EnableViewState="false" ResourceString="general.codename"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:CodeName ID="txtCurrencyName" runat="server" CssClass="TextBoxField" MaxLength="200"
                    EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" Display="Dynamic"
                    ValidationGroup="Currency" ControlToValidate="txtCurrencyName" EnableViewState="false" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCurrencyCode" EnableViewState="false" CssClass="FloatLeft"
                    ResourceString="Currency_Edit.CurrencyCodeLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtCurrencyCode" runat="server" CssClass="TextBoxField" MaxLength="50"
                    EnableViewState="false" />
                <asp:Image ID="imgHelp" runat="server" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvCurrencyCode" runat="server" Display="Dynamic"
                    EnableViewState="false" ValidationGroup="Currency" ControlToValidate="txtCurrencyCode" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblFormatString" EnableViewState="false" ResourceString="Currency_Edit.lblFormatString" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtFormatString" runat="server" CssClass="TextBoxField"
                    MaxLength="100" EnableViewState="false" />
                <br />
                <cms:CMSRequiredFieldValidator ID="rfvFormatString" runat="server" Display="Dynamic"
                    EnableViewState="false" ValidationGroup="Currency" ControlToValidate="txtFormatString:textbox" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:LocalizedLabel runat="server" ID="lblFormatStringnInfo" EnableViewState="false"
                    ResourceString="Currency_Edit.lblFormatStringInfo" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCurrencyRoundTo" EnableViewState="false"
                    ResourceString="Currency_Edit.CurrencyRoundToLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtCurrencyRoundTo" runat="server" CssClass="TextBoxField" Text="0"
                    MaxLength="2" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvRoundTo" runat="server" Display="Dynamic" ValidationGroup="Currency"
                    ControlToValidate="txtCurrencyRoundTo" EnableViewState="false" />
                <cms:CMSRegularExpressionValidator ID="revRoundTo" runat="server" Display="Dynamic"
                    ValidationGroup="Currency" ControlToValidate="txtCurrencyRoundTo" EnableViewState="false" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCurrencyEnabled" EnableViewState="false"
                    ResourceString="general.enabled" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkCurrencyEnabled" runat="server" CssClass="CheckBoxMovedLeft"
                    Checked="true" EnableViewState="false" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" ValidationGroup="Currency" />
            </td>
            <td>
            </td>
        </tr>
    </table>
</asp:Content>
