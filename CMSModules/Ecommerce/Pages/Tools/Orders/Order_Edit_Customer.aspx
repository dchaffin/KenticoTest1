<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Customer"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Order - edit customer" CodeFile="Order_Edit_Customer.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/Customers/CustomerEdit.ascx"
    TagName="CustomerEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:CustomerEdit ID="customerEdit" runat="server" AllowEditUser="false" HideOKButton="true" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton ID="btnOk" CssClass="SubmitButton" OnClick="btnOK_Click" ValidationGroup="Address"
            runat="server" EnableViewState="false" /><cms:CMSButton ID="btnCancel" CssClass="SubmitButton"
                runat="server" OnClientClick="return CloseDialog();" EnableViewState="false" />
    </div>
</asp:Content>
