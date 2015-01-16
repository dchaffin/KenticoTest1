using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_ViewState : ViewStateLog
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

        try
        {
            if (Log != null)
            {
                // Get the log table
                DataTable dt = Log.LogTable;
                DataView dv = new DataView(dt);

                if (!DataHelper.DataSourceIsEmpty(dv))
                {
                    Visible = true;

                    gridStates.Columns[1].HeaderText = GetString("ViewStateLog.ID");
                    gridStates.Columns[2].HeaderText = GetString("ViewStateLog.IsDirty");
                    gridStates.Columns[3].HeaderText = GetString("ViewStateLog.ViewState");
                    gridStates.Columns[4].HeaderText = GetString("ViewStateLog.Size");

                    if (DisplayHeader)
                    {
                        ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("ViewStateLog.Info") + "</div>";
                    }

                    // Bind to the grid
                    if (DisplayOnlyDirty)
                    {
                        dv.RowFilter = "HasDirty = 1";
                    }

                    MaxSize = DataHelper.GetMaximumValue<int>(dv, "ViewStateSize");

                    gridStates.DataSource = dv;
                    gridStates.DataBind();
                }
            }
        }
        catch (Exception)
        {
            ltlInfo.Text = "Unable to acquire ViewState from the controls collection.";
            Visible = true;
        }
    }


    protected int GetIndex()
    {
        return ++index;
    }


    protected string ColourYesNo(object value)
    {
        string str = ValidationHelper.GetString(value, String.Empty);
        string[] values = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        str = String.Empty;

        foreach (string val in values)
        {
            str += UniGridFunctions.ColoredSpanYesNoReversed(val) + "<br />";
        }

        return str;
    }
}