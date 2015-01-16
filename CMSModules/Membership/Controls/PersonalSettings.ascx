<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Controls_PersonalSettings" CodeFile="PersonalSettings.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Avatars/UserPictureEdit.ascx" TagName="UserPictureFormControl"
    TagPrefix="upfc" %>
<div class="MyForum">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <table>
        <tr>
            <td class="FieldLabel">
                <asp:Label runat="server" AssociatedControlID="txtNickName" ID="lblNickName" />
            </td>
            <td>
                <cms:ExtendedTextBox runat="server" EnableEncoding="true" ID="txtNickName" CssClass="TextBoxField"
                    MaxLength="200" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label runat="server" AssociatedControlID="txtEmail" ID="lblEmail" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtEmail" runat="Server" CssClass="TextBoxField" MaxLength="100" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label runat="server" ID="lblAvatar" />
            </td>
            <td>
                <upfc:UserPictureFormControl ID="UserPictureFormControl" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label runat="server" AssociatedControlID="txtSignature" ID="lblSignature" EnableViewState="false" />
            </td>
            <td>
                <cms:ExtendedTextBox runat="server" EnableEncoding="true" ID="txtSignature" CssClass="MyProfileUserSignature"
                    TextMode="MultiLine" Rows="3"></cms:ExtendedTextBox>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:CMSButton runat="server" ID="btnOk" CssClass="ContentButton" OnClick="btnOk_Click" />
            </td>
        </tr>
    </table>
</div>
