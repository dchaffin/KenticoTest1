<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Products_Frameset.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Products_Frameset" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="E-commerce - Products" Theme="Default" %>

<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Content runat="server" ID="cntContent" ContentPlaceHolderID="plcContent">
    <cms:ContentUrlRetriever runat="server" ID="urlRetriever" />
    <input type="hidden" id="selectedNodeId" name="selectedNodeId" value="<%=ResultNodeID%>" />
    <input type="hidden" id="selectedCulture" name="selectedCulture" value="<%=CurrentUser.PreferredCultureCode%>" />
    <input type="hidden" id="selectedSplitModeCulture" name="selectedSplitModeCulture"
        value="<%=CMSContext.SplitModeCultureCode %>" />
    <input type="hidden" id="selectedDevice" name="selectedDevice" value="<%= ResultDevice%>" />
    <input type="hidden" id="selectedMode" name="selectedMode" value="page" />
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="contenttree" runat="server" Direction="West" SpacingOpen="7"
                SpacingClosed="7" Size="269" ControlPath="~/CMSModules/Ecommerce/Controls/UI/ProductControlPanel.ascx"
                RenderAs="div" IsLiveSite="False" PaneClass="ProductSectionNavigation" UseUpdatePanel="False" />
            <cms:UILayoutPane ID="contentview" runat="server" Direction="Center" RenderAs="Iframe"
                MaskContents="true" />
        </Panes>
    </cms:UILayout>
</asp:Content>
