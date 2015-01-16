<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PageTemplate_Footer.aspx.cs"
    Inherits="CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_Footer" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Theme="Default" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <div class="PageFooterLine">
        <div class="FloatRight">
            <cms:LocalizedButton ID="btnCancel" runat="server" ResourceString="dialogs.actions.cancel"
                CssClass="SubmitButton" EnableViewState="false" OnClientClick="return CloseDialog();" />
            <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        </div>
    </div>
</asp:Content>
