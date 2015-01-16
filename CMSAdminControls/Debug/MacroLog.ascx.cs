using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;

public partial class CMSAdminControls_Debug_MacroLog : MacroLog
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
            ScriptHelper.RegisterTooltip(Page);

            // Get the log table
            DataTable dt = Log.LogTable;
            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                Visible = true;

                gridMacros.Columns[1].HeaderText = GetString("MacroLog.Expression");
                gridMacros.Columns[2].HeaderText = GetString("MacroLog.Result");
                gridMacros.Columns[3].HeaderText = GetString("General.User");
                gridMacros.Columns[4].HeaderText = GetString("General.Context");
                gridMacros.Columns[5].HeaderText = GetString("MacroLog.Duration");

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("MacroLog.Info") + "</div>";
                }

                MaxDuration = DataHelper.GetMaximumValue<double>(dt, "Duration");

                gridMacros.DataSource = dt;
                gridMacros.DataBind();
            }
        }
    }


    protected string GetIndex(object ind)
    {
        int indent = ValidationHelper.GetInteger(ind, 0);
        if (indent == 0)
        {
            return (++index).ToString();
        }
        else
        {
            return null;
        }
    }


    protected string GetBeginIndent(object ind)
    {
        int indent = ValidationHelper.GetInteger(ind, 0);
        string result = "";
        for (int i = 0; i < indent; i++)
        {
            result += "&gt;"; //"<div style=\"padding-left: 10px;\">";
        }
        if (result != "")
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


    protected string GetExpression(object indent, object expression)
    {
        string result = null;

        bool main = (ValidationHelper.GetInteger(indent, 0) <= 0);
        if (main)
        {
            result += "<strong>";
        }
        else
        {
            result += "<span style=\"color: #888888;\">";
        }

        string expr = ValidationHelper.GetString(expression, "").Replace("%\\}", "%}").Replace("\\n", "\n").Replace("\\r", "\r");
        bool isReturnStatement = expr.StartsWithCSafe("return", true);

        result += GetBeginIndent(indent);
        result += (isReturnStatement ? "<strong>" : "");
        result += HTMLHelper.HTMLEncode(MacroResolver.RemoveSecurityParameters(expr, false, null));
        result += (isReturnStatement ? "</strong>" : "");

        if (main)
        {
            result += "</strong>";
        }
        else
        {
            result += "</span>";
        }

        result += GetEndIndent(indent);

        return result;
    }


    protected string GetContext(object indent, object context)
    {
        if (ValidationHelper.GetInteger(indent, 0) <= 0)
        {
            return GetContext(context);
        }

        return "";
    }


    protected string GetResult(object result, object ind, object expression)
    {
        if ((result == null) || (result == DBNull.Value))
        {
            return null;
        }

        string stringResult = "";

        bool main = ValidationHelper.GetInteger(ind, 0) <= 0;
        if (main)
        {
            stringResult = "<strong>";
        }
        else
        {
            stringResult += "<span style=\"color: #888888;\">";
        }

        string original = ValidationHelper.GetString(result, "");
        bool isReturnStatement = ValidationHelper.GetString(expression, "").StartsWithCSafe("return", true);

        if (original.Length > 100)
        {
            stringResult += (isReturnStatement ? "<strong>" : "");
            stringResult += String.Format("<div onmouseover=\"Tip('{0}')\" onmouseout=\"UnTip()\">", ScriptHelper.FormatTooltipString(original, true));
            stringResult += HTMLHelper.HTMLEncode(TextHelper.LimitLength(original, 100)) + "</div>" + (isReturnStatement ? "</strong>" : "");
        }
        else
        {
            stringResult += (isReturnStatement ? "<strong>" : "") + HttpUtility.HtmlEncode(original) + (isReturnStatement ? "</strong>" : "");
        }

        if (main)
        {
            stringResult += "</strong>";
        }
        else
        {
            stringResult += "</span>";
        }

        return stringResult;
    }
}