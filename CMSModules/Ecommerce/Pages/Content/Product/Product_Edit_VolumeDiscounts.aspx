<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_VolumeDiscounts"
    Theme="Default" CodeFile="Product_Edit_VolumeDiscounts.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:PlaceHolder runat="server" ID="plcProductPrice">
        <table style="vertical-align: top;">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblProductPriceLabel" EnableViewState="false"
                        ResourceString="com_sku_volume_discount.skupricelabel" DisplayColon="true" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblProductPrice" EnableViewState="false"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
    </asp:PlaceHolder>
    <cms:UniGrid runat="server" ID="gridElem" GridName="Product_Edit_VolumeDiscount_List.xml"
        IsLiveSite="false" OrderBy="VolumeDiscountID" />
    <script type="text/javascript">
        //<![CDATA[
        // Refreshes current page when volume discount level properties are changed in modal dialog window
        function RefreshPage() {
            window.location.replace(window.location.href);
        }
        //]]>
    </script>
</asp:Content>
