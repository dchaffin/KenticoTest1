<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Controls_ResetPassword" CodeFile="ResetPassword.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>
<cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
<cms:LocalizedLabel runat="server" ID="lblLogonLink" EnableViewState="false"></cms:LocalizedLabel>
<asp:Panel runat="server" ID="pnlReset">
    <table>
        <tr>
            <td class="FieldLabel FieldLabelTop">
                <cms:LocalizedLabel ID="lblPassword" runat="server" EnableViewState="false" ResourceString="general.password"
                    DisplayColon="true"></cms:LocalizedLabel>
            </td>
            <td>
                <cms:PasswordStrength runat="server" ID="passStrength" TextBoxClass="LogonTextBox"
                    ValidationGroup="PasswordReset" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblConfirmPassword" runat="server" EnableViewState="false"
                    ResourceString="general.confirmpassword" DisplayColon="true"></cms:LocalizedLabel>
            </td>
            <td>
                <cms:CMSTextBox ID="txtConfirmPassword" runat="server" TextMode="Password" MaxLength="200"
                    CssClass="LogonTextBox"></cms:CMSTextBox>
                <cms:CMSRequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                    ValidationGroup="PasswordReset" Display="Dynamic"></cms:CMSRequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:CMSButton runat="server" ID="btnReset" EnableViewState="false" CssClass="SubmitButton"
                    OnClick="btnReset_Click" ValidationGroup="PasswordReset" />
            </td>
        </tr>
    </table>
</asp:Panel>
