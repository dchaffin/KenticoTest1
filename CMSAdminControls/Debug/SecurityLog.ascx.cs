using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;

public partial class CMSAdminControls_Debug_SecurityLog : SecurityLog
{
    /// <summary>
    /// If true, the log initiates itself from the request guid query parameter
    /// </summary>
    public bool InitFromRequest 
    { 
        get; 
        set; 
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        EnableViewState = false;
        Visible = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        // Load the security log
        if (InitFromRequest)
        {
            ltlInfo.Visible = false;
            if (SettingsKeyProvider.DevelopmentMode)
            {
                Guid requestGuid = QueryHelper.GetGuid("requestguid", Guid.Empty);
                if (requestGuid != Guid.Empty)
                {
                    RequestLog log = CMSFunctions.FindRequestLog(requestGuid);
                    if (log != null)
                    {
                        var logs = log.ParentLogs;
                        if (logs != null)
                        {
                            Log = logs.SecurityLog;
                        }
                    }
                }
            }
            if (Log == null)
            {
                return;
            }
        }

        if (Log != null)
        {
            // Get the log table
            DataTable dt = Log.LogTable;
            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                // Check the duplicity
                if (!dt.Columns.Contains("Duplicit"))
                {
                    DataHelper.MarkDuplicitRows(dt, "Indent <= 0", "Duplicit", "UserName", "SecurityOperation", "Resource", "Name", "SiteName");
                }

                Visible = true;

                int index = 1;
                gridSec.Columns[index++].HeaderText = GetString("SecurityLog.UserName");
                gridSec.Columns[index++].HeaderText = GetString("SecurityLog.Operation");
                gridSec.Columns[index++].HeaderText = GetString("SecurityLog.Result");
                gridSec.Columns[index++].HeaderText = GetString("SecurityLog.Resource");
                gridSec.Columns[index++].HeaderText = GetString("SecurityLog.Name");
                gridSec.Columns[index++].HeaderText = GetString("SecurityLog.Site");
                gridSec.Columns[index++].HeaderText = GetString("General.Context");

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("SecurityLog.Info") + "</div>";
                }

                gridSec.DataSource = dt;
                gridSec.DataBind();
            }
        }
    }


    protected string GetIndex(object ind)
    {
        int indent = ValidationHelper.GetInteger(ind, 0);
        if (indent == 0)
        {
            return (++index).ToString();
        }
        else
        {
            return null;
        }
    }


    protected object GetUserName(object userName, object indent)
    {
        if (ValidationHelper.GetInteger(indent, 0) > 0)
        {
            return null;
        }

        return userName;
    }
}