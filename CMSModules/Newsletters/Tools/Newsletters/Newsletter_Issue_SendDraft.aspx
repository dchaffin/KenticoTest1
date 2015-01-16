<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Newsletter_Issue_SendDraft.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SendDraft"
    Title="Newsletter issue - Send draft" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:LocalizedLabel ID="lblSendDraft" runat="server" ResourceString="newsletterissue_send.senddraft"
            EnableViewState="false" CssClass="InfoLabel" />
        <div>
            <cms:LocalizedLabel ID="lblDraftEmail" runat="server" ResourceString="newsletterissue_send.emails"
                DisplayColon="true" AssociatedControlID="txtSendDraft" EnableViewState="false"
                Style="vertical-align: middle" />&nbsp;
            <cms:CMSTextBox ID="txtSendDraft" runat="server" MaxLength="450" CssClass="TextBoxField"
                Style="vertical-align: middle" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnSend" runat="server" ShowAsButton="true" CssClass="SubmitButton"
            OnClick="btnSend_Click" ResourceString="general.send" />
        <cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton" EnableViewState="False"
            ResourceString="general.close" OnClientClick="CloseDialog();return false;" />
    </div>
</asp:Content>
