<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Product_Edit_Categories.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Categories"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Categories/Controls/MultipleCategoriesSelector.ascx"
    TagName="MultipleCategoriesSelector" TagPrefix="cms" %>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
    <strong>
        <cms:LocalizedLabel ID="lblTitle" runat="server" ResourceString="categories.documentassignedto"
            DisplayColon="true" EnableViewState="false" CssClass="InfoLabel" />
    </strong>
    <cms:MultipleCategoriesSelector ID="categoriesElem" runat="server" IsLiveSite="false" />
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
