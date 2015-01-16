<%@ Page Language="C#" AutoEventWireup="true" CodeFile="User_Edit_Dialog.aspx.cs"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Inherits="CMSModules_Membership_Pages_Users_User_Edit_Dialog" %>

<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <iframe width="100%" height="100%" id="pageview" name="pageview" scrolling="auto"
        frameborder="0" src="<%=viewpage%>" class="ContentFrame"></iframe>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton ID="btnClose" CssClass="SubmitButton" runat="server" OnClientClick="return CloseDialog();"
            EnableViewState="false" />
    </div>
</asp:Content>
