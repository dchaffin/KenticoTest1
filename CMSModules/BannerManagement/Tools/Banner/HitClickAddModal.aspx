<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Tools_Banner_HitClickAddModal" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" CodeFile="HitClickAddModal.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="content">
    <div style="margin: 10px;">
        <cms:LocalizedLabel ID="lblNumberToAdd" runat="server" DisplayColon="true"></cms:LocalizedLabel> <asp:TextBox ID="txtNumberToAdd" runat="server"></asp:TextBox>
        <div style="margin-top: 7px;">
            <cms:LocalizedLabel ID="lblFormatError" runat="server" style="display: none; color: Red;" ResourceString="banner.integerneeded"></cms:LocalizedLabel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton ID="btnAdd" CssClass="SubmitButton" runat="server" EnableViewState="false" />
        <cms:CMSButton ID="btnClose" CssClass="SubmitButton" runat="server" OnClientClick="return CloseDialog();" EnableViewState="false" />
    </div>
</asp:Content>

