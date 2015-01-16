<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_General"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Customer properties"
    CodeFile="Customer_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/Customers/CustomerEdit.ascx"
    TagName="CustomerEdit" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CustomerEdit ID="customerEdit" runat="server" />
</asp:Content>
