<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditDialog.aspx.cs" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Inherits="CMSModules_Chat_Pages_Tools_ChatUser_EditDialog" %>

<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatUser/Edit.ascx" TagName="ChatUserEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatUserEdit ID="editElem" runat="server" IsLiveSite="false" IsOnModalDialog="true" />   
</asp:Content>

<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton ID="btnClose" CssClass="SubmitButton" runat="server" OnClientClick="return CloseDialog();" EnableViewState="false" />
    </div>
</asp:Content>
