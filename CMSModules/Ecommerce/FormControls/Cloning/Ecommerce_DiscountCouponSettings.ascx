<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ecommerce_DiscountCouponSettings.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_DiscountCouponSettings" %>
<table>
    <tr>
        <td>
            <cms:LocalizedLabel runat="server" ID="lblCode" ResourceString="discouncoupon_edit.discountcouponcodelabel" EnableViewState="false"
                DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox runat="server" ID="txtCode" runat="server" CssClass="TextBoxField" />
        </td>
    </tr>
</table>
