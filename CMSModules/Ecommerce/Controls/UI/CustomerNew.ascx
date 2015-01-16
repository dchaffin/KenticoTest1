<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomerNew.ascx.cs" Inherits="CMSModules_Ecommerce_Controls_UI_CustomerNew" %>
<%@ Register Src="~/CMSFormControls/CountrySelector.ascx" TagName="CountrySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMessages" runat="server" />
<div style="width: 700px;">
    <%-- General fields --%>
    <asp:Panel ID="pnlGeneral" runat="server">
        <table style="vertical-align: top">
            <tr>
                <td class="FieldLabel" style="width: 110px;">
                    <cms:LocalizedLabel runat="server" ResourceString="customers_edit.customerfirstnamelabel"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerFirstName" runat="server" CssClass="TextBoxField"
                        MaxLength="200" EnableViewState="false" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ResourceString="customers_edit.customerlastnamelabel"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerLastName" runat="server" CssClass="TextBoxField" MaxLength="200"
                        EnableViewState="false" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ResourceString="customers_edit.customercompanylabel"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerCompany" runat="server" CssClass="TextBoxField" MaxLength="200"
                        EnableViewState="false" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ResourceString="customers_edit.lblorganizationid"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtOrganizationID" runat="server" CssClass="TextBoxField" MaxLength="50"
                        EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ResourceString="customers_edit.lbltaxregistrationid"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtTaxRegistrationID" runat="server" CssClass="TextBoxField"
                        MaxLength="50" EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel" style="vertical-align: top; padding-top: 6px">
                    <cms:LocalizedLabel runat="server" ResourceString="customers_edit.customercountry"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CountrySelector ID="drpCountry" runat="server" UseCodeNameForSelection="false"
                        AddSelectCountryRecord="false" AddNoneRecord="true" IsLiveSite="false" />
                </td>
                <td>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <%-- Contacts fields --%>
    <asp:Panel ID="pnlContacts" runat="server">
        <table style="vertical-align: top">
            <tr>
                <td class="FieldLabel" style="width: 110px;">
                    <cms:LocalizedLabel runat="server" ResourceString="general.email" DisplayColon="true"
                        EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerEmail" runat="server" CssClass="TextBoxField" MaxLength="200"
                        EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ResourceString="customers_edit.customerphone"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerPhone" runat="server" CssClass="TextBoxField" MaxLength="50"
                        EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ResourceString="customers_edit.customerfax" DisplayColon="true"
                        EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtCustomerFax" runat="server" CssClass="TextBoxField" MaxLength="50"
                        EnableViewState="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <%-- User fields --%>
    <asp:Panel ID="pnlUser" runat="server">
        <cms:CMSUpdatePanel runat="server" ID="updUserFields" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table style="vertical-align: top">
                    <tr>
                        <td colspan="3">
                            <cms:LocalizedCheckBox ID="chkCreateLogin" runat="server" CssClass="CheckBoxMovedLeft"
                                EnableViewState="false" AutoPostBack="true" ResourceString="customer_edit_login_edit.lblhaslogin" />
                        </td>
                    </tr>
                    <asp:PlaceHolder runat="server" ID="plcUserFields">
                        <tr>
                            <td class="FieldLabel" style="width: 110px;">
                                <cms:LocalizedLabel runat="server" ResourceString="customers_edit.username" DisplayColon="true"
                                    EnableViewState="false" />
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
                                <cms:LocalizedLabel runat="server" ResourceString="customer_edit_login_edit.lblpassword1"
                                    DisplayColon="true" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr id="loginLine3">
                            <td class="FieldLabel">
                                <cms:LocalizedLabel runat="server" ResourceString="customer_edit_login_edit.lblpassword2"
                                    DisplayColon="true" EnableViewState="false" />
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
                    </asp:PlaceHolder>
                </table>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</div>
