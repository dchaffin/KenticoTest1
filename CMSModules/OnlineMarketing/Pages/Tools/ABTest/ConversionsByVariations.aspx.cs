using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls;

public partial class CMSModules_OnlineMarketing_Pages_Tools_ABTest_ConversionsByVariations : CMSABTestPage
{
    #region "Variables"

    private bool isSaved = false;
    private bool reportDisplayed = false;
    private IDisplayReport ucDisplayReport;

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
        CheckWebAnalyticsUI("ABTest.ConversionsByVariations");
        CurrentMaster.PanelContent.CssClass = "";
        UIHelper.AllowUpdateProgress = false;

        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlDisplayReport.Controls.Add((Control)ucDisplayReport);

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        String siteName = CMSContext.CurrentSiteName;     
     
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        ucABTests.ReturnColumnName = "ABTestName";
        ucABTests.AllowEmpty = false;
        ucABTests.ReloadData(false);
        ucABTests.PostbackOnChange = true;
        ucABTests.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
        ucSelectVariation.WhereCondition = GetWhereCondition();
        ucSelectVariation.ShowAllVariations = true;
    }


    /// <summary>
    /// VerifyRenderingInServerForm.
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!isSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
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
    /// Returns where condition for variation selector.
    /// </summary>
    private string GetWhereCondition()
    {
        string testName = ValidationHelper.GetString(ucABTests.Value, String.Empty);
        return String.Format("ABVariantTestID  IN (SELECT ABTestID FROM OM_ABTest WHERE ABTestName = N'{0}' AND ABTestSiteID = {1})",SqlHelperClass.GetSafeQueryString(testName, false), CMSContext.CurrentSiteID);
    }


    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        ucSelectVariation.WhereCondition = GetWhereCondition();
        ucSelectVariation.ReloadData(true);
    }


    /// <summary>
    /// Displays the report
    /// </summary>
    /// <param name="reload">If true display reload control is reloaded</param>
    private void DisplayReport(bool reload)
    {
        if (reportDisplayed)
        {
            return;
        }

        ucGraphType.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable dtp = new DataTable();

        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CodeName", typeof(string));
        dtp.Columns.Add("TestName", typeof(string));
        dtp.Columns.Add("VariationName", typeof(string));

        object[] parameters = new object[5];

        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "abtests";
        parameters[3] = ValidationHelper.GetString(ucABTests.Value, String.Empty);
        parameters[4] = ValidationHelper.GetString(ucSelectVariation.Value, String.Empty);

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();

        String reportName = ucGraphType.GetReportName("abtestconversionsbyvariations.yearreport;abtestconversionsbyvariations.monthreport;abtestconversionsbyvariations.weekreport;abtestconversionsbyvariations.dayreport;abtestconversionsbyvariations.hourreport");
        ucDisplayReport.ReportName = reportName;

        // Conversion count
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

            if (reload)
            {
                ucDisplayReport.ReloadData(true);
            }
        }

        reportDisplayed = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport(true);

        reportHeader.ReportName = ucDisplayReport.ReportName;
        reportHeader.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeader.ManageDataCodeName = "abtest";
        base.OnPreRender(e);
    }


    /// <summary>
    /// Saves the graph report.
    /// </summary>
    private void Save()
    {
        // Check web analytics save permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.WebAnalytics", "SaveReports"))
        {
            RedirectToCMSDeskAccessDenied("CMS.WebAnalytics", "SaveReports");
        }

        DisplayReport(false);

        // Saves the report        
        isSaved = true;

        if (ucDisplayReport.SaveReport() > 0)
        {
            ShowConfirmation(String.Format(GetString("Analytics_Report.ReportSavedTo"), ucDisplayReport.ReportDisplayName + " - " + DateTime.Now.ToString()));
        }

        isSaved = false;
    }

    #endregion
}