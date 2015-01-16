<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" CodeFile="GooglePlusAccessTokenPage.aspx.cs" Inherits="CMSFormControls_Documents_SocialMediaPublishing_GooglePlusAccessTokenPage" Theme="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Label ID="lblStatus" runat="server" CssClass="AdvancedLabel Error" style="margin: 10px;"></asp:Label>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="plcFooter" runat="server">
    <div style="float:right">
        <cms:LocalizedButton ID="btnClose" runat="server" CssClass="SubmitButton" ResourceString="general.finish" OnClientClick="CloseDialog()"/>
    </div>
</asp:Content>