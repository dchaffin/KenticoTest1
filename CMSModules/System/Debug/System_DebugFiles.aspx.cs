using System;
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

public partial class CMSModules_System_Debug_System_DebugFiles : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("DebugFiles.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        if (StorageHelper.IsDefaultStorageExternal)
        {
            drpOperationType.Visible = true;
            lblOperationType.Visible = true;

            if (!RequestHelper.IsPostBack())
            {
                drpOperationType.Items.Add(new ListItem(GetString("general.selectall"), ""));
                drpOperationType.Items.Add(new ListItem(GetString("general.general"), IOOperationType.General.ToString()));
                drpOperationType.Items.Add(new ListItem(GetString("FilesLog.TypeAzureBlob"), IOOperationType.AzureBlob.ToString()));
                drpOperationType.Items.Add(new ListItem(GetString("FilesLog.TypeAmazonS3"), IOOperationType.AmazonS3.ToString()));
            }
        }

        if (!File.DebugFiles)
        {
            ShowWarning(GetString("DebugFiles.NotConfigured"), null, null);
        }
        else
        {
            plcLogs.Controls.Clear();

            for (int i = File.LastLogs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the log
                    RequestLog log = (RequestLog)File.LastLogs[i];
                    if (log != null)
                    {
                        // Load the table
                        DataTable dt = log.LogTable;
                        if (!DataHelper.DataSourceIsEmpty(dt))
                        {
                            bool display = true;
                            IOOperationType operationType = IOOperationType.All;
                            if (StorageHelper.IsDefaultStorageExternal)
                            {
                                string type = (drpOperationType.SelectedValue == null ? "" : drpOperationType.SelectedValue.ToLowerCSafe());
                                switch (type)
                                {
                                    case "azureblob":
                                        operationType = IOOperationType.AzureBlob;
                                        break;

                                    case "general":
                                        operationType = IOOperationType.General;
                                        break;

                                    case "amazons3":
                                        operationType = IOOperationType.AmazonS3;
                                        break;
                                }

                                // Apply operation type filter
                                DataView dv = new DataView(dt);
                                if (operationType != IOOperationType.All)
                                {
                                    dv.RowFilter = "OperationType = '" + operationType.ToString() + "'";
                                }

                                display = !DataHelper.DataSourceIsEmpty(dv);
                            }

                            if (display)
                            {
                                // Load the control
                                FilesLog logCtrl = (FilesLog)LoadUserControl("~/CMSAdminControls/Debug/FilesLog.ascx");
                                logCtrl.ID = "filesLog_" + i;
                                logCtrl.EnableViewState = false;
                                logCtrl.Log = log;
                                logCtrl.DisplayHeader = false;
                                logCtrl.ShowCompleteContext = chkCompleteContext.Checked;
                                logCtrl.OperationTypeFilter = operationType;

                                // Add to the output
                                plcLogs.Controls.Add(new LiteralControl("<div>&lrm;<strong>&nbsp;" + GetRequestLink(log.RequestURL, log.RequestGUID) + "</strong> (" + log.RequestTime.ToString("hh:mm:ss") + ")&lrm;<br /><br />"));
                                plcLogs.Controls.Add(logCtrl);
                                plcLogs.Controls.Add(new LiteralControl("</div><br /><br />"));
                            }
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
        ReloadData();
    }
}