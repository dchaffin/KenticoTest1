<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderFilter.ascx.cs" Inherits="CMSModules_Ecommerce_Controls_Filters_OrderFilter" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/OrderStatusSelector.ascx" TagName="OrderStatusSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/ShippingSelector.ascx" TagName="ShippingSelector"
    TagPrefix="cms" %>
<table>
    <%-- Site --%>
    <asp:PlaceHolder ID="plcSiteFilter" runat="server">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblSites" runat="server" EnableViewState="false" ResourceString="OrderList.SitesLabel" />
            </td>
            <td>
                <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" PostbackOnDropDownChange="true"
                    AllowSetSpecialFields="true" AllowAll="true" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <%-- Order ID or Invoice number --%>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblOrderID" runat="server" EnableViewState="false" ResourceString="OrderList.OrderIdLabel" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtOrderId" runat="server" CssClass="TextBoxField" MaxLength="20"
                EnableViewState="false" />
        </td>
    </tr>
    <%-- Customer name or company --%>
    <asp:PlaceHolder ID="plcCustomerFilter" runat="server">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblCustomerNameOrCompany" runat="server" EnableViewState="false"
                    ResourceString="OrderList.CustomerLastName" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtCustomerNameOrCompany" runat="server" CssClass="TextBoxField"
                    EnableViewState="false" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <%-- Status --%>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblStatus" runat="server" EnableViewState="false" ResourceString="OrderList.StatusLabel" />
        </td>
        <td>
            <cms:OrderStatusSelector runat="server" AddAllItemsRecord="true" ID="statusSelector"
                UseStatusNameForSelection="false" IsLiveSite="false" DisplayOnlyEnabled="false" />
        </td>
    </tr>
    <%-- Order is paid --%>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblIsPaid" runat="server" ResourceString="com.orderlist.orderispaid"
                DisplayColon="true" EnableViewState="false" />
        </td>
        <td>
            <cms:LocalizedDropDownList runat="server" ID="drpOrderIsPaid" UseResourceStrings="true"
                CssClass="DropDownField" EnableViewState="false" />
        </td>
    </tr>
    <asp:PlaceHolder ID="plcAdvancedGroup" runat="server">
        <%-- Created from --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblCreatedFrom" runat="server" ResourceString="com.orderlist.createdfrom"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:DateTimePicker ID="dtpFrom" runat="server" EnableViewState="false" DisplayNow="false" />
            </td>
        </tr>
        <%-- Created to --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblCreatedTo" runat="server" ResourceString="com.orderlist.createdto"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:DateTimePicker ID="dtpCreatedTo" runat="server" DisplayNow="false" />
            </td>
        </tr>
        <%-- Currency --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblCurrency" runat="server" ResourceString="com.orderlist.currency"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:CurrencySelector runat="server" ID="CurrencySelector" CssClass="DropDownField"
                    AddAllItemsRecord="true" DisplayOnlyEnabled="false" DisplayOnlyWithExchangeRate="false" />
            </td>
        </tr>
        <%-- Payment method --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblPaymentMethod" runat="server" ResourceString="com.orderlist.paymentmethod"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:PaymentSelector runat="server" ID="PaymentSelector" CssClass="DropDownField"
                    AddAllItemsRecord="true" DisplayOnlyEnabled="false" />
            </td>
        </tr>
        <%-- Shipping options --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblShippingOptions" runat="server" ResourceString="com.orderlist.shippingoptions"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:ShippingSelector runat="server" ID="ShippingSelector" DisplayOnlyEnabled="false"
                    AddAllItemsRecord="true" CssClass="DropDownField" />
            </td>
        </tr>
        <%-- Tracking number --%>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblTrackingNumber" runat="server" ResourceString="com.orderlist.trackingnumber"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtTrackingNumber" runat="server" CssClass="TextBoxField" EnableViewState="false" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td>
        </td>
        <td>
            <cms:CMSButton ID="btnFilter" runat="server" CssClass="ContentButton" EnableViewState="false" />
            <asp:LinkButton ID="btnReset" runat="server" Style="line-height: 2em; margin-left: 1em"
                EnableViewState="false" />
        </td>
    </tr>
</table>
<asp:PlaceHolder ID="plcAdvancedFilter" runat="server">
    <div>
        <asp:Image runat="server" ID="imgArrowUp" CssClass="NewItemImage" /><cms:LocalizedLinkButton
            ID="btnAdvancedFilter" runat="server" EnableViewState="false" ResourceString="general.displayadvancedfilter"
            OnClick="btnAdvancedFilter_Click" />
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcSimpleFilter" runat="server">
    <div>
        <asp:Image runat="server" ID="imgArrowDown" CssClass="NewItemImage" /><cms:LocalizedLinkButton
            ID="btnSimpleFilter" runat="server" EnableViewState="false" ResourceString="general.displaysimplefilter"
            OnClick="btnAdvancedFilter_Click" />
    </div>
</asp:PlaceHolder>
<br />
