using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.WorkflowEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.GlobalHelper;

// Parent object
[EditedObject(WorkflowObjectType.AUTOMATIONPROCESS, "processId")]

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Trigger_List : CMSAutomationPage
{
    #region "Properties"

    /// <summary>
    ///  Whether adding triggers is allowed.
    /// </summary>
    protected bool IsAddingAllowed
    { 
        get
        {
            return SelectedSiteID > 0 || SelectedSiteID == UniSelector.US_GLOBAL_RECORD;
        }
    }


    /// <summary>
    /// ID of selected site.
    /// </summary>
    protected int SelectedSiteID
    {
        get
        {
            if (IsSiteManager)
            {
                return siteSelector.SiteID;
            } 
            else 
            {
                return siteOrGlobalSelector.SiteID;
            }
        }
        set
        {
            if (IsSiteManager)
            {
                siteSelector.SiteID = value;
            }
            else
            {
                siteOrGlobalSelector.SiteID = value;
            }
        }
    }


    /// <summary>
    /// Where condition on selected site.
    /// </summary>
    protected string SiteWhereCondition
    {
        get
        {
            if (IsSiteManager)
            {
                return siteSelector.GetSiteWhereCondition("TriggerSiteID");
            }
            else
            {
                return siteOrGlobalSelector.GetSiteWhereCondition("TriggerSiteID");
            }
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        siteOrGlobalSelector.Visible = !IsSiteManager;
        siteSelector.Visible = IsSiteManager;

        // Initialize site selector with site id from query string
        SelectedSiteID = SiteID;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int processId = QueryHelper.GetInteger("processId", 0);

        gridElem.WhereCondition = "TriggerWorkflowID = " + processId;

        // Add query parameters for breadcrumb to edit link
        gridElem.EditActionUrl = AddSiteQuery(gridElem.EditActionUrl, SelectedSiteID) + "&processId=" + processId;

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;

        if (CurrentUser.IsGlobalAdministrator)
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;
            gridElem.WhereCondition = SqlHelperClass.AddWhereCondition(gridElem.WhereCondition, SiteWhereCondition);
        }
        else
        {
            gridElem.WhereCondition = SqlHelperClass.AddWhereCondition(gridElem.WhereCondition, "TriggerSiteID = " + SiteID);
        }

        // Add new action
        headerActions.AddAction(new HeaderAction()
        {
            ImageUrl = GetImageUrl("Objects/CMS_ObjectWorkflowTrigger/add.png"),
            Text = GetString("ma.trigger.new"),
            RedirectUrl = AddSiteQuery("Edit.aspx?processId=" + processId, SelectedSiteID),
            Enabled = IsAddingAllowed
        });        
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Check permissions to create new record
        if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
        {
            headerActions.Enabled = false;
        }

        // Allow new button only for particular sites or (global) site
        else if (!IsAddingAllowed)
        {
            headerActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
    }


    private void gridElem_OnBeforeDataReload()
    {
        // Show site column only when "all" or "global and this site" are selected
        gridElem.NamedColumns["sitename"].Visible = ((SelectedSiteID < 0) && (SelectedSiteID != UniSelector.US_GLOBAL_RECORD));
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "condition":
                return MacroRuleTree.GetRuleText(ValidationHelper.GetString(parameter, String.Empty));

            case "type":
                DataRowView row = parameter as DataRowView;
                if (row != null)
                {
                    ObjectWorkflowTriggerInfo trigger = new ObjectWorkflowTriggerInfo(row.Row);
                    if (!string.IsNullOrEmpty(trigger.TriggerTargetObjectType))
                    {
                        return GetTriggerDescription(trigger);
                    }
                    else
                    {
                        return AutomationHelper.GetTriggerName(trigger.TriggerType, trigger.TriggerObjectType);
                    }
                }
                return parameter;

            case "delete":
                if (!WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
                {
                    ImageButton btn = (ImageButton)sender;
                    btn.Enabled = false;
                    btn.Attributes.Add("src", GetImageUrl("Design/Controls/UniGrid/Actions/DeleteDisabled.png"));
                }
                return parameter;

            default:
                return parameter;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns transformation for trigger type.
    /// </summary>
    /// <param name="trigger">Trigger info object</param>
    private object GetTriggerDescription(ObjectWorkflowTriggerInfo trigger)
    {
        ObjectTransformation tr = new ObjectTransformation(trigger.TriggerTargetObjectType, trigger.TriggerTargetObjectID);
        string objectType = GetString(TypeHelper.GetObjectTypeResourceKey(trigger.TriggerObjectType));
        switch (trigger.TriggerObjectType)
        {
            case PredefinedObjectType.SCORE:
                tr.Transformation = string.Format("{0} '{{% DisplayName %}}' {1} {2} '{3}'", objectType, GetString("ma.trigger.exceeded").ToLowerCSafe(), GetString("General.Value").ToLowerCSafe(), trigger.TriggerParameters["ScoreValue"]);
                return tr;

            case PredefinedObjectType.ACTIVITY:
                if(trigger.TriggerTargetObjectID == 0)
                {
                    return GetString("ma.trigger.anyActivityPerformed");
                }
                tr.Transformation = string.Format("{0} '{{% DisplayName %}}' {1}", objectType, GetString("ma.trigger.performed").ToLowerCSafe());
                return tr;
        }
        return null;
    }

    #endregion
}
