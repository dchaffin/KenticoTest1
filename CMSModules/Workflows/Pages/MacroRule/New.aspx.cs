using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.WorkflowEngine;

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "macros.macrorule.list", "~/CMSModules/Workflows/Pages/MacroRule/List.aspx", null)]
[Breadcrumb(1, "macros.macrorule.new")]
public partial class CMSModules_Workflows_Pages_MacroRule_New : SiteManagerPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.ResourceName = WorkflowObjectType.WORKFLOW;

        CurrentMaster.Title.HelpTopicName = "macros_macrorule_general";
    }

    #endregion
}
