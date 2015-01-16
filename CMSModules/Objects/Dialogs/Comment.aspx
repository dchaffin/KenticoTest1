<%@ Page Title="Localize string" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Objects_Dialogs_Comment"
    CodeFile="Comment.aspx.cs" %>

<%@ Register Src="~/CMSModules/Objects/Controls/Locking/Comment.ascx" TagName="Comment"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" CssClass="PageContent" runat="server">
        <asp:PlaceHolder ID="plcContent" runat="server">
            <cms:Comment ID="ucComment" runat="server" IsLiveSite="false" />
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>
<asp:Content ID="footer" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" CssClass="SubmitButton" ResourceString="general.Save"
            EnableViewState="false" />
        <cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton" ResourceString="general.cancel"
            EnableViewState="false" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
