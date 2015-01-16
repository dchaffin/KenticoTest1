using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Controls_Members_MemberFilter : CMSUserControl
{
    #region "Variables"

    private string mWhereCondition = string.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Where condition built unsig the filter options.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Page_load event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeForm();
        BuildWhereCondition();
    }

    #endregion


    #region "Private methods"

    private void InitializeForm()
    {
        if (drpMemberName.Items.Count <= 0)
        {
            drpMemberName.Items.Add(new ListItem("LIKE", "0"));
            drpMemberName.Items.Add(new ListItem("NOT LIKE", "1"));
            drpMemberName.Items.Add(new ListItem("=", "2"));
            drpMemberName.Items.Add(new ListItem("<>", "3"));
        }

        if (drpMemberStatus.Items.Count <= 0)
        {
            drpMemberStatus.Items.Add(new ListItem(GetString("general.selectall"), "0"));
            drpMemberStatus.Items.Add(new ListItem(GetString("groups.status.waitingforapproval"), "1"));
            drpMemberStatus.Items.Add(new ListItem(GetString("general.approved"), "2"));
            drpMemberStatus.Items.Add(new ListItem(GetString("general.rejected"), "3"));

            // Preselect all
            drpMemberStatus.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Builds where condition and raises search event.
    /// </summary>
    private void BuildWhereCondition()
    {
        // Member name
        string memberName = txtMemberName.Text.Trim().Replace("'", "''");
        if (!String.IsNullOrEmpty(memberName))
        {
            // Get proper operator name
            int sqlOperatorNumber = ValidationHelper.GetInteger(drpMemberName.SelectedValue, 0);
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
                memberName = "%" + memberName + "%";
            }

            mWhereCondition = "(MemberUserID IN (SELECT UserID FROM CMS_User WHERE UserName " + sqlOperatorName + " N'" + memberName + "'))";
        }

        // Member status
        int sqlStatusNumber = ValidationHelper.GetInteger(drpMemberStatus.SelectedValue, 0);
        string sqlStatusCode = "";
        switch (sqlStatusNumber)
        {
            case 1:
                sqlStatusCode = "MemberStatus = 2";
                break;
            case 2:
                sqlStatusCode = "MemberStatus = 0";
                break;
            case 3:
                sqlStatusCode = "MemberStatus = 1";
                break;
            default:
                sqlStatusCode = "";
                break;
        }

        if (!String.IsNullOrEmpty(sqlStatusCode))
        {
            if (!String.IsNullOrEmpty(mWhereCondition))
            {
                mWhereCondition += " AND ";
            }

            mWhereCondition += "(" + sqlStatusCode + ")";
        }
    }

    #endregion
}