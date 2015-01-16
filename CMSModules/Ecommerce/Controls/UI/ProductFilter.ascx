<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductFilter.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_UI_ProductFilter" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/PublicStatusSelector.ascx"
    TagName="PublicStatusSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/InternalStatusSelector.ascx"
    TagName="InternalStatusSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/ManufacturerSelector.ascx"
    TagName="ManufacturerSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/SupplierSelector.ascx" TagName="SupplierSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/DepartmentSelector.ascx" TagName="DepartmentSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="SelectProductType" Src="~/CMSModules/Ecommerce/FormControls/SelectProductType.ascx" %>
<table>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblName" runat="server" CssClass="ContentLabel" EnableViewState="false"
                ResourceString="com.sku.ProductNameOrNumber" DisplayColon="true" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtNameOrNumber" runat="server" CssClass="TextBoxField" MaxLength="450"
                EnableViewState="false" />
        </td>
    </tr>
    <asp:PlaceHolder ID="plcSite" runat="server">
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSite" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    ResourceString="com.sku.ProductsSite" DisplayColon="true" />
            </td>
            <td>
                <cms:SiteSelector runat="server" ID="siteElem" IsLiveSite="false" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblDepartment" runat="server" CssClass="ContentLabel" EnableViewState="false"
                ResourceString="com.sku.departmentid" DisplayColon="true" />
        </td>
        <td>
            <cms:DepartmentSelector runat="server" ID="departmentElem" AddAllMyRecord="true"
                AddAllItemsRecord="false" IsLiveSite="false" AddNoneRecord="false" AddWithoutDepartmentRecord="true"
                UseDepartmentNameForSelection="false" />
        </td>
    </tr>
    <asp:PlaceHolder ID="plcAdvancedFilterType" runat="server">
        <asp:PlaceHolder ID="plcAdvancedDocumentType" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblProperties" runat="server" ResourceString="com.sku.properties"
                        DisplayColon="true" />
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="drpDocTypes" CssClass="DropDownField" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblType" runat="server" ResourceString="com.sku.producttype"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:SelectProductType runat="server" ID="selectProductTypeElem" AllowAll="true"
                    AllItemResourceString="General.SelectAnything" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcAdvancedFilterGeneral" runat="server">
        <tr>
            <td colspan="2" style="padding-top: 10px;">
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblManufacturer" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    ResourceString="com.sku.manufacturerid" DisplayColon="true" />
            </td>
            <td>
                <cms:ManufacturerSelector ID="manufacturerElem" runat="server" AddAllItemsRecord="true"
                    AddNoneRecord="true" UseStatusNameForSelection="false" DisplayOnlyEnabled="false" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSupplier" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    ResourceString="com.sku.supplierid" DisplayColon="true" />
            </td>
            <td>
                <cms:SupplierSelector ID="supplierElem" runat="server" AddAllItemsRecord="true" AddNoneRecord="true"
                    UseStatusNameForSelection="false" DisplayOnlyEnabled="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="padding-top: 10px;">
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblStoreStatus" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    ResourceString="com.sku.publicstatusid" DisplayColon="true" />
            </td>
            <td>
                <cms:PublicStatusSelector runat="server" ID="publicStatusElem" AddAllItemsRecord="true"
                    AddNoneRecord="true" IsLiveSite="false" UseStatusNameForSelection="false" DisplayOnlyEnabled="false" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblInternalStatus" runat="server" CssClass="ContentLabel"
                    EnableViewState="false" ResourceString="com.sku.internalstatusid" DisplayColon="true" />
            </td>
            <td>
                <cms:InternalStatusSelector runat="server" ID="internalStatusElem" AddAllItemsRecord="true"
                    AddNoneRecord="true" UseStatusNameForSelection="false" DisplayOnlyEnabled="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="padding-top: 10px;">
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblAllowForSale" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    ResourceString="com.sku.enabled" DisplayColon="true" />
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlAllowForSale" CssClass="ShortDropDownList" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblNeedsShipping" runat="server" CssClass="ContentLabel"
                    EnableViewState="false" ResourceString="com.sku.needsshipping" DisplayColon="true" />
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlNeedsShipping" CssClass="ShortDropDownList" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="padding-top: 10px;">
            </td>
        </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcSubsectionProducts" runat="server">
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblShowAllChildren" runat="server" CssClass="ContentLabel"
                    EnableViewState="false" ResourceString="com.sku.fromsubsections" DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkShowAllChildren" runat="server" CssClass="ContentLabel" Checked="true"
                    TextAlign="Left" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td>
        </td>
        <td>
            <cms:CMSButton ID="btnFilter" runat="server" CssClass="ContentButton" EnableViewState="false" />
            <asp:LinkButton ID="btnReset" runat="server" Style="line-height: 2em; margin: 1em" EnableViewState="false" />
        </td>
    </tr>
</table>
<asp:PlaceHolder ID="plcAdvancedFilter" runat="server">
    <div>
        <asp:Image runat="server" ID="imgShowSimpleFilter" CssClass="NewItemImage" />
        <asp:LinkButton ID="lnkShowSimpleFilter" runat="server" OnClick="lnkShowSimpleFilter_Click" />
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcSimpleFilter" runat="server">
    <div>
        <asp:Image runat="server" ID="imgShowAdvancedFilter" CssClass="NewItemImage" />
        <asp:LinkButton ID="lnkShowAdvancedFilter" runat="server" OnClick="lnkShowAdvancedFilter_Click" />
    </div>
</asp:PlaceHolder>
<br />
