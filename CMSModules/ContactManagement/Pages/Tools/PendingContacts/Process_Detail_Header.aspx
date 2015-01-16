<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Process_Detail_Header.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Contact management"
    Inherits="CMSModules_ContactManagement_Pages_Tools_PendingContacts_Process_Detail_Header"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlHeader" CssClass="TabsPageHeader">
        <asp:Panel runat="server" ID="pnlTitle" CssClass="DialogsPageHeader">
            <cms:PageTitle ID="titleElem" runat="server" />
        </asp:Panel>
        <asp:Panel runat="server" ID="PanelSeparator" CssClass="HeaderSeparator">
            &nbsp;
        </asp:Panel>
    </asp:Panel>
</asp:Content>
