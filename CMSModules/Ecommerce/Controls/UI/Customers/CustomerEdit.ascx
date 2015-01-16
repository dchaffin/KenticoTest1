<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomerEdit.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_UI_Customers_CustomerEdit" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/ShippingSelector.ascx" TagName="ShippingSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/CountrySelector.ascx" TagName="CountrySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/DiscountLevelSelector.ascx"
    TagName="DiscountLevelSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>
<div style="width: 700px;">
    <%-- General information --%>
    <asp:Panel ID="pnlGeneral" runat="server">
        <table style="vertical-align: top">
            <tr>
                <td class="FieldLabel" style="width: 175px;">
                    <cms:LocalizedLabel runat="server" ID="lblCustomerFirstName" EnableViewState="false"
                        ResourceString="Customers_Edit.CustomerFirstNameLabel" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerFirstName" runat="server" CssClass="TextBoxField"
                        MaxLength="200" EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblCustomerLastName" EnableViewState="false"
                        ResourceString="Customers_Edit.CustomerLastNameLabel" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerLastName" runat="server" CssClass="TextBoxField" MaxLength="200"
                        EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblCustomerCompany" EnableViewState="false"
                        ResourceString="Customers_Edit.CustomerCompanyLabel" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerCompany" runat="server" CssClass="TextBoxField" MaxLength="200"
                        EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblOrganizationID" EnableViewState="false"
                        ResourceString="Customers_Edit.lblOrganizationID" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtOraganizationID" runat="server" CssClass="TextBoxField" MaxLength="50"
                        EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblTaxRegistrationID" EnableViewState="false"
                        ResourceString="Customers_Edit.lblTaxRegistrationID" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtTaxRegistrationID" runat="server" CssClass="TextBoxField"
                        MaxLength="50" EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel" style="vertical-align: top; padding-top: 6px">
                    <cms:LocalizedLabel runat="server" ID="lblCustomerCountry" EnableViewState="false"
                        ResourceString="Customers_Edit.CustomerCountry" />
                </td>
                <td>
                    <cms:CountrySelector ID="drpCountry" runat="server" AddNoneRecord="true" UseCodeNameForSelection="false"
                        AddSelectCountryRecord="false" IsLiveSite="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <%-- Contacts --%>
    <asp:Panel ID="pnlContacts" runat="server">
        <table style="vertical-align: top">
            <tr>
                <td class="FieldLabel" style="width: 175px;">
                    <cms:LocalizedLabel runat="server" ID="lblCustomerEmail" DisplayColon="true" EnableViewState="false"
                        ResourceString="general.email" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerEmail" runat="server" CssClass="TextBoxField" MaxLength="200"
                        EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblCustomerPhone" EnableViewState="false"
                        ResourceString="customers_edit.CustomerPhone" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerPhone" runat="server" CssClass="TextBoxField" MaxLength="50"
                        EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblCustomerFax" DisplayColon="true" EnableViewState="false"
                        ResourceString="customers_edit.CustomerFax" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerFax" runat="server" CssClass="TextBoxField" MaxLength="50"
                        EnableViewState="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <%-- User info--%>
    <asp:Panel ID="pnlUserInfo" runat="server">
        <asp:Panel ID="pnlEdit" runat="server">
            <table style="vertical-align: top">
                <tr>
                    <td class="ContentGroupHeader" colspan="2">
                        <cms:LocalizedLabel runat="server" ID="lblLogin" EnableViewState="false" ResourceString="com.customeredit.login" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel" colspan="2">
                        <asp:CheckBox ID="chkHasLogin" runat="server" CssClass="CheckBoxMovedLeft" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        &nbsp;
                    </td>
                </tr>
                <tr id="loginLine1">
                    <td class="FieldLabel" style="width: 175px;">
                        <cms:LocalizedLabel runat="server" ID="lblUserName" EnableViewState="false" ResourceString="general.username"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtUserName" runat="server" CssClass="TextBoxField" MaxLength="100"
                            EnableViewState="false" />
                    </td>
                    <td>
                        <cms:CMSRequiredFieldValidator ID="rqvUserName" runat="server" ControlToValidate="txtUserName"
                            ValidationGroup="Login" EnableViewState="false" />
                    </td>
                </tr>
                <tr id="loginLine2">
                    <td class="FieldLabel FieldLabelTop">
                        <cms:LocalizedLabel runat="server" ID="lblPassword1" EnableViewState="false" ResourceString="Customer_Edit_Login_Edit.lblPassword1" />
                    </td>
                    <td>
                        <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr id="loginLine3">
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblPassword2" EnableViewState="false" ResourceString="Customer_Edit_Login_Edit.lblPassword2" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtPassword2" runat="server" CssClass="TextBoxField" TextMode="Password"
                            MaxLength="100" EnableViewState="false" />
                    </td>
                    <td>
                        <cms:CMSRequiredFieldValidator ID="rqvPassword2" runat="server" ControlToValidate="txtPassword2"
                            ValidationGroup="Login" EnableViewState="false" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlStatic" runat="server">
            <table style="vertical-align: top">
                <tr>
                    <td class="ContentGroupHeader" colspan="2">
                        <cms:LocalizedLabel runat="server" ID="lblLogin1" EnableViewState="false" ResourceString="com.customeredit.login" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel" nowrap="nowrap" style="width: 175px;">
                        <cms:LocalizedLabel ID="lblUserName1" runat="server" EnableViewState="false" ResourceString="general.username"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblUserNameStaticValue" EnableViewState="false" />
                    </td>
                    <td>
                        <cms:CMSButton ID="btnEditUser" runat="server" CssClass="ContentButton" Visible="false"
                            EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblCustomerEnabled" EnableViewState="false"
                            ResourceString="general.enabled" DisplayColon="true" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkCustomerEnabled" runat="server" CssClass="CheckBoxMovedLeft"
                            EnableViewState="false" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table style="vertical-align: top">
            <asp:PlaceHolder runat="server" ID="plcPreferences">
                <tr>
                    <td class="ContentGroupHeader" colspan="2">
                        <cms:LocalizedLabel runat="server" ID="lblPreferences" EnableViewState="false" ResourceString="com.customeredit.preferences" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel" style="width: 175px;">
                        <cms:LocalizedLabel runat="server" ID="lblCustomerPreferredCurrency" EnableViewState="false"
                            ResourceString="customers_edit.CustomerCurrency" />
                    </td>
                    <td>
                        <cms:CurrencySelector ID="drpCurrency" runat="server" AddNoneRecord="true" IsLiveSite="false" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblCustomerPrefferedPaymentOption" EnableViewState="false"
                            ResourceString="customers_edit.CustomerPayment" />
                    </td>
                    <td>
                        <cms:PaymentSelector ID="drpPayment" runat="server" AddNoneRecord="true" IsLiveSite="false" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblCustomerPreferredShippingOption" EnableViewState="false"
                            ResourceString="customers_edit.CustomerShipping" />
                    </td>
                    <td>
                        <cms:ShippingSelector ID="drpShipping" runat="server" AddNoneRecord="true" IsLiveSite="false" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcDiscounts" runat="server">
                <tr>
                    <td class="ContentGroupHeader" colspan="2">
                        <cms:LocalizedLabel runat="server" ID="lblDiscounts" EnableViewState="false" ResourceString="com.customeredit.discounts" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcGlobalDiscount" runat="server">
                    <tr>
                        <td class="FieldLabel" style="width: 175px;">
                            <cms:LocalizedLabel runat="server" ID="lblCustomerGlobalDiscountLevel" EnableViewState="false"
                                ResourceString="Customers_Edit.lblCustomerGlobalDiscountLevel" />
                        </td>
                        <td>
                            <cms:DiscountLevelSelector ID="drpGlobalDiscountLevel" runat="server" AddNoneRecord="true"
                                UseCodeNameForSelection="false" IsLiveSite="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcSiteDiscount" runat="server">
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblCustomerDiscountLevel" EnableViewState="false"
                                ResourceString="Customers_Edit.lblCustomerDiscountLevel" />
                        </td>
                        <td>
                            <cms:DiscountLevelSelector ID="drpDiscountLevel" runat="server" AddNoneRecord="false"
                                UseCodeNameForSelection="false" IsLiveSite="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </asp:PlaceHolder>
        </table>
    </asp:Panel>
    <asp:PlaceHolder ID="plcButton" runat="server">
        <table style="vertical-align: top; margin-top: 10px;">
            <tr>
                <td>
                    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
                </td>
                <td>
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />