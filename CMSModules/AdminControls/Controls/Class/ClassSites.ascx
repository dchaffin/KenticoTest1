<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_ClassSites"
    CodeFile="ClassSites.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Label ID="lblAvialable" runat="server" CssClass="BoldInfoLabel" EnableViewState="false" />
<cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
    SelectionMode="Multiple" ResourcePrefix="sitesselect" />
