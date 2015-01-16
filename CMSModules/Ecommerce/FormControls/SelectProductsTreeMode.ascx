<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectProductsTreeMode.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_SelectProductsTreeMode" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hdnField" runat="server" EnableViewState="false" />
        <asp:CheckBox ID="chkTreeMode" runat="server" OnCheckedChanged="chkTreeMode_CheckedChanged"
            AutoPostBack="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
