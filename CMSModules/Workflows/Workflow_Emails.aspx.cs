using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Actions(1)]
[SaveAction(0)]
public partial class CMSModules_Workflows_Workflow_Emails : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (WorkflowId > 0)
        {
            // Set edited object
            EditedObject = CurrentWorkflow;
            ucEmails.WorkflowID = WorkflowId;
        }
    }
}
