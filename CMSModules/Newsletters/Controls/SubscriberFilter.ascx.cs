using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSModules_Newsletters_Controls_SubscriberFilter : CMSAbstractBaseFilterControl
{
    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition(txtEmail.Text);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!RequestHelper.IsPostBack())
        {
            Reload();
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    protected void Reload()
    {
        filter.Items.Clear();
        filter.Items.Add(new ListItem("LIKE", "LIKE"));
        filter.Items.Add(new ListItem("NOT LIKE", "NOT LIKE"));
        filter.Items.Add(new ListItem("=", "="));
        filter.Items.Add(new ListItem("<>", "<>"));
    }


    /// <summary>
    /// Generates WHERE condition.
    /// </summary>
    private string GenerateWhereCondition(string txtEmail)
    {
        if (String.IsNullOrEmpty(txtEmail))
        {
            return null;
        }
        else
        {
            string mOperator = "LIKE";
            string email = SqlHelperClass.GetSafeQueryString(txtEmail, false);

            // Get filter operator (LIKE, NOT LIKE, =, <>)
            if (filter.SelectedValue != null)
            {
                mOperator = filter.SelectedValue;
            }

            if ((mOperator == "LIKE") || (mOperator == "NOT LIKE"))
            {
                email = "%" + email + "%";
            }

            return string.Format("((SubscriberEmail {0} '{1}') OR (Email {0} '{1}'))", mOperator, email);
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("condition", filter.SelectedValue);
        state.AddValue("email", txtEmail.Text);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        filter.SelectedValue = state.GetString("condition");
        txtEmail.Text = state.GetString("email");
    }


    /// <summary>
    /// Resets the filter settings.
    /// </summary>
    public override void ResetFilter()
    {
        filter.SelectedIndex = 0;
        txtEmail.Text = String.Empty;
    }

    #endregion

}