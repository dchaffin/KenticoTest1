<%@ Page Language="C#" AutoEventWireup="true" Title="Online marketing settings" CodeFile="Default.aspx.cs"
    Theme="Default" Inherits="CMSModules_Content_CMSDesk_OnlineMarketing_Settings_Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="ConversionSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectCampaign.ascx" TagName="SelectCampaign"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSFormControls/Inputs/TextboxDoubleValidator.ascx" TagPrefix="cms"
    TagName="DoubleValidator" %>
<asp:Content ID="plcHeader" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:CMSDocumentPanel ID="pnlDoc" runat="server" />
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="PageHeaderLine">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <table>
        <tr>
            <td class="FieldLabel" style="vertical-align: top;">
                <cms:LocalizedLabel ID="lblCampaign" runat="server" EnableViewState="false" ResourceString="doc.urls.trackcampaign"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:SelectCampaign runat="server" ID="usSelectCampaign" IsLiveSite="false" SelectionMode="SingleTextBox" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblConversionName" runat="server" EnableViewState="false"
                    ResourceString="om.trackconversionname" DisplayColon="true" />
            </td>
            <td>
                <cms:ConversionSelector runat="server" ID="ucConversionSelector" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblConversionValue" runat="server" EnableViewState="false"
                    ResourceString="om.trackconversionvalue" DisplayColon="true" />
            </td>
            <td>
                <cms:DoubleValidator ID="txtConversionValue" runat="server" CssClass="TextBoxField"
                    MaxLength="200" />
            </td>
        </tr>
    </table>
</asp:Content>
