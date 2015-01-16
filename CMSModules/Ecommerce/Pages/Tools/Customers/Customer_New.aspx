<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Customers_Customer_New" Theme="Default"
    Title="Customer - New" CodeFile="Customer_New.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="CustomerNew" Src="~/CMSModules/Ecommerce/Controls/UI/CustomerNew.ascx" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CustomerNew runat="server" ID="ucCustomerNew" IsLiveSite="false" />
    <br />
    <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false" />
</asp:Content>
