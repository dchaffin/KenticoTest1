<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Development_Module_Edit_Sites"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Module Edit - Sites"
    CodeFile="Module_Edit_Sites.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Label ID="lblAvialable" runat="server" CssClass="BoldInfoLabel" EnableViewState="false" />
    <cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
        SelectionMode="Multiple" ResourcePrefix="sitesselect" />
</asp:Content>
