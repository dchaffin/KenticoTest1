using System;

using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSModules_Workflows_Workflow_Step_Edit : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterWOpenerScript(Page);
        int workflowStepId = QueryHelper.GetInteger("objectid", 0);

        int displayedTab = 0;
        if(!string.IsNullOrEmpty(QueryHelper.GetString("sourcepoint", String.Empty)))
        {
            displayedTab = 1;
        }

        string headerUrl = String.Format("Workflow_Step_Header.aspx?workflowstepid={0}&selectedtab={1}", workflowStepId, displayedTab);
        Guid sourcePointGuid = QueryHelper.GetGuid("sourcepoint", Guid.Empty);
        string contentUrl = GetContentUrl(workflowStepId, sourcePointGuid);
        if (QueryHelper.GetBoolean("saved", false))
        {
            contentUrl += "&saved=1";
        }

        // Load dialog footer buttons only for advanced workflow designer popup
        if (QueryHelper.GetBoolean("dialog", false))
        {
            frameHeader.Attributes.Add("src", headerUrl + "&dialog=1");
            frameContent.Attributes.Add("src", contentUrl + "&dialog=1");

            frameFooter.Attributes.Add("src", String.Format("Workflow_Step_Footer.aspx?workflowstepid={0}&graph={1}", workflowStepId, QueryHelper.GetString("graph", String.Empty)));
            frameFooter.Visible = true;
            rowsFrameset.Attributes.Add("rows", TabsBreadHeadFrameHeight + ", *, 43");
        }
        else
        {
            rowsFrameset.Attributes.Add("rows", TabsBreadFrameHeight + ", *");
            frameHeader.Attributes.Add("src", headerUrl);
            frameContent.Attributes.Add("src", contentUrl);
        }
    }


    /// <summary>
    /// For condition and single exit steps returns URL to general tab, else to cases tab.
    /// </summary>
    /// <param name="workflowStepId">Edited workflow step ID</param>
    /// <param name="sourcePointGuid">Edited source point GUID</param>
    private string GetContentUrl(int workflowStepId, Guid sourcePointGuid)
    {
        string url = null;
        if (sourcePointGuid != Guid.Empty)
        {
            url = String.Format("Pages/WorkflowStep/SourcePoint/frameset.aspx?sourcePointGuid={0}&", sourcePointGuid);
        }
        else
        {
            url = "Workflow_Step_General.aspx?";
        }
        return url + "workflowstepid=" + workflowStepId;
    }
}