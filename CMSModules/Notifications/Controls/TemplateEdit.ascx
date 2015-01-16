<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Notifications_Controls_TemplateEdit"
    CodeFile="TemplateEdit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<table>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblDisplayName" CssClass="ContentLabel" DisplayColon="true"
                EnableViewState="false" />
        </td>
        <td>
            <cms:LocalizableTextBox runat="server" ID="txtDisplayName" CssClass="TextBoxField"
                MaxLength="250" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator runat="server" ControlToValidate="txtDisplayName:textbox"
                ID="valDisplayName" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblCodeName" CssClass="ContentLabel" DisplayColon="true"
                EnableViewState="false" />
        </td>
        <td>
            <cms:CodeName runat="server" ID="txtCodeName" CssClass="TextBoxField" MaxLength="250"
                EnableViewState="false" />
            <cms:CMSRequiredFieldValidator runat="server" ControlToValidate="txtCodeName" ID="valCodeName"
                EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
        </td>
    </tr>
</table>
