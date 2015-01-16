using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_System_Debug_System_DebugSQL : CMSDebugPage
{
    private ArrayList logs = new ArrayList();


    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("DebugSQL.ClearLog");
        btnClearCache.Text = GetString("Administration-System.btnClearCache");

        ReloadData();
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if (logs.Count > 0)
        {
            double totalDuration = 0;

            // Summarize all logs
            foreach (LogControl log in logs)
            {
                totalDuration += log.TotalDuration;
            }

            lblInfo.Text = String.Format(GetString("RequestLog.Total"), totalDuration, logs.Count);
        }
        else
        {
            if (SqlHelperClass.DebugQueries)
            {
                lblInfo.Text = GetString("RequestLog.NotFound");
            }
        }

        base.Render(writer);
    }


    protected void ReloadData()
    {
        if (!SqlHelperClass.DebugQueries)
        {
            ShowWarning(GetString("DebugSQL.NotConfigured"), null, null);
        }
        else
        {
            plcLogs.Controls.Clear();

            for (int i = SqlHelperClass.LastLogs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the log
                    RequestLog log = (RequestLog)SqlHelperClass.LastLogs[i];
                    if (log != null)
                    {
                        // Load the table
                        DataTable dt = log.LogTable;
                        if (!DataHelper.DataSourceIsEmpty(dt))
                        {
                            // Load the control
                            QueryLog logCtrl = (QueryLog)LoadUserControl("~/CMSAdminControls/Debug/QueryLog.ascx");
                            logCtrl.ID = "logSQL" + i;
                            logCtrl.EnableViewState = false;
                            logCtrl.Log = log;
                            logCtrl.DisplayHeader = false;
                            logCtrl.ShowCompleteContext = chkCompleteContext.Checked;

                            // Add to the output
                            plcLogs.Controls.Add(new LiteralControl("<div>&lrm;<strong>&nbsp;" + GetRequestLink(log.RequestURL, log.RequestGUID) + "</strong> (" + log.RequestTime.ToString("hh:mm:ss") + ")&lrm;<br /><br />"));
                            plcLogs.Controls.Add(logCtrl);
                            plcLogs.Controls.Add(new LiteralControl("</div><br /><br />"));

                            logs.Add(logCtrl);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        SqlHelperClass.LastLogs.Clear();
        logs.Clear();
        ReloadData();
    }


    protected void btnClearCache_Click(object sender, EventArgs e)
    {
        CMSFunctions.ClearCache();

        ShowConfirmation(GetString("Administration-System.ClearCacheSuccess"));

        ReloadData();
    }
}