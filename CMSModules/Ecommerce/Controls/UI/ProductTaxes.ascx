<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_ProductTaxes"
    CodeFile="ProductTaxes.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<strong>
    <asp:Label runat="server" ID="lblSiteTitle" CssClass="InfoLabel" EnableViewState="false" /></strong>
<cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="ecommerce.taxclass"
    SelectionMode="Multiple" ResourcePrefix="taxselect" />
