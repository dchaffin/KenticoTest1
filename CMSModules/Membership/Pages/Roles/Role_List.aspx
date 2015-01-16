<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Roles_Role_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Roles - Role List"
    CodeFile="Role_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/Controls/Roles/RoleList.ascx" TagName="RoleList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="pnlSites" runat="server" CssClass="PageHeaderLine SiteHeaderLine">
        <cms:LocalizedLabel runat="server" ID="lblSites" EnableViewState="false" ResourceString="general.site"
            DisplayColon="true" />
        <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowGlobal="true" />
    </asp:Panel>
    <cms:HeaderActions ID="headerActions" runat="server" PanelCssClass="CMSEditMenu" />
    <asp:Panel ID="pnlUsers" runat="server" CssClass="PageContent">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" ShowProgress="true">
            <ContentTemplate>
                <cms:RoleList ID="roleListElem" runat="server" IsLiveSite="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" />
</asp:Content>
