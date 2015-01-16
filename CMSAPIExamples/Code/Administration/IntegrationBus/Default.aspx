<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Administration_IntegrationBus_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<%@ Register Src="~/CMSAPIExamples/Controls/APIExampleSection.ascx" TagName="APIExampleSection" TagPrefix="cms" %>
<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Integration connector --%>
    <cms:APIExamplePanel ID="pnlCreateIntegrationConnector" runat="server" GroupingText="Integration connector">
        <cms:APIExample ID="apiCreateIntegrationConnector" runat="server" ButtonText="Create connector" InfoMessage="Connector 'My new connector' was created." />
        <cms:APIExample ID="apiGetAndUpdateIntegrationConnector" runat="server" ButtonText="Get and update connector" APIExampleType="ManageAdditional" InfoMessage="Connector 'My new connector' was updated." ErrorMessage="Connector 'My new connector' was not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateIntegrationConnectors" runat="server" ButtonText="Get and bulk update connectors" APIExampleType="ManageAdditional" InfoMessage="All connectors matching the condition were updated." ErrorMessage="Connectors matching the condition were not found." />
    </cms:APIExamplePanel>
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Integration connector --%>
    <cms:APIExamplePanel ID="pnlDeleteIntegrationConnector" runat="server" GroupingText="Integration connector">
        <cms:APIExample ID="apiDeleteIntegrationConnector" runat="server" ButtonText="Delete connector" APIExampleType="CleanUpMain" InfoMessage="Connector 'My new connector' and all its dependencies were deleted." ErrorMessage="Connector 'My new connector' was not found." />
    </cms:APIExamplePanel>
</asp:Content>
