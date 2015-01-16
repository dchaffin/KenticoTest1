<%@ Page Title="Localize string" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Automation_Pages_Comment"
    CodeFile="Comment.aspx.cs" %>

<%@ Register Src="~/CMSModules/Automation/Controls/Comment.ascx" TagName="Comment"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" CssClass="PageContent" runat="server">
        <asp:PlaceHolder ID="plcContent" runat="server">
            <cms:CMSAutomationManager ID="autoMan" runat="server" />
            <cms:Comment ID="ucComment" runat="server" IsLiveSite="false" />
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>
<asp:Content ID="footer" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" CssClass="LongSubmitButton" ResourceString="general.Save"
            EnableViewState="false" />
        <cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton" ResourceString="general.cancel"
            EnableViewState="false" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
