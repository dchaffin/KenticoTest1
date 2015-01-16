<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Mvtest properties" Inherits="CMSModules_OnlineMarketing_Dialogs_MVTVariantEdit"
    Theme="Default" CodeFile="MVTVariantEdit.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/MVTVariant/Edit.ascx"
    TagName="MvtVariantEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:MvtVariantEdit ID="editElem" runat="server" IsLiveSite="false" />
    </div>
    <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
</asp:Content>
<asp:Content runat="server" ID="cntFooter" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnClose" runat="server" CssClass="SubmitButton" EnableViewState="False"
            ResourceString="general.close" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
