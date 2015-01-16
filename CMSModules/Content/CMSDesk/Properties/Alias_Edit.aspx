<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Alias_Edit"
    Theme="Default" CodeFile="Alias_Edit.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Documents/DocumentURLPath.ascx" TagName="DocumentURLPath"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectCampaign.ascx" TagName="SelectCampaign"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcActions" runat="server" ID="cntHeader">
    <cms:HeaderActions ID="rightHeaderActions" runat="server" PanelCssClass="FloatRight" />
</asp:Content>
<asp:Content runat="server" ID="pnlContent" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server" CssClass="PropertiesPanel">
        <cms:DocumentURLPath runat="server" ID="ctrlURL" HideCustom="true" />
        <br />
        <table>
            <tr>
                <td class="FieldLabel" style="width: 125px;">
                    <cms:LocalizedLabel runat="server" ID="lblAction" ResourceString="doc.urls.aliasaction"
                        DisplayColon="true" />
                </td>
                <td>
                    <cms:LocalizedDropDownList runat="server" ID="drpAction" CssClass="DropDownField" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <asp:Label ID="lblDocumentCulture" runat="server" />
                </td>
                <td>
                    <cms:SiteCultureSelector runat="server" ID="cultureSelector" IsLiveSite="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <asp:Label ID="lblURLExtensions" runat="server" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtURLExtensions" runat="server" CssClass="TextBoxField" MaxLength="100" /><br />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <asp:Label ID="lblTrackCampaign" runat="server" />
                </td>
                <td>
                    <cms:SelectCampaign runat="server" ID="usSelectCampaign" IsLiveSite="false" SelectionMode="SingleTextBox" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="pnlUrlInfo">
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblaliasInfo" ResourceString="aliasnode.text"
                            DisplayColon="true" />
                    </td>
                    <td>
                        <asp:Label ID="lblUrlInfoText" runat="server" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td>
                </td>
                <td>
                    <br />
                    <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOK_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div class="Clear"></div>
</asp:Content>
