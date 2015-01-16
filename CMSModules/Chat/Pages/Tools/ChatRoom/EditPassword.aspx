<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat room properties – Password"
    Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_EditPassword" Theme="Default" CodeFile="EditPassword.aspx.cs" %>            
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <table>
            <tr>
                <td class="FieldLabel FieldLabelTop" style="width:150px;">
                    <cms:FormLabel runat="server" ID="LabelPassword" DisplayColon="true" ResourceString="general.password" ></cms:FormLabel>
                </td>
                <td>
                  <div style="width: 292px">
                    <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
                  </div>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:FormLabel runat="server" ID="LabelConfirmPassword" DisplayColon="true" ResourceString="general.confirmpassword" ></cms:FormLabel>
                </td>
                <td>
                    <cms:CMSTextBox ID="TextBoxConfirmPassword" runat="server" TextMode="Password" CssClass="TextBoxField"
                        MaxLength="100"></cms:CMSTextBox ></td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <br />
                    <cms:CMSButton ID="btnSetPassword" runat="server" Text="" OnClick="ButtonSetPassword_Click"
                                    CssClass="LongSubmitButton" EnableViewState="false" />
                    <cms:CMSButton ID="btnRemovePassword" runat="server" Text="" OnClick="ButtonRemovePassword_Click"
                                    CssClass="LongSubmitButton" EnableViewState="false" Visible="false" />
               </td>
            </tr>
        </table>
    </asp:PlaceHolder>
</asp:Content>