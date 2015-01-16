<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Automation process – Contacts" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Contacts"
    Theme="Default" CodeFile="Tab_Contacts.aspx.cs" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/Contacts.ascx" TagName="Contacts" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx"
    TagName="ContactSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>

<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true" ResourceString="General.Site" />
    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="sg" runat="server" />
    <cms:SiteSelector ID="siteSelector" runat="server" ShortID="s" AllowGlobal="true" AllowAll="true" IsLiveSite="false" PostbackOnDropDownChange="true" />
</asp:Content>
<asp:Content ContentPlaceHolderID="plcBeforeActions" runat="server" ID="cntBeforeActions">
    <cms:ContactSelector ID="ucSelector" runat="server" Enabled="false" IsLiveSite="false" CssClass="FloatLeft Pad" />
    <cms:LocalizedLabel ID="lblWarnStart" runat="server" ResourceString="ma.chooseglobalorsitetostart" EnableViewState="false" Visible="false" CssClass="ActionsInfoLabel" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:Contacts ID="listContacts" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
