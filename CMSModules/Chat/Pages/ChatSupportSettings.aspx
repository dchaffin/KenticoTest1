<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Chat_Pages_ChatSupportSettings" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" CodeFile="ChatSupportSettings.aspx.cs" %>

<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatSupportCannedResponse/List.ascx" TagName="ChatSupportCannedResponseList" TagPrefix="cms" %>

<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="content">
    <asp:Panel runat="server" ID="pnlCannedResponseList" CssClass="PageContent">
        <cms:ChatSupportCannedResponseList ID="listElem" runat="server" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton ID="btnClose" CssClass="SubmitButton" runat="server" OnClientClick="return CloseDialog();" EnableViewState="false" />
    </div>
</asp:Content>
