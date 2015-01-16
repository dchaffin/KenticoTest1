using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_Frameset : CMSWorkflowPage
{
    #region "Variables"

    protected string headerTargetUrl = "";
    protected string contentTargetUrl = "";

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        Guid sourcePointGuid = QueryHelper.GetGuid("sourcepointguid", Guid.Empty);
        int workflowStepId = QueryHelper.GetInteger("workflowstepid", 0);

        headerTargetUrl = String.Format("Header.aspx?workflowstepid={0}&sourcepointguid={1}", workflowStepId, sourcePointGuid);
        contentTargetUrl = String.Format("General.aspx?workflowstepid={0}&sourcepointguid={1}", workflowStepId, sourcePointGuid);
        if (QueryHelper.GetBoolean("saved", false))
        {
            contentTargetUrl += "&saved=1";
        }
    }
}