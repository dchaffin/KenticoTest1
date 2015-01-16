<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Mvtest properties" Inherits="CMSModules_OnlineMarketing_Dialogs_ContentPersonalizationVariantList"
    Theme="Default" CodeFile="ContentPersonalizationVariantList.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ContentPersonalizationVariant/List.ascx"
    TagName="VariantList" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:VariantList ID="listElem" runat="server" IsLiveSite="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton ID="btnClose" runat="server" CssClass="SubmitButton" OnClientClick="CloseDialog();" />
    </div>
</asp:Content>
