using System;
using System.Data;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.UIControls;
using System.Collections.Generic;

public partial class CMSModules_System_Debug_System_DebugThreads : CMSDebugPage
{
    #region "Variables"

    protected string cmsVersion = null;
    protected int index = 0;
    protected double maxDuration = 0;

    protected TimeSpan totalDuration = new TimeSpan(0);
    protected DateTime now = DateTime.Now;

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        now = DateTime.Now;

        titleThreads.TitleText = GetString("Debug.RunningThreads");
        titleFinished.TitleText = GetString("Debug.FinishedThreads");

        btnRunDummy.Text = GetString("DebugThreads.Test");

        cmsVersion = GetString("Footer.Version") + "&nbsp;" + CMSContext.GetFriendlySystemVersion(true);

        gridThreads.Columns[1].HeaderText = GetString("unigrid.actions");
        gridThreads.Columns[2].HeaderText = GetString("ThreadsLog.Context");
        gridThreads.Columns[3].HeaderText = GetString("ThreadsLog.ThreadID");
        gridThreads.Columns[4].HeaderText = GetString("ThreadsLog.Status");
        gridThreads.Columns[5].HeaderText = GetString("ThreadsLog.Started");
        gridThreads.Columns[6].HeaderText = GetString("ThreadsLog.Duration");

        gridFinished.Columns[1].HeaderText = GetString("ThreadsLog.Context");
        gridFinished.Columns[2].HeaderText = GetString("ThreadsLog.ThreadID");
        gridFinished.Columns[3].HeaderText = GetString("ThreadsLog.Status");
        gridFinished.Columns[4].HeaderText = GetString("ThreadsLog.Started");
        gridFinished.Columns[5].HeaderText = GetString("ThreadsLog.Finished");
        gridFinished.Columns[6].HeaderText = GetString("ThreadsLog.Duration");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Cancel", ScriptHelper.GetScript(
            @"function CancelThread(threadGuid) {
                if (confirm('" + GetString("ViewLog.CancelPrompt") + @"')) {
                    document.getElementById('" + hdnGuid.ClientID + "').value = threadGuid;" +
            Page.ClientScript.GetPostBackEventReference(btnCancel, null) +
            @"}
              }"));
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
    }


    protected DataTable GetList(List<CMSThread> items)
    {
        // Process data
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("MethodClassName", typeof(string)));
        dt.Columns.Add(new DataColumn("MethodName", typeof(string)));
        dt.Columns.Add(new DataColumn("ThreadStarted", typeof(DateTime)));
        dt.Columns.Add(new DataColumn("ThreadID", typeof(int)));
        dt.Columns.Add(new DataColumn("ThreadFinished", typeof(DateTime)));
        dt.Columns.Add(new DataColumn("RequestUrl", typeof(string)));
        dt.Columns.Add(new DataColumn("Status", typeof(string)));
        dt.Columns.Add(new DataColumn("ThreadGUID", typeof(Guid)));
        dt.Columns.Add(new DataColumn("HasLog", typeof(bool)));
        dt.Columns.Add(new DataColumn("Duration", typeof(double)));

        for (int i = items.Count - 1; i >= 0; i--)
        {
            try
            {
                // Get the log
                CMSThread thread = (CMSThread)items[i];
                if (thread != null)
                {
                    DateTime started = thread.ThreadStarted;
                    DateTime finished = thread.ThreadFinished;

                    DataRow dr = dt.NewRow();

                    dr["MethodClassName"] = thread.MethodClassName;
                    dr["MethodName"] = thread.MethodName;
                    dr["ThreadStarted"] = started;
                    dr["ThreadID"] = thread.ThreadID;
                    dr["ThreadFinished"] = finished;
                    dr["RequestUrl"] = thread.RequestUrl;
                    dr["Status"] = thread.InnerThread.ThreadState.ToString();
                    dr["ThreadGUID"] = thread.ThreadGUID;
                    dr["HasLog"] = (thread.Log != null);

                    if (finished == DateTime.MinValue)
                    {
                        finished = now;
                    }
                    double duration = (finished - started).TotalSeconds;
                    dr["Duration"] = duration;

                    dt.Rows.Add(dr);
                }
            }
            catch
            {
            }
        }

        return dt;
    }


    protected void ReloadData()
    {
        LoadGrid(gridThreads, CMSThread.LiveThreads);
        LoadGrid(gridFinished, CMSThread.FinishedThreads);
    }


    /// <summary>
    /// Loads the grid with the data.
    /// </summary>
    /// <param name="grid">Grid to load</param>
    /// <param name="threads">List of threads</param>
    protected void LoadGrid(GridView grid, List<CMSThread> threads)
    {
        index = 0;
        totalDuration = new TimeSpan();

        // Get the data
        DataTable dt = GetList(threads);
        maxDuration = DataHelper.GetMaximumValue<double>(dt, "Duration");

        // Bind the grid
        grid.DataSource = dt;
        grid.DataBind();
    }


    protected void btnRunDummy_Click(object sender, EventArgs e)
    {
        LogContext log = LogContext.EnsureLog(Guid.NewGuid());
        log.Reversed = true;
        log.LineSeparator = "<br />";

        CMSThread dummy = new CMSThread(RunTest);
        dummy.Start();

        Thread.Sleep(100);
        ReloadData();
    }


    private void RunTest()
    {
        for (int i = 0; i < 50; i++)
        {
            Thread.Sleep(100);
            LogContext.AppendLine("Sample log " + i.ToString());
        }
    }


    /// <summary>
    /// Gets the item index.
    /// </summary>
    protected int GetIndex()
    {
        return ++index;
    }


    /// <summary>
    /// Gets the duration of the thread.
    /// </summary>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    protected string GetDuration(object startTime, object endTime)
    {
        TimeSpan duration = ValidationHelper.GetDateTime(endTime, now).Subtract(ValidationHelper.GetDateTime(startTime, now));
        totalDuration = totalDuration.Add(duration);

        return GetDurationString(duration) + "<br />" + LogControl.GetChart(maxDuration, duration.TotalSeconds, 1, 0, 0);
    }


    /// <summary>
    /// Gets the duration as formatted string.
    /// </summary>
    /// <param name="duration">Duration to get</param>
    protected string GetDurationString(TimeSpan duration)
    {
        string result = null;
        if (duration.TotalHours >= 1)
        {
            result += duration.Hours + ":";
            result += duration.Minutes.ToString().PadLeft(2, '0') + ":";
            result += duration.Seconds.ToString().PadLeft(2, '0');
        }
        else if (duration.TotalMinutes >= 1)
        {
            result += duration.Minutes.ToString() + ":";
            result += duration.Seconds.ToString().PadLeft(2, '0');
        }
        else
        {
            result = duration.TotalSeconds.ToString("F3");
        }

        return result;
    }


    /// <summary>
    /// Gets the actions for the thread.
    /// </summary>
    /// <param name="hasLog">Log presence</param>
    /// <param name="threadGuid">Thread GUID</param>
    /// <param name="status">Status</param>
    protected string GetActions(object hasLog, object threadGuid, object status)
    {
        string result = null;

        if (ValidationHelper.GetString(status, null) != "AbortRequested")
        {
            result += "<a href=\"#\" onclick=\"CancelThread('" + threadGuid + "')\"><img src=\"" + ResolveUrl(GetImageUrl("Design/Controls/UniGrid/Actions/Delete.png")) + "\" style=\"border: none;\" alt=\"" + GetString("General.Cancel") + "\" /></a> ";
        }

        bool logAvailable = ValidationHelper.GetBoolean(hasLog, false);
        if (logAvailable)
        {
            result += "<a href=\"System_ViewLog.aspx?threadGuid=" + threadGuid.ToString() + "\" target=\"_blank\"><img src=\"" + ResolveUrl(GetImageUrl("Design/Controls/UniGrid/Actions/View.png")) + "\" style=\"border: none;\" alt=\"" + GetString("General.View") + "\" /></a> ";
        }

        return result;
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Guid threadGuid = ValidationHelper.GetGuid(hdnGuid.Value, Guid.Empty);
        CMSThread thread = CMSThread.GetThread(threadGuid);
        if (thread != null)
        {
            thread.Stop();
        }
    }


    protected void timRefresh_Tick(object sender, EventArgs e)
    {
    }
}