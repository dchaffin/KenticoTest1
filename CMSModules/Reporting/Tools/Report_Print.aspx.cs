using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.UIControls;

public partial class CMSModules_Reporting_Tools_Report_Print : CMSReportingModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Set cultures
        SetCulture();
        IFormatProvider culture = DateTimeHelper.DefaultIFormatProvider;
        IFormatProvider currentCulture = new CultureInfo(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);

        // Get report info
        int reportId = ValidationHelper.GetInteger(Request.QueryString["reportid"], 0);
        ReportInfo report = ReportInfoProvider.GetReportInfo(reportId);
        
        if (report != null)
        {
            // Get report parameters
            string parameters = QueryHelper.GetString("parameters", "");
            DataRow drParameters = ReportHelper.GetReportParameters(report, parameters, null, culture, currentCulture);

            // Init report
            if (drParameters != null)
            {
                DisplayReport1.LoadFormParameters = false;
                DisplayReport1.ReportParameters = drParameters;
            }

            DisplayReport1.ReportName = report.ReportName;
            DisplayReport1.DisplayFilter = false;

            Page.Title = GetString("Report_Print.lblPrintReport") + " " + HTMLHelper.HTMLEncode(report.ReportDisplayName);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        ManagersContainer = pnlManager;
    }
}