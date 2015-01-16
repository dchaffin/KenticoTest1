<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Reporting_ReportSettings.ascx.cs"
    Inherits="CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSettings" %>
<%@ Register Src="~/CMSModules/Reporting/FormControls/SelectReportCategory.ascx"
    TagName="SelectReportCategory" TagPrefix="cms" %>
<table>
    <tr>
        <td class="CloneParamTableCol">
            <cms:LocalizedLabel runat="server" ID="lblCategory" ResourceString="clonning.settings.report.category"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <cms:SelectReportCategory ID="categorySelector" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblCloneSavedReports" ResourceString="clonning.settings.report.savedreports"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCloneSavedReports" Checked="false" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblSubscriptions" ResourceString="clonning.settings.report.subscriptions"
                EnableViewState="false" DisplayColon="true" />
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkSubscriptions" Checked="false" />
        </td>
    </tr>
</table>
