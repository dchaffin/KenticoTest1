<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_SearchIndex_General"
    Title="Search Index - General" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" CodeFile="SearchIndex_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_General.ascx"
    TagName="IndexGeneral" TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:IndexGeneral ID="IndexGeneral" runat="server" Visible="true" IsLiveSite="false" />
</asp:Content>
