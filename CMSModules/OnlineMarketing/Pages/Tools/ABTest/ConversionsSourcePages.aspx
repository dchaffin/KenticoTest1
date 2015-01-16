<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="A/B test conversions source pages" Theme="Default" CodeFile="ConversionsSourcePages.aspx.cs"
    Inherits="CMSModules_OnlineMarketing_Pages_Tools_ABTest_ConversionsSourcePages"
    EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ABTest/ConversionReportViewer.ascx"
    TagPrefix="cms" TagName="ConversionReportViewer" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <cms:ReportHeader runat="server" ID="reportHeader" />
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="PageHeaderLine">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ConversionReportViewer runat="server" ID="ucConversionReportViewer" IsLiveSite="false" />
</asp:Content>
