<%@ Page Language="C#" AutoEventWireup="True"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Automation process – Reports" EnableEventValidation="false"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Report" Theme="Default" CodeFile="Tab_Report.aspx.cs" %> 
               
<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>
    
<asp:Content ID="cntSiteSelector" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true" ResourceString="General.Site" />
    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="sg" runat="server" />
    <cms:SiteSelector ID="siteSelector" runat="server" ShortID="s" PostbackOnDropDownChange="true" AllowGlobal="true" />
</asp:Content>

<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcActions">
    <cms:ReportHeader ID="reportHeader" runat="server" PanelCssClass="" />
    <cms:HeaderActions ID="rightHeaderActions" runat="server" PanelCssClass="FloatRight" />
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisplayReport ID="ucReport" runat="server" BodyCssClass="DisplayReportBody" RenderCssClasses="true" />
</asp:Content>