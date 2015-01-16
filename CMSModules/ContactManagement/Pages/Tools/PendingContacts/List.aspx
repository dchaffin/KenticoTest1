<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_ContactManagement_Pages_Tools_PendingContacts_List" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/PendingContacts.ascx" TagName="PendingContacts" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector" TagPrefix="cms" %>

<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true" ResourceString="General.Site" />
    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="sg" runat="server" ShowSiteAndGlobal="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedLabel ID="lblListInfo" runat="server" CssClass="BoldInfoLabel" EnableViewState="false" DisplayColon="true" ResourceString="ma.pendingcontact.list" />
    <cms:PendingContacts ID="listContacts" runat="server" />
</asp:Content>