<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Reports_Tree"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Tree.master" CodeFile="Tree.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIMenu.ascx" TagName="UIMenu"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcTree">
    <cms:UIMenu runat="server" ID="treeElem" ModuleName="CMS.Ecommerce" ElementName="ECReports"
        EnableRootSelect="false" TargetFrame="ecommreports" QueryParameterName="reportname" />
</asp:Content>
