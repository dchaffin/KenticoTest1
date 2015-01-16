using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Controls_GroupFilter : CMSAbstractBaseFilterControl
{
  
    #region "Public properties"

    /// <summary>
    /// Where condition built using the filter options.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return BuildWhereCondition();
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!RequestHelper.IsPostBack())
        {
            InitializeForm();
        }

        btnReset.Text = GetString("general.reset");
        btnReset.Click += btnReset_Click;
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }
    #endregion


    #region "Private methods"


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtGroupName.Text = String.Empty;
        drpGroupStatus.SelectedIndex = 0;
        drpGroupName.SelectedIndex = 0;
    }

    /// <summary>
    /// Initializes filter controls.
    /// </summary>
    private void InitializeForm()
    {
        // Initialize first dropdown lists
        drpGroupName.Items.Add(new ListItem("LIKE", "0"));
        drpGroupName.Items.Add(new ListItem("NOT LIKE", "1"));
        drpGroupName.Items.Add(new ListItem("=", "2"));
        drpGroupName.Items.Add(new ListItem("<>", "3"));

        drpGroupStatus.Items.Add(new ListItem(GetString("general.selectall"), "0"));
        drpGroupStatus.Items.Add(new ListItem(GetString("groups.status.waitingforapproval"), "1"));
        drpGroupStatus.Items.Add(new ListItem(GetString("general.approved"), "2"));
        drpGroupStatus.Items.Add(new ListItem(GetString("general.rejected"), "3"));

        // Preselect all
        drpGroupStatus.SelectedIndex = 0;
    }


    /// <summary>
    /// Builds where condition.
    /// </summary>
    /// <returns>Filter where condition</returns>
    private string BuildWhereCondition()
    {
        string whereCondition = String.Empty;
        // Group name
        string groupName = txtGroupName.Text.Trim().Replace("'", "''");
        if (!String.IsNullOrEmpty(groupName))
        {
            // Get proper operator name
            int sqlOperatorNumber = ValidationHelper.GetInteger(drpGroupName.SelectedValue, 0);
            string sqlOperatorName = "LIKE";
            switch (sqlOperatorNumber)
            {
                case 1:
                    sqlOperatorName = "NOT LIKE";
                    break;
                case 2:
                    sqlOperatorName = "=";
                    break;
                case 3:
                    sqlOperatorName = "<>";
                    break;
                default:
                    sqlOperatorName = "LIKE";
                    break;
            }

            if ((sqlOperatorName == "LIKE") || (sqlOperatorName == "NOT LIKE"))
            {
                groupName = "%" + groupName + "%";
            }

            whereCondition = "(GroupDisplayName " + sqlOperatorName + " N'" + groupName + "')";
        }

        // Group status
        int sqlStatusNumber = ValidationHelper.GetInteger(drpGroupStatus.SelectedValue, 0);
        string sqlStatusCode = "";
        switch (sqlStatusNumber)
        {
            case 1:
                sqlStatusCode = "GroupApproved IS NULL";
                break;
            case 2:
                sqlStatusCode = "GroupApproved = 1";
                break;
            case 3:
                sqlStatusCode = "GroupApproved = 0";
                break;
            default:
                sqlStatusCode = "";
                break;
        }

        if (!String.IsNullOrEmpty(sqlStatusCode))
        {
            if (!String.IsNullOrEmpty(whereCondition))
            {
                whereCondition += " AND ";
            }

            whereCondition += "(" + sqlStatusCode + ")";
        }

        return whereCondition;
    }

    #endregion
}