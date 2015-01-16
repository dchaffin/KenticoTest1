<%@ Page Title="LinkedIn Access Token" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" Inherits="CMSFormControls_Documents_SocialMediaPublishing_LinkedInAccessTokenPage"
    Theme="Default" CodeFile="LinkedInAccessTokenPage.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Label ID="lblStatus" runat="server" CssClass="AdvancedLabel Error" style="margin: 10px;"></asp:Label>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plcFooter" runat="server">
    <div style="float: right">
        <cms:LocalizedButton ID="btnClose" runat="server" CssClass="SubmitButton" ResourceString="general.finish"
            OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
