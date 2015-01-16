using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Scheduler;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Scheduler_Controls_UI_List : CMSAdminListControl
{
    #region "Properties"

    /// <summary>
    /// Indicates whether list should display standard or system tasks.
    /// </summary>
    public bool SystemTasks
    {
        get;
        set;
    }


    /// <summary>
    /// Site ID or selected site ID.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// URL to task edit page.
    /// </summary>
    public string EditURL
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        gridElem.WhereCondition = GenerateWhereCondition();

        if (SystemTasks)
        {
            gridElem.ObjectType = SchedulerObjectType.SYSTEMSCHEDULEDTASKLIST;
        }
        else
        {
            gridElem.ObjectType = SchedulerObjectType.SCHEDULEDTASK;

            // Some non-object scheduled tasks are system
            gridElem.WhereCondition = SqlHelperClass.AddWhereCondition(gridElem.WhereCondition, "TaskType IS NULL OR TaskType != " + (int)ScheduledTaskTypeEnum.System);
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                if (!String.IsNullOrEmpty(EditURL))
                {
                    URLHelper.Redirect(String.Format(EditURL, actionArgument.ToString()));
                }
                break;

            case "delete":
                {
                    // Check "modify" permission
                    if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.ScheduledTasks", "Modify"))
                    {
                        RedirectToAccessDenied("CMS.ScheduledTasks", "Modify");
                    }

                    // Delete the task
                    try
                    {
                        int taskId = Convert.ToInt32(actionArgument);

                        TaskInfo ti = TaskInfoProvider.GetTaskInfo(taskId);
                        if (ti != null)
                        {
                            ti.Generalized.LogSynchronization = SynchronizationTypeEnum.LogSynchronization;
                            ti.Generalized.LogIntegration = true;
                            ti.Generalized.LogEvents = true;
                            TaskInfoProvider.DeleteTaskInfo(ti);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(GetString("Task_List.DeleteError"), ex.Message, null);
                    }
                }
                break;

            case "execute":
                {
                    // Check "modify" permission
                    if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.ScheduledTasks", "Modify"))
                    {
                        RedirectToAccessDenied("CMS.ScheduledTasks", "Modify");
                    }

                    TaskInfo ti = TaskInfoProvider.GetTaskInfo(Convert.ToInt32(actionArgument));
                    if (ti != null)
                    {
                        SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                        if (!ti.TaskEnabled)
                        {
                            // Task is not enabled (won't be executed at the end of request), run it now
                            SchedulingExecutor.ExecuteTask(ti.TaskID, (si != null) ? si.SiteName : CMSContext.CurrentSiteName);
                        }
                        else
                        {
                            TaskInterval interval = new TaskInterval();
                            interval = SchedulingHelper.DecodeInterval(ti.TaskInterval);

                            if ((ti.TaskNextRunTime != DateTimeHelper.ZERO_TIME) || (interval.Period == SchedulingHelper.PERIOD_ONCE))
                            {
                                ti.TaskNextRunTime = DateTime.Now;

                                // Update the task
                                TaskInfoProvider.SetTaskInfo(ti);

                                // Run the task
                                SchedulingTimer.RunSchedulerImmediately = true;
                                if (si != null)
                                {
                                    SchedulingTimer.SchedulerRunImmediatelySiteName = si.SiteName;
                                }
                            }
                            else
                            {
                                ShowWarning(GetString("ScheduledTask.TaskAlreadyrunning"), null, null);
                                return;
                            }
                        }

                        ShowConfirmation(GetString("ScheduledTask.WasExecuted"));
                    }
                }
                break;
        }
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv = null;
        bool taskEnabled = false;

        switch (sourceName.ToLowerCSafe())
        {
            case "useexternalservice":
                // Use external service
                {
                    ImageButton imgButton = sender as ImageButton;
                    if (imgButton != null)
                    {
                        bool visible = false;
                        // Only if setting 'Use external service' is allowed
                        if (SchedulingHelper.UseExternalService)
                        {
                            drv = UniGridFunctions.GetDataRowView(imgButton.Parent as DataControlFieldCell);
                            if (drv != null)
                            {
                                // Indicates whether the task is processed by an external service
                                bool taskUseExternalService = ValidationHelper.GetBoolean(drv["TaskUseExternalService"], false);
                                // Indicates whether the task is enabled
                                taskEnabled = ValidationHelper.GetBoolean(drv["TaskEnabled"], false);

                                if (taskUseExternalService && taskEnabled)
                                {
                                    DateTime taskLastRunTime = ValidationHelper.GetDateTime(drv["TaskLastRunTime"], DateTimeHelper.ZERO_TIME);

                                    if (taskLastRunTime != DateTimeHelper.ZERO_TIME)
                                    {
                                        // Task period
                                        string interval = ValidationHelper.GetString(drv["TaskInterval"], null);
                                        DateTime now = DateTime.Now;
                                        TimeSpan period = SchedulingHelper.GetNextTime(interval, taskLastRunTime, now).Subtract(now);
                                        // Actual different time between now date and last run time
                                        TimeSpan actualDifferent = now.Subtract(taskLastRunTime);

                                        // Show image if actual different time is three times larger than task period
                                        if ((period.TotalSeconds > 0) && (actualDifferent.TotalSeconds > (period.TotalSeconds * 3)))
                                        {
                                            imgButton.ToolTip = GetString("scheduledtask.useservicewarning");
                                            visible = true;
                                        }
                                    }
                                }
                            }
                        }

                        imgButton.Visible = visible;
                        if (imgButton.Visible)
                        {
                            imgButton.OnClientClick = "return false;";
                            imgButton.Style.Add(HtmlTextWriterStyle.Cursor, "default");
                        }
                    }
                }
                break;

            case "taskexecutions":
                if (string.IsNullOrEmpty(Convert.ToString(parameter)))
                {
                    return 0;
                }
                break;

            case "runactions":
                // Image "run" button
                ImageButton runButton = ((ImageButton)sender);

                // Data row and task enabled value
                drv = UniGridFunctions.GetDataRowView(runButton.Parent as DataControlFieldCell);
                taskEnabled = ValidationHelper.GetBoolean(drv["TaskEnabled"], false);

                if (!taskEnabled)
                {
                    // If not enabled add confirmation dialog
                    runButton.OnClientClick = "if (!confirm('" + GetString("taskdisabled.sure") + "')) return false;";
                }

                break;

        }
        return parameter;
    }


    /// <summary>
    /// Generates where condition for unigrid.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        if (SiteID > 0)
        {
            return "TaskSiteID = " + SiteID;
        }
        else
        {
            return "TaskSiteID IS NULL";
        }
    }
}