<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSSiteManager_Tools_leftmenu"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Tree.master" CodeFile="LeftMenu.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIMenu.ascx" TagName="UIMenu"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcTree">
    <cms:UIMenu runat="server" ID="treeElem" ModuleName="CMS.SiteManager" ElementName="Tools"
        RootTargetURL="~/CMSSiteManager/tools/tools.aspx" ExpandAll="true" />
</asp:Content>
