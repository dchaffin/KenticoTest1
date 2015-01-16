<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Workflow action list"
    Inherits="CMSModules_Workflows_Pages_WorkflowAction_List" Theme="Default" CodeFile="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowAction/List.ascx" TagName="WorkflowActionList" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:WorkflowActionList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
