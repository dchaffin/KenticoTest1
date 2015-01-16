using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;

public partial class CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_Security : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        securityElem.SiteID = CMSContext.CurrentSiteID;
        securityElem.WorkflowStepID = QueryHelper.GetInteger("workflowstepid", 0);
        securityElem.SourcePointGuid = QueryHelper.GetGuid("sourcepointguid", Guid.Empty);
    }
}