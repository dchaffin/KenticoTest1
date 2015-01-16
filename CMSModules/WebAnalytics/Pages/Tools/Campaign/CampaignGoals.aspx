<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CampaignGoals.aspx.cs" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignGoals"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Campaign goals"
    EnableEventValidation="false" Theme="Default" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx"
    TagName="GraphType" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectCampaign.ascx" TagName="SelectCampaign"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="PageHeaderLine">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
    <cms:ReportHeader runat="server" ID="reportHeader" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
    <div class="PageHeaderLine">
        <cms:GraphType runat="server" ID="ucGraphType" />
    </div>
    <div class="ReportBody">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
        <table>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblCampaign" runat="server" ResourceString="analytics.campaign"
                        DisplayColon="true" />
                </td>
                <td>
                    <cms:SelectCampaign runat="server" ID="ucSelectCampaign" AllowAll="true" PostbackOnChange="true"
                        CssClass="CampaignReportSelctor" />
                </td>
            </tr>
        </table>
        <br />
        <asp:PlaceHolder runat="server" ID="pnlDisplayReport"></asp:PlaceHolder>
    </div>
</asp:Content>
