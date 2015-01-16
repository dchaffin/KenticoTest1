<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_EmailTemplates_Pages_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Email Templates - Template List"
    CodeFile="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/EmailTemplates/Controls/EmailTemplateList.ascx" TagName="EmailTemplateList" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>

<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="pnlSites" runat="server" CssClass="PageHeaderLine SiteHeaderLine">
        <cms:LocalizedLabel runat="server" ID="lblSites" EnableViewState="false" ResourceString="general.site"
            DisplayColon="true" />
        <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowGlobal="true" />
    </asp:Panel>
    <cms:HeaderActions ID="headerActions" runat="server" PanelCssClass="CMSEditMenu" />
    <asp:Panel ID="pnlUsers" runat="server" CssClass="PageContent">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:EmailTemplateList ID="emailTemplateListElem" runat="server" IsLiveSite="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
