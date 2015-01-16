using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSAdminControls_Debug_AnalyticsLog : AnalyticsLog
{
    string ip = String.Empty;
    string agent = String.Empty;

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
            // Update user agent and IP
            gridAnalytics.RowDataBound += new GridViewRowEventHandler(gridAnalytics_RowDataBound);

            // Get the log table
            DataTable dt = Log.LogTable;
            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                Visible = true;

                gridAnalytics.Columns[1].HeaderText = GetString("General.CodeName");
                gridAnalytics.Columns[2].HeaderText = GetString("General.Object");
                gridAnalytics.Columns[3].HeaderText = GetString("General.Count");
                gridAnalytics.Columns[4].HeaderText = GetString("General.SiteName");
                gridAnalytics.Columns[5].HeaderText = GetString("General.Context");

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("AnalyticsLog.Info") + "</div>";
                }

                gridAnalytics.DataSource = dt;
                gridAnalytics.DataBind();
            }
        }
    }

    
    /// <summary>
    /// Update footer values
    /// </summary>
    void gridAnalytics_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // Keep current user agent and ip
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ip = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "IP"));
            agent = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "UserAgent"));
        }
        // Add IP and UserAgent to the footer
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            // IP
            Literal ltlIp = e.Row.FindControl("ltlIp") as Literal;
            if (ltlIp != null)
            {
                ltlIp.Text = HTMLHelper.HTMLEncode(ip);
            }

            // User agent
            Literal ltlAgent = e.Row.FindControl("ltlAgent") as Literal;
            if (ltlAgent != null)
            {
                ltlAgent.Text = HTMLHelper.HTMLEncode(agent);
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


    /// <summary>
    /// Gets the item information
    /// </summary>
    /// <param name="data">Activity data</param>
    /// <param name="objectName">Object name</param>
    /// <param name="culture">Culture</param>
    /// <param name="objectId">Object ID</param>
    protected string GetInformation(object data, object objectName, object culture, object objectId)
    {
        StringBuilder sb = new StringBuilder();

        // Get activity data
        ActivityData activityData = null;
        if (data is ActivityData)
        {
            activityData = (ActivityData)data;
        }
        else
        {
            // Standard web analytics action
            sb.Append(objectName);

            int id = ValidationHelper.GetInteger(objectId, 0);
            if (id > 0)
            {
                sb.Append(objectId);
            }

            string stringCulture = ValidationHelper.GetString(culture, null);
            if (!String.IsNullOrEmpty(stringCulture))
            {
                sb.Append(" (", stringCulture, ")");
            }
        }

        return sb.ToString();
    }


    /// <summary>
    /// Gets the count/value information
    /// </summary>
    /// <param name="count">Count</param>
    /// <param name="value">Value</param>
    protected string GetCount(object count, object value)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(count);

        double dValue = ValidationHelper.GetDouble(value, 0);
        if (dValue > 0)
        {
            sb.Append(" (", dValue, ")");
        }

        return sb.ToString();
    }
}