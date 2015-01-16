<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectSKUBinding.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_SelectSKUBinding" %>
<%@ Register TagPrefix="cms" TagName="SKUSelector" Src="~/CMSModules/Ecommerce/FormControls/SKUSelector.ascx" %>
<div class="EditingFormLabel">
    <%--Info text--%>
    <cms:LocalizedLabel ID="lblInfo" runat="server" Visible="false" CssClass="InfoLabel"></cms:LocalizedLabel>
    <%-- Create new --%>
    <div>
        <cms:LocalizedRadioButton ID="radCreateNew" runat="server" ResourceString="com.selectskubinding.createnew"
            GroupName="SelectSKUBindingRadioGroup" Checked="true" AutoPostBack="true" />
    </div>
    <%-- Create new global --%>
    <asp:Panel ID="pnlCreateNewGlobal" runat="server" Visible="false">
        <cms:LocalizedRadioButton ID="radCreateNewGlobal" runat="server" ResourceString="com.selectskubinding.createnewglobal"
            GroupName="SelectSKUBindingRadioGroup" AutoPostBack="true" />
    </asp:Panel>
    <%-- Use existing --%>
    <div>
        <cms:LocalizedRadioButton ID="radUseExisting" runat="server" ResourceString="com.selectskubinding.useexisting"
            GroupName="SelectSKUBindingRadioGroup" Visible="false" AutoPostBack="true" />
    </div>
    <asp:Panel ID="pnlSkuSelector" runat="server" Style="margin: 8px 0 6px 20px;" Visible="false">
        <cms:SKUSelector runat="server" ID="skuSelectorElem" IsLiveSite="false" />
    </asp:Panel>
    <%-- Do not create --%>
    <asp:Panel ID="pnlDoNotCreate" runat="server" Visible="false">
        <cms:LocalizedRadioButton ID="radDoNotCreate" runat="server" ResourceString="com.selectskubinding.donotcreate"
            GroupName="SelectSKUBindingRadioGroup" AutoPostBack="true" />
    </asp:Panel>
</div>
