using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Controls_OpenedByFilter : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            // Get condition from name and email parts of the filter
            string whereCond = GetSubscriberCondition();
            if (ShowDateFilter)
            {
                // Get condition from date part of the filter
                whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltOpenedBetween.GetCondition());
            }
            if (plcVariants.Visible)
            {
                // Get condition from variant part of the filter
                whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltVariants.GetWhereCondition());
            }

            return whereCond;
        }
    }


    /// <summary>
    /// Indicates if date part of the filter should be visible.
    /// </summary>
    public bool ShowDateFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets issue ID of or to variant selector. Used for A/B test issues.
    /// </summary>
    public int IssueId
    {
        get
        {
            return ValidationHelper.GetInteger(fltVariants.Value, 0);
        }
        set
        {
            fltVariants.Value = value;

            // Display variant part of the filter
            plcVariants.Visible = fltVariants.Visible = true;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Display/hide date part of the filter
        plcDate.Visible = ShowDateFilter;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets complex where condition for name and email fields.
    /// </summary>
    protected string GetSubscriberCondition()
    {
        string result = string.Empty;

        // Get condition from subscriber filter fields
        string baseCondition = string.Empty;
        string nameCondition = fltSubscriberName.GetCondition();
        string emailCondition = fltEmail.GetCondition();

        // Prepare base condition for subscriber email and name
        baseCondition = SqlHelperClass.AddWhereCondition(nameCondition, emailCondition);

        if (!string.IsNullOrEmpty(emailCondition))
        {
            // Extend the condition for 'Email' column
            fltEmail.Column = "Email";
            emailCondition = SqlHelperClass.AddWhereCondition(emailCondition, fltEmail.GetCondition(), "OR");
        }

        if (!string.IsNullOrEmpty(baseCondition))
        {
            string subscriberCondition = SqlHelperClass.AddWhereCondition(nameCondition, emailCondition);
            result = BuildCondition(baseCondition, subscriberCondition, !string.IsNullOrEmpty(emailCondition));
        }

        return result;
    }


    /// <summary>
    /// Builds where condition with subscriber IDs from main DB who meet base filter condition.
    /// </summary>
    /// <param name="baseCondition">Base condition</param>
    /// <param name="subscriberCondition">Condition for subscribers from main DB</param>
    /// <param name="useView">Indicates if subscriber view should be used</param>
    protected string BuildCondition(string baseCondition, string subscriberCondition, bool useView)
    {
        string result = string.Empty;

        // Get IDs of subscribers from current site who meet the original condition
        string tempWhere = SqlHelperClass.AddWhereCondition("SubscriberSiteID=" + CMSContext.CurrentSiteID, subscriberCondition);
        DataSet ds = null;
        if (useView)
        {
            ds = SubscriberInfoProvider.GetSubscribersFromView(tempWhere, null, -1, "SubscriberID");
        }
        else
        {
            ds = SubscriberInfoProvider.GetSubscribers(tempWhere, null, -1, "SubscriberID");
        }

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            List<string> subscriberIDs = SystemDataHelper.GetStringValues(ds.Tables[0], "SubscriberID");

            string subscriberIDCond = string.Empty;
            int maxItems = 10000;
            int maxToProcess = 0;
            do
            {
                maxToProcess = (subscriberIDs.Count > maxItems) ? maxItems : subscriberIDs.Count;

                // Get condition in form 'SubscriberID IN (....)' with maximally 10.000 items
                subscriberIDCond = SqlHelperClass.AddWhereCondition(subscriberIDCond, SqlHelperClass.GetWhereCondition<string>("SubscriberID", subscriberIDs.GetRange(0, maxToProcess), false), "OR");

                subscriberIDs.RemoveRange(0, maxToProcess);
            }
            while (subscriberIDs.Count > 0);

            // Add the condition that only clasic subscribers should be filtered with the IDs condition
            result = SqlHelperClass.AddWhereCondition("SubscriberID>0", subscriberIDCond);
        }

        // Add original condition for contact group members
        return SqlHelperClass.AddWhereCondition(result, string.Format("(SubscriberID=0 AND {0})", baseCondition), "OR");
    }

    #endregion
}