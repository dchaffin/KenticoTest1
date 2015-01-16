<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_WebAnalytics_Tools_Analytics_Statistics"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Tree.master" CodeFile="Analytics_Statistics.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIMenu.ascx" TagName="UIMenu"
    TagPrefix="cms" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcTree">
    <cms:UIMenu runat="server" ID="treeElem" ModuleName="CMS.WebAnalytics" TargetFrame="analyticsDefault"
        EnableRootSelect="false" QueryParameterName="node" ModuleAvailabilityForSiteRequired="true" />
    <asp:HiddenField ID="hdnMonthsSelectedBar" runat="server" />
    <asp:HiddenField ID="hdnDaysSelectedBar" runat="server" />
    <asp:HiddenField ID="hdnYearSelected" runat="server" />
</asp:Content>