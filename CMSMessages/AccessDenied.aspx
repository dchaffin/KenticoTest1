<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_AccessDenied"
    Theme="Default" EnableEventValidation="false" CodeFile="AccessDenied.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Debug/SecurityLog.ascx" TagName="SecurityLog"
    TagPrefix="cms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>CMS - Access denied</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
    <asp:Panel ID="PanelBody" runat="server" CssClass="PageBody">
        <asp:Panel ID="PanelTitle" runat="server" CssClass="PageHeader">
            <cms:pagetitle id="titleElem" runat="server" />
        </asp:Panel>
        <asp:Panel ID="PanelContent" runat="server" CssClass="PageContent">
            <asp:Label ID="lblMessage" runat="server" Text="Label" /><br />
            <br />
            <cms:LocalizedHyperLink ID="lnkGoBack" runat="server" NavigateUrl="~/default.aspx" ResourceString="CMSDesk.GoBack" /><br />
            <br />
            <br />
            <cms:localizedbutton id="btnSignOut" visible="false" runat="server" cssclass="SubmitButton"
                onclick="btnSignOut_Click" resourcestring="signoutbutton.signout" />
            <cms:localizedbutton id="btnLogin" visible="false" runat="server" cssclass="SubmitButton"
                onclick="btnLogin_Click" resourcestring="webparts_membership_signoutbutton.signin" />
            <asp:Literal ID="ltlScript" runat="server" Text="" />
            <cms:SecurityLog ID="logSec" runat="server" InitFromRequest="true" />
        </asp:Panel>
    </asp:Panel>
    </form>
</body>
</html>
