using System;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.Controls.Configuration;

// Set edited object
[EditedObject("cms.workflow", "workflowid")]

[Tabs("workflowsContent")]
public partial class CMSModules_Workflows_Workflow_Header : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Workflow Edit - Header";

        // Set edited object
        EditedObject = CurrentWorkflow;

        const string workflowListUrl = "~/CMSModules/Workflows/Workflow_List.aspx";
        string workflows = GetString("Development-Workflow_Edit.Workflows");
        string currentWorkflow = string.Empty;

        if (CurrentWorkflow != null)
        {
            currentWorkflow = CurrentWorkflow.WorkflowDisplayName;
        }

        InitalizeMenu();

        if (!RequestHelper.IsPostBack())
        {

            if (QueryHelper.GetString("showtab", string.Empty) == "steps")
            {
                CurrentMaster.Tabs.SelectedTab = 1;
            }
        }

        // Initialize master page title
        InitializeMasterPage(workflows, workflowListUrl, currentWorkflow);
    }


    /// <summary>
    // Initializes the master page title
    /// </summary>
    private void InitializeMasterPage(string workflows, string workflowListUrl, string currentWorkflow)
    {
        // Set the master page title
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "workflow_general";

        // Initialize master page
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = workflows;
        pageTitleTabs[0, 1] = workflowListUrl;
        pageTitleTabs[0, 2] = "_parent";
        pageTitleTabs[1, 0] = currentWorkflow;
        pageTitleTabs[1, 1] = string.Empty;
        pageTitleTabs[1, 2] = string.Empty;

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }


    /// <summary>
    /// Initializes workflow edit menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        // General tab
        TabItem general = new TabItem()
                            {
                                Text = GetString("general.general"),
                                RedirectUrl = "Workflow_General.aspx?workflowId=" + WorkflowId,
                                OnClientClick = "SetHelpTopic('helpTopic', 'workflow_general');"
                            };
        SetTab(general);

        // Steps tab
        TabItem steps = new TabItem()
                            {
                                Text = GetString("Development-Workflow_Edit.Steps"),
                                RedirectUrl = CurrentWorkflow.IsBasic ? "Workflow_Steps.aspx?workflowId=" + WorkflowId : "Workflow_Designer.aspx?workflowId=" + WorkflowId,
                                OnClientClick = CurrentWorkflow.IsBasic ? "SetHelpTopic('helpTopic', 'workflow_steps');" : "SetHelpTopic('helpTopic', 'workflow_designer');"
                            };
        SetTab(steps);

        // Display additional tabs for document workflow
        if (CurrentWorkflow.IsDocumentWorkflow)
        {
            // Hide if no site or no running site
            if ((SiteInfoProvider.GetSitesCount() > 0))
            {
                // Scopes tab
                TabItem scopes = new TabItem()
                {
                    Text = GetString("Development-Workflow_Edit.Scopes"),
                    RedirectUrl = "Workflow_Scopes.aspx?workflowId=" + WorkflowId,
                    OnClientClick = "SetHelpTopic('helpTopic', 'workflow_scope');"
                };
                SetTab(scopes);
            }
        }

        // E-mails tab
        TabItem emails = new TabItem()
        {
            Text = GetString("Development-Workflow_Edit.Emails"),
            RedirectUrl = "Workflow_Emails.aspx?workflowId=" + WorkflowId,
            OnClientClick = "SetHelpTopic('helpTopic', 'workflow_emails');"
        };
        SetTab(emails);

        // Display additional tabs for document workflow
        if (CurrentWorkflow.IsDocumentWorkflow)
        {
            // Documents tab
            TabItem docs = new TabItem()
            {
                Text = GetString("general.documents"),
                RedirectUrl = "Workflow_Documents.aspx?workflowId=" + WorkflowId,
                OnClientClick = "SetHelpTopic('helpTopic', 'workflow_documents');"
            };
            SetTab(docs);
        }
    }
}