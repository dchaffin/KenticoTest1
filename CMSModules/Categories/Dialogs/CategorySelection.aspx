<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CategorySelection.aspx.cs"
    Inherits="CMSModules_Categories_Dialogs_CategorySelection" Title="Selection dialog"
    ValidateRequest="false" Theme="default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Categories/Controls/CategorySelectionDialog.ascx"
    TagName="SelectionDialog" TagPrefix="cms" %>
<asp:Content ID="cntActions" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlActions">
        <ContentTemplate>
            <div class="CMSEditMenu">
                <cms:HeaderActions ID="actionsElem" ShortID="a" runat="server" IsLiveSite="false" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:SelectionDialog ID="SelectionElem" runat="server" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" CssClass="SubmitButton" EnableViewState="False"
            ResourceString="general.ok" OnClientClick="return US_Submit();" /><cms:LocalizedButton ID="btnCancel" runat="server"
                CssClass="SubmitButton" EnableViewState="False" ResourceString="general.cancel" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
