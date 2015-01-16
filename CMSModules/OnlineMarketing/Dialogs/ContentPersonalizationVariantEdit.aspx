<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Variant properties" Inherits="CMSModules_OnlineMarketing_Dialogs_ContentPersonalizationVariantEdit"
    Theme="Default" CodeFile="ContentPersonalizationVariantEdit.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ContentPersonalizationVariant/Edit.ascx"
    TagName="VariantEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:VariantEdit ID="editElem" runat="server" IsLiveSite="false" />
        <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
    </div>
</asp:Content>
<asp:Content runat="server" ID="cntFooter" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:LocalizedButton runat="server" ID="btnSubmit" OnClick="btnSubmit_Click" ResourceString="general.ok"
            CssClass="SubmitButton" /><cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton"
                EnableViewState="False" ResourceString="general.cancel" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
