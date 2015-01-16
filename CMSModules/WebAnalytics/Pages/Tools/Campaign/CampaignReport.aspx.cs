using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls;

public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignReport : CMSCampaignAndConversionPage
{
    #region "Variables"

    private bool isSaved = false;
    private bool reportDisplayed = false;
    private String dataCodeName = String.Empty;
    private String reportCodeNames = String.Empty;
    private int conversionID = 0;

    private string allDetailReport = "campaigns.alldetails";
    private string singleDetailReport = "campaigns.singledetails";
    private IDisplayReport ucDisplayReport;

    private bool allowNoTimeSelection = false;
    private string deleteParam = "campaigns";

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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        conversionID = QueryHelper.GetInteger("conversionid", 0);
        CurrentMaster.PanelContent.CssClass = "";
        UIHelper.AllowUpdateProgress = false;

        reportHeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;";
        ucDisabledModule.InfoText = GetString("WebAnalytics.Disabled");
        ucDisabledModule.ParentPanel = pnlDisabled;

        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlDisplayReport.Controls.Add((Control)ucDisplayReport);

        dataCodeName = QueryHelper.GetString("dataCodeName", String.Empty);
        reportCodeNames = QueryHelper.GetString("reportCodeName", String.Empty);

        // Control initialization (based on displayed report)
        switch (dataCodeName)
        {
            // Overview
            case "campaign":
                CheckWebAnalyticsUI("campaign.overview");
                ucReportHeader.CampaignAllowAll = true;
                ucReportHeader.ShowConversionSelector = false;
                break;

            // Conversion count 
            case "campconversioncount":
                dataCodeName = "campconversion";
                CheckWebAnalyticsUI("CampaignConversionCount");
                ucReportHeader.CampaignAllowAll = false;
                break;

            // Conversion value 
            case "campconversionvalue":
                dataCodeName = "campconversion";
                CheckWebAnalyticsUI("campaignsConversionValue");
                ucReportHeader.CampaignAllowAll = false;
                break;

            // Campaign compare
            case "campaigncompare":
                CheckWebAnalyticsUI("CampaignCompare");
                ucReportHeader.ShowCampaignSelector = false;
                dataCodeName = ucReportHeader.CodeNameByGoal;
                ucReportHeader.ShowGoalSelector = true;
                ucReportHeader.ShowSiteSelector = true;

                // Get table column name
                string name = "analytics.hits";
                switch (ucReportHeader.SelectedGoal.ToLowerCSafe())
                {
                    case "view":
                        name = "analytics.view";
                        break;
                    case "count":
                        name = "conversion.count";
                        break;
                    case "value":
                        name = "om.trackconversionvalue";
                        break;
                }

                string[,] dynamicMacros = new string[1, 2];
                dynamicMacros[0, 0] = "ColumnName";
                dynamicMacros[0, 1] = GetString(name);

                ucDisplayReport.DynamicMacros = dynamicMacros;
                break;

            // Campaign detail
            case "campaigndetails":
                CheckWebAnalyticsUI("CampaignDetails");
                ucReportHeader.ShowConversionSelector = false;
                String selectedCampaign = ValidationHelper.GetString(ucReportHeader.SelectedCampaign, String.Empty);
                reportCodeNames = (selectedCampaign == ucReportHeader.AllRecordValue || selectedCampaign == String.Empty) ? allDetailReport : singleDetailReport;
                ucGraphType.ShowIntervalSelector = false;
                allowNoTimeSelection = true;
                ucGraphType.AllowEmptyDate = true;
                break;

            case "conversion":
                CheckWebAnalyticsUI("Conversion.Overview");
                ucReportHeader.ShowCampaignSelector = false;
                ucReportHeader.ShowGoalSelector = true;
                ucReportHeader.ShowVisitsInGoalSelector = false;
                deleteParam = "conversion";
                break;

            case "conversiondetail":
                CheckWebAnalyticsUI("Conversion.Details");
                ucReportHeader.ShowCampaignSelector = false;
                ucReportHeader.ShowConversionSelector = (conversionID != 0) ? false : true;
                ucGraphType.ShowIntervalSelector = false;
                allowNoTimeSelection = true;
                ucGraphType.AllowEmptyDate = true;
                deleteParam = "conversion";
                break;
        }

        // Set table same first column width for all time
        ucDisplayReport.TableFirstColumnWidth = Unit.Percentage(20);
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                Save();
                break;
        }
    }


    /// <summary>
    /// Displays the given report
    /// </summary>
    private void DisplayReport()
    {
        // If report was already displayed .. return
        if (reportDisplayed)
        {
            return;
        }

        ucGraphType.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable dtp = new DataTable();

        // In case of hidden datetime -> for save purpose pass from (to) as now to query parameter
        DateTime from = ((ucGraphType.From == DateTimeHelper.ZERO_TIME) && !pnlHeader.Visible) ? DateTime.Now : ucGraphType.From;
        DateTime to = ((ucGraphType.To == DateTimeHelper.ZERO_TIME) && !pnlHeader.Visible) ? DateTime.Now : ucGraphType.To;

        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CodeName", typeof(string));
        dtp.Columns.Add("CampaignName", typeof(string));
        dtp.Columns.Add("ConversionName", typeof(string));
        dtp.Columns.Add("Goal", typeof(string));
        dtp.Columns.Add("SiteID", typeof(int));

        object[] parameters = new object[7];

        parameters[0] = (allowNoTimeSelection && from == DateTimeHelper.ZERO_TIME) ? (DateTime?)null : from;
        parameters[1] = (allowNoTimeSelection && to == DateTimeHelper.ZERO_TIME) ? (DateTime?)null : to;
        parameters[2] = dataCodeName;
        parameters[3] = "";
        parameters[4] = "";
        parameters[5] = ucReportHeader.SelectedGoal;
        parameters[6] = ucReportHeader.SelectedSiteID;

        // Get report name from query
        String reportName = ucGraphType.GetReportName(reportCodeNames);

        // Filter campaign if any campaign selected
        string campaignName = ValidationHelper.GetString(ucReportHeader.SelectedCampaign, String.Empty);
        if ((campaignName != ucReportHeader.AllRecordValue) && (!String.IsNullOrEmpty(campaignName)))
        {
            parameters[3] = campaignName;
        }

        if (conversionID == 0)
        {
            // Filter conversion
            String conversionName = ValidationHelper.GetString(ucReportHeader.SelectedConversion, String.Empty);
            if ((conversionName != ucReportHeader.AllRecordValue) && (!String.IsNullOrEmpty(conversionName)))
            {
                parameters[4] = conversionName;
            }
        }
        else
        {
            ConversionInfo ci = ConversionInfoProvider.GetConversionInfo(conversionID);
            if (ci != null)
            {
                parameters[4] = ci.ConversionName;
                deleteParam = "singleconversion;" + ci.ConversionName;
            }
        }

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();

        ucDisplayReport.ReportName = reportName;

        // Set display report
        if (!ucDisplayReport.IsReportLoaded())
        {
            ShowError(String.Format(GetString("Analytics_Report.ReportDoesnotExist"), reportName));
        }
        else
        {
            ucDisplayReport.LoadFormParameters = false;
            ucDisplayReport.DisplayFilter = false;
            ucDisplayReport.ReportParameters = dtp.Rows[0];
            ucDisplayReport.GraphImageWidth = 100;
            ucDisplayReport.IgnoreWasInit = true;
            ucDisplayReport.UseExternalReload = true;
            ucDisplayReport.UseProgressIndicator = true;
            ucDisplayReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(ucGraphType.SelectedInterval);
            ucDisplayReport.ReloadData(true);
        }

        // Mark as report displayed
        reportDisplayed = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport();

        reportHeaderActions.ReportName = ucDisplayReport.ReportName;
        reportHeaderActions.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeaderActions.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeaderActions.ManageDataCodeName = deleteParam;

        base.OnPreRender(e);
    }


    /// <summary>
    /// Used in rendering to control outside render stage (save method) 
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!isSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }


    /// <summary>
    /// Saves the graph report.
    /// </summary>
    private void Save()
    {
        DisplayReport();

        // Check web analytics save permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.WebAnalytics", "SaveReports"))
        {
            RedirectToCMSDeskAccessDenied("CMS.WebAnalytics", "SaveReports");
        }

        isSaved = true;

        if (ucDisplayReport.SaveReport() > 0)
        {
            ShowConfirmation(String.Format(GetString("Analytics_Report.ReportSavedTo"), ucDisplayReport.ReportDisplayName + " - " + DateTime.Now.ToString()));
        }

        isSaved = false;
    }

    #endregion
}