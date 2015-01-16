<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_PaymentOptions_PaymentOption_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="PaymentOption_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/OrderStatusSelector.ascx" TagName="OrderStatusSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagPrefix="cms"
    TagName="File" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <table style="vertical-align: top">
        <%-- Display name --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblPaymentOptionDisplayName" ResourceString="general.displayname"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtPaymentOptionDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" ControlToValidate="txtPaymentOptionDisplayName:textbox"
                    runat="server" Display="Dynamic" ValidationGroup="PaymentOptions" EnableViewState="false" />
            </td>
        </tr>
        <%-- Code name --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblPaymentOptionName" ResourceString="general.codename"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:CodeName ID="txtPaymentOptionName" runat="server" CssClass="TextBoxField"
                    MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvCodeName" ControlToValidate="txtPaymentOptionName"
                    runat="server" Display="Dynamic" ValidationGroup="PaymentOptions" EnableViewState="false" />
            </td>
        </tr>
        <%-- Description --%>
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
        <%-- Thumbnail --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblImage" EnableViewState="false" ResourceString="com.paymentmethod.thumbnail"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:File ID="file" runat="server" />
            </td>
        </tr>
        <%-- Allow if no shipping is supplied --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblAllowIfNoShipping" ResourceString="paymentoption_edit.allowifnoshipping"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <asp:CheckBox runat="server" ID="chkAllowIfNoShipping" Checked="false" />
            </td>
        </tr>
        <%-- Enabled --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblPaymentOptionEnabled" EnableViewState="false"
                    ResourceString="general.enabled" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkPaymentOptionEnabled" runat="server" CssClass="CheckBoxMovedLeft"
                    Checked="true" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="padding-top: 10px; font-weight: bold;">
                <cms:LocalizedLabel runat="server" ID="lblPaymentGateway" ResourceString="PaymentOption_Edit.GatewaySettings"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblGateUrl" ResourceString="PaymentOption_Edit.GateUrl"
                    EnableViewState="false" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtGateUrl" runat="server" CssClass="TextBoxField" MaxLength="500"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblPaymentAssemblyName" ResourceString="PaymentOption_Edit.PaymentAssemblyName"
                    EnableViewState="false" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtPaymentAssemblyName" runat="server" CssClass="TextBoxField"
                    MaxLength="200" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblPaymentClassName" ResourceString="PaymentOption_Edit.PaymentClassName"
                    EnableViewState="false" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtPaymentClassName" runat="server" CssClass="TextBoxField" MaxLength="200"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblStatusSucceed" ResourceString="PaymentOption_Edit.PaymentSucceedStatus"
                    EnableViewState="false" />
            </td>
            <td colspan="2">
                <cms:OrderStatusSelector runat="server" ID="succeededElem" AddAllItemsRecord="false"
                    AddNoneRecord="true" IsLiveSite="false" UseStatusNameForSelection="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblStatusFailed" ResourceString="PaymentOption_Edit.PaymentFailedStatus"
                    EnableViewState="false" />
            </td>
            <td colspan="2">
                <cms:OrderStatusSelector runat="server" ID="failedElem" AddAllItemsRecord="false"
                    AddNoneRecord="true" IsLiveSite="false" UseStatusNameForSelection="false" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" ValidationGroup="PaymentOptions" />
            </td>
        </tr>
    </table>
</asp:Content>
