<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Macro rule properties – General" Inherits="CMSModules_Macros_Pages_Tools_MacroRule_Tab_General"
    Theme="Default" CodeFile="Tab_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Macros/Controls/UI/MacroRule/Edit.ascx" TagName="MacroRuleEdit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MacroRuleEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
