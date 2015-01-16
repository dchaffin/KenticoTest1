using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.LicenseProvider;
using CMS.WorkflowEngine;

public partial class CMSModules_Workflows_Workflow_Designer : CMSWorkflowPage
{
    #region "Constants"

    private const string SERVICEURL = "~/CMSModules/Workflows/Services/WorkflowDesignerService.svc";

    #endregion


    #region "Event handlers"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        designerElem.ServiceUrl = SERVICEURL;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (WorkflowId <= 0)
        {
            designerElem.StopProcessing = true;
            return;
        }

        designerElem.WorkflowID = WorkflowId;
        designerElem.WorkflowType = WorkflowTypeEnum.Approval;

        if(!LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.AdvancedWorkflow))
        {
            designerElem.ReadOnly = true;
            MessagesPlaceHolder.OffsetY = 10;
            MessagesPlaceHolder.UseRelativePlaceHolder = false;
            ShowInformation(GetString("wf.licenselimitation"));
        }

        EditedObject = CurrentWorkflow;
    }

    #endregion
}