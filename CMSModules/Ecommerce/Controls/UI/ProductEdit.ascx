<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductEdit.ascx.cs" Inherits="CMSModules_Ecommerce_Controls_UI_ProductEdit" %>
<%@ Register TagPrefix="cms" Assembly="CMS.UIControls" Namespace="CMS.UIControls" %>
<%@ Register TagPrefix="cms" Assembly="CMS.ExtendedControls" Namespace="CMS.ExtendedControls" %>
<%@ Register TagPrefix="cms" Assembly="CMS.FormControls" Namespace="CMS.FormControls" %>
<%@ Register TagPrefix="cms" TagName="EditMenuUC" Src="~/CMSModules/Content/Controls/EditMenu.ascx" %>
<%@ Register TagPrefix="cms" TagName="HeaderActionsUC" Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" %>
<%@ Register TagPrefix="cms" TagName="QuickNavigation" Src="~/CMSModules/Ecommerce/Controls/UI/QuickNavigation.ascx" %>
<%@ Register TagPrefix="cms" TagName="SelectSKUBinding" Src="~/CMSModules/Ecommerce/FormControls/SelectSKUBinding.ascx" %>
<%-- Header --%>
<div id="CMSHeaderDiv">
    <asp:PlaceHolder ID="plcHeaderActions" runat="server">
        <%-- Edit menu --%>
        <cms:EditMenuUC ID="editMenuElem" runat="server" ShortID="em" Visible="false" StopProcessing="true" />
        <%-- Header actions --%>
        <asp:Panel ID="pnlHeaderActions" runat="server" CssClass="CMSEditMenu" Visible="false">
            <cms:HeaderActionsUC ID="headerActionsElem" runat="server" LinkCssClass="MenuItemEdit"
                RenderSeparator="false" StopProcessing="true" />
        </asp:Panel>
    </asp:PlaceHolder>
    <%-- CK toolbar --%>
    <div id="CKToolbar">
    </div>
</div>
<%-- Quick navigation --%>
<cms:QuickNavigation ID="quickNavigationElem" runat="server" ShortID="q" CssClass="QuickNavigation" />
<%-- Forms --%>
<cms:CMSPanel ID="pnlForms" runat="server" CssClass="ProductEditForms" ShortID="pf">
    <cms:CMSPanel ID="pnlFormsInner" runat="server" ShortID="pfi">
        <%-- Messages --%>
        <cms:CMSPanel ID="pnlMessages" runat="server" CssClass="MessagesWrapper">
            <cms:MessagesPlaceHolder ID="plcMessages" runat="server" WrapperControlID="pnlMessages" />
        </cms:CMSPanel>
        <%-- SKU binding - create --%>
        <cms:CMSPanel ID="pnlCreateSkuBinding" runat="server" ShortID="psb" CssClass="FormPanel EditingFormFieldSet"
            Visible="false">
            <cms:SelectSKUBinding ID="selectSkuBindingElem" runat="server" ShortID="sb" />
        </cms:CMSPanel>
        <span class="ClearBoth"></span>
        <%-- General SKU properties --%>
        <cms:BasicForm ID="skuGeneralForm" runat="server" ShortID="g" HtmlAreaToolbarLocation="Out:CKToolbar"
            DefaultFormLayout="FieldSets" SetDefaultValuesToDisabledFields="false" />
        <span class="ClearBoth"></span>
        <%-- Membership SKU properties --%>
        <cms:BasicForm ID="skuMembershipForm" runat="server" ShortID="m" HtmlAreaToolbarLocation="Out:CKToolbar"
            DefaultFormLayout="FieldSets" SetDefaultValuesToDisabledFields="false" Visible="false" />
        <%-- E-product SKU properties --%>
        <cms:BasicForm ID="skuEproductForm" runat="server" ShortID="e" HtmlAreaToolbarLocation="Out:CKToolbar"
            DefaultFormLayout="FieldSets" SetDefaultValuesToDisabledFields="false" Visible="false" />
        <%-- Donation SKU properties --%>
        <cms:BasicForm ID="skuDonationForm" runat="server" ShortID="d" HtmlAreaToolbarLocation="Out:CKToolbar"
            DefaultFormLayout="FieldSets" SetDefaultValuesToDisabledFields="false" Visible="false" />
        <%-- Bundle SKU properties --%>
        <cms:BasicForm ID="skuBundleForm" runat="server" ShortID="b" HtmlAreaToolbarLocation="Out:CKToolbar"
            DefaultFormLayout="FieldSets" SetDefaultValuesToDisabledFields="false" Visible="false" />
        <span class="ClearBoth"></span>
        <%-- Custom properties --%>
        <cms:CMSPanel ID="pnlCustomProperties" runat="server" ShortID="cp" CssClass="FormPanel EditingFormFieldSet"
            Visible="false">
            <%-- Custom SKU properties --%>
            <cms:BasicForm ID="skuCustomForm" runat="server" ShortID="cf" HtmlAreaToolbarLocation="Out:CKToolbar"
                DefaultFormLayout="Standard" SetDefaultValuesToDisabledFields="false" Visible="false" />
            <span class="ClearBoth"></span>
            <%-- Custom document properties --%>
            <cms:CMSForm ID="documentForm" runat="server" ShortID="df" HtmlAreaToolbarLocation="Out:CKToolbar"
                SetDefaultValuesToDisabledFields="false" Visible="false" />
        </cms:CMSPanel>
        <span class="ClearBoth"></span>
        <%-- Other SKU properties --%>
        <cms:BasicForm ID="skuOtherForm" runat="server" ShortID="of" HtmlAreaToolbarLocation="Out:CKToolbar"
            DefaultFormLayout="FieldSets" SetDefaultValuesToDisabledFields="false" />
        <span class="ClearBoth"></span>
        <%-- SKU binding - remove --%>
        <cms:CMSPanel ID="pnlRemoveSkuBinding" runat="server" ShortID="rsb" CssClass="FormPanel EditingFormFieldSet"
            Visible="false">
            <asp:LinkButton ID="btnRemoveSkuBinding" runat="server" CssClass="MenuItemEdit" EnableViewState="false">
                <cms:CMSImage ID="imgRemoveSkuBinding" runat="server" />
                <cms:LocalizedLabel runat="server" ResourceString="com.skubinding.unbind" />
            </asp:LinkButton>
        </cms:CMSPanel>
    </cms:CMSPanel>
</cms:CMSPanel>
