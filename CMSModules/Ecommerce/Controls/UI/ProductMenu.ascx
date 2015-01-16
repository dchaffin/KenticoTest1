<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductMenu.ascx.cs" Inherits="CMSModules_Ecommerce_Controls_UI_ProductMenu" %>
<%@ Register Src="~/CMSModules/Content/Controls/LanguageMenu.ascx" TagName="LanguageMenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/Content/ContentMenu.ascx" TagName="ContentMenu"
    TagPrefix="cms" %>
<div class="ProductsMenu">
    <div class="ContentMenuGroup" id="contentMenu">
        <asp:Panel runat="server" ID="pnlLanguageMenu" CssClass="UniMenuContent ProductsLanguageMenu">
            <cms:LanguageMenu runat="server" ID="mnuLanguage" CompactMode="true" />
        </asp:Panel>
        <div class="UniMenuContent ProductsActionMenu">
            <cms:ContentMenu runat="server" ID="mnuContent" AllButtonsAreSmall="true" TooltipStringPrefix="com.productsection." />
        </div>
    </div>
</div>
