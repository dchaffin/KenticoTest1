<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_WinnerMailout"
    CodeFile="Newsletter_Issue_WinnerMailout.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" %>

<asp:Content runat="server" ID="cntBody" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
    <asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false" /><br />
    <cms:LocalizedLabel runat="server" ID="lblMailoutTime" ResourceString="newsletter_winnermailout.mailouttime"
        EnableViewState="false" DisplayColon="true" AssociatedControlID="dtpMailout" />
    <cms:DateTimePicker runat="server" ID="dtpMailout" SupportFolder="~/CMSAdminControls/Calendar" />
    </div>
</asp:Content>
<asp:Content runat="server" ID="cntFooter" ContentPlaceHolderID="plcFooter">
<div class="AlignRight">
    <cms:LocalizedButton ID="btnSend" runat="server" EnableViewState="false" ResourceString="general.ok"
        CssClass="SubmitButton" />
    <cms:LocalizedButton ID="btnClose" runat="server" EnableViewState="false" ResourceString="general.cancel"
        CssClass="SubmitButton" CausesValidation="false" OnClientClick="CloseDialog();return false;" />
</div>
</asp:Content>
