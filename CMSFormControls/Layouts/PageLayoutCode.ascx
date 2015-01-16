<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PageLayoutCode.ascx.cs"
    Inherits="CMSFormControls_Layouts_PageLayoutCode" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Layouts/CSSStylesEditor.ascx" TagName="CSSStylesEditor"
    TagPrefix="cms" %>
<div class="LayoutCodeControl">
    <div class="LayoutControlHeaderInfo" style="padding-top: 5px; padding-bottom: 5px;">
        <asp:Panel runat="server" ID="pnl" CssClass="FloatLeft">
            <cms:LocalizedLabel runat="server" ID="LocalizedLabel1" ResourceString="PageLayout.LayoutElement"
                DisplayColon="true" />
            <asp:DropDownList CssClass="DropDownFieldSmall" runat="server" ID="drpElements">
            </asp:DropDownList>
            <cms:CMSButton Style="margin-top: 0px;" CssClass="ContentButton" runat="server" ID="btn" />
        </asp:Panel>
        <asp:Panel ID="pnlType" CssClass="FloatRight" runat="server">
            <cms:LocalizedLabel runat="server" ID="lblType" ResourceString="PageLayout.Type"
                DisplayColon="true" Width="80px" />&nbsp;<asp:DropDownList runat="server" ID="drpType"
                    OnSelectedIndexChanged="drpType_selectedIndexChanged" AutoPostBack="true" />
        </asp:Panel>
        <div class="ClearBoth">
        </div>
    </div>
    <asp:Panel runat="server" ID="plcDirectives" CssClass="NORTL CodeDirectives">
        <asp:Label runat="server" ID="ltlDirectives" EnableViewState="false" />
    </asp:Panel>
    <cms:MacroEditor ID="tbLayoutCode" runat="server" Width="98%" Height="300px" />
</div>
