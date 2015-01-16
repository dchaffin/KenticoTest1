using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSModules_Workflows_Pages_WorkflowAction_Frameset : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frm.FrameHeight = TabsBreadFrameHeight;
    }
}