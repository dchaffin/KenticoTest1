<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailTemplateEdit.ascx.cs"
    Inherits="CMSModules_EmailTemplates_Controls_EmailTemplateEdit" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroSelector.ascx" TagName="MacroSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/EmailTemplates/FormControls/EmailTemplateTypeSelector.ascx"
    TagName="EmailTypeSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<cms:ObjectLockingPanel runat="server" ID="pnlObjectLocking">
    <table style="vertical-align: top; width: 100%">
        <tr>
            <td class="FieldLabel" style="width: 150px;">
                <cms:LocalizedLabel runat="server" ID="lblDisplayName" EnableViewState="false" ResourceString="General.DisplayName"
                    ShowRequiredMark="true" DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorDisplayName" runat="server"
                    EnableViewState="false" ControlToValidate="txtDisplayName:textbox" CssClass="ContentValidator"
                    Display="dynamic" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCodeName" EnableViewState="false" ResourceString="General.CodeName"
                    ShowRequiredMark="true" DisplayColon="true" />
            </td>
            <td>
                <cms:CodeName ID="txtCodeName" runat="server" CssClass="TextBoxField" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorCodeName" runat="server"
                    EnableViewState="false" ControlToValidate="txtCodeName" CssClass="ContentValidator"
                    Display="dynamic" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblEmailType" EnableViewState="false" ResourceString="EmailTemplate_Edit.EmailType"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:EmailTypeSelector ID="drpEmailType" runat="server" AllowAll="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblFrom" EnableViewState="false" ResourceString="General.FromEmail"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtFrom" runat="server" CssClass="TextBoxField" MaxLength="250" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCc" EnableViewState="false" ResourceString="EmailTemplate_Edit.Cc"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtCc" runat="server" CssClass="TextBoxField" MaxLength="250" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblBcc" EnableViewState="false" ResourceString="EmailTemplate_Edit.Bcc"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtBcc" runat="server" CssClass="TextBoxField" MaxLength="250" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblSubject" EnableViewState="false" ResourceString="General.Subject"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtSubject" runat="server" CssClass="VeryLongTextBox"
                    MaxLength="250" />
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top; padding-top: 5px" class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblText" EnableViewState="false" ResourceString="EmailTemplate_Edit.HTMLVersion"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:MacroEditor ID="txtText" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td style="padding-top: 5px;">
                <cms:MacroSelector ID="macroSelectorElm" runat="server" JavaScripFunction="InsertMacroRich"
                    IsLiveSite="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top; padding-top: 5px" class="FieldLabel">
                <cms:LocalizedLabel ID="lblTextPlain" runat="server" EnableViewState="false" ResourceString="EmailTemplate_Edit.PlainTextVersion"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:MacroEditor ID="txtPlainText" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td style="padding-top: 5px;">
                <cms:MacroSelector ID="macroSelectorPlain" runat="server" JavaScripFunction="InsertMacroPlain"
                    IsLiveSite="false" />
            </td>
        </tr>
    </table>
</cms:ObjectLockingPanel>
