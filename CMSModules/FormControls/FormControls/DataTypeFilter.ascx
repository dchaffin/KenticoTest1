<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DataTypeFilter.ascx.cs"
    Inherits="CMSModules_FormControls_FormControls_DataTypeFilter" %>
<%@ Register Src="~/CMSFormControls/System/UserControlTypeSelector.ascx" TagPrefix="cms"
    TagName="TypeSelector" %>
<asp:Panel CssClass="Filter" runat="server" ID="pnlSearch">
    <cms:LocalizedLabel AssociatedControlID="drpTypeSelector" ID="lblTypeSelector" runat="server"
        EnableViewState="false" ResourceString="fieldeditor.formfieldtype" DisplayColon="true" />
    <cms:TypeSelector ID="drpTypeSelector" runat="server" CssClass="DropDownField" IncludeAllItem="true"
        AutoPostBack="true" />
</asp:Panel>
