<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" CodeFile="Tab_InsertMacroCode.aspx.cs"
    Inherits="CMSAdminControls_UI_Macros_Dialogs_Tab_InsertMacroCode" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:MacroEditor ID="macroEditor" runat="server" MixedMode="false" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnInsert" runat="server" CssClass="SubmitButton" OnClick="btnInsert_Click"
            EnableViewState="False" ResourceString="dialogs.actions.insert" />
        <cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton" EnableViewState="False"
            ResourceString="dialogs.actions.cancel" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
