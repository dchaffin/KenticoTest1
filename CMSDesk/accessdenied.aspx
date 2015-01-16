<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSDesk_accessdenied" Theme="Default"
    EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="CMS Desk - Access denied" CodeFile="accessdenied.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/Debug/SecurityLog.ascx" TagName="SecurityLog"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <asp:Label ID="lblMessage" runat="server" Text="Label" EnableViewState="false" />
    <asp:HyperLink ID="lnkGoBack" runat="server" NavigateUrl="~/default.aspx" EnableViewState="false" /><br />
    <br />
    <br />
    <cms:LocalizedButton ID="btnSignOut" runat="server" CssClass="SubmitButton" OnClick="btnSignOut_Click"
        EnableViewState="false" ResourceString="signoutbutton.signout" />
    <asp:Literal ID="ltlScript" runat="server" Text="" EnableViewState="false" />
    <cms:SecurityLog ID="logSec" runat="server" InitFromRequest="true" />
</asp:Content>
