<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AbuseReportFilter.ascx.cs"
    Inherits="CMSModules_AbuseReport_Controls_AbuseReportFilter" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<table>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTitle" runat="server" ResourceString="general.title" DisplayColon="true"
                EnableViewState="false" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtTitle" runat="server" CssClass="TextBoxField" MaxLength="50" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblStatus" runat="server" ResourceString="abuse.status" DisplayColon="true"
                EnableViewState="false" />
        </td>
        <td>
            <cms:LocalizedDropDownList ID="drpStatus" runat="server" CssClass="DropDownField"
                AutoPostBack="false" />
        </td>
    </tr>
    <asp:PlaceHolder ID="plcSites" runat="Server">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblSites" runat="server" ResourceString="general.site" DisplayColon="true"
                    EnableViewState="false" />
            </td>
            <td>
                <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td>
        </td>
        <td>
            <cms:LocalizedButton ID="btnShow" ResourceString="General.Show" runat="server" CssClass="ContentButton" EnableViewState="false" OnClick="btnShow_Click" />
            <asp:LinkButton ID="btnReset" runat="server" Style="line-height: 2em; margin: 1em"
                EnableViewState="false" />
        </td>
    </tr>
</table>
