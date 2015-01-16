<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Users_User_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Users - User List"
    CodeFile="User_list.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
<asp:Panel ID="pnlUsers" runat="server" CssClass="UsersList">
    <cms:UniGrid ID="gridElem" runat="server" GridName="User_List.xml" OrderBy="UserName"
        Columns="UserID, UserName, FullName, Email, UserNickName, UserCreated, UserEnabled, (CASE WHEN UserPassword IS NULL OR UserPassword = '' THEN 0 ELSE 1 END) AS UserHasPassword, UserIsGlobalAdministrator, UserIsExternal"
        IsLiveSite="false" />
</asp:Panel>
</asp:Content>
