<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_Roles_RoleUsers"
    CodeFile="RoleUsers.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlBasic" runat="server" UpdateMode="Conditional">    
    <ContentTemplate>
    <asp:Label ID="lblAvialable" runat="server" CssClass="BoldInfoLabel" />    
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<cms:SelectUser ID="usUsers" runat="server" SelectionMode="Multiple" />
