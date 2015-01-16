using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_System_Debug_System_ViewLog : CMSDebugPage
{
    private CMSThread thread = null;
    private Guid threadGuid = Guid.Empty;


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        threadGuid = QueryHelper.GetGuid("threadguid", Guid.Empty);

        thread = CMSThread.GetThread(threadGuid);
        ctlAsync.OnRequestLog += new EventHandler(ctlAsync_OnRequestLog);

        if (!IsCallback)
        {
            // Set the title
            CurrentMaster.Title.TitleText = GetString("ViewLog.Title");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/__GLOBAL__/Object.png");
            Page.Title = GetString("ViewLog.Title");

            if ((thread != null) && (thread.Log != null))
            {
                pnlLog.Visible = true;
                btnCancel.Text = GetString("general.cancel");
                btnCancel.OnClientClick = "return confirm('" + GetString("ViewLog.CancelPrompt") + "')";


                ctlAsync.RunAsync();
            }
            else
            {
                pnlLog.Visible = false;
                ShowError(GetString("ViewLog.ThreadNotRunning"));
            }
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (thread != null)
        {
            thread.Stop();
        }

        pnlLog.Visible = false;
        ShowError(GetString("ViewLog.ThreadNotRunning"));
    }


    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        if (thread != null)
        {
            ctlAsync.Log = thread.Log.Log;
        }
    }

    #endregion
}