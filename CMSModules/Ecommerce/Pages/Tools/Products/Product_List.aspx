<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_List" Theme="Default"
    MaintainScrollPositionOnPostback="true" Title="Product list" CodeFile="Product_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/DocumentList.ascx" TagName="DocumentList"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:PlaceHolder runat="server" ID="plcSKUListing">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:UniGrid ID="gridData" runat="server" ShortID="g" GridName="Product_List.xml"
                    OrderBy="SKUName" IsLiveSite="false" Columns="SKUID, SKUName, SKUNumber, SKUPrice, SKUAvailableItems, SKUEnabled, SKUSiteID, SKUReorderAt, SKUPublicStatusID" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcDocumentListing">
        <asp:Panel ID="pnlListingInfo" runat="server" EnableViewState="false">
            <asp:Label ID="lblListingInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
        </asp:Panel>
        <cms:DocumentList runat="server" ID="docList" ShowAllLevels="true" ShowDocumentTypeIcon="true"
            DocumentNameAsLink="false" ShowDocumentTypeIconTooltip="true" />
    </asp:PlaceHolder>
</asp:Content>
