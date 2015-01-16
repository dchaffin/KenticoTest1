<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductControlPanel.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_UI_ProductControlPanel" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<cms:UILayout runat="server" ID="layoutElem">
    <Panes>
        <cms:UILayoutPane ID="menu" runat="server" Direction="North" Closable="false" Resizable="false"
            SpacingOpen="0" ControlPath="~/CMSModules/Ecommerce/Controls/UI/ProductMenu.ascx" />
        <cms:UILayoutPane ID="tree" runat="server" Direction="Center" PaneClass="TreeBody"
            ControlPath="~/CMSModules/Content/Controls/CMSDeskTree.ascx" />
    </Panes>
</cms:UILayout>
