using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_QueryLog : QueryLog
{
    /// <summary>
    /// Total parameters size.
    /// </summary>
    public virtual long TotalParamSize
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

        if (Log != null)
        {
            // Get the log table
            DataTable dt = Log.LogTable;
            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                // Check the duplicity
                if (!dt.Columns.Contains("Duplicit"))
                {
                    DataHelper.MarkDuplicitRows(dt, null, "Duplicit", "QueryText", "QueryName", "QueryParameters", "QueryResults");
                }

                Visible = true;

                gridQueries.Columns[1].HeaderText = GetString("QueryLog.QueryText");
                //gridQueries.Columns[2].HeaderText = GetString("QueryLog.QueryParameters");
                gridQueries.Columns[2].HeaderText = GetString("General.Context");
                gridQueries.Columns[3].HeaderText = GetString("QueryLog.QueryDuration");

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("QueryLog.Info") + "</div>";
                }

                MaxDuration = DataHelper.GetMaximumValue<double>(dt, "QueryDuration");

                // Prepare maximum size
                MaxSize = DataHelper.GetMaximumValue<int>(dt, "QueryResultsSize");

                int paramSize = DataHelper.GetMaximumValue<int>(dt, "QueryParametersSize");
                if (paramSize > MaxSize)
                {
                    MaxSize = paramSize;
                }

                gridQueries.DataSource = dt;
                gridQueries.DataBind();
            }
        }
    }


    protected object GetIndex(object isInformation, object resultsSize, object parametersSize, object queryName, object queryText)
    {
        if (ValidationHelper.GetBoolean(isInformation, false))
        {
            return GetIcon("Design/Controls/UniGrid/Actions/Information.png", queryName);
        }

        if (queryText == DBNull.Value)
        {
            return null;
        }

        TotalSize += ValidationHelper.GetInteger(resultsSize, 0);
        TotalParamSize += ValidationHelper.GetInteger(parametersSize, 0);

        return ++index;
    }
}