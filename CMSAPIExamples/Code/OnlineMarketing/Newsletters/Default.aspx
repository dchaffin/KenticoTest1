<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_OnlineMarketing_Newsletters_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAPIExamples/Controls/APIExampleSection.ascx" TagName="APIExampleSection"
    TagPrefix="cms" %>
<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <cms:APIExampleSection ID="secEmailTemplates" runat="server" Title="Email templates" />
    <%-- Subscription template --%>
    <cms:APIExamplePanel ID="pnlCreateSubscriptionTemplate" runat="server" GroupingText="Subscription template">
        <cms:APIExample ID="apiCreateSubscriptionTemplate" runat="server" ButtonText="Create template"
            InfoMessage="Template 'My new subscription template' was created." />
    </cms:APIExamplePanel>
    <%-- Unsubscription template --%>
    <cms:APIExamplePanel ID="pnlCreateUnsubscriptionTemplate" runat="server" GroupingText="Unsubscription template">
        <cms:APIExample ID="apiCreateUnsubscriptionTemplate" runat="server" ButtonText="Create template"
            InfoMessage="Template 'My new unsubscription template' was created." />
    </cms:APIExamplePanel>
    <%-- Issue template --%>
    <cms:APIExamplePanel ID="pnlCreateIssueTemplate" runat="server" GroupingText="Issue template">
        <cms:APIExample ID="apiCreateIssueTemplate" runat="server" ButtonText="Create template" InfoMessage="Template 'My new issue template' was created." />
        <cms:APIExample ID="apiGetAndUpdateIssueTemplate" runat="server" ButtonText="Get and update template"
            APIExampleType="ManageAdditional" InfoMessage="Template 'My new issue template' was updated."
            ErrorMessage="Template 'My new issue template' was not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateIssueTemplates" runat="server" ButtonText="Get and bulk update templates"
            APIExampleType="ManageAdditional" InfoMessage="All templates matching the condition were updated."
            ErrorMessage="Templates matching the condition were not found." />
    </cms:APIExamplePanel>
    <cms:APIExampleSection ID="secNewsletters" runat="server" Title="Newsletters" />
    <%-- Static newsletter --%>
    <cms:APIExamplePanel ID="pnlCreateStaticNewsletter" runat="server" GroupingText="Static newsletter">
        <cms:APIExample ID="apiCreateStaticNewsletter" runat="server" ButtonText="Create newsletter" InfoMessage="Newsletter 'My new static newsletter' was created." 
            ErrorMessage="Template 'My new subscription template', template 'My new unsubscription template' or template 'My new issue template' were not found." />
        <cms:APIExample ID="apiGetAndUpdateStaticNewsletter" runat="server" ButtonText="Get and update newsletter"
            APIExampleType="ManageAdditional" InfoMessage="Newsletter 'My new static newsletter' was updated."
            ErrorMessage="Newsletter 'My new static newsletter' was not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateStaticNewsletters" runat="server" ButtonText="Get and bulk update newsletters"
            APIExampleType="ManageAdditional" InfoMessage="All newsletters matching the condition were updated."
            ErrorMessage="Newsletters matching the condition were not found." />
    </cms:APIExamplePanel>
    <%-- Dynamic newsletter --%>
    <cms:APIExamplePanel ID="pnlCreateDynamicNewsletter" runat="server" GroupingText="Dynamic newsletter">
        <cms:APIExample ID="apiCreateDynamicNewsletter" runat="server" ButtonText="Create newsletter" InfoMessage="Newsletter 'My new dynamic newsletter' was created." 
            ErrorMessage="Template 'My new subscription template' or template 'My new unsubscription template' were not found." />
        <cms:APIExample ID="apiGetAndUpdateDynamicNewsletter" runat="server" ButtonText="Get and update newsletter"
            APIExampleType="ManageAdditional" InfoMessage="Newsletter 'My new dynamic newsletter' was updated."
            ErrorMessage="Newsletter 'My new dynamic newsletter' was not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateDynamicNewsletters" runat="server" ButtonText="Get and bulk update newsletters"
            APIExampleType="ManageAdditional" InfoMessage="All newsletters matching the condition were updated."
            ErrorMessage="Newsletters matching the condition were not found." />
    </cms:APIExamplePanel>
    <cms:APIExampleSection ID="secSubscribers" runat="server" Title="Subscribers" />
    <%-- Subscriber --%>
    <cms:APIExamplePanel ID="pnlCreateSubscriber" runat="server" GroupingText="Subscriber">
        <cms:APIExample ID="apiCreateSubscriber" runat="server" ButtonText="Create subscriber"
            InfoMessage="Subscriber 'subscriber@localhost.local' was created." />
        <cms:APIExample ID="apiGetAndUpdateSubscriber" runat="server" ButtonText="Get and update subscriber"
            APIExampleType="ManageAdditional" InfoMessage="Subscriber 'subscriber@localhost.local' was updated."
            ErrorMessage="Subscriber 'subscriber@localhost.local' was not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateSubscribers" runat="server" ButtonText="Get and bulk update subscribers"
            APIExampleType="ManageAdditional" InfoMessage="All subscribers matching the condition were updated."
            ErrorMessage="Subscribers matching the condition were not found." />
    </cms:APIExamplePanel>
    <%-- Newsletter subscriber --%>
    <cms:APIExamplePanel ID="pnlSubscribeToNewsletter" runat="server" GroupingText="Newsletter subscriber">
        <cms:APIExample ID="apiSubscribeToNewsletter" runat="server" ButtonText="Subscribe to newsletter" InfoMessage="Subscriber 'subscriber@localhost.local' was subscribed to 'My new static newsletter'." 
            ErrorMessage="Subscriber 'subscriber@localhost.local' or newsletter 'My new static newsletter' were not found." />
    </cms:APIExamplePanel>
    <cms:APIExampleSection ID="secIssues" runat="server" Title="Issues" />
     <%-- Static issue --%>
    <cms:APIExamplePanel ID="pnlCreateStaticIssue" runat="server" GroupingText="Static issue">
        <cms:APIExample ID="apiCreateStaticIssue" runat="server" ButtonText="Create issue" InfoMessage="Issue 'My new static issue' was created." ErrorMessage="Newsletter 'My new static newsletter' was not found." />
        <cms:APIExample ID="apiGetAndUpdateStaticIssue" runat="server" ButtonText="Get and update issue" APIExampleType="ManageAdditional" InfoMessage="Issue 'My new static issue' was updated." ErrorMessage="Issue 'My new static issue' or newsletter 'My new static newsletter' were not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateStaticIssues" runat="server" ButtonText="Get and bulk update issues" APIExampleType="ManageAdditional" InfoMessage="All issues matching the condition were updated." ErrorMessage="Issues matching the condition or newsletter 'My new static newsletter' were not found." />
    </cms:APIExamplePanel>
    <%-- Dynamic issue --%>
    <cms:APIExamplePanel ID="pnlCreateDynamicIssue" runat="server" GroupingText="Dynamic issue">
        <cms:APIExample ID="apiCreateDynamicIssue" runat="server" ButtonText="Create issue" InfoMessage="Issue 'My new dynamic issue' was created." ErrorMessage="Newsletter 'My new dynamic newsletter' was not found" />
        <cms:APIExample ID="apiGetAndUpdateDynamicIssue" runat="server" ButtonText="Get and update issue" APIExampleType="ManageAdditional" InfoMessage="Issue 'My new dynamic issue' was updated." ErrorMessage="Issue 'My new dynamic issue' or newsletter 'My new dynamic newsletter' were not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateDynamicIssues" runat="server" ButtonText="Get and bulk update issues" APIExampleType="ManageAdditional" InfoMessage="All issues matching the condition were updated." ErrorMessage="Issues matching the condition or newsletter 'My new dynamic newsletter' were not found." />
    </cms:APIExamplePanel>
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <cms:APIExampleSection ID="APIExampleSection1" runat="server" Title="Issues" />
    <%-- Dynamic issue --%>
    <cms:APIExamplePanel ID="pnlDeleteDynamicIssue" runat="server" GroupingText="Dynamic issue">
        <cms:APIExample ID="apiDeleteDynamicIssue" runat="server" ButtonText="Delete issue" APIExampleType="CleanUpMain" InfoMessage="Issue 'My new dynamic issue' and all its dependencies were deleted." ErrorMessage="Issue 'My new dynamic issue' or newsletter 'My new dynamic newsletter' were not found." />
    </cms:APIExamplePanel>
     <%-- Static issue --%>
    <cms:APIExamplePanel ID="pnlDeleteStaticIssue" runat="server" GroupingText="Static issue">
        <cms:APIExample ID="apiDeleteStaticIssue" runat="server" ButtonText="Delete issue" APIExampleType="CleanUpMain" InfoMessage="Issue 'My new static issue' and all its dependencies were deleted." ErrorMessage="Issue 'My new static issue' or newsletter 'My new static newsletter' were not found." />
    </cms:APIExamplePanel>
    <cms:APIExampleSection ID="APIExampleSection2" runat="server" Title="Subscribers" />
     <%-- Newsletter subscriber --%>
    <cms:APIExamplePanel ID="pnlUnsubscribeSubscriber" runat="server" GroupingText="Newsletter subscriber">
        <cms:APIExample ID="apiUnsubscribeFromNewsletter" runat="server" ButtonText="Unsubscribe from newsletter" APIExampleType="CleanUpMain" InfoMessage="Subscriber 'subscriber@localhost.local' was unsubscribed from 'My new static newsletter'." 
            ErrorMessage="Subscriber 'subscriber@localhost.local' or newsletter 'My new static newsletter' were not found." />
    </cms:APIExamplePanel>
    <%-- Subscriber --%>
    <cms:APIExamplePanel ID="pnlDeleteSubscriber" runat="server" GroupingText="Subscriber">
        <cms:APIExample ID="apiDeleteSubscriber" runat="server" ButtonText="Delete subscriber" APIExampleType="CleanUpMain" 
            InfoMessage="Subscriber 'subscriber@localhost.local' and all its dependencies were deleted."
            ErrorMessage="Subscriber 'subscriber@localhost.local' was not found." />
    </cms:APIExamplePanel>
    <cms:APIExampleSection ID="APIExampleSection3" runat="server" Title="Newsletters" />
    <%-- Dynamic newsletter --%>
    <cms:APIExamplePanel ID="pnlDeleteDynamicNewsletter" runat="server" GroupingText="Dynamic newsletter">
        <cms:APIExample ID="apiDeleteDynamicNewsletter" runat="server" ButtonText="Delete newsletter"
            APIExampleType="CleanUpMain" InfoMessage="Newsletter 'My new dynamic newsletter' and all its dependencies were deleted."
            ErrorMessage="Newsletter 'My new dynamic newsletter' was not found." />
    </cms:APIExamplePanel>
    <%-- Static newsletter --%>
    <cms:APIExamplePanel ID="pnlDeleteStaticNewsletter" runat="server" GroupingText="Static newsletter">
        <cms:APIExample ID="apiDeleteStaticNewsletter" runat="server" ButtonText="Delete newsletter"
            APIExampleType="CleanUpMain" InfoMessage="Newsletter 'My new static newsletter' and all its dependencies were deleted."
            ErrorMessage="Newsletter 'My new static newsletter' was not found." />
    </cms:APIExamplePanel>
    <cms:APIExampleSection ID="APIExampleSection4" runat="server" Title="Email templates" />
    <%-- Subscription template --%>
    <cms:APIExamplePanel ID="pnlDeleteSubscriptionTemplate" runat="server" GroupingText="Subscription template">
        <cms:APIExample ID="apiDeleteSubscriptionTemplate" runat="server" ButtonText="Delete template"
            APIExampleType="CleanUpMain" InfoMessage="Template 'My new subscription template' and all its dependencies were deleted."
            ErrorMessage="Template 'My new subscription template' was not found." />
    </cms:APIExamplePanel>
    <%-- Unsubscription template --%>
    <cms:APIExamplePanel ID="pnlDeleteUnsubscriptionTemplate" runat="server" GroupingText="Unsubscription template">
        <cms:APIExample ID="apiDeleteUnsubscriptionTemplate" runat="server" ButtonText="Delete template"
            APIExampleType="CleanUpMain" InfoMessage="Template 'My new unsubscription template' and all its dependencies were deleted."
            ErrorMessage="Template 'My new unsubscription template' was not found." />
    </cms:APIExamplePanel>
    <%-- Issue template --%>
    <cms:APIExamplePanel ID="pnlDeleteIssueTemplate" runat="server" GroupingText="Issue template">
        <cms:APIExample ID="apiDeleteIssueTemplate" runat="server" ButtonText="Delete template"
            APIExampleType="CleanUpMain" InfoMessage="Template 'My new issue template' and all its dependencies were deleted."
            ErrorMessage="Template 'My new issue template' was not found." />
    </cms:APIExamplePanel>
</asp:Content>
