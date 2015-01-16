<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Pages_Tools_Header"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" CodeFile="Header.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:frameresizer id="FrameResizer" runat="server" minsize="6, *" vertical="True"
        framesetname="rowsFrameset" />
    <asp:Panel runat="server" ID="pnlHeader" CssClass="TabsPageHeader">
        <asp:Panel runat="server" ID="pnlTitle" CssClass="PageHeader">
            <cms:pagetitle id="titleElem" runat="server" />
        </asp:Panel>
        <asp:Panel ID="pnlActions" runat="server" CssClass="PageHeaderLine SiteHeaderLine NoBorderLine">
            <cms:localizedlabel id="lblSite" runat="server" enableviewstate="false" resourcestring="general.site"
                displaycolon="true" />
            <cms:siteselector id="siteSelector" runat="server" islivesite="false" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlTabsContainer" CssClass="TabsPageTabs LightTabs BreadTabs">
            <asp:Panel runat="server" ID="pnlLeft" CssClass="TabsLeft">
                &nbsp;
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlTabs" CssClass="TabsTabs">
                <asp:Panel runat="server" ID="pnlWhiteTabs" CssClass="TabsWhite">
                    <cms:basictabcontrol id="BasicTabControlMenu" runat="server" useclientscript="true" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlTabsRight" CssClass="TabsRight">
                &nbsp;
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="PanelSeparator" CssClass="HeaderSeparator">
            &nbsp;
        </asp:Panel>
    </asp:Panel>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
