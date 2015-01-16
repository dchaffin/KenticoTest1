using System;
using System.Data;
using System.Collections;
using System.Data.SqlTypes;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.Scheduler;
using CMS.SettingsProvider;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_ContactGroup_Edit : CMSAdminEditControl
{
    #region "Variables"

    private int mSiteID = 0;

    private int deleteScheduledTaskId = 0;

    private TaskInfo task = null;

    private HeaderAction mBtnEvaluate;

    private HeaderAction mBtnSave;

    string scheduleInterval = string.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return EditForm;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }

    /// <summary>
    /// SiteID of current contact group.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
            if ((mSiteID > 0) && !CMSContext.CurrentUser.UserSiteManagerAdmin)
            {
                mSiteID = CMSContext.CurrentSiteID;
            }
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditForm.OnBeforeSave += new EventHandler(EditForm_OnBeforeSave);
        EditForm.OnAfterSave += new EventHandler(EditForm_OnAfterSave);
        InitHeaderActions(false);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string url = "Frameset.aspx?groupid={%EditedObject.ID%}&saved=1";
        url = URLHelper.AddParameterToUrl(url, "siteid", SiteID.ToString());
        if (ContactHelper.IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "issitemanager", "1");
        }
        EditForm.RedirectUrlAfterCreate = url;

        // Get edited contact group
        ContactGroupInfo cgi = (ContactGroupInfo)EditForm.EditedObject;

        // Get scheduled task
        if (cgi.ContactGroupScheduledTaskID > 0)
        {
            task = TaskInfoProvider.GetTaskInfo(cgi.ContactGroupScheduledTaskID);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Hide dialog for condition when creating new contact group
            plcUpdate.Visible = (cgi.ContactGroupID > 0);

            chkSchedule.Checked = schedulerInterval.Visible = ((task != null) && task.TaskEnabled);

            if (schedulerInterval.Visible)
            {
                // Initialize interval control
                schedulerInterval.ScheduleInterval = task.TaskInterval;
            }
        }

        // Set proper resolver to condition builder
        conditionBuilder.ResolverName = "ContactResolver";

        if (task != null)
        {
            // Display info panel for dynamic contact group
            pnlInfo.Visible = true;

            // Display basic info about dynamic contact group
            InitInfoPanel(cgi, false);
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Set site ID only when creating new object
        if ((EditForm.EditedObject != null))
        {
            int groupId = EditForm.EditedObject.Generalized.ObjectID;

            if (groupId == 0)
            {
                if (SiteID > 0)
                {
                    EditForm.Data["ContactGroupSiteID"] = SiteID;
                }
                else
                {
                    EditForm.Data["ContactGroupSiteID"] = DBNull.Value;
                }
            }
            else
            {
                if (!chkDynamic.Checked)
                {
                    // Remove dynamic condition
                    EditForm.Data.SetValue("ContactGroupDynamicCondition", null);

                    // Remove dynamicaly created members
                    if (ValidationHelper.GetBoolean(hdnConfirmDelete.Value, false))
                    {
                        ContactGroupMemberInfoProvider.DeleteContactGroupMembers("ContactGroupMemberContactGroupID = " + groupId + " AND (ContactGroupMemberFromCondition = 1 AND (ContactGroupMemberFromAccount = 0 OR ContactGroupMemberFromAccount IS NULL) AND (ContactGroupMemberFromManual = 0 OR ContactGroupMemberFromManual IS NULL))", groupId, false, false);
                    }
                }
                else
                {
                    // Get new condition
                    string condition = ((FormEngineUserControl)EditForm.FieldControls["ContactGroupDynamicCondition"]).Value.ToString();

                    // Display error if the condition is empty
                    if (string.IsNullOrEmpty(condition))
                    {
                        EditForm.StopProcessing = true;
                        EditForm.ErrorLabel.Text = GetString("om.contactgroup.nocondition");
                    }
                    else
                    {
                        // Get current object to compare dynamic conditions
                        ContactGroupInfo currentGroup = ContactGroupInfoProvider.GetContactGroupInfo(EditForm.EditedObject.Generalized.ObjectID);
                        if ((currentGroup != null) && (!condition.EqualsCSafe(currentGroup.ContactGroupDynamicCondition, true)))
                        {
                            // Set 'Rebuild required' status
                            EditForm.Data["ContactGroupStatus"] = 2;
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        // Get edited contact group
        ContactGroupInfo cgi = (ContactGroupInfo)EditForm.EditedObject;

        if (chkDynamic.Checked)
        {
            // Set info for scheduled task
            task = GetScheduledTask(cgi);

            // Update scheduled task
            if (chkSchedule.Checked)
            {
                if (!schedulerInterval.CheckOneDayMinimum())
                {
                    // If problem occurred while setting schedule interval
                    EditForm.ErrorLabel.Text = GetString("Newsletter_Edit.NoDaySelected");
                    EditForm.ErrorLabel.Visible = true;
                    EditForm.StopProcessing = true;
                    return;
                }

                if (!IsValidDate(SchedulingHelper.DecodeInterval(scheduleInterval).StartTime))
                {
                    // Start date is not in valid format
                    EditForm.ErrorLabel.Text = GetString("Newsletter.IncorrectDate");
                    EditForm.ErrorLabel.Visible = true;
                    EditForm.StopProcessing = true;
                    return;
                }

                task.TaskInterval = scheduleInterval;
                task.TaskNextRunTime = SchedulingHelper.GetNextTime(task.TaskInterval, new DateTime(), new DateTime());
                task.TaskEnabled = true;
            }
            else
            {
                task.TaskInterval = scheduleInterval;
                task.TaskNextRunTime = TaskInfoProvider.NO_TIME;
                task.TaskEnabled = false;
            }
            TaskInfoProvider.SetTaskInfo(task);

            cgi.ContactGroupScheduledTaskID = task.TaskID;
            pnlInfo.Visible = true;
            InitInfoPanel(cgi, true);
        }
        else
        {
            if (cgi.ContactGroupScheduledTaskID > 0)
            {
                // Store task ID for deletion
                deleteScheduledTaskId = cgi.ContactGroupScheduledTaskID;
            }
            cgi.ContactGroupScheduledTaskID = 0;
            cgi.ContactGroupStatus = ContactGroupStatusEnum.Unspecified;
            schedulerInterval.Visible = false;
            pnlInfo.Visible = false;
        }

        // Update contact group
        ContactGroupInfoProvider.SetContactGroupInfo(cgi);

        if (deleteScheduledTaskId > 0)
        {
            // Delete scheduled task if schedule evaluation was unchecked
            TaskInfoProvider.DeleteTaskInfo(deleteScheduledTaskId);
        }

        InitHeaderActions(false);
        ((CMSPage)Page).CurrentMaster.HeaderActions.ReloadData();

        // Refresh breadcrumbs after data are loaded
        ScriptHelper.RefreshTabHeader(Page, null);
    }


    /// <summary>
    /// Checkbox chkDynamic event handler.
    /// </summary>
    protected void chkDynamic_CheckedChanged(object sender, EventArgs e)
    {
        plcDynamic.Visible = chkDynamic.Checked;

        // Set confirmation dialog
        if (!chkDynamic.Checked)
        {
            ContactGroupInfo cgi = ContactGroupInfoProvider.GetContactGroupInfo(EditForm.EditedObject.Generalized.ObjectID);
            if ((cgi != null) && (!String.IsNullOrEmpty(cgi.ContactGroupDynamicCondition)))
            {
                InitHeaderActions(true);
                ((CMSPage)Page).CurrentMaster.HeaderActions.ReloadData();
            }
        }
    }


    /// <summary>
    /// Checkbox chkSchedule event handler.
    /// </summary>
    protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
    {
        schedulerInterval.Visible = chkSchedule.Checked;

        if (schedulerInterval.Visible && (task != null))
        {
            // Set scheduled interval
            schedulerInterval.ScheduleInterval = task.TaskInterval;
            schedulerInterval.ReloadData();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions(bool confirmDelete)
    {
        bool isDynamic = !String.IsNullOrEmpty(ValidationHelper.GetString(EditForm.Data["ContactGroupDynamicCondition"], null));

        if (!RequestHelper.IsPostBack())
        {
            plcDynamic.Visible = chkDynamic.Checked = isDynamic;
        }

        mBtnSave = mBtnSave ?? new SaveAction(Page);

        if (confirmDelete)
        {
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "ConfirmDelete", ScriptHelper.GetScript(
                @"
function ConfirmDelete()
{
    document.getElementById('" + hdnConfirmDelete.ClientID + "').value = confirm('" + GetString("om.contactgroup.ConfirmDelete") + @"');
}"));
            mBtnSave.OnClientClick = "ConfirmDelete();";
        }
        else
        {
            mBtnSave.OnClientClick = String.Empty;
        }

        mBtnEvaluate = mBtnEvaluate ?? new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("om.contacgroup.rebuild"),
            ImageUrl = GetImageUrl("CMSModules/CMS_ContactManagement/evaluate.png"),
            CommandName = "evaluate",
            CommandArgument = "false",
            Visible = isDynamic
        };

        HeaderActions.AddAction(mBtnSave);
        HeaderActions.AddAction(mBtnEvaluate);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (ContactGroupHelper.AuthorizedModifyContactGroup(SiteID, true))
        {
            switch (e.CommandName.ToLowerCSafe())
            {
                case "save":
                    if (chkDynamic.Checked)
                    {
                        // Get scheduled interval string
                        scheduleInterval = schedulerInterval.ScheduleInterval;
                    }

                    // Save changes in the contact group
                    if (EditForm.SaveData(null))
                    {
                        mBtnEvaluate.Visible = chkDynamic.Checked;
                        ScriptHelper.RefreshTabHeader(Page, null);
                    }
                    break;

                case "evaluate":
                    if (EditForm.EditedObject != null)
                    {
                        ContactGroupInfo cgi = (ContactGroupInfo)EditForm.EditedObject;
                        if (cgi != null)
                        {
                            // Set 'Rebuilding' status
                            cgi.ContactGroupStatus = ContactGroupStatusEnum.Rebuilding;
                            ContactGroupInfoProvider.SetContactGroupInfo(cgi);

                            // Evaluate the membership of the contact group
                            ContactGroupEvaluator evaluator = new ContactGroupEvaluator();
                            evaluator.ContactGroupID = cgi.ContactGroupID;
                            evaluator.Execute(null);

                            EditForm.InfoLabel.Text = GetString("om.contactgroup.evaluationstarted");
                            EditForm.InfoLabel.Visible = true;

                            // Get scheduled task and update last run time
                            if (cgi.ContactGroupScheduledTaskID > 0)
                            {
                                task = TaskInfoProvider.GetTaskInfo(cgi.ContactGroupScheduledTaskID);
                                if (task != null)
                                {
                                    task.TaskLastRunTime = DateTime.Now;
                                    TaskInfoProvider.SetTaskInfo(task);

                                    // Initialize interval control
                                    schedulerInterval.ScheduleInterval = task.TaskInterval;
                                    schedulerInterval.ReloadData();
                                }
                            }

                            // Display basic info about dynamic contact group
                            InitInfoPanel(cgi, false);
                        }
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Returns scheduled task of the contact group or creates new one.
    /// </summary>
    /// <param name="cgi">Contact group info</param>
    private TaskInfo GetScheduledTask(ContactGroupInfo cgi)
    {
        if (cgi == null)
        {
            return null;
        }

        if (cgi.ContactGroupScheduledTaskID > 0)
        {
            return TaskInfoProvider.GetTaskInfo(ValidationHelper.GetInteger(cgi.ContactGroupScheduledTaskID, 0)) ??
                   CreateScheduledTask(cgi);
        }
        else
        {
            return CreateScheduledTask(cgi);
        }
    }


    /// <summary>
    /// Creates new scheduled task with basic properties set.
    /// </summary>
    /// <param name="cgi">Contact group info</param>
    private TaskInfo CreateScheduledTask(ContactGroupInfo cgi)
    {
        return new TaskInfo()
                   {
                       TaskAssemblyName = "CMS.OnlineMarketing",
                       TaskClass = "CMS.OnlineMarketing.ContactGroupEvaluator",
                       TaskEnabled = true,
                       TaskLastResult = string.Empty,
                       TaskSiteID = cgi.ContactGroupSiteID,
                       TaskData = cgi.ContactGroupID.ToString(),
                       TaskDisplayName = "Contact group '" + cgi.ContactGroupDisplayName + "' rebuild",
                       TaskName = "ContactGroupRebuild_" + cgi.ContactGroupName,
                       TaskType = ScheduledTaskTypeEnum.System
                   };
    }


    /// <summary>
    /// Returns if start date of the scheduled interval is valid.
    /// </summary>
    /// <param name="date">Start date of the scheduled interval</param>
    private bool IsValidDate(DateTime date)
    {
        return (date > SqlDateTime.MinValue.Value) && (date < SqlDateTime.MaxValue.Value);
    }


    /// <summary>
    /// Initializes panel with basic info.
    /// </summary>
    /// <param name="cgi">Contact group info object</param>
    /// <param name="forceReload">If TRUE number of group members is reloaded</param>
    private void InitInfoPanel(ContactGroupInfo cgi, bool forceReload)
    {
        if (cgi != null && task != null)
        {
            pnlInfo.GroupingText = GetString("om.contactgroup.info");
            pnlInfo.Attributes.Add("style", "margin:0 30px;");

            // Last evaluation time
            if (task.TaskLastRunTime != DateTimeHelper.ZERO_TIME)
            {
                lblLastEvalValue.Text = task.TaskLastRunTime.ToString();
            }
            else
            {
                lblLastEvalValue.Text = GetString("general.na");
            }

            // Display contact group status...
            switch (cgi.ContactGroupStatus)
            {
                case ContactGroupStatusEnum.Rebuilding:
                    // Status and progress if the status is 'Rebuilding'
                    ltrProgress.Text = String.Empty;

                    string buildStr = GetString("om.contactgroup.rebuilding");
                    ltrProgress.Text = "<img style=\"width:12px;height:12px;\" src=\"" + UIHelper.GetImageUrl(Page, "Design/Preloaders/preload16.gif") + "\" alt=\"" + buildStr + "\" tooltip=\"" + buildStr + "\"  />";
                    lblStatusValue.Text = "<span class=\"StatusDisabled\">" + buildStr + "</span>";
                    break;

                case ContactGroupStatusEnum.Ready:
                    // 'Ready' status
                    lblStatusValue.Text = "<span class=\"StatusEnabled\">" + GetString("om.contactgroup.ready") + "</span>";
                    break;

                case ContactGroupStatusEnum.ConditionChanged:
                    // 'Condition changed' status
                    lblStatusValue.Text = "<span class=\"StatusDisabled\">" + GetString("om.contactgroup.rebuildrequired") + "</span>";
                    break;
            }

            // Display current number of contacts in the group
            if (!RequestHelper.IsPostBack() || (cgi.ContactGroupStatus == ContactGroupStatusEnum.Rebuilding)
                || (DateTime.Now.Subtract(cgi.ContactGroupLastModified).TotalSeconds < 5) || forceReload)
            {
                lblNumberOfItemsValue.Text = ContactGroupMemberInfoProvider.GetNumberOfContactsInGroup(cgi.ContactGroupID, true).ToString();
            }
        }
    }

    #endregion
}