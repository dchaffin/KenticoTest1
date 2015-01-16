<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_OrderStatus_OrderStatus_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="E-commerce Configuration - Order status"
    CodeFile="OrderStatus_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" GridName="OrderStatus_List.xml" OrderBy="StatusOrder"
        IsLiveSite="false" Columns="StatusID,StatusDisplayName,StatusEnabled,StatusColor,StatusSendNotification,StatusOrderIsPaid" />
</asp:Content>
