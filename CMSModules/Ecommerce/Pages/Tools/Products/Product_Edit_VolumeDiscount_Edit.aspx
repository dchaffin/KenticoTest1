<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_VolumeDiscount_Edit"
    Theme="Default" Title="Product edit - volume discount edit" CodeFile="Product_Edit_VolumeDiscount_Edit.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <table style="vertical-align: top">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblProductPriceLabel" EnableViewState="false"
                    ResourceString="com_sku_volume_discount.skupricelabel" DisplayColon="true" />
            </td>
            <td>
                <asp:Label runat="server" ID="lblProductPrice" EnableViewState="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel" style="height: 32px">
                <asp:Label runat="server" ID="lblVolumeDiscountMinCount" EnableViewState="false" />
            </td>
            <td style="height: 32px">
                <cms:CMSTextBox ID="txtVolumeDiscountMinCount" runat="server" CssClass="TextBoxField"
                    MaxLength="9" EnableViewState="false" />
                &nbsp;&nbsp; <span runat="server" id="spanMinCount">
                    <cms:CMSRequiredFieldValidator ID="rfvMinCount" runat="server" ControlToValidate="txtVolumeDiscountMinCount"
                        Display="Dynamic" EnableViewState="false"></cms:CMSRequiredFieldValidator>
                    <cms:CMSRangeValidator ID="rvMinCount" runat="server" ControlToValidate="txtVolumeDiscountMinCount"
                        MinimumValue="1" Type="Integer" Display="Dynamic" EnableViewState="false"></cms:CMSRangeValidator>
                </span>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label runat="server" ID="lblVolumeDiscountValue" EnableViewState="false" />
            </td>
            <td>
                <asp:RadioButton ID="radDiscountRelative" runat="server" GroupName="group1" EnableViewState="false" />
                <asp:RadioButton ID="radDiscountAbsolute" runat="server" GroupName="group1" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
            </td>
            <td>
                <cms:CMSTextBox ID="txtVolumeDiscountValue" runat="server" CssClass="TextBoxField"
                    MaxLength="9" EnableViewState="false" />&nbsp;<asp:Label ID="lblCurrency" runat="server" />&nbsp;&nbsp;<span
                        runat="server" id="spanValue">
                        <cms:CMSRequiredFieldValidator ID="rfvDiscountValue" runat="server" ControlToValidate="txtVolumeDiscountValue"
                            Display="Dynamic" EnableViewState="false"></cms:CMSRequiredFieldValidator>
                        <cms:CMSRangeValidator ID="rvDiscountValue" runat="server" ControlToValidate="txtVolumeDiscountValue"
                            Type="Double" Display="Dynamic" MinimumValue="0" EnableViewState="false"></cms:CMSRangeValidator>
                    </span>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
            </td>
        </tr>
    </table>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
