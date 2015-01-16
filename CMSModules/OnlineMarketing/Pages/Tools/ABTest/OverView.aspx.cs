using System;
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

public partial class CMSModules_OnlineMarketing_Pages_Tools_AbTest_OverView : CMSABTestPage
{
    private IDisplayReport ucTestValuesReport;
    private IDisplayReport ucConversionRateReport;
    private IDisplayReport ucDisplayReport;


    protected void Page_Load(object sender, EventArgs e)
    {
        CheckWebAnalyticsUI("ABTest.Overview");

        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlOverview.Controls.Add((Control)ucDisplayReport);

        ucConversionRateReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlConversionRate.Controls.Add((Control)ucConversionRateReport);

        ucTestValuesReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlConversionValue.Controls.Add((Control)ucTestValuesReport);

        listElem.ShowObjectMenu = false;
        CurrentMaster.PanelContent.CssClass = "";
        String siteName = CMSContext.CurrentSiteName;
        UIHelper.AllowUpdateProgress = false;


        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        listElem.ShowFilter = false;
        listElem.DelayedReload = true;
        listElem.Grid.GridName = "~/CMSModules/OnlineMarketing/Controls/UI/ABTest/ListWithValues.xml";

        lnkTestRate.NavigateUrl = "#";
        lnkTestValue.NavigateUrl = "#";
        lnkTestValue.Attributes["OnClick"] = "if (parent.parent.selectTreeNode != null) parent.parent.selectTreeNode('#node_abtestconversionsvalue');parent.location=\"abtest_frameset.aspx?page=conversionsvalue&displayTab=abtesting.conversionsvalue\"";
        lnkTestRate.Attributes["OnClick"] = "if (parent.parent.selectTreeNode != null) parent.parent.selectTreeNode('#node_abtestconversionsrate');parent.location=\"abtest_frameset.aspx?page=conversionsrate&displayTab=abtesting.conversionrate\"";

        ucABTests.AllowAll = true;
        ucABTests.AllowEmpty = false;
        ucABTests.IsLiveSite = false;
        ucABTests.ReturnColumnName = "ABTestName";
        ucABTests.PostbackOnChange = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        ucGraphType.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable dtp = new DataTable();

        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CodeName", typeof(string));
        dtp.Columns.Add("TestName", typeof(string));
        dtp.Columns.Add("ConversionName", typeof(string));

        object[] parameters = new object[5];

        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "abtests";
        parameters[3] = String.Empty;
        parameters[4] = String.Empty;

        String reportName = ucGraphType.GetReportName("abtests.yearreport;abtests.monthreport;abtests.weekreport;abtests.dayreport;abtests.hourreport");
        String conversionRateReportName = ucGraphType.GetReportName("abtestconversionrate.yearreport;abtestconversionrate.monthreport;abtestconversionrate.weekreport;abtestconversionrate.dayreport;abtestconversionrate.hourreport");
        String valuesReportName = ucGraphType.GetReportName("abtestsvalue.yearreport;abtestsvalue.monthreport;abtestsvalue.weekreport;abtestsvalue.dayreport;abtestsvalue.hourreport");

        string ABTestName = ValidationHelper.GetString(ucABTests.Value, String.Empty);
        if ((ABTestName != ucABTests.AllRecordValue) && (!String.IsNullOrEmpty(ABTestName)))
        {
            parameters[3] = ABTestName;

            listElem.Grid.WhereCondition = SqlHelperClass.AddWhereCondition(listElem.Grid.WhereCondition, "ABTestName='" + SqlHelperClass.GetSafeQueryString(ABTestName, false) + "'");
            listElem.Grid.ReloadData();
        }

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();

        ucDisplayReport.ReportName = reportName;
        ucTestValuesReport.ReportName = valuesReportName;
        ucConversionRateReport.ReportName = conversionRateReportName;

        // Conversion count
        if (!ucDisplayReport.IsReportLoaded())
        {
            lblErrorConversions.Visible = true;
            lblErrorConversions.Text = String.Format(GetString("Analytics_Report.ReportDoesnotExist"), reportName);
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

        // Conversion value
        if (!ucTestValuesReport.IsReportLoaded())
        {
            lblErrorValues.Visible = true;
            lblErrorValues.Text = String.Format(GetString("Analytics_Report.ReportDoesnotExist"), valuesReportName);
        }
        else
        {
            ucTestValuesReport.LoadFormParameters = false;
            ucTestValuesReport.DisplayFilter = false;
            ucTestValuesReport.ReportParameters = dtp.Rows[0];
            ucTestValuesReport.GraphImageWidth = 50;
            ucTestValuesReport.AreaMaxWidth = ucDisplayReport.AreaMaxWidth;
            ucTestValuesReport.IgnoreWasInit = true;
            ucTestValuesReport.UseExternalReload = true;
            ucTestValuesReport.UseProgressIndicator = true;
            ucTestValuesReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(ucGraphType.SelectedInterval);
            ucTestValuesReport.ReloadData(true);
        }

        // Conversion rate
        if (!ucConversionRateReport.IsReportLoaded())
        {
            lblErrorRate.Visible = true;
            lblErrorRate.Text = String.Format(GetString("Analytics_Report.ReportDoesnotExist"), conversionRateReportName);
        }
        else
        {
            ucConversionRateReport.LoadFormParameters = false;
            ucConversionRateReport.DisplayFilter = false;
            ucConversionRateReport.ReportParameters = dtp.Rows[0];
            ucConversionRateReport.GraphImageWidth = 50;
            ucConversionRateReport.AreaMaxWidth = ucDisplayReport.AreaMaxWidth;
            ucConversionRateReport.UseExternalReload = true;
            ucConversionRateReport.UseProgressIndicator = true;
            ucConversionRateReport.IgnoreWasInit = true;
            ucConversionRateReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(ucGraphType.SelectedInterval);
            ucConversionRateReport.ReloadData(true);
        }

        base.OnPreRender(e);
    }
}