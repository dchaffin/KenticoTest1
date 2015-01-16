<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_General"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="BizForm General"
    Theme="Default" CodeFile="BizForm_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlGeneral" runat="server">
        <table>
            <tr>
                <td class="FieldLabel" style="width: 160px">
                    <cms:LocalizedLabel ID="lblDisplayName" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.DisplayName" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtDisplayName" runat="server" CssClass="TextBoxField"
                        MaxLength="100" />
                    <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName:textbox"
                        Display="dynamic" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblCodeName" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.lblCodeName" />
                </td>
                <td>
                    <cms:CodeName ID="txtCodeName" runat="server" CssClass="TextBoxField" MaxLength="100"
                        ReadOnly="True" Enabled="False" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblTableName" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.lblTableName" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtTableName" runat="server" CssClass="TextBoxField" MaxLength="100"
                        ReadOnly="True" Enabled="False" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlFormSettings" runat="server">
        <table>
            <tr>
                <td class="FieldLabel" style="width: 160px">
                    <cms:LocalizedLabel ID="lblAfterSubmited" runat="server" EnableViewState="false"
                        ResourceString="BizFormGeneral.lblAfterSubmited" />
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    &nbsp;<cms:LocalizedRadioButton ID="radDisplay" runat="server" GroupName="grpAfterSubmited"
                        AutoPostBack="True" OnCheckedChanged="radDisplay_CheckedChanged" ResourceString="BizFormGeneral.radDisplay" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtDisplay" runat="server" MaxLength="200" CssClass="TextBoxField" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    &nbsp;<cms:LocalizedRadioButton ID="radRedirect" runat="server" GroupName="grpAfterSubmited"
                        AutoPostBack="True" OnCheckedChanged="radRedirect_CheckedChanged" ResourceString="BizFormGeneral.radRedirect" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtRedirect" runat="server" MaxLength="200" CssClass="TextBoxField" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    &nbsp;<cms:LocalizedRadioButton ID="radClear" runat="server" GroupName="grpAfterSubmited"
                        AutoPostBack="True" OnCheckedChanged="radClear_CheckedChanged" ResourceString="BizFormGeneral.radClear" />
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    &nbsp;<cms:LocalizedRadioButton ID="radContinue" runat="server" GroupName="grpAfterSubmited"
                        AutoPostBack="True" OnCheckedChanged="radClear_CheckedChanged" ResourceString="BizFormGeneral.radContinue" />
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblButtonText" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.lblButtonText" />
                </td>
                <td>
                    <cms:LocalizableTextBox ID="txtButtonText" runat="server" MaxLength="200" CssClass="TextBoxField" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel ID="lblSubmitButtonImage" runat="server" EnableViewState="False"
                        ResourceString="BizFormGeneral.lblSubmitButtonImage" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtSubmitButtonImage" runat="server" MaxLength="200" CssClass="TextBoxField" />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    <cms:FormSubmitButton ID="btnOk" runat="server" CssClass="SubmitButton" OnClick="btnOk_Click"
                        ResourceString="general.ok" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
