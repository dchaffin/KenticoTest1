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

public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignGoals : CMSCampaignAndConversionPage
{
    #region "Variables"

    private bool isSaved = false;
    private bool reportDisplayed = false;
    private string statCodeName = String.Empty;
    private string dataCodeName = String.Empty;
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


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";
        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        statCodeName = QueryHelper.GetString("statCodeName", String.Empty);
        dataCodeName = QueryHelper.GetString("dataCodeName", String.Empty);

        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlDisplayReport.Controls.Add((Control)ucDisplayReport);

        CheckWebAnalyticsUI(dataCodeName);

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;";
        ucDisabledModule.InfoText = GetString("WebAnalytics.Disabled");
        ucDisabledModule.ParentPanel = pnlDisabled;

        CurrentMaster.Title.TitleText = GetString("analytics_codename.campaign") + " - " + GetString("analytics_codename." + HTMLHelper.HTMLEncode(statCodeName));

        // Icon
        string iconName = QueryHelper.GetString("icon", String.Empty);
        string imageUrl = GetImageUrl("CMSModules/CMS_WebAnalytics/Details/" + iconName + ".png");

        if (!FileHelper.FileExists(imageUrl))
        {
            imageUrl = GetImageUrl("Objects/Reporting_ReportCategory/object.png");
        }

        CurrentMaster.Title.TitleImage = imageUrl;
        CurrentMaster.Title.HelpTopicName = Server.UrlEncode(statCodeName).Replace(".", "_");
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                DisplayReport(true);
                Save();
                break;
        }
    }


    /// <summary>
    /// OnPreRender - Display report
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport(true);

        reportHeader.ReportName = ucDisplayReport.ReportName;
        reportHeader.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeader.DisplayManageData = false;

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
    /// Displays the given report
    /// </summary>
    private void DisplayReport(bool reloadInnerReport)
    {
        // If report was already displayed .. return
        if (reportDisplayed)
        {
            return;
        }

        ucGraphType.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable dtp = new DataTable();

        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CampaignName", typeof(string));

        object[] parameters = new object[3];

        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "";

        // Get report name from query
        string reportName = ucGraphType.GetReportName(QueryHelper.GetString("reportCodeName", String.Empty));

        if (CMSString.Compare(Convert.ToString(ucSelectCampaign.Value), "-1", true) != 0)
        {
            parameters[2] = ucSelectCampaign.Value;
        }
        else
        {
            reportName = "all" + reportName;
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
            if (reloadInnerReport)
            {
                ucDisplayReport.ReloadData(true);
            }
        }

        if (reloadInnerReport)
        {
            // Mark as report displayed
            reportDisplayed = true;
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

        isSaved = true;

        if (ucDisplayReport.SaveReport() > 0)
        {
            ShowConfirmation(String.Format(GetString("Analytics_Report.ReportSavedTo"), ucDisplayReport.ReportDisplayName + " - " + DateTime.Now.ToString()));
        }

        isSaved = false;
    }
}