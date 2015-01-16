<%@ Page Title="Workflow schemes - Macro rule list" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="CMSModules_Workflows_Pages_MacroRule_List" Theme="Default" %>

<%@ Register Src="~/CMSModules/Macros/Controls/UI/MacroRule/List.ascx" TagName="MacroRuleList"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:MacroRuleList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
