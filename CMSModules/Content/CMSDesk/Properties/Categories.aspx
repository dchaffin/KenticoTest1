<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Categories"
    Theme="Default" CodeFile="Categories.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Categories/Controls/MultipleCategoriesSelector.ascx"
    TagName="MultipleCategoriesSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" ShowSave="false" HelpTopicName="category" IsLiveSite="false" />
    <cms:CMSDocumentPanel ID="pnlDocInfo" runat="server" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server" CssClass="PropertiesPanel">
        <strong>
            <cms:LocalizedLabel ID="lblTitle" runat="server" ResourceString="categories.documentassignedto"
                DisplayColon="true" EnableViewState="false" CssClass="InfoLabel" />
        </strong>
        <cms:MultipleCategoriesSelector ID="categoriesElem" runat="server" IsLiveSite="false" />
    </asp:Panel>
    <div class="Clear">
    </div>
</asp:Content>
