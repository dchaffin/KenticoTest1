<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConversionsCount.aspx.cs"
    EnableEventValidation="false" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="AB test conversions count" Inherits="CMSModules_OnlineMarketing_Pages_Tools_ABTest_ConversionsCount" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ABTest/ConversionReportViewer.ascx"
    TagPrefix="cms" TagName="ConversionReportViewer" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <cms:ReportHeader runat="server" ID="reportHeader" />
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="PageHeaderLine">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" CssClass="PageHeaderLine" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ConversionReportViewer runat="server" ID="ucConversionReportViewer" IsLiveSite="false" />
</asp:Content>
