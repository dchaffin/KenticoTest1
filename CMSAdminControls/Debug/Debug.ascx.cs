using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_Debug : CMSUserControl
{
    protected override void Render(HtmlTextWriter writer)
    {
        // Do not render if nothing is displayed
        if (logCache.Visible ||
            logSQL.Visible ||
            logFiles.Visible ||
            logSec.Visible ||
            logState.Visible ||
            logMac.Visible ||
            logAn.Visible ||
            logReq.Visible)
        {
            base.Render(writer);

            RequestStockHelper.Add("DebugPresent", true, true);
        }
    }
}