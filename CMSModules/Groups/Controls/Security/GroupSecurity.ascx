<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Controls_Security_GroupSecurity" CodeFile="GroupSecurity.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Table runat="server" ID="tblMatrix" GridLines="horizontal" CssClass="PermissionMatrix"
    CellPadding="3" EnableViewState="false" CellSpacing="0">
</asp:Table>
<br />
<cms:UniMatrix ID="gridMatrix" runat="server" QueryName="Community.GroupRolePermission.getpermissionMatrix"
    RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemCodeNameColumn="RoleName" RowItemDisplayNameColumn="RoleDisplayName"
    ColumnItemDisplayNameColumn="PermissionDisplayName" RowTooltipColumn="RowDisplayName"
    ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription"
    FirstColumnsWidth="30" FixedWidth="18" UsePercentage="true" />
