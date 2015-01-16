<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSSiteManager_Development_leftmenu"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Tree.master" CodeFile="LeftMenu.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIMenu.ascx" TagName="UIMenu"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcTree">
    <cms:UIMenu runat="server" ID="treeElem" ModuleName="CMS.SiteManager" ElementName="Development"
        RootTargetURL="~/CMSSiteManager/development/development.aspx" />
    <cms:UITreeView ID="treeElemDev" ShortID="t" runat="server" ShowLines="true" ShowExpandCollapse="true"
        EnableViewState="false" />
</asp:Content>
