<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChangePassword.aspx.cs" Inherits="CMSModules_Membership_Pages_ChangePassword"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Change password" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/ChangePassword.ascx"
    TagName="ChangePassword" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel ID="Panel1" runat="server" CssClass="PageContent">
        <cms:ChangePassword ID="ucChangePassword" runat="server" Visible="true" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:CMSButton ID="btnClose" runat="server" CssClass="SubmitButton" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
