<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PendingContacts.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_ContactManagement_Pages_Tools_PendingContacts_MyPendingContacts_PendingContacts" Theme="Default" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/PendingContacts.ascx" TagName="PendingContacts" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector" TagPrefix="cms" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true" ResourceString="General.Site" />
    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="sg" runat="server" ShowSiteAndGlobal="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PendingContacts ID="listContacts" runat="server"/>
</asp:Content>