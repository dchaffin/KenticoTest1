<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Support_Pages_leftmenu"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Tree.master" CodeFile="LeftMenu.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIMenu.ascx" TagName="UIMenu"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcTree">
    <cms:UIMenu runat="server" ID="treeElem" ModuleName="CMS.SiteManager" ElementName="Support"
        RootTargetURL="~/CMSModules/Support/Pages/Support.aspx" />
</asp:Content>
