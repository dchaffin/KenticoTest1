using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.WebAnalytics;

public partial class CMSModules_Reporting_Controls_ReportValue : AbstractReportControl
{
    #region "Variables"

    private ReportValueInfo mValueInfo = null;
    private string mParameter = string.Empty;
    private ReportInfo ri = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Value info for direct input (no load from DB).
    /// </summary>
    public ReportValueInfo ValueInfo
    {
        get
        {
            if (mValueInfo == null)
            {
                mValueInfo = ReportValueInfoProvider.GetReportValueInfo(Parameter);
            }

            return mValueInfo;
        }
        set
        {
            mValueInfo = value;
        }
    }


    /// <summary>
    /// Report value connection string
    /// </summary>
    public override string ConnectionString
    {
        get
        {
            String valueConn = (ValueInfo == null) ? String.Empty : ValueInfo.ValueConnectionString;
            if (String.IsNullOrEmpty(valueConn))
            {
                return (ri == null) ? String.Empty : ri.ReportConnectionString;
            }

            return valueConn;
        }
    }


    /// <summary>
    /// Value name - prevent using viewstate  (problems with displayreportcontrol and postback).
    /// </summary>
    public override string Parameter
    {
        get
        {
            return mParameter;
        }
        set
        {
            mParameter = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if graph belongs to report.
    /// </summary>
    /// <param name="report">Report to validate</param>
    public override bool IsValid(ReportInfo report)
    {
        ReportValueInfo rvi = ValueInfo;

        if ((report != null) && (rvi != null) && (report.ReportID == rvi.ValueReportID))
        {
            return true;
        }

        return false;
    }


    protected override void OnLoad(EventArgs e)
    {
        ItemType = ReportItemType.Value;
        base.OnLoad(e);
    }


    /// <summary>
    /// OnPreRender handler - register progress script.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Register context menu for export - if allowed
        EnableExport = false;

        if (ValueInfo != null)
        {
            menuCont.RenderAsTag = HtmlTextWriterTag.Span;
            RegisterSubscriptionScript(ValueInfo.ValueReportID, "valueid", ValueInfo.ValueID, menuCont);
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        try
        {
            // Load value info object
            ReportValueInfo rvi = ValueInfo;

            if (rvi != null)
            {
                //Test security
                ri = ReportInfoProvider.GetReportInfo(rvi.ValueReportID);
                if (ri.ReportAccess != ReportAccessEnum.All)
                {
                    if (!CMSContext.CurrentUser.IsAuthenticated())
                    {
                        Visible = false;
                        return;
                    }
                }

                EnableSubscription = (EnableSubscription && ValidationHelper.GetBoolean(ValueInfo.ValueSettings["SubscriptionEnabled"], true) && ri.ReportEnableSubscription);
                if (EmailMode && !EnableSubscription)
                {
                    this.Visible = false;
                    return;
                }

                ContextResolver resolver = CMSContext.CurrentResolver;
                // Resolve dynamic data macros
                if (DynamicMacros != null)
                {
                    for (int i = 0; i <= DynamicMacros.GetUpperBound(0); i++)
                    {
                        resolver.AddDynamicParameter(DynamicMacros[i, 0], DynamicMacros[i, 1]);
                    }
                }

                QueryIsStoredProcedure = rvi.ValueQueryIsStoredProcedure;
                QueryText = resolver.ResolveMacros(rvi.ValueQuery);

                //Set default parametrs directly if not set
                if (ReportParameters == null)
                {
                    if (ri != null)
                    {
                        FormInfo fi = new FormInfo(ri.ReportParameters);
                        // Get datarow with required columns
                        ReportParameters = fi.GetDataRow(false);
                        fi.LoadDefaultValues(ReportParameters, true);
                    }
                }
            }

            // Only use base parameters in case of stored procedure
            if (QueryIsStoredProcedure)
            {
                AllParameters = SpecialFunctions.ConvertDataRowToParams(ReportParameters, null);
            }

            // Load data
            DataSet ds = LoadData();

            // If datasource is emptry, create empty dataset
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Set literal text
                string value = rvi.ValueFormatString;
                if ((value == null) || (value == ""))
                {
                    value = ValidationHelper.GetString(ds.Tables[0].Rows[0][0], "");
                }
                else
                {
                    value = string.Format(value, ds.Tables[0].Rows[0].ItemArray);
                }

                if (EmailMode)
                {
                    ltlEmail.Text = HTMLHelper.HTMLEncode(ResolveMacros(value));
                    ltlEmail.Visible = true;
                    menuCont.Visible = false;
                }
                else
                {
                    lblValue.Text = HTMLHelper.HTMLEncode(ResolveMacros(value));
                }
            }
            else if (EmailMode && SendOnlyNonEmptyDataSource)
            {
                Visible = false;
            }
        }
        catch (Exception ex)
        {
            // Display error message, if data load fail
            lblError.Visible = true;
            lblError.Text = "[ReportValue.ascx] Error loading the data: " + ex.Message;
            EventLogProvider ev = new EventLogProvider();
            ev.LogEvent("Report value", "E", ex);
        }
    }

    #endregion
}