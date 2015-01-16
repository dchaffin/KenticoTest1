<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BadWords_BadWords_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Bad words - List" CodeFile="BadWords_List.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="UniGrid" GridName="BadWords_List.xml" IsLiveSite="false" HideFilterButton="true" />
</asp:Content>
