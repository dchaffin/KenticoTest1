<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_CssStylesheets_Pages_CssStylesheet_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Css stylesheets - List"
    CodeFile="CssStylesheet_List.aspx.cs" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid ID="UniGridCssStyleSheets" runat="server" OrderBy="StylesheetDisplayName"
        IsLiveSite="false" ObjectType="cms.cssstylesheet" Columns="StylesheetID, StylesheetDisplayName, StylesheetName">
        <GridActions Parameters="StylesheetID">
            <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
            <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="StylesheetDisplayName" Caption="$general.displayname$" Wrap="false"
                Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="StylesheetName" Caption="$general.codename$" Wrap="false" />
            <ug:Column Source="StylesheetID" Caption="$general.sitename$" Wrap="false" Visible="false">
                <Filter Type="site" />
            </ug:Column>
            <ug:Column Width="100%" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
