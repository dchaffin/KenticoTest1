using System;
using System.Data.SqlTypes;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.Scheduler;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

using TextBoxControl = CMSFormControls_Basic_TextBoxControl;

public partial class CMSModules_Scoring_Controls_UI_Score_Edit : CMSAdminEditControl
{
    #region "Variables"

    private TaskInfo task = null;

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

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditForm.OnBeforeSave += new EventHandler(EditForm_OnBeforeSave);
        EditForm.OnAfterSave += new EventHandler(EditForm_OnAfterSave);
        InitHeaderActions();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get edited score
        ScoreInfo score = (ScoreInfo)EditForm.EditedObject;

        EditForm.OnItemValidation += EditForm_OnItemValidation;

        string script = @"
function StartRefreshTimer()
{
    var timer = $find('" + timRefresh.ClientID + @"');
    timer._startTimer();
}
function StopRefreshTimer()
{
    var timer = $find('" + timRefresh.ClientID + @"');
    timer._stopTimer();
}

function OnRecalculateButtonClick()
{
    StopRefreshTimer();
    if (confirm(" + ScriptHelper.GetString(GetString("om.score.recalculatewarning")) + @"))
    {
        return true;
    }
    else
    {
        StartRefreshTimer();
        return false;
    }
    
}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RecalculateScoreTimer", ScriptHelper.GetScript(script));

        // Get scheduled task
        if (score.ScoreScheduledTaskID > 0)
        {
            task = TaskInfoProvider.GetTaskInfo(score.ScoreScheduledTaskID);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Hide advanced properties when creating new score
            plcUpdate.Visible = (score.ScoreID > 0);

            chkSchedule.Checked = schedulerInterval.Visible = ((task != null) && task.TaskEnabled);

            if (schedulerInterval.Visible)
            {
                // Initialize interval control
                schedulerInterval.ScheduleInterval = task.TaskInterval;
            }
        }

        if (score.ScoreID > 0)
        {
            // Display info panel
            pnlInfo.Visible = true;

            // Display basic info about score
            InitInfoPanel(score);
        }

        // Set tab name of editing UI to refresh upon change
        EditForm.RefreshTab = GetString("general.general");

        var sendAtScoreControl = (TextBoxControl)fSendAtScore.EditingControl;
        sendAtScoreControl.MaxLength = ValidationHelper.MAX_INT_LENGTH;
        sendAtScoreControl.FilterType = AjaxControlToolkit.FilterTypes.Numbers | AjaxControlToolkit.FilterTypes.Custom;
        sendAtScoreControl.ValidChars = "-";

        // Allow multiple addresses in e-mail input control
        fEmail.EditingControl.SetValue("allowmultipleaddresses", true);
    }


    void EditForm_OnItemValidation(object sender, ref string errorMessage)
    {
        Control ctrl = (Control)sender;
        if (CMSString.Compare(ctrl.ID, "fcScoreNotificationEmail", true) == 0 || CMSString.Compare(ctrl.ID, "fcScoreEmailAtScore", true) == 0)
        {
            var sendAtScoreControl = (TextBoxControl)fSendAtScore.EditingControl;
            var emailControl = (FormEngineUserControl)fEmail.EditingControl;

            if (String.IsNullOrEmpty(sendAtScoreControl.Value.ToString()) != String.IsNullOrEmpty(emailControl.Value.ToString()))
            {
                errorMessage = GetString("om.score.requiredemailandscore");
            }
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        if ((EditForm.EditedObject != null))
        {
            if (EditForm.EditedObject.Generalized.ObjectID == 0)
            {
                // Set site ID only when creating new object
                EditForm.Data["ScoreSiteID"] = CMSContext.CurrentSiteID;
                // Set 'New' status
                EditForm.Data["ScoreStatus"] = 2;
            }
        }
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        // Get edited contact group
        ScoreInfo score = (ScoreInfo)EditForm.EditedObject;

        // Set info for scheduled task
        task = ScoreInfoProvider.EnsureScheduledTask(score, schedulerInterval.ScheduleInterval, TaskInfoProvider.NO_TIME, false, false);

        // Update scheduled task
        if (chkSchedule.Checked)
        {
            if (!schedulerInterval.CheckOneDayMinimum())
            {
                // If problem occurred while setting schedule interval
                EditForm.ShowError(GetString("Newsletter_Edit.NoDaySelected"));
                EditForm.StopProcessing = true;
                return;
            }

            if (!IsValidDate(SchedulingHelper.DecodeInterval(scheduleInterval).StartTime))
            {
                // Start date is not in valid format
                EditForm.ShowError(GetString("Newsletter.IncorrectDate"));
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

        score.ScoreScheduledTaskID = task.TaskID;
        pnlInfo.Visible = true;
        InitInfoPanel(score);

        // Update score
        ScoreInfoProvider.SetScoreInfo(score);

        InitHeaderActions();
        ((CMSPage)Page).CurrentMaster.HeaderActions.ReloadData();
    }


    /// <summary>
    /// Checkbox chkSchedule event handler.
    /// </summary>
    protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
    {
        if (chkSchedule.Checked)
        {
            string script = @"alert(" + ScriptHelper.GetString(GetString("om.score.recalculatewarning")) + @");";

            ScriptHelper.RegisterClientScriptBlock(chkSchedule, typeof(string), "ChkScheduleChange", ScriptHelper.GetScript(script));
        }

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
    private void InitHeaderActions()
    {
        bool isNew = ((EditForm.EditedObject != null) && (EditForm.EditedObject.Generalized.ObjectID == 0));

        // Header actions
        string[,] actions;
        if (isNew)
        {
            actions = new string[1, 11];
        }
        else
        {
            actions = new string[2, 11];
        }

        // Init save button
        actions[0, 0] = HeaderActions.TYPE_SAVEBUTTON;
        actions[0, 1] = GetString("General.Save");
        actions[0, 5] = GetImageUrl("CMSModules/CMS_Content/EditMenu/save.png");
        actions[0, 6] = "save";
        actions[0, 8] = "true";

        if (!isNew)
        {
            // Init recalculate button
            actions[1, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[1, 1] = GetString("om.score.recalculate");
            actions[1, 2] = "return OnRecalculateButtonClick();";
            actions[1, 5] = GetImageUrl("CMSModules/CMS_ContactManagement/evaluate.png");
            actions[1, 6] = "recalculate";
            actions[1, 8] = "false";
        }

        ((CMSPage)Page).CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        ((CMSPage)Page).CurrentMaster.HeaderActions.Actions = actions;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check modify permission
        CheckPermissions("cms.scoring", "modify");

        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                if (chkSchedule.Checked)
                {
                    // Get scheduled interval string
                    scheduleInterval = schedulerInterval.ScheduleInterval;
                }

                // Save changes in the contact group
                EditForm.SaveData(null);
                break;

            case "recalculate":
                if (EditForm.EditedObject != null)
                {
                    ScoreInfo score = (ScoreInfo)EditForm.EditedObject;
                    if (score != null)
                    {
                        // Set 'Recalculating' status
                        score.ScoreStatus = ScoreStatusEnum.Recalculating;
                        // Ensure scheduled task
                        if (score.ScoreScheduledTaskID == 0)
                        {
                            // Create and initialize new scheduled task
                            task = ScoreInfoProvider.EnsureScheduledTask(score, schedulerInterval.ScheduleInterval, TaskInfoProvider.NO_TIME, false, true);

                            // Update score info
                            score.ScoreScheduledTaskID = task.TaskID;
                        }
                        ScoreInfoProvider.SetScoreInfo(score);

                        // Recalculate the score
                        ScoreEvaluator evaluator = new ScoreEvaluator();
                        evaluator.ScoreID = score.ScoreID;
                        evaluator.CheckScoreEnabled = false;
                        evaluator.Execute(null);

                        EditForm.ShowConfirmation(GetString("om.score.recalculationstarted"));

                        // Get scheduled task and update last run time
                        if (task == null)
                        {
                            task = TaskInfoProvider.GetTaskInfo(score.ScoreScheduledTaskID);
                        }
                        if (task != null)
                        {
                            task.TaskLastRunTime = DateTime.Now;
                            TaskInfoProvider.SetTaskInfo(task);

                            // Initialize interval control
                            schedulerInterval.ScheduleInterval = task.TaskInterval;
                            schedulerInterval.ReloadData();
                        }

                        // Display basic info about score
                        InitInfoPanel(score);
                    }
                }
                break;
        }
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
    /// <param name="score">ScoreInfo object</param>
    private void InitInfoPanel(ScoreInfo score)
    {
        if (score != null)
        {
            pnlInfo.GroupingText = GetString("om.score.info");
            pnlInfo.Attributes.Add("style", "margin:0 30px;");

            // Last evaluation time
            if ((task != null) && (task.TaskLastRunTime != DateTimeHelper.ZERO_TIME))
            {
                lblLastEvalValue.Text = task.TaskLastRunTime.ToString();
            }
            else
            {
                lblLastEvalValue.Text = GetString("general.na");
            }

            // Display score status...
            switch (score.ScoreStatus)
            {
                case ScoreStatusEnum.Recalculating:
                    // Status and progress if the status is 'Recalculating'
                    ltrProgress.Text = String.Empty;

                    string buildStr = GetString("om.score.recalculating");
                    ltrProgress.Text = "<img style=\"width:12px;height:12px;\" src=\"" + UIHelper.GetImageUrl(Page, "Design/Preloaders/preload16.gif") + "\" alt=\"" + buildStr + "\" tooltip=\"" + buildStr + "\"  />";
                    lblStatusValue.Text = "<span class=\"StatusDisabled\">" + buildStr + "</span>";
                    break;

                case ScoreStatusEnum.Ready:
                    // 'Ready' status
                    lblStatusValue.Text = "<span class=\"StatusEnabled\">" + GetString("om.contactgroup.ready") + "</span>";
                    break;

                case ScoreStatusEnum.New:
                    // 'Condition changed' status
                    lblStatusValue.Text = "<span class=\"StatusDisabled\">" + GetString("om.score.recalcrequired") + "</span>";
                    break;

                case ScoreStatusEnum.Failed:
                    // 'Recalculation failed' status
                    lblStatusValue.Text = "<span class=\"StatusDisabled\" title=\"" + GetString("general.seeeventlog") + "\">" + GetString("om.score.recalcfailed") + "</span>";
                    break;

                case ScoreStatusEnum.Unspecified:
                    // 'Unspecified' status
                    lblStatusValue.Text = "<span class=\"StatusDisabled\">" + GetString("general.na") + "</span>";
                    break;
            }
        }
    }

    #endregion
}