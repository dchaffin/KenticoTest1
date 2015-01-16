<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectBundleInventoryType.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_SelectBundleInventoryType" %>
<!-- Remove bundle -->
<cms:LocalizedRadioButton runat="server" ID="radRemoveBundle" GroupName="RemoveFromInventory"
    Checked="true" ResourceString="com.bundle.removebundle" />
<br />
<!-- Remove products -->
<cms:LocalizedRadioButton runat="server" ID="radRemoveProducts" GroupName="RemoveFromInventory"
    ResourceString="com.bundle.removeproducts" />
<br />
<!-- Remove bundle and products -->
<cms:LocalizedRadioButton runat="server" ID="radRemoveBundleAndProducts" GroupName="RemoveFromInventory"
    ResourceString="com.bundle.removebundleandproducts" />