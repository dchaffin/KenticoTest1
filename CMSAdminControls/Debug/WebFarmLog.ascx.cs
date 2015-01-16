using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_WebFarmLog : WebFarmLog
{
    #region "Variables"

    protected string cmsVersion = null;

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
            // Get the log table
            DataTable dt = Log.LogTable;
            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                Visible = true;

                cmsVersion = GetString("Footer.Version") + "&nbsp;" + CMSContext.GetFriendlySystemVersion(true);

                gridQueries.Columns[1].HeaderText = GetString("WebFarmLog.TaskType");
                gridQueries.Columns[2].HeaderText = GetString("WebFarmLog.Target");
                gridQueries.Columns[3].HeaderText = GetString("WebFarmLog.TextData");
                gridQueries.Columns[4].HeaderText = GetString("General.Context");

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("WebFarmLog.Info") + "</div>";
                }

                gridQueries.DataSource = dt;
                gridQueries.DataBind();
            }
        }
    }


    protected string GetData(object textData, object binaryData)
    {
        string text = ValidationHelper.GetString(textData, null);
        string binary = ValidationHelper.GetString(binaryData, null);

        if (String.IsNullOrEmpty(text))
        {
            if (!String.IsNullOrEmpty(binary))
            {
                return "byte[]: " + binary;
            }
        }
        else
        {
            string result = TextHelper.EnsureLineEndings(text, "<br />");

            if (!String.IsNullOrEmpty(binary))
            {
                result += " (byte[]: " + binary + ")";
            }

            return result;
        }

        return "";
    }


    protected int GetIndex()
    {
        return ++index;
    }
}