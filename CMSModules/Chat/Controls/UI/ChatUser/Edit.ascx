<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Chat_Controls_UI_ChatUser_Edit" CodeFile="Edit.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UserSelector" TagPrefix="cms" %>
<cms:UIForm runat="server" ID="EditForm" ObjectType="chat.user" RedirectUrlAfterCreate="Edit.aspx?userid={%EditedObject.ID%}&saved=1" OnOnBeforeValidate="UIFormOnOnBeforeValidate">
    <LayoutTemplate>
        <cms:FormField runat="server" ID="fUserID" Field="ChatUserUserID" ResourceString="chat.user.userid" DisplayColon="true">
            <cms:UserSelector runat="server" ID="fUserSelector" ObjectType="cms.user" SelectionMode="SingleTextBox" />
            <asp:Panel runat="server" ID="pnlStaticUser">
                <cms:CMSTextBox runat="server" ID="txtUserNameStaticValue" ReadOnly="true" CssClass="SelectorTextBox" />
                <cms:CMSButton runat="server" ID="btnEditUser" CssClass="ContentButton" EnableViewState="false" />
            </asp:Panel>
        </cms:FormField>
        <cms:FormField runat="server" ID="fNickname" Field="ChatUserNickname" FormControl="TextBoxControl" ResourceString="chat.user.nickname" DisplayColon="true" />
        <cms:FormSubmit runat="server" ID="fSubmit" />
    </LayoutTemplate>
</cms:UIForm>
