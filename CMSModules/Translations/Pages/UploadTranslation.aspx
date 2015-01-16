<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Pages_UploadTranslation"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    CodeFile="UploadTranslation.aspx.cs" Title="Upload translation" %>

<%@ Register Src="~/CMSModules/Translations/Controls/UI/TranslationSubmission/UploadTranslation.ascx"
    TagName="UploadTranslation" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="PageContent">
        <cms:UploadTranslation runat="server" ID="uploadElem" />
    </div>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
<asp:Content ContentPlaceHolderID="plcFooter" runat="server" ID="plcFooter">
    <div class="FloatRight">
        <cms:LocalizedButton runat="server" ID="btnUpload" EnableViewState="false" ResourceString="general.upload"
            CssClass="SubmitButton" />
        <cms:LocalizedButton runat="server" ID="btnCancel" EnableViewState="false" ResourceString="general.cancel"
            OnClientClick="return CloseDialog();" CssClass="SubmitButton" />
    </div>
</asp:Content>
