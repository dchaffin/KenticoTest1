<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Workflow_Emails"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" EnableEventValidation="false"
    Theme="Default" Title="Workflows - Workflow E-mails" CodeFile="Workflow_Emails.aspx.cs" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/Workflow/Emails.ascx" TagName="WorkflowEmails"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="FormPanel">
        <cms:WorkflowEmails ID="ucEmails" runat="server" />
    </div>
</asp:Content>
