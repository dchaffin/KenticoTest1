<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSInstall_Controls_WizardSteps_WagDialog" CodeFile="WagDialog.ascx.cs" %>
<asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
    Visible="false" />
<table class="InstallWizard" border="0" cellpadding="0" cellspacing="10">
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblFreeKeyForDomain" runat="server" ResourceString="Install.wag.lblFreeKeyForDomain"
                EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td>
            <table border="0" cellpadding="0" cellspacing="5">
                <tr>
                    <td nowrap="nowrap" align="right" style="width:80px;">
                        <cms:LocalizedLabel ID="lblUserDomain" runat="server" ResourceString="Install.wag.lblUserDomain"
                            EnableViewState="false" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtUserDomain" CssClass="InstallFormTextBox" runat="server" /><br />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td nowrap="nowrap" align="right">
                    </td>
                    <td>
                        <cms:LocalizedLabel ID="lblDomainFormat" runat="server" ResourceString="Install.wag.lblDomainFormat"
                            EnableViewState="false" />
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblOptionalForm" runat="server" ResourceString="Install.wag.lblOptionalForm"
                EnableViewState="false" DisplayColon="true" />
        </td>
    </tr>
    <tr>
        <td>
            <table border="0" cellpadding="0" cellspacing="5">
                <tr>
                    <td nowrap="nowrap" align="right" style="width:80px;">
                        <cms:LocalizedLabel ID="lblUserFirstName" runat="server" ResourceString="Install.wag.lblFirstName"
                            EnableViewState="false" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtUserFirstName" CssClass="InstallFormTextBox" runat="server" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td nowrap="nowrap" align="right">
                        <cms:LocalizedLabel ID="lblUserLastName" runat="server" ResourceString="Install.wag.lblLastName"
                            EnableViewState="false" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtUserLastName" CssClass="InstallFormTextBox" runat="server" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td nowrap="nowrap" align="right">
                        <cms:LocalizedLabel ID="lblUserEmail" runat="server" ResourceString="Install.wag.lblUserEmail"
                            EnableViewState="false" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtUserEmail" CssClass="InstallFormTextBox" runat="server" />
                    </td>
                    <td>
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcPass" runat="server" Visible="false">
                    <tr>
                    <td nowrap="nowrap" align="right">
                            <cms:LocalizedLabel ID="LocalizedLabel1" runat="server" ResourceString="Install.wag.lblPassword"
                                EnableViewState="false" DisplayColon="true" />
                        </td>
                        <td>
                            <cms:CMSTextBox ID="txtPassword" CssClass="InstallFormTextBox" runat="server" TextMode="Password" />
                        </td>
                        <td>
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>
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
</table>
