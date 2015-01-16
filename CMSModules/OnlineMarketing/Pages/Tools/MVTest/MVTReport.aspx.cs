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
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.ExtendedControls;

public partial class CMSModules_OnlineMarketing_Pages_Tools_MVTest_MVTReport : CMSMVTestPage
{
    #region "Variables"

    private bool isSaved = false;

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
        CheckWebAnalyticsUI(QueryHelper.GetString("datacodename", String.Empty));

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;
        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlDisplayReport.Controls.Add((Control)ucDisplayReport);

        CurrentMaster.PanelContent.CssClass = "";
        UIHelper.AllowUpdateProgress = false;

        String siteName = CMSContext.CurrentSiteName;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSMVTEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("mvt.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        ucMVTests.ReturnColumnName = "MVTestName";
        ucMVTests.AllowEmpty = false;
        ucMVTests.PostbackOnChange = true;
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


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport(true);

        reportHeader.ReportName = ucDisplayReport.ReportName;
        reportHeader.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeader.ManageDataCodeName = "mvtest";

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


    /// <summary>
    /// Displays the report
    /// </summary>
    /// <param name="reload">If true display reload control is reloaded</param>
    private void DisplayReport(bool reload)
    {
        ucGraphType.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable dtp = new DataTable();

        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CodeName", typeof(string));
        dtp.Columns.Add("MVTestName", typeof(string));
        dtp.Columns.Add("ConversionName", typeof(string));
        dtp.Columns.Add("CombinationName", typeof(string));

        object[] parameters = new object[6];

        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "";
        parameters[3] = ValidationHelper.GetString(ucMVTests.Value, String.Empty);

        // Conversion
        String conversion = ValidationHelper.GetString(ucConversions.Value, String.Empty);
        if (conversion == ucConversions.AllRecordValue)
        {
            conversion = String.Empty;
        }

        parameters[4] = ValidationHelper.GetString(conversion, String.Empty);

        // Combination
        String combination = ValidationHelper.GetString(usCombination.Value, String.Empty);
        if (combination == usCombination.AllRecordValue)
        {
            combination = String.Empty;
        }

        parameters[5] = ValidationHelper.GetString(combination, String.Empty);

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();

        string reportName = ucGraphType.GetReportName(QueryHelper.GetString("reportCodeName", String.Empty));

        // Hide show selectors by report name
        if (reportName.Contains("mvtestconversionsbycombinations"))
        {
            pnlConversion.Visible = false;
        }
        else
        {
            pnlCultures.Visible = false;
            pnlCombination.Visible = false;
        }

        // Load conversion by mvt code name
        if (pnlConversion.Visible)
        {
            ucConversions.MVTestName = ValidationHelper.GetString(ucMVTests.Value, String.Empty);
            ucConversions.PostbackOnDropDownChange = true;
            ucConversions.ReloadData(true);
        }

        // Load combination by mvt name
        if (pnlCombination.Visible)
        {
            string name = ValidationHelper.GetString(ucMVTests.Value, String.Empty);
            if ((name != String.Empty) && (name != ucMVTests.AllRecordValue))
            {
                string cultureWhere = String.Empty;
                string siteName = CMSContext.CurrentSiteName;

                MVTestInfo mvt = MVTestInfoProvider.GetMVTestInfo(name, siteName);
                if (mvt != null)
                {
                    string culture = ValidationHelper.GetString(usCulture.Value, String.Empty);
                    if ((culture != String.Empty) && (culture != usCulture.AllRecordValue))
                    {
                        cultureWhere = " AND DocumentCulture = '" + culture.Replace("'", "''") + "'";
                    }

                    // Get the used page template column
                    string colName = "DocumentPageTemplateID";

                    try
                    {
                        PageInfo pi = PageInfoProvider.GetPageInfo(siteName, mvt.MVTestPage, culture, null,
                                                                   SiteInfoProvider.CombineWithDefaultCulture(siteName));
                        if (pi != null)
                        {
                            colName = pi.GetUsedPageTemplateIdColumn();
                        }
                    }
                    catch{}

                    // Prepare where condition
                    string where = String.Format("MVTCombinationPageTemplateID IN (SELECT {0} FROM View_CMS_Tree_Joined WHERE NodeSiteID = {1} AND NodeAliasPath ='{2}'{3})", colName, CMSContext.CurrentSiteID, SqlHelperClass.GetSafeQueryString(mvt.MVTestPage), cultureWhere);

                    usCombination.WhereCondition = SqlHelperClass.AddWhereCondition(usCombination.WhereCondition, where);
                    usCombination.ReloadData(true);
                }
            }
        }

        // Set report
        ucDisplayReport.ReportName = reportName;

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

            // Reload data only if parameter is set
            if (reload)
            {
                ucDisplayReport.ReloadData(true);
            }
        }
    }

    #endregion
}