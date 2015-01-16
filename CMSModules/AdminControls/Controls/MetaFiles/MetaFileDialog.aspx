<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MetaFileDialog.aspx.cs" Inherits="CMSModules_AdminControls_Controls_MetaFiles_MetaFileDialog"
     MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="MetaFile - Attachments" Theme="Default" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileList.ascx" TagPrefix="cms" TagName="FileList" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent ClearBorder Newsletters">
        <cms:FileList ID="AttachmentList" runat="server" ShortID="fl" />
    </div>
    <asp:HiddenField ID="hdnCount" runat="server" EnableViewState="false" Value="-1" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton"
                EnableViewState="False" ResourceString="general.close" OnClientClick="UpdateAttachmentCount();CloseDialog();return false;" />
    </div>
</asp:Content>