<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TextFilter.ascx.cs" Inherits="CMSFormControls_Filters_TextFilter" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <asp:DropDownList ID="drpOperator" AutoPostBack="true" runat="server" CssClass="ExtraSmallDropDown" />
        <cms:CMSTextBox ID="txtText" runat="server" CssClass="FilterTextBox" />
        <cms:CMSRequiredFieldValidator Display="Dynamic" ID="validator" runat="server" ControlToValidate="txtText"/>
    </ContentTemplate>
</cms:CMSUpdatePanel>
