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
using CMS.ExtendedControls.ActionsConfig;


public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_ABTestReports : CMSABTestPage
{
    #region "Variables"

    private bool isSaved = false;
    private bool reportLoaded = false;
    private string testName = String.Empty;
    String siteName = String.Empty;

    private IDisplayReport ucDisplayReport = null;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;
        pnlContent.Controls.Add((Control)ucDisplayReport);

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled"));
        ucDisabledModule.ParentPanel = pnlWarning;

        CurrentMaster.PanelContent.CssClass = "";

        UIHelper.AllowUpdateProgress = false;

        // ABTest Info
        int abTestID = QueryHelper.GetInteger("abtestid", 0);
        ABTestInfo abInfo = ABTestInfoProvider.GetABTestInfo(abTestID);
        if (abInfo == null)
        {
            return;
        }

        siteName = CMSContext.CurrentSiteName;
        testName = abInfo.ABTestName;

        // Variants condition
        ucSelectVariation.WhereCondition = "ABVariantTestID  IN ( SELECT ABTestID FROM OM_ABTest WHERE ABTestName = N'" + testName + "')";
        ucGraphType.ProcessChartSelectors(false);

        // Enables/disables radio buttons (based on UI elements)
        CurrentUserInfo ui = CMSContext.CurrentUser;

        if (!RequestHelper.IsPostBack())
        {
            if (!ui.IsGlobalAdministrator)
            {
                rbCount.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "ABTest.ConversionsCount");
                rbRate.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "ABTest.ConversionsRate");
                rbValue.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "ABTest.ConversionsValue");
                rbSourcePages.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "ABTest.ConversionsSourcePages");
                rbVariants.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "ABTest.ConversionsByVariations");

                bool checkedButton = false;

                // Check first enabled button 
                foreach (Control ctrl in pnlRadios.Controls)
                {
                    RadioButton rb = ctrl as RadioButton;
                    if (rb != null)
                    {
                        if (rb.Enabled)
                        {
                            rb.Checked = true;
                            checkedButton = true;
                            break;
                        }
                    }
                }

                // No report avaible -> redirect to access denied
                if (!checkedButton)
                {
                    RedirectToAccessDenied(GetString("abtest.noreportavaible"));
                }
            }
            else
            {
                // Admin check first radio button
                rbCount.Checked = true;
            }
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
    /// Display report
    /// </summary>
    /// <param name="reload">If true, display report control is reloaded</param>
    private void DisplayReport(bool reload)
    {
        if (reportLoaded)
        {
            return;
        }

        String siteName = CMSContext.CurrentSiteName;

        // Set reports name
        String conversionCount = "abtestconversioncount.yearreport;abtestconversioncount.monthreport;abtestconversioncount.weekreport;abtestconversioncount.dayreport;abtestconversioncount.hourreport";
        String conversionRate = "abtestdetailconversionrate.yearreport;abtestdetailconversionrate.monthreport;abtestdetailconversionrate.weekreport;abtestdetailconversionrate.dayreport;abtestdetailconversionrate.hourreport";
        String conversionValue = "abtestconversionvalue.yearreport;abtestconversionvalue.monthreport;abtestconversionvalue.weekreport;abtestconversionvalue.dayreport;abtestconversionvalue.hourreport";
        String conversionSourcePages = "abtestconversionsource.yearreport;abtestconversionsource.monthreport;abtestconversionsource.weekreport;abtestconversionsource.dayreport;abtestconversionsource.hourreport";
        String conversionVariant = "abtestconversionsbyvariations.yearreport;abtestconversionsbyvariations.monthreport;abtestconversionsbyvariations.weekreport;abtestconversionsbyvariations.dayreport;abtestconversionsbyvariations.hourreport";

        pnlVariant.Visible = false;

        if (rbCount.Checked)
        {
            CheckWebAnalyticsUI("ABTest.ConversionsCount");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionCount);
        }

        if (rbRate.Checked)
        {
            CheckWebAnalyticsUI("ABTest.ConversionsRate");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionRate);
        }

        if (rbValue.Checked)
        {
            CheckWebAnalyticsUI("ABTest.ConversionsValue");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionValue);
        }

        if (rbSourcePages.Checked)
        {
            CheckWebAnalyticsUI("ABTest.ConversionsSourcePages");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionSourcePages);
        }

        if (rbVariants.Checked)
        {
            CheckWebAnalyticsUI("ABTest.ConversionsByVariations");
            pnlVariant.Visible = true;
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionVariant);
        }

        // Conversion
        ucConversions.PostbackOnDropDownChange = true;
        ucConversions.ABTestName = testName;
        ucConversions.ReloadData(true);

        String conversion = ValidationHelper.GetString(ucConversions.Value, String.Empty);
        if (conversion == ucConversions.AllRecordValue)
        {
            conversion = String.Empty;
        }

        // General report data
        ucDisplayReport.LoadFormParameters = false;
        ucDisplayReport.DisplayFilter = false;
        ucDisplayReport.GraphImageWidth = 100;
        ucDisplayReport.IgnoreWasInit = true;
        ucDisplayReport.TableFirstColumnWidth = Unit.Percentage(30);
        ucDisplayReport.UseExternalReload = true;
        ucDisplayReport.UseProgressIndicator = true;
        ucDisplayReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(ucGraphType.SelectedInterval);

        // Resolve report macros 
        DataTable dtp = new DataTable();
        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CodeName", typeof(string));
        dtp.Columns.Add("TestName", typeof(string));
        dtp.Columns.Add("ConversionName", typeof(string));
        dtp.Columns.Add("VariationName", typeof(string));

        object[] parameters = new object[6];
        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "pageviews";
        parameters[3] = testName;
        parameters[4] = conversion;
        parameters[5] = ValidationHelper.GetString(ucSelectVariation.Value, String.Empty);

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();
        ucDisplayReport.ReportParameters = dtp.Rows[0];

        if (reload)
        {
            ucDisplayReport.ReloadData(true);
        }

        reportLoaded = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport(true);

        reportHeader.ReportName = ucDisplayReport.ReportName;
        reportHeader.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeader.ManageDataCodeName = "abconversion;" + testName + ";%";

        base.OnPreRender(e);
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