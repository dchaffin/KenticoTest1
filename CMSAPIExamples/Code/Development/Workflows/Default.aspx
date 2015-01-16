<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Development_Workflows_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<%@ Register Src="~/CMSAPIExamples/Controls/APIExampleSection.ascx" TagName="APIExampleSection" TagPrefix="cms" %>
<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <cms:APIExampleSection ID="secBasicPublishing" runat="server" Title="Basic publishing" /> 
    <%-- Workflow --%>
    <cms:APIExamplePanel ID="pnlCreateWorkflow" runat="server" GroupingText="Workflow">
        <cms:APIExample ID="apiCreateWorkflow" runat="server" ButtonText="Create workflow" InfoMessage="Workflow 'My new workflow' was created." />
        <cms:APIExample ID="apiGetAndUpdateWorkflow" runat="server" ButtonText="Get and update workflow" APIExampleType="ManageAdditional" InfoMessage="Workflow 'My new workflow' was updated." ErrorMessage="Workflow 'My new workflow' was not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateWorkflows" runat="server" ButtonText="Get and bulk update workflows" APIExampleType="ManageAdditional" InfoMessage="All workflows matching the condition were updated." ErrorMessage="Workflows matching the condition were not found." />
    </cms:APIExamplePanel>
    <%-- Workflow step --%>
    <cms:APIExamplePanel ID="pnlCreateStep" runat="server" GroupingText="Workflow step">
        <cms:APIExample ID="apiCreateWorkflowStep" runat="server" ButtonText="Create workflow step" InfoMessage="Workflow step 'My new workflow step' was created." ErrorMessage="Workflow 'My new workflow' was not found." />
        <cms:APIExample ID="apiAddRoleToStep" runat="server" ButtonText="Add role to step" APIExampleType="ManageAdditional" InfoMessage="Role 'CMS Editors' was assigned to the step." ErrorMessage="Workflow 'My new workflow' was not found." />
    </cms:APIExamplePanel>
    <%-- Workflow scope --%>
    <cms:APIExamplePanel ID="pnlCreateWorkflowScope" runat="server" GroupingText="Workflow scope">
        <cms:APIExample ID="apiCreateWorkflowScope" runat="server" ButtonText="Create scope" InfoMessage="Scope 'My new scope' was created." ErrorMessage="Workflow 'My new workflow' was not found" />
        <cms:APIExample ID="apiGetAndUpdateWorkflowScope" runat="server" ButtonText="Get and update scope" APIExampleType="ManageAdditional" InfoMessage="Scope 'My new scope' was updated." ErrorMessage="The workflow 'My new workflow' was not found." />
    </cms:APIExamplePanel>
    <cms:APIExampleSection ID="secAdvancedPublishing" runat="server" Title="Advanced publishing" /> 
    <%-- Advanced workflow --%>
    <cms:APIExamplePanel ID="pnlConvertToAdvancedWorkflow" runat="server" GroupingText="Workflow">
        <cms:APIExample ID="apiConvertToAdvancedWorkflow" runat="server" ButtonText="Convert to advanced workflow" InfoMessage="Workflow 'My new workflow' was converted to advanced workflow." ErrorMessage="Workflow 'My new workflow' was not found." />
    </cms:APIExamplePanel>
    <%-- Workflow action --%>
    <cms:APIExamplePanel ID="pnlCreateActions" runat="server" GroupingText="Workflow action">
        <cms:APIExample ID="apiCreateAction" runat="server" ButtonText="Create action" APIExampleType="ManageMain" InfoMessage="Action 'My new action' was created." ErrorMessage="Action 'My new action' was not created." />
        <cms:APIExample ID="apiGetAndUpdateAction" runat="server" ButtonText="Get and update action" APIExampleType="ManageAdditional" InfoMessage="Action 'My new action' was updated." ErrorMessage="Action 'My new action' was not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateActions" runat="server" ButtonText="Get and bulk update actions" APIExampleType="ManageAdditional" InfoMessage="All actions matching the condition were updated." ErrorMessage="Actions matching the condition were not found." />
    </cms:APIExamplePanel>
    <%-- Steps and transitions--%>
    <cms:APIExamplePanel ID="pnlCreateTransitions" runat="server" GroupingText="Workflow transition">
        <cms:APIExample ID="apiCreateStepForTransition" runat="server" ButtonText="Create step" InfoMessage="Step 'My new step' was created." ErrorMessage="Workflow 'My new workflow' was not found." />
        <cms:APIExample ID="apiCreateTransition" runat="server" ButtonText="Create transition" InfoMessage="Transition was created." ErrorMessage="Workflow 'My new workflow' or steps 'My new step' or 'Published' were not found." />
    </cms:APIExamplePanel>
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Workflow action --%>
    <cms:APIExamplePanel ID="pnlDeleteAction" runat="server" GroupingText="Workflow action">
        <cms:APIExample ID="apiDeleteAction" runat="server" ButtonText="Delete action" APIExampleType="CleanUpMain" InfoMessage="Action 'My new action' was deleted." ErrorMessage="Action 'My new action' was not found." />
    </cms:APIExamplePanel>
    <%-- Workflow transition --%>
    <cms:APIExamplePanel ID="pnlDeleteTransition" runat="server" GroupingText="Workflow transition">
        <cms:APIExample ID="apiDeleteTransition" runat="server" ButtonText="Delete transition" APIExampleType="CleanUpMain" InfoMessage="Transition leading from step 'My new step' was deleted." ErrorMessage="Workflow 'My new workflow' or step 'My new step' were not found." />
    </cms:APIExamplePanel>
    <%-- Workflow scope --%>
    <cms:APIExamplePanel ID="pnlDeleteWorkflowScope" runat="server" GroupingText="Workflow scope">
        <cms:APIExample ID="apiDeleteWorkflowScope" runat="server" ButtonText="Delete scope" APIExampleType="CleanUpMain" InfoMessage="The workflow scope was deleted." ErrorMessage="The scope was not found." />
    </cms:APIExamplePanel>
    <%-- Workflow step --%>
    <cms:APIExamplePanel ID="pnlDeleteStep" runat="server" GroupingText="Workflow step">
        <cms:APIExample ID="apiRemoveRoleFromStep" runat="server" ButtonText="Remove role from step" APIExampleType="CleanUpMain" InfoMessage="Role 'CMS Editors' was removed from the step." ErrorMessage="Workflow 'My new workflow' was not found." />
        <cms:APIExample ID="apiDeleteWorkflowStep" runat="server" ButtonText="Delete workflow step" APIExampleType="CleanUpMain" InfoMessage="Step 'My new workflow step' was deleted." ErrorMessage="Workflow 'My new workflow' was not found." />
    </cms:APIExamplePanel>
    <%-- Workflow --%>
    <cms:APIExamplePanel ID="pnlDeleteWorkflow" runat="server" GroupingText="Workflow">
        <cms:APIExample ID="apiDeleteWorkflow" runat="server" ButtonText="Delete workflow" APIExampleType="CleanUpMain" InfoMessage="Workflow 'My new workflow' was deleted." ErrorMessage="Workflow 'My new workflow' was not found." />
    </cms:APIExamplePanel>
</asp:Content>

