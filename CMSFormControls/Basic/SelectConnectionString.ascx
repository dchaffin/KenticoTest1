<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectConnectionString.ascx.cs"
    Inherits="CMSFormControls_Basic_SelectConnectionString" %>
<asp:Panel ID="pnlMain" runat="server">
    <asp:DropDownList runat="server" ID="drpConnString" CssClass="DropDownField" />
    <asp:PlaceHolder runat="server" ID="pnlInherit" Visible="false">
        <br />
        <cms:LocalizedCheckBox runat="server" ID="chkInherit" ResourceString="generalproperties.radinherit" />
    </asp:PlaceHolder>
</asp:Panel>
