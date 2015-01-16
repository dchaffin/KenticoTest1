<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MVTestReport.aspx.cs" Inherits="CMSModules_OnlineMarketing_Pages_Content_MVTest_MVTestReport"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="MV test reports"
    EnableEventValidation="false" Theme="Default" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx"
    TagName="GraphType" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="SelectConversion"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/OnlineMarketing/FormControls/SelectMVTCombination.ascx"
    TagName="SelectCombinaton" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <cms:ReportHeader runat="server" ID="reportHeader" />
    <div class="PageHeaderLine">
        <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
        <cms:GraphType runat="server" ID="ucGraphType" />
    </div>
    <asp:Panel ID="pnlRadioButtons" runat="server" CssClass="PageHeaderLine">
        <cms:LocalizedRadioButton runat="server" ID="rbCount" ResourceString="conversion.count"
            AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
        <cms:LocalizedRadioButton runat="server" ID="rbValue" ResourceString="conversions.value"
            AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
        <cms:LocalizedRadioButton runat="server" ID="rbRate" ResourceString="abtesting.conversionrate"
            AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
        <cms:LocalizedRadioButton runat="server" ID="rbCombinations" ResourceString="mvtests.conversionsbycombination"
            AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlWarning" CssClass="PageHeaderLine">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcContent">
    <div class="ReportBody">
        <table>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblConversions" runat="server" ResourceString="abtesting.conversions"
                        DisplayColon="true" />
                </td>
                <td>
                    <cms:SelectConversion runat="server" ID="ucConversions" SelectionMode="SingleDropDownList" />
                </td>
                <asp:PlaceHolder runat="server" ID="pnlCombination">
                    <td>
                        &nbsp;&nbsp;
                        <cms:LocalizedLabel ID="lblCombination" runat="server" ResourceString="mvt.combination"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <cms:SelectCombinaton runat="server" ID="usCombination" PostbackOnChange="true" AllowAll="true"
                            AllowEmpty="false" ReturnColumnName="MVTCombinationName" />
                    </td>
                </asp:PlaceHolder>
            </tr>
        </table>
        <asp:Panel runat="server" ID="pnlContent">
        </asp:Panel>
    </div>
</asp:Content>
