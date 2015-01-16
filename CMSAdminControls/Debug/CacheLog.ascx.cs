using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_CacheLog : CacheLog
{
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
                Visible = true;

                gridCache.Columns[1].HeaderText = GetString("CacheLog.Operation");
                gridCache.Columns[2].HeaderText = GetString("CacheLog.Data");
                gridCache.Columns[3].HeaderText = GetString("General.Context");

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("CacheLog.Info") + "</div>";
                }

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
}