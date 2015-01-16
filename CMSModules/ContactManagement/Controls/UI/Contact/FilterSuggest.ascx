<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterSuggest.ascx.cs"
    Inherits="CMSModules_ContactManagement_Controls_UI_Contact_FilterSuggest" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnFilter" CssClass="FilterPanel">
    <table cellpadding="0" cellspacing="2">
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSuggest" runat="server" EnableViewState="false" ResourceString="om.contact.suggest"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizedCheckBox ID="chkEmail" runat="server" ResourceString="general.emaildomain" ToolTipResourceString="om.contact.suggestcontactsbyemail.tooltip" /> 
            </td>
            <td>
                <cms:LocalizedCheckBox ID="chkPhone" runat="server" ResourceString="general.phone" ToolTipResourceString="om.contact.suggestcontactsbyphone.tooltip" />
            </td>
            <td>
                <cms:LocalizedCheckBox ID="chkAddress" runat="server" ResourceString="om.contact.postaddress" ToolTipResourceString="om.contact.suggestcontactsbyaddress.tooltip" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:LocalizedCheckBox ID="chkBirthDay" runat="server" ResourceString="om.contact.birthday" ToolTipResourceString="om.contact.suggestcontactsbybirthday.tooltip" />
            </td>
            <td>
                <cms:LocalizedCheckBox ID="chkMembership" runat="server" ResourceString="om.membership" ToolTipResourceString="om.contact.suggestcontactsbymembership.tooltip" />
            </td>
            <td>
                <cms:LocalizedCheckBox ID="chkIPaddress" runat="server" ResourceString="om.contact.ipaddress" ToolTipResourceString="om.contact.suggestcontactsbyip.tooltip" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcSite" runat="server" Visible="false">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblSite" runat="server" EnableViewState="false" ResourceString="general.site"
                        DisplayColon="true" />
                </td>
                <td colspan="3">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowGlobal="true" />
                    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" runat="server" IsLiveSite="false" AutoPostBack="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
            </td>
            <td>
                <cms:LocalizedButton ID="btnFilter" runat="server" CssClass="ContentButton" ResourceString="general.show" />
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
    </table>
</asp:Panel>
