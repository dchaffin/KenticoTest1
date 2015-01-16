using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.SettingsProvider;

public partial class CMSModules_OnlineMarketing_Pages_Tools_ABTest_ConversionsValue : CMSABTestPage
{
    #region "Variables"

    private bool isSaved = false;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckWebAnalyticsUI("ABTest.ConversionsValue");

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        CurrentMaster.PanelContent.CssClass = "";
        UIHelper.AllowUpdateProgress = false;
        
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        ucConversionReportViewer.ReportsName = "abtestconversionvalue.yearreport;abtestconversionvalue.monthreport;abtestconversionvalue.weekreport;abtestconversionvalue.dayreport;abtestconversionvalue.hourreport";
        ucConversionReportViewer.ShowAllConversions = true;
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

        // Saves the report
        isSaved = true;
        ucConversionReportViewer.SaveReport();
        isSaved = false;
    }

    
    protected override void OnPreRender(EventArgs e)
    {
        ucConversionReportViewer.DisplayReport(true);

        reportHeader.ReportName = ucConversionReportViewer.ReportName;
        reportHeader.ReportParameters = ucConversionReportViewer.ReportParameters;
        reportHeader.SelectedInterval = ucConversionReportViewer.SelectedInterval;
        reportHeader.ManageDataCodeName = "abtest";
        
        base.OnPreRender(e);
    }

    #endregion
}