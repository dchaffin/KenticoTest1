using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.FormControls;
using CMS.ExtendedControls;

public partial class CMSModules_Automation_Controls_Process_List : CMSUserControl
{
    #region "Private variables"

    // Contact site id
    private int contactSiteId = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Type of object of states in list.
    /// </summary>
    public string ObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// ID of object of states in list.
    /// </summary>
    public int ObjectID
    {
        get;
        set;
    }


    /// <summary>
    /// Item edit URL
    /// </summary>
    public string EditActionUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Uni grid
    /// </summary>
    public UniGrid UniGrid
    {
        get
        {
            return gridState;
        }
    }


    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        gridState.OnExternalDataBound += gridState_OnExternalDataBound;
        gridState.OnAction += gridState_OnAction;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        gridState.WhereCondition = String.Format("StateObjectID = {0} AND StateObjectType = '{1}'", ObjectID, SqlHelperClass.GetSafeQueryString(ObjectType));
        gridState.EditActionUrl = EditActionUrl;

        BaseInfo contact = BaseAbstractInfoProvider.GetInfoById(PredefinedObjectType.CONTACT, ObjectID);
        if (contact != null)
        {
            contactSiteId = contact.Generalized.ObjectSiteID;
        }
    }

    #endregion


    #region "Grid events"

    private void gridState_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                int stateId = ValidationHelper.GetInteger(actionArgument, 0);
                AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
                var obj = CMSObjectHelper.GetObjectById(ObjectType, ObjectID);
                var state = AutomationStateInfoProvider.GetAutomationStateInfo(stateId);
                manager.RemoveProcess(obj, state);
                break;
        }
    }


    /// <summary>
    /// External history binding.
    /// </summary>
    protected object gridState_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "stepname":
                return HTMLHelper.HTMLEncode(GetStep(parameter).StepDisplayName);

            case "processname":
                return HTMLHelper.HTMLEncode(GetProcess(parameter).WorkflowDisplayName);

            case "status":
                return AutomationHelper.GetProcessStatus((ProcessStatusEnum)ValidationHelper.GetInteger(parameter, 0));

            case "delete":
                ImageButton btn = (ImageButton)sender;
                btn.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(string.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(ResHelper.GetString(TypeHelper.GetObjectTypeResourceKey(PredefinedObjectType.CONTACT)).ToLowerCSafe()))) + ")) { return false; }";
                if (!WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(contactSiteId)))
                {
                    btn.Enabled = false;
                    btn.Attributes.Add("src", GetImageUrl("Design/Controls/UniGrid/Actions/DeleteDisabled.png"));
                }
                break;
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns step info.
    /// </summary>
    /// <param name="id">Step ID</param>
    /// <returns>Step info object</returns>
    private WorkflowStepInfo GetStep(object id)
    {
        int stepId = ValidationHelper.GetInteger(id, 0);
        return WorkflowStepInfoProvider.GetWorkflowStepInfo(stepId);
    }


    /// <summary>
    /// Returns process info.
    /// </summary>
    /// <param name="id">Process ID</param>
    /// <returns>Workflow info object</returns>
    private WorkflowInfo GetProcess(object id)
    {
        int workflowID = ValidationHelper.GetInteger(id, 0);
        return WorkflowInfoProvider.GetWorkflowInfo(workflowID);
    }

    #endregion
}
