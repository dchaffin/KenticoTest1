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
using CMS.WebAnalytics;

public partial class CMSModules_System_Debug_System_DebugAnalytics : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("DebugAnalytics.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        if (!AnalyticsHelper.DebugAnalytics)
        {
            ShowWarning(GetString("DebugAnalytics.NotConfigured"), null, null);
        }
        else
        {
            plcLogs.Controls.Clear();

            for (int i = AnalyticsHelper.LastLogs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the log
                    RequestLog log = (RequestLog)AnalyticsHelper.LastLogs[i];
                    if (log != null)
                    {
                        // Load the table
                        DataTable dt = log.LogTable;
                        if (!DataHelper.DataSourceIsEmpty(dt))
                        {
                            // Load the control
                            AnalyticsLog logCtrl = (AnalyticsLog)LoadUserControl("~/CMSAdminControls/Debug/AnalyticsLog.ascx");
                            logCtrl.ID = "anLog_" + i;
                            logCtrl.EnableViewState = false;
                            logCtrl.Log = log;
                            logCtrl.DisplayHeader = false;
                            logCtrl.ShowCompleteContext = chkCompleteContext.Checked;

                            // Add to the output
                            plcLogs.Controls.Add(new LiteralControl("<div>&lrm;<strong>&nbsp;" + GetRequestLink(log.RequestURL, log.RequestGUID) + "</strong> (" + log.RequestTime.ToString("hh:mm:ss") + ")&lrm;<br /><br />"));
                            plcLogs.Controls.Add(logCtrl);
                            plcLogs.Controls.Add(new LiteralControl("</div><br /><br />"));
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
        AnalyticsHelper.LastLogs.Clear();
        ReloadData();
    }
}