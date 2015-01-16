<%@ Page Title="Facebook Access Token" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" AutoEventWireup="true" Inherits="CMSFormControls_Documents_SocialMediaPublishing_FacebookAccessTokenPage" Theme="Default" CodeFile="FacebookAccessTokenPage.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Label ID="lblStatus" runat="server" CssClass="InfoLabel"></asp:Label>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="plcFooter" runat="server">
    <div style="float:right">
        <cms:LocalizedButton ID="btnClose" runat="server" CssClass="SubmitButton" ResourceString="general.close" OnClientClick="CloseDialog()"/>
    </div>
</asp:Content>

