using System;
using System.Data;
using System.Linq;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSModules_Workflows_Workflow_Step_Security : CMSWorkflowPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucSecurity.SiteID = CMSContext.CurrentSiteID;
        ucSecurity.WorkflowStepID = QueryHelper.GetInteger("workflowStepId", 0);
    }

    #endregion
}