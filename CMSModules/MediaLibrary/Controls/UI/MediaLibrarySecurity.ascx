<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_UI_MediaLibrarySecurity"
    CodeFile="MediaLibrarySecurity.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix"
    TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:LocalizedLabel ID="lblInfo" runat="server" Visible="false" CssClass="InfoLabel" EnableViewState="false" />
<asp:Table runat="server" ID="tblMatrix" CssClass="PermissionMatrix" CellPadding="3"
    CellSpacing="0" EnableViewState="false" GridLines="Horizontal">
</asp:Table>
<br />
<cms:UniMatrix ID="gridMatrix" runat="server" QueryName="media.library.getpermissionmatrix"
    RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemCodeNameColumn="RoleName"
    RowItemDisplayNameColumn="RoleDisplayName" ColumnItemDisplayNameColumn="PermissionDisplayName"
    ColumnItemTooltipColumn="PermissionDescription" RowTooltipColumn="RowDisplayName"
    ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription"
    FirstColumnsWidth="28" FixedWidth="12" UsePercentage="true" AddGlobalObjectSuffix="true" SiteIDColumnName="SiteID" />
