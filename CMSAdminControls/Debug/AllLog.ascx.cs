using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSAdminControls_Debug_AllLog : AllLog
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EnableViewState = false;
        Visible = true;

        if (!StopProcessing)
        {
            gridDebug.Columns[1].HeaderText = GetString("AllLog.DebugType");
            gridDebug.Columns[2].HeaderText = GetString("AllLog.Information");
            gridDebug.Columns[3].HeaderText = GetString("AllLog.Result");
            gridDebug.Columns[4].HeaderText = GetString("General.Context");
            gridDebug.Columns[5].HeaderText = GetString("AllLog.TotalDuration");
            gridDebug.Columns[6].HeaderText = GetString("AllLog.Duration");

            gridDebug.DataSource = MergeTables(Logs, Page, ShowCompleteContext);
            gridDebug.DataBind();
        }
    }
}