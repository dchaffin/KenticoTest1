using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.SettingsProvider;

public partial class CMSModules_WebAnalytics_Tools_Analytics_Report : CMSWebAnalyticsPage
{
    #region "Variables"

    private bool isSaved = false;
    private string statCodeName;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check analytics UI
        CheckWebAnalyticsUI();

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        CurrentMaster.PanelContent.CssClass = "";
        ScriptHelper.RegisterDialogScript(Page);

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;";
        ucDisabledModule.InfoText = GetString("WebAnalytics.Disabled");
        ucDisabledModule.ParentPanel = pnlDisabled;

        UIHelper.AllowUpdateProgress = false;

        statCodeName = QueryHelper.GetString("statCodeName", String.Empty);
        string reportCodeName = QueryHelper.GetString("reportCodeName", String.Empty);
        string dataCodeName = QueryHelper.GetString("dataCodeName", String.Empty);

        ucReportViewer.DataName = dataCodeName;
        ucReportViewer.ReportsCodeName = reportCodeName;

        bool displayTitle = QueryHelper.GetBoolean("DisplayTitle", true);
        if (displayTitle)
        {
            CurrentMaster.Title.TitleText = GetString("analytics_codename." + HTMLHelper.HTMLEncode(statCodeName));
            String iName = "CMSModules/CMS_WebAnalytics/Details/" + dataCodeName + ".png";
            string imageUrl = GetImageUrl(iName);
            String corePath = URLHelper.ResolveUrl(UIHelper.GetImagePath(Page, iName, false, false));

            if (!FileHelper.FileExists(corePath))
            {
                imageUrl = GetImageUrl("Objects/Reporting_ReportCategory/object.png");
            }

            CurrentMaster.Title.TitleImage = imageUrl;


            if (!QueryHelper.GetBoolean("IsCustom", false))
            {
                CurrentMaster.Title.HelpTopicName = Server.UrlEncode(statCodeName).Replace(".", "_");
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


    protected override void OnPreRender(EventArgs e)
    {
        ucReportViewer.DisplayReport(false);
        reportHeader.ReportName = ucReportViewer.ReportName;
        reportHeader.ReportParameters = ucReportViewer.ReportParameters;
        reportHeader.SelectedInterval = ucReportViewer.SelectedInterval;
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

        isSaved = true;

        // Saves the report 
        ucReportViewer.SaveReport();

        isSaved = false;
    }

    #endregion
}