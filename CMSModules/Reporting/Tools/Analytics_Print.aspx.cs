using System;
using System.Data;
using System.Threading;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

using CultureInfo = System.Globalization.CultureInfo;

public partial class CMSModules_Reporting_Tools_Analytics_Print : CMSWebAnalyticsPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Check security
        CheckSecurity();

        // Set cultures
        SetCulture();
        IFormatProvider culture = DateTimeHelper.DefaultIFormatProvider;
        IFormatProvider currentCulture = new CultureInfo(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);

        // Get report info
        string reportName = ValidationHelper.GetString(Request.QueryString["reportname"], "");
        ReportInfo report = ReportInfoProvider.GetReportInfo(reportName);

        if (report != null)
        {
            // Get report parameters
            string parameters = QueryHelper.GetString("parameters", "");
            DataRow drParameters = ReportHelper.GetReportParameters(report, parameters, AnalyticsHelper.PARAM_SEMICOLON, culture, currentCulture);

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
        ManagersContainer = plcMenu;
    }


    /// <summary>
    /// Checks security.
    /// </summary>
    private void CheckSecurity()
    {
        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Reporting", CMSContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Reporting");
        }

        // Check 'Read' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Reporting", "Read"))
        {
            RedirectToAccessDenied("CMS.Reporting", "Read");
        }
    }
}