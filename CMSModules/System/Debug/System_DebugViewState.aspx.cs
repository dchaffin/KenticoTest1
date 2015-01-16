using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_System_Debug_System_DebugViewState : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("DebugViewState.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        if (!CMSControlsHelper.DebugViewState)
        {
            ShowWarning(GetString("DebugViewState.NotConfigured"), null, null);
        }
        else
        {
            plcLogs.Controls.Clear();

            for (int i = CMSControlsHelper.LastLogs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the log
                    RequestLog log = (RequestLog)CMSControlsHelper.LastLogs[i];
                    if (log != null)
                    {
                        // Load the table
                        DataTable dt = log.LogTable;
                        if (!DataHelper.DataSourceIsEmpty(dt))
                        {
                            // Load the control
                            ViewStateLog logCtrl = (ViewStateLog)LoadUserControl("~/CMSAdminControls/Debug/ViewState.ascx");
                            logCtrl.ID = "viewStateLog_" + i;
                            logCtrl.EnableViewState = false;
                            logCtrl.DisplayTotalSize = false;
                            logCtrl.Log = log;
                            logCtrl.DisplayHeader = false;
                            logCtrl.DisplayOnlyDirty = chkOnlyDirty.Checked;

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
        CMSControlsHelper.LastLogs.Clear();
        ReloadData();
    }
}