<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_ChangeCurrency"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    CodeFile="StoreSettings_ChangeCurrency.aspx.cs" %>

<%@ Register Src="~/CMSModules/ECommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="PageContent">
        <table>
            <asp:PlaceHolder runat="server" ID="plcOldCurrency">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblOldMainLabel" runat="server" EnableViewState="false" ResourceString="StoreSettings_ChangeCurrency.OldMainCurrency" />&nbsp;
                    </td>
                    <td>
                        <asp:Label ID="lblOldMainCurrency" runat="server" EnableViewState="false" />&nbsp;
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblNewMainCurrency" runat="server" EnableViewState="false"
                        ResourceString="StoreSettings_ChangeCurrency.NewMainCurrency" />&nbsp;
                </td>
                <td>
                    <cms:CurrencySelector runat="server" ID="currencyElem" AddSiteDefaultCurrency="false"
                        ExcludeSiteDefaultCurrency="true" IsLiveSite="false" DisplayItems="Enabled" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcRecalculationDetails">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblExchangeRate" runat="server" EnableViewState="false" ResourceString="StoreSettings_ChangeCurrency.ExchangeRate" />&nbsp;
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtEchangeRate" runat="server" MaxLength="10" EnableViewState="false"
                            CssClass="EditableTextTextBox" />
                        <asp:Image ID="imgHelp" runat="server" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblRound" runat="server" EnableViewState="false" ResourceString="StoreSettings_ChangeCurrency.Round" />&nbsp;
                    </td>
                    <td>
                        <cms:CMSTextBox ID="txtRound" runat="server" EnableViewState="false" CssClass="EditableTextTextBox"
                            MaxLength="1" Text="2" />
                        <asp:Image ID="imgRoundHelp" runat="server" EnableViewState="false" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
        <asp:PlaceHolder runat="server" ID="plcObjectsSelection">
            <br />
            <table>
                <asp:PlaceHolder ID="plcRecalculateFromGlobal" runat="server">
                    <tr>
                        <td colspan="2">
                            <cms:LocalizedCheckBox ID="chkRecalculateFromGlobal" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeSiteFromGlobal"
                                Checked="true" EnableViewState="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkExchangeRates" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeExchangeRates"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkProductPrices" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeProductPrices"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkFlatTaxes" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeTaxes"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkFlatDiscountsCoupons" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeDiscountCoupons"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkFlatVolumeDiscounts" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeVolumeDiscounts"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkCredit" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeCredit"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkOrders" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeOrder"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkShipping" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeShipping"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcRecountDocuments" runat="server">
                    <tr>
                        <td colspan="2">
                            <cms:LocalizedCheckBox ID="chkDocuments" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeDocuments"
                                Checked="true" EnableViewState="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td colspan="2">
                        <cms:LocalizedCheckBox ID="chkFreeShipping" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeFreeShipping"
                            Checked="true" EnableViewState="false" />
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
    </div>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton ID="btnOk" runat="server" CssClass="SubmitButton" OnClick="btnOk_Click"
            EnableViewState="false" /><cms:CMSButton ID="btnCancel" runat="server" CssClass="SubmitButton"
                OnClientClick="return CloseDialog();" EnableViewState="false" />
    </div>
</asp:Content>
