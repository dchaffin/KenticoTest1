using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.OutputFilter;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_System_Debug_System_ViewRequest : CMSDebugPage
{
    private Guid guid = Guid.Empty;


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup title
        CurrentMaster.Title.TitleText = GetString("ViewRequest.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/__GLOBAL__/Object.png");

        guid = QueryHelper.GetGuid("guid", Guid.Empty);
        if (guid != Guid.Empty)
        {
            // Find the root log
            RequestLog log = CMSFunctions.FindRequestLog(guid);

            if (log != null)
            {
                // Setup the logs
                RequestLogs logs = log.ParentLogs;

                plcLogs.Controls.Add(new LiteralControl("<div><strong>&nbsp;" + logs.RequestURL + "</strong> (" + logs.RequestTime.ToString("hh:MM:ss") + ")</div><br />"));

                logFiles.Log = logs.FilesLog;
                logCache.Log = logs.CacheLog;
                logOutput.Log = logs.OutputLog;
                logSec.Log = logs.SecurityLog;
                logMac.Log = logs.MacrosLog;
                logSQL.Log = logs.QueryLog;
                logState.Log = logs.ViewStateLog;
                logReq.Log = logs.RequestLog;
                logFarm.Log = logs.WebFarmLog;
            }
        }
    }

    #endregion
}