using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_System_Debug_System_DebugAll : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("DebugAll.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        if (!RequestHelper.DebugRequests)
        {
            ShowWarning(GetString("DebugRequests.NotConfigured"), null, null);
        }
        else
        {
            plcLogs.Controls.Clear();

            for (int i = RequestHelper.LastLogs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the request log
                    RequestLog log = (RequestLog)RequestHelper.LastLogs[i];
                    if (log != null)
                    {
                        List<DataTable> logs = AllLog.GetLogs(log);

                        // Load the control only if there is more than only request log
                        if (logs.Count > 1)
                        {
                            AllLog logCtrl = (AllLog)LoadUserControl("~/CMSAdminControls/Debug/AllLog.ascx");
                            logCtrl.ID = "allLog_" + i;
                            logCtrl.EnableViewState = false;
                            logCtrl.Logs = logs;
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
        File.LastLogs.Clear();
        SqlHelperClass.LastLogs.Clear();
        SecurityHelper.LastLogs.Clear();
        MacroResolver.LastLogs.Clear();
        CacheHelper.LastLogs.Clear();
        RequestHelper.LastLogs.Clear();
        ReloadData();
    }
}