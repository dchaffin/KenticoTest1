using System;
using System.Data;
using System.Threading;
using System.Web.UI;

using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.UIControls;

using CultureInfo = System.Globalization.CultureInfo;

public partial class CMSModules_Reporting_Tools_Ecommerce_Report_Print : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Get report name
        string reportName = QueryHelper.GetString("reportName", null);

        // Check permissions
        bool isEcommerceReport = ReportInfoProvider.IsEcommerceReport(reportName);
        CMSEcommerceReportsPage.CheckPermissions(isEcommerceReport);

        SetCulture();

        ReportInfo report = ReportInfoProvider.GetReportInfo(reportName);
        if (report != null)
        {
            // Get report parameters
            string parameters = QueryHelper.GetString("parameters", "");
            IFormatProvider culture = DateTimeHelper.DefaultIFormatProvider;
            IFormatProvider currentCulture = new CultureInfo(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
            DataRow drParameters = ReportHelper.GetReportParameters(report, parameters, null, culture, currentCulture);

            // Init report
            if (drParameters != null)
            {
                DisplayReport1.LoadFormParameters = false;
                DisplayReport1.ReportParameters = drParameters;
            }

            DisplayReport1.ReportName = reportName;
            DisplayReport1.DisplayFilter = false;

            Page.Title = GetString("report_print.lblprintreport") + " " + HTMLHelper.HTMLEncode(report.ReportDisplayName);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ManagersContainer = pnlManager;
    }
}