using System;

using CMS.UIControls;
using CMS.WorkflowEngine;

// Actions
[Actions(1)]
[Action(0, "Objects/CMS_MacroRule/add.png", "macros.macrorule.new", "~/CMSModules/Workflows/Pages/MacroRule/New.aspx")]
public partial class CMSModules_Workflows_Pages_MacroRule_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        listElem.ResourceName = WorkflowObjectType.WORKFLOW;
    }
}