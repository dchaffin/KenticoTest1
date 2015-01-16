using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.ExtendedControls.ActionsConfig;
using CMS.WorkflowEngine;
using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.LicenseProvider;
using CMS.CMSHelper;

public partial class CMSModules_Workflows_Controls_UI_Workflow_Edit : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// Workflow being edited.
    /// </summary>
    public WorkflowInfo CurrentWorkflow
    {
        get
        {
            return (WorkflowInfo)editForm.EditedObject;
        }
    }


    /// <summary>
    /// UIForm editing current workflow.
    /// </summary>
    public UIForm Form
    {
        get
        {
            return editForm;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (editForm.IsInsertMode)
        {
            InitializeWorkflow();
        }

        editForm.OnBeforeSave += EditForm_OnBeforeSave;
        editForm.OnBeforeRedirect += EditForm_OnBeforeRedirect;
    }


    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        if (editForm.IsInsertMode)
        {
            // To prevent from creating multiple versions
            CurrentWorkflow.Generalized.SupportsVersioning = false;
        }
        SaveCustomFormValues();
    }


    protected void EditForm_OnBeforeRedirect(object sender, EventArgs e)
    {
        if (editForm.IsInsertMode)
        {
            WorkflowStepInfoProvider.CreateDefaultWorkflowSteps(CurrentWorkflow);

            // To prevent from creating multiple versions - turn versioning back on
            CurrentWorkflow.Generalized.SupportsVersioning = true;
            // Create one version manually
            if (LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.ObjectVersioning) && ObjectVersionManager.AllowObjectVersioning(CurrentWorkflow))
            {
                ObjectVersionManager.CreateVersion(CurrentWorkflow);
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        LoadCustomFormValues();
        VisibilitySettings();
    }


    /// <summary>
    /// Initializes newly created workflow info.
    /// </summary>
    private void InitializeWorkflow()
    {
        // Null means "Use site settings" - otherwise would be set to False by default UIForm behavior.
        CurrentWorkflow.WorkflowSendEmails = null;
        CurrentWorkflow.WorkflowUseCheckinCheckout = null;
    }


    /// <summary>
    /// Placeholders visibility settings.
    /// </summary>
    private void VisibilitySettings()
    {
        // Show document approval workflow settings
        plcDocSettings.Visible = CurrentWorkflow.IsDocumentWorkflow;
        if (CurrentWorkflow.IsBasic)
        {
            plcAllowedObjects.Visible = false;
            plcAutomation.Visible = false;
        }
        else if (CurrentWorkflow.IsApproval)
        {
            plcVersioning.Visible = false;
            plcAutomation.Visible = false;
        }
        else if (CurrentWorkflow.IsAutomation)
        {
            plcAllowedObjects.Visible = false;
            plcType.Visible = false;
        }

        if (editForm.IsInsertMode)
        {
            plcType.Visible = false;
            plcDocSettings.Visible = false;
            plcAutomation.Visible = false;
        }
    }


    private void SaveCustomFormValues()
    {
        CurrentWorkflow.WorkflowUseCheckinCheckout = ValidationHelper.GetNullableBoolean(rblUseCheckInCheckOut.SelectedValue, null);
        if (rblRecurrenceType.Visible)
        {
            CurrentWorkflow.WorkflowRecurrenceType = (ProcessRecurrenceTypeEnum)Enum.Parse(typeof(ProcessRecurrenceTypeEnum), rblRecurrenceType.SelectedValue);
        }
    }


    /// <summary>
    /// Handles values on workflow info that can not be directly edited by UIForm.
    /// </summary>
    private void LoadCustomFormValues()
    {
        // Set workflow type label
        lblTypeValue.Text = WorkflowHelper.GetWorkflowTypeString(CurrentWorkflow.WorkflowType, CurrentWorkflow.WorkflowAutoPublishChanges);
        rblUseCheckInCheckOut.SelectedValue = CurrentWorkflow.WorkflowUseCheckinCheckout.ToString();
        if (rblRecurrenceType.Visible)
        {
            rblRecurrenceType.SelectedValue = ((int)CurrentWorkflow.WorkflowRecurrenceType).ToString();
        }
    }

    #endregion
}