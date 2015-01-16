<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Scheduler_Pages_Header"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/TabsHeader.master" Title="Scheduled tasks - Header"
    CodeFile="Header.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcSiteSelector" runat="server">
    <cms:LocalizedLabel runat="server" ID="lblSites" EnableViewState="false" ResourceString="general.site" DisplayColon="true" />
    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" OnlyRunningSites="false" AllowAll="false" AllowGlobal="true" AllowEmpty="false" />
</asp:Content>
