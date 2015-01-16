<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_SearchIndex_Sites" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Search Index - Sites List" CodeFile="SearchIndex_Sites.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    <asp:PlaceHolder ID="plcTable" runat="server">
    <asp:Label ID="lblAvialable" runat="server" CssClass="BoldInfoLabel" EnableViewState="false" />
        <cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
            SelectionMode="Multiple" ResourcePrefix="sitesselect" />
    </asp:PlaceHolder>
</asp:Content>
