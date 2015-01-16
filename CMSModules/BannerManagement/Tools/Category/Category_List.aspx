<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Tools_Category_Category_List"
    Title="Banner Management - Category List" Theme="Default" CodeFile="Category_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcSiteSelector" runat="server">
    <cms:LocalizedLabel ID="lblSites" runat="server" DisplayColon="true" ResourceString="general.site" CssClass="FieldLabel" EnableViewState="false" />
    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="c" runat="server" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid ID="gridBannerManagement" runat="server" IsLiveSite="false" OrderBy="BannerCategoryDisplayName" ObjectType="cms.bannercategory" EditActionUrl="Category_Edit.aspx?siteid={%SelectedSiteID%}&categoryid={0}"
                Columns="BannerCategoryID, BannerCategoryDisplayName, BannerCategoryEnabled, BannerCategorySiteID, (SELECT COUNT(*) FROM CMS_Banner B WHERE B.BannerCategoryID = CMS_BannerCategory.BannerCategoryID AND B.BannerEnabled = 1) BannerCounts, (SELECT COUNT(*) FROM CMS_Banner B WHERE B.BannerCategoryID = CMS_BannerCategory.BannerCategoryID) BannerCountsTotal" RememberStateByParam="">
                <GridActions Parameters="BannerCategoryID">
                    <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
                    <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$general.confirmdelete$" ExternalSourceName="delete" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="BannerCategoryDisplayName" Caption="$general.name$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="BannerCategoryEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$" Wrap="false">
                        <Filter Type="bool" />
                    </ug:Column>
                    <ug:Column Source="BannerCounts" Caption="$banner.enabledbanners$" Wrap="false" AllowSorting="false" >
                        <Filter Type="integer" />
                    </ug:Column>
                    <ug:Column Source="BannerCountsTotal" Caption="$banner.totalcount$" Wrap="false" AllowSorting="false">
                        <Filter Type="integer" />
                    </ug:Column>
                    <ug:Column Width="100%">
                    </ug:Column>
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    
    <asp:Button ID="btnHiddenPostBackButton" runat="server" style="display: none;" />
</asp:Content>
