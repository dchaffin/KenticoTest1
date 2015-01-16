<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Pages_Users_User_Edit_Password" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="User Edit - Password" CodeFile="User_Edit_Password.aspx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <table>
            <tr>
                <td class="FieldLabel FieldLabelTop" style="width:150px;">
                    <asp:Label ID="LabelPassword" runat="server" Text="Label" />
                </td>
                <td>
                  <div style="width: 292px">
                    <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
                  </div>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <asp:Label ID="LabelConfirmPassword" runat="server" Text="Label" /></td>
                <td>
                    <cms:CMSTextBox ID="TextBoxConfirmPassword" runat="server" TextMode="Password" CssClass="TextBoxField"
                        MaxLength="100"></cms:CMSTextBox ></td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <asp:CheckBox ID="chkSendEmail" runat="server" Visible="true" CssClass="CheckBoxMovedLeft"
                        Checked="true" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <cms:CMSButton ID="ButtonSetPassword" runat="server" Text="" OnClick="ButtonSetPassword_Click"
                                    CssClass="LongSubmitButton" EnableViewState="false" />
               </td>
            </tr>
        </table>
    </asp:PlaceHolder>
</asp:Content>
