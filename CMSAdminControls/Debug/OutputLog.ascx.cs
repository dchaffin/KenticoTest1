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

public partial class CMSAdminControls_Debug_OutputLog : OutputLog
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
            // Get the output
            string output = ValidationHelper.GetString(Log.Value, null);
            if (!String.IsNullOrEmpty(output))
            {
                Visible = true;

                StringBuilder sb = new StringBuilder(output.Length + 200);
                int size = output.Length;

                if (DisplayHeader)
                {
                    sb.Append("<div class=\"LogInfo\">", GetString("OutputLog.Info"), "</div>");
                }

                // Size chart
                MaxSize = 102400;
                if (size > MaxSize)
                {
                    MaxSize = size;
                }

                sb.Append("<table class=\"OutputInfo\"><tr><td><strong>");
                sb.Append(GetSizeChart(size, 0, 0, 0));
                sb.Append("</td><td><strong>");
                sb.Append(DataHelper.GetSizeString(size));
                sb.Append("</strong></td></tr></table>");

                sb.Append("<div><textarea class=\"Output\" readonly=\"true\">");
                sb.Append(HTMLHelper.HTMLEncode(output));
                sb.Append("</textarea></div>");

                TotalSize = size;

                ltlLog.Text = sb.ToString();
            }
        }
    }
}