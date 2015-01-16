<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_UserSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_UserSettings" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblEmail" ResourceString="clonning.settings.user.email"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtEmail" CssClass="TextBoxField" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblGeneratePassword" ResourceString="clonning.settings.user.generatepassword"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkGeneratePassword" Checked="true" />
        </td>
    </tr>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblPassword" ResourceString="clonning.settings.user.password"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:PasswordStrength runat="server" ID="txtPassword" AllowEmpty="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblPermissions" ResourceString="clonning.settings.user.permissions"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkPermissions" Checked="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblPersonalization" ResourceString="clonning.settings.user.personalization"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkPersonalization" Checked="true" />
        </td>
    </tr>
</table>
