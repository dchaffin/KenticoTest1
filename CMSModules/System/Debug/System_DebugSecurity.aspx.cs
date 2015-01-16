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

public partial class CMSModules_System_Debug_System_DebugSecurity : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("DebugSecurity.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        if (!SecurityHelper.DebugSecurity)
        {
            ShowWarning(GetString("DebugSecurity.NotConfigured"), null, null);
        }
        else
        {
            plcLogs.Controls.Clear();

            for (int i = SecurityHelper.LastLogs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the log
                    RequestLog log = (RequestLog)SecurityHelper.LastLogs[i];
                    if (log != null)
                    {
                        // Load the table
                        DataTable dt = log.LogTable;
                        if (!DataHelper.DataSourceIsEmpty(dt))
                        {
                            // Log the control
                            SecurityLog logCtrl = (SecurityLog)LoadUserControl("~/CMSAdminControls/Debug/SecurityLog.ascx");
                            logCtrl.ID = "logSec_" + i;
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
                catch //(Exception ex)
                {
                }
            }
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        SecurityHelper.LastLogs.Clear();
        ReloadData();
    }
}