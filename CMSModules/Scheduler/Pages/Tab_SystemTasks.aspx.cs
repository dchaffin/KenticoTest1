using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.GlobalHelper;
using CMS.Scheduler;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Scheduler_Pages_Tab_SystemTasks : CMSScheduledTasksPage
{
    #region "Variables"

    private SiteInfo si = null;

    // Actions event names
    private const string TASKS_RESTART_TIMER = "TASKSRESTART";
    private const string TASKS_RUN = "TASKSRUN";

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Local header actions
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return headerActions;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        rightHeaderActions.ActionPerformed += rightHeaderActions_ActionPerformed;            

        // Show info that scheduler is disabled
        if (!SchedulingHelper.EnableScheduler)
        {
            ShowWarning(GetString("scheduledtask.disabled"), null, null);
        }

        // When no site is selected, load for Current site
        if ((SiteID == 0) && (SelectedSiteID == 0))
        {
            SelectedSiteID = CMSContext.CurrentSiteID;
        }

        int siteId = (SiteID > 0) ? SiteID : SelectedSiteID;

        si = SiteInfoProvider.GetSiteInfo(siteId);

        listElem.SiteID = siteId;
        listElem.EditURL = "Task_Edit.aspx?taskname={0}&" + GetSiteOrSelectedSiteQuery();

        // Refresh action
        HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("General.Refresh"),
            ImageUrl = GetImageUrl("Objects/CMS_ScheduledTask/refresh.png"),
            RedirectUrl = "Tab_SystemTasks.aspx?" + GetSiteOrSelectedSiteQuery()
        });

        if (si != null)
        {
            bool usesTimer = SchedulingHelper.UseAutomaticScheduler || !SchedulingHelper.RunSchedulerWithinRequest;
            if (usesTimer)
            {
                // Restart timer action
                rightHeaderActions.AddAction(new HeaderAction()
                {
                    Text = GetString("Task_List.Restart"),
                    ImageUrl = GetImageUrl("Objects/CMS_ScheduledTask/timer_restart.png"),
                    EventName = TASKS_RESTART_TIMER
                });
            }

            // Run execution ASAP action
            rightHeaderActions.AddAction(new HeaderAction()
            {
                Text = GetString("Task_List.RunNow"),
                ImageUrl = GetImageUrl("Objects/CMS_ScheduledTask/timer_run.png"),
                EventName = TASKS_RUN,
                Enabled = SchedulingHelper.EnableScheduler && (!usesTimer || SchedulingTimer.TimerExists(si.SiteName))
            });
        }

        // Force action buttons to cause full postback so that tasks can be properly executed in global.asax
        ControlsHelper.RegisterPostbackControl(listElem);
        ControlsHelper.RegisterPostbackControl(rightHeaderActions);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (si != null)
        {
            if (SchedulingHelper.UseAutomaticScheduler || !SchedulingHelper.RunSchedulerWithinRequest)
            {
                lblLastRun.Visible = true;

                string siteName = si.SiteName.ToLowerCSafe();
                if (SchedulingTimer.TimerExists(siteName))
                {
                    DateTime lastRun = ValidationHelper.GetDateTime(SchedulingTimer.LastRuns[siteName], DateTimeHelper.ZERO_TIME);
                    if (lastRun != DateTimeHelper.ZERO_TIME)
                    {
                        lblLastRun.Text = GetString("Task_List.LastRun") + " " + lastRun.ToString();
                    }
                    else
                    {
                        lblLastRun.Text = GetString("Task_List.Running");
                    }
                }
                else
                {
                    lblLastRun.Text = GetString("Task_List.NoRun");
                }
            }
            else
            {
                lblLastRun.Visible = false;
            }
        }

        ScriptHelper.RegisterStartupScript(this.Page, typeof(string), "SchedulerRefreshScript", "function RefreshGrid() { " + ControlsHelper.GetPostBackEventReference(this.btnRefresh, null) + "; }", true);

        base.OnPreRender(e);
    }

    #endregion


    /// <summary>
    /// Handles action performed event for right HeaderActions.
    /// </summary>
    protected void rightHeaderActions_ActionPerformed(object sender, CommandEventArgs args)
    {
        switch (args.CommandName)
        {
            case TASKS_RESTART_TIMER:

                // Check "modify" permission
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.ScheduledTasks", "Modify"))
                {
                    RedirectToAccessDenied("CMS.ScheduledTasks", "Modify");
                }

                SchedulingTimer.RestartTimer(si.SiteName);
                break;

            case TASKS_RUN:

                // Check "modify" permission
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.ScheduledTasks", "Modify"))
                {
                    RedirectToAccessDenied("CMS.ScheduledTasks", "Modify");
                }

                SchedulingTimer.RunSchedulerASAP(si.SiteName);
                SchedulingTimer.SchedulerRunImmediatelySiteName = si.SiteName;
                break;
        }
    }
}