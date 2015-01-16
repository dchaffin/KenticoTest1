<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditFromSettings.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Chat support canned response properties" Inherits="CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_EditFromSettings" Theme="Default" %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatSupportCannedResponse/Edit.ascx"
    TagName="ChatSupportCannedResponseEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlCannedResponseList" CssClass="PageContent">
        <cms:ChatSupportCannedResponseEdit ID="editElem" runat="server" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton ID="btnClose" CssClass="SubmitButton" runat="server" OnClientClick="return CloseDialog();" EnableViewState="false" />
    </div>
</asp:Content>
