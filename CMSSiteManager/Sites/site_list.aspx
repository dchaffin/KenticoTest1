<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSSiteManager_Sites_site_list"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="CMSSiteManager - Sites"
    CodeFile="site_list.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:unigrid id="UniGridSites" shortid="g" runat="server" gridname="site_list.xml"
        orderby="SiteDisplayName" islivesite="false" objecttype="cms.site" />
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
