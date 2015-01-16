using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_RequestLog : RequestProcessLog
{
    #region "Variables"

    protected DateTime firstTime = DateTime.MinValue;
    protected DateTime lastTime = DateTime.MinValue;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        EnableViewState = false;
        Visible = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        if (Log != null)
        {
            Visible = true;

            // Log request values
            RequestHelper.LogRequestValues(true, true, true);

            if (Log.ValueCollections != null)
            {
                tblResC.Title = GetString("RequestLog.ResponseCookies");
                tblResC.Table = Log.ValueCollections.Tables["ResponseCookies"];

                tblReqC.Title = GetString("RequestLog.RequestCookies");
                tblReqC.Table = Log.ValueCollections.Tables["RequestCookies"];

                tblVal.Title = GetString("RequestLog.Values");
                tblVal.Table = Log.ValueCollections.Tables["Values"];
            }

            // Get the log table
            DataTable dt = Log.LogTable;
            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                EnsureDurationColumn(dt);

                gridCache.Columns[1].HeaderText = GetString("RequestLog.Operation");
                gridCache.Columns[2].HeaderText = GetString("RequestLog.Parameter");
                gridCache.Columns[3].HeaderText = GetString("RequestLog.FromStart");
                gridCache.Columns[4].HeaderText = GetString("RequestLog.Duration");

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("RequestLog.Info") + "</div>";
                }

                MaxDuration = DataHelper.GetMaximumValue<double>(dt, "Duration");

                // Bind the data
                gridCache.DataSource = dt;
                gridCache.DataBind();
            }
        }
    }


    /// <summary>
    /// Gets the item index.
    /// </summary>
    protected int GetIndex()
    {
        return ++index;
    }


    protected string GetBeginIndent(object ind)
    {
        int indent = ValidationHelper.GetInteger(ind, 0);
        string result = "";
        for (int i = 0; i < indent; i++)
        {
            result += "&gt;"; //"<div style=\"padding-left: 10px;\">";
        }

        if (indent > 0)
        {
            result += "&nbsp;";
        }

        return result;
    }


    protected string GetEndIndent(object ind)
    {
        int indent = ValidationHelper.GetInteger(ind, 0);
        string result = "";
        /*
        for (int i = 0; i < indent; i++)
        {
            result += "</div>";
        }*/

        return result;
    }


    /// <summary>
    /// Gets the time from the first action.
    /// </summary>
    /// <param name="time">Time of the action</param>
    protected string GetFromStart(object time)
    {
        DateTime t = ValidationHelper.GetDateTime(time, DateTime.MinValue);
        if (firstTime == DateTime.MinValue)
        {
            firstTime = t;
        }

        lastTime = t;
        TotalDuration = t.Subtract(firstTime).TotalSeconds;

        return TotalDuration.ToString("F3");
    }
}