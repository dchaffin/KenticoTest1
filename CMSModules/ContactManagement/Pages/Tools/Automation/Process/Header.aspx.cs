using System;

using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.Controls.Configuration;
using CMS.OnlineMarketing;

// Set edited object
[EditedObject(WorkflowObjectType.AUTOMATIONPROCESS, "processid")]

[Help("ma_process_general", "helpTopic")]

[Tabs("processContent")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Header : CMSAutomationPage
{
    #region "Variables"

    private int mProcessID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Process ID from query string
    /// </summary>
    public int ProcessID
    {
        get
        {
            if (mProcessID <= 0)
            {
                mProcessID = QueryHelper.GetInteger("processid", 0);
            }
            return mProcessID;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        string workflowName = string.Empty;

        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo(ProcessID);
        if (workflow != null)
        {
            workflowName = workflow.WorkflowDisplayName;
        }

        InitalizeMenu();

        if (!RequestHelper.IsPostBack())
        {
            switch (QueryHelper.GetString("showtab", String.Empty).ToLowerCSafe())
            {
                case "steps":
                    CurrentMaster.Tabs.SelectedTab = 1;
                    break;

                case "triggers":
                    CurrentMaster.Tabs.SelectedTab = 2;
                    break;

                case "contacts":
                    CurrentMaster.Tabs.SelectedTab = 3;
                    break;
            }
        }

        // Initialize master page title
        InitializeMasterPage(workflowName);
    }


    /// <summary>
    /// Initializes the master page title.
    /// </summary>
    private void InitializeMasterPage(string currentWorkflow)
    {
        if (!CurrentUser.IsGlobalAdministrator)
        {
            PageTitle title = CurrentMaster.Title;
            title.TitleText = GetString("ma.process.list");
            title.TitleImage = GetImageUrl("Objects/MA_AutomationProcess/object.png");
            title.HelpName = "helpTopic";
            title.HelpTopicName = "ma_process_general";
        }

        // Initialize master page
        string[,] breadcrumb = new string[2, 3];
        breadcrumb[0, 0] = GetString("ma.process.list");
        breadcrumb[0, 1] = AddSiteQuery("~/CMSModules/ContactManagement/Pages/Tools/Automation/List.aspx", null);
        breadcrumb[0, 2] = "_parent";
        breadcrumb[1, 0] = currentWorkflow;
        breadcrumb[1, 1] = string.Empty;
        breadcrumb[1, 2] = string.Empty;

        CurrentMaster.Title.Breadcrumbs = breadcrumb;
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
            RedirectUrl = "Tab_General.aspx?processId=" + ProcessID,
            OnClientClick = "SetHelpTopic('helpTopic', 'ma_process_general');"
        };
        SetTab(general);

        // Steps tab
        TabItem steps = new TabItem()
        {
            Text = GetString("ma.process.steps"),
            RedirectUrl = "Tab_Steps.aspx?processId=" + ProcessID,
            OnClientClick = "SetHelpTopic('helpTopic', 'ma_process_designer');"
        };
        SetTab(steps);

        // Triggers tab
        TabItem triggers = new TabItem()
        {
            Text = GetString("ma.trigger.list"),
            RedirectUrl = AddSiteQuery("Trigger/List.aspx?processId=" + ProcessID, (IsSiteManager ? UniSelector.US_ALL_RECORDS : SiteID)),
            OnClientClick = "SetHelpTopic('helpTopic', 'ma_process_triggers');"
        };
        SetTab(triggers);

        // Contacts tab
        TabItem contacts = new TabItem()
        {
            Text = GetString("om.contact.list"),
            RedirectUrl = AddSiteQuery("Tab_Contacts.aspx?processId=" + ProcessID, (IsSiteManager ? UniSelector.US_ALL_RECORDS : SiteID)),
            OnClientClick = "SetHelpTopic('helpTopic', 'ma_process_contacts');"
        };
        SetTab(contacts);

        // Reports tab
        TabItem reports = new TabItem()
        {
            Text = GetString("general.report"),
            RedirectUrl = AddSiteQuery("Tab_Report.aspx?processId=" + ProcessID, (IsSiteManager ? UniSelector.US_ALL_RECORDS : SiteID)),
            OnClientClick = "SetHelpTopic('helpTopic', 'ma_process_report');"
        };
        SetTab(reports);
    }
}