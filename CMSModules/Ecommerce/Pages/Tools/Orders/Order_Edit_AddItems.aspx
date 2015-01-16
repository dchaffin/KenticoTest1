<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_AddItems"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Ecommerce - Add order item" CodeFile="Order_Edit_AddItems.aspx.cs" %>

<%@ Import Namespace="CMS.Ecommerce" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/Controls/ProductOptions/ShoppingCartItemSelector.ascx"
    TagName="ShoppingCartItemSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/DepartmentSelector.ascx" TagName="DepartmentSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        function AddProducts(productIDs, quantities, options, price, isPrivate) {
            wopener.AddProduct(productIDs, quantities, options, price, isPrivate);
            CloseDialog();
            return false;
        }
        function RefreshCart() {
            wopener.RefreshCart();
            CloseDialog();
            return false;
        }
        //]]>
    </script>
    <div class="PageContent">
        <%-- Products --%>
        <asp:PlaceHolder ID="plcProducts" runat="server">
            <asp:Panel ID="PanelNewItem" runat="server">
                <table>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel ID="lblProductName" runat="server" EnableViewState="false" ResourceString="Order_Edit_AddItems.ProductName" />
                        </td>
                        <td>
                            <cms:CMSTextBox ID="txtProductName" runat="server" CssClass="TextBoxField" MaxLength="450"
                                EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel ID="lblProductCode" runat="server" EnableViewState="false" ResourceString="Order_Edit_AddItems.ProductCode" />
                        </td>
                        <td>
                            <cms:CMSTextBox ID="txtProductCode" runat="server" CssClass="TextBoxField" MaxLength="200"
                                EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel" style="padding-top: 5px; vertical-align: top;">
                            <cms:LocalizedLabel ID="lblDepartment" runat="server" EnableViewState="false" ResourceString="Order_Edit_AddItems.Department" />
                        </td>
                        <td>
                            <cms:DepartmentSelector runat="server" ID="departmentElem" AddAllItemsRecord="true"
                                AddNoneRecord="false" UseDepartmentNameForSelection="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <cms:CMSButton ID="btnShow" runat="server" CssClass="ContentButton" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <cms:UniGrid runat="server" ID="gridProducts" ShortID="g" OrderBy="SKUName" IsLiveSite="false"
                ObjectType="ecommerce.sku" RememberState="false">
                <GridColumns>
                    <ug:Column Source="##ALL##" ExternalSourceName="SKUName" Caption="$Order_Edit_AddItems.GridProductName$"
                        Sort="SKUName" Width="100%" Wrap="false" Style="text-align: left;">
                    </ug:Column>
                    <ug:Column Source="SKUNumber" Caption="$Order_Edit_AddItems.GridProductCode$" Wrap="false"
                        Style="text-align: left;" />
                    <ug:Column Source="##ALL##" ExternalSourceName="Price" Caption="$Order_Edit_AddItems.GridUnitPrice$"
                        Wrap="false" Style="text-align: right;" Sort="SKUPrice" />
                    <ug:Column Source="SKUID" ExternalSourceName="Quantity" Caption="$Order_Edit_AddItems.GridQuantity$"
                        Wrap="false" Style="text-align: center;" AllowSorting="false" />
                </GridColumns>
                <PagerConfig DefaultPageSize="10" />
            </cms:UniGrid>
        </asp:PlaceHolder>
        <%-- Shopping cart item selector --%>
        <asp:PlaceHolder ID="plcSelector" runat="server">
            <div style="padding: 10px;">
                <asp:Label runat="server" ID="lblSKUName" EnableViewState="false" CssClass="BoldInfoLabel" />
                <asp:Label runat="server" ID="lblPrice" EnableViewState="false" />&nbsp;&nbsp;
                <asp:Label runat="server" ID="lblPriceValue" EnableViewState="false" />
                <br />
                <br />
                <cms:LocalizedLabel runat="server" ID="lblTitle" CssClass="InfoLabel" />
                <cms:ShoppingCartItemSelector ID="cartItemSelector" runat="server" ShowProductOptions="true"
                    ShowDonationProperties="true" ShowUnitsTextBox="true" ShowTotalPrice="true" DialogMode="True"
                    IsLiveSite="false" />
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <asp:PlaceHolder ID="plcFooter" runat="server">
        <div class="FloatRight">
            <cms:CMSButton ID="btnAdd" runat="server" OnClick="BtnAdd_Click" CssClass="SubmitButton"
                EnableViewState="false" Enabled="false" /><cms:LocalizedButton ID="btnCancel" runat="server"
                    ResourceString="general.cancel" CssClass="SubmitButton" OnClientClick="return CloseDialog();"
                    EnableViewState="False" />
        </div>
    </asp:PlaceHolder>
</asp:Content>
