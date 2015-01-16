<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Ecommerce_EcommerceSettingsChecker" CodeFile="~/CMSWebParts/Ecommerce/EcommerceSettingsChecker.ascx.cs" %>
<%@ Register TagPrefix="cms" Assembly="CMS.ExtendedControls" Namespace="CMS.ExtendedControls" %>

<cms:LocalizedLabel runat="server" ID="lblNa" ResourceString="com.settingschecker.na" />
<cms:CMSPanel runat="server" ID="pnlError" GroupingTextResourceString="com.settingschecker.check">
    <cms:LocalizedLabel runat="server" ID="lblOk" ResourceString="com.settingschecker.ok" CssClass="Approved EditingFormLabel" />
    <cms:LocalizedLabel runat="server" ID="lblErrorMsg" ResourceString="com.settingschecker.lblerror" DisplayColon="true" CssClass="EditingFormLabel" />
    <asp:Label ID="lblSpacer" runat="server"><br /><br /></asp:Label>
</cms:CMSPanel>