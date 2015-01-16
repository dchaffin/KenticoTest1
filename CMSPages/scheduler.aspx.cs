using System;

using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.Scheduler;
using CMS.UIControls;

public partial class CMSPages_scheduler : CMSPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (!DebugHelper.DebugScheduler)
        {
            DisableDebugging();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetNoStore();

        // Run the tasks
        SchedulingExecutor.ExecuteScheduledTasks(CMSContext.CurrentSiteName, WebSyncHelperClass.ServerName);
    }
}