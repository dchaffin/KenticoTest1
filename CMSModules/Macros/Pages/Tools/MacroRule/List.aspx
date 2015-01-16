<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Macro rule list" Inherits="CMSModules_Macros_Pages_Tools_MacroRule_List"
    Theme="Default" CodeFile="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Macros/Controls/UI/MacroRule/List.ascx" TagName="MacroRuleList"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MacroRuleList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
