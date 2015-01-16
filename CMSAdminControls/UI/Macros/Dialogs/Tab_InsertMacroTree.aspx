<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" CodeFile="Tab_InsertMacroTree.aspx.cs"
    Inherits="CMSAdminControls_UI_Macros_Dialogs_Tab_InsertMacroTree" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/Trees/MacroTree.ascx" TagName="MacroTree"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:MacroTree ID="macroTree" ShortID="t" runat="server" MixedMode="false" MacroExpression="CMSContext.Current"
            OnNodeClickHandler="InsertMacro" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton" EnableViewState="False"
            ResourceString="dialogs.actions.cancel" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
