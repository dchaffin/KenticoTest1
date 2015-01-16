using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_ABTest_ListFilter : CMSAdminListControl
{
    #region "Variables"

    private QueryDataParameters mParameters = null;


    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return GetWhereCondition();
        }
    }


    /// <summary>
    /// Parameters used for from and to filter.
    /// </summary>
    public QueryDataParameters Parameters
    {
        get
        {
            return mParameters;
        }
    }

    #endregion


    /// <summary>
    /// Returns where condition.
    /// </summary>
    private string GetWhereCondition()
    {
        String where = String.Empty;

        if (!dtpFrom.IsValidRange() || !dtpTo.IsValidRange())
        {
            ShowError(GetString("general.errorinvaliddatetimerange"));
            return String.Empty;
        }

        where = SqlHelperClass.AddWhereCondition(where, tsfFrom.GetCondition());
        where = SqlHelperClass.AddWhereCondition(where, tsfSource.GetCondition());

        mParameters = new QueryDataParameters();

        DateTime from = dtpFrom.SelectedDateTime;
        DateTime to = dtpTo.SelectedDateTime;

        if (from != DateTimeHelper.ZERO_TIME)
        {
            where = SqlHelperClass.AddWhereCondition(where, "ABTestOpenFrom > @from");

            mParameters.Add("@from", from);
        }

        if (to != DateTimeHelper.ZERO_TIME)
        {
            where = SqlHelperClass.AddWhereCondition(where, "ABTestOpenTo < @to");

            mParameters.Add("@to", to);
        }

        return where;
    }


    /// <summary>
    /// Returns count od date filter parameters.
    /// </summary>    
    private int GetParamsCount()
    {
        int count = 0;
        if (dtpFrom.SelectedDateTime != DateTimeHelper.ZERO_TIME)
        {
            count++;
        }
        if (dtpTo.SelectedDateTime != DateTimeHelper.ZERO_TIME)
        {
            count++;
        }
        return count;
    }
}