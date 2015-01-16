<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Development_DocumentTypes_DocumentType_Edit_Ecommerce"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Document Type Edit - Ecommerce"
    CodeFile="DocumentType_Edit_Ecommerce.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/FormControls/DepartmentSelector.ascx" TagName="DepartmentSelector"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="SelectProductType" Src="~/CMSModules/Ecommerce/FormControls/SelectProductType.ascx" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PropertiesPanel">
        <cms:CMSPanel runat="server" ID="pnlProductSection" ShortID="pp">
            <cms:LocalizedCheckBox runat="server" ID="chkIsProduct" ResourceString="DocType.Ecommerce.IsProduct"
                CssClass="InfoLabel" AutoPostBack="true" />
            <cms:LocalizedCheckBox runat="server" ID="chkIsProductSection" ResourceString="DocType.Ecommerce.IsProductSection"
                CssClass="InfoLabel" />
        </cms:CMSPanel>
        <br />
        <cms:CMSPanel runat="server" ID="pnlProductTypeProperties" ShortID="pr">
            <cms:CMSPanel runat="server" ID="pnlFieldsMappings" ShortID="p">
                <table>
                    <tr>
                        <td colspan="2">
                            <cms:LocalizedLabel runat="server" ID="lblMappingWarning" CssClass="InfoLabel" EnableViewState="false"
                                ResourceString="DocType.Ecommerce.mappingwarning" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cms:LocalizedLabel runat="server" ID="lblTitle" CssClass="InfoLabel" EnableViewState="false"
                                ResourceString="DocType.Ecommerce.lblTitle" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel" style="width: 175px;">
                            <cms:LocalizedLabel runat="server" ID="lblGeneral" ResourceString="general.general"
                                Font-Bold="true" />
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblName" EnableViewState="false" ResourceString="DocType.Ecommerce.lblName"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpName" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblPrice" EnableViewState="false" ResourceString="DocType.Ecommerce.lblPrice"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpPrice" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblShortDescription" EnableViewState="false"
                                ResourceString="DocType.Ecommerce.lblShortDescription" DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpShortDescription" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblDescription" EnableViewState="false" ResourceString="DocType.Ecommerce.lblDescription"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpDescription" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblImage" EnableViewState="false" ResourceString="DocType.Ecommerce.lblImage"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpImage" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                            <cms:LocalizedLabel runat="server" ID="lblDimensions" ResourceString="com.product.dimensions"
                                Font-Bold="true" />
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblWeight" EnableViewState="false" ResourceString="DocType.Ecommerce.lblWeight"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpWeight" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblHeight" EnableViewState="false" ResourceString="DocType.Ecommerce.lblHeight"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpHeight" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblWidth" EnableViewState="false" ResourceString="DocType.Ecommerce.lblWidth"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpWidth" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblDepth" EnableViewState="false" ResourceString="DocType.Ecommerce.lblDepth"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpDepth" runat="server" CssClass="DropDownField" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:PlaceHolder runat="server" ID="plcCustom">
                    <asp:Panel runat="server" ID="pnlCustomProperties">
                    </asp:Panel>
                </asp:PlaceHolder>
            </cms:CMSPanel>
            <br />
            <cms:CMSPanel runat="server" ID="pnlAutoCreation" ShortID="pa">
                <table>
                    <tr>
                        <td class="FieldLabel" style="width: 175px;">
                            <cms:LocalizedLabel runat="server" ID="lblDepartments" EnableViewState="false" ResourceString="DocType.Ecommerce.lblDepartments" />
                        </td>
                        <td>
                            <cms:DepartmentSelector runat="server" ID="departmentElem" DropDownListMode="false"
                                UseDepartmentNameForSelection="false" ShowAllSites="true" AddNoneRecord="true"
                                IsLiveSite="false" />
                        </td>
                    </tr>
                    <%-- Default product type --%>
                    <asp:PlaceHolder runat="server" ID="plcDefaultProductType">
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel ID="LocalizedLabel1" runat="server" ResourceString="doctype.ecommerce.defaultproducttype"
                                    EnableViewState="false" DisplayColon="true" />
                            </td>
                            <td>
                                <cms:SelectProductType runat="server" ID="productTypeElem" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </table>
                <br />
                <cms:LocalizedCheckBox runat="server" ID="chkGenerateSKU" ResourceString="DocType.Ecommerce.lblGenerateSKU"
                    CssClass="InfoLabel" />
            </cms:CMSPanel>
        </cms:CMSPanel>
        <br />
        <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
    </div>
    <asp:Literal ID="ltlScrpt" runat="server" EnableViewState="false" />
</asp:Content>
