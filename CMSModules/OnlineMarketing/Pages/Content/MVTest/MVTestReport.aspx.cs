using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_MVTestReport : CMSOnlineMarketingPage
{
    #region "Variables"

    protected string mSave = null;
    protected string mPrint = null;
    protected string mDeleteData = null;
    private bool isSaved = false;
    private bool reportLoaded = false;
    private string testName = String.Empty;
    private IDisplayReport ucDisplayReport = null;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUserInfo cui = CMSContext.CurrentUser;

        // Check UI Permissions
        if ((cui == null) || (!cui.IsAuthorizedPerUIElement("CMS.Content", "OnlineMarketing.MVTests")))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "OnlineMarketing.MVTests");
        }

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.MVTesting);
        }

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSMVTEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("mvt.disabled"));
        ucDisabledModule.ParentPanel = pnlWarning;

        // Register actions
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, (s, args) => { Save(); });

        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlContent.Controls.Add((Control)ucDisplayReport);

        CurrentMaster.PanelContent.CssClass = "";
        UIHelper.AllowUpdateProgress = false;

        // MVTest Info
        int mvTestID = QueryHelper.GetInteger("mvTestID", 0);
        MVTestInfo mvInfo = MVTestInfoProvider.GetMVTestInfo(mvTestID);
        if (mvInfo == null)
        {
            return;
        }

        // Load combination by current template ID and culture
        if (pnlCombination.Visible)
        {
            int nodeID = QueryHelper.GetInteger("NodeID", 0);

            // Create condition for current template combinations            
            TreeProvider tree = new TreeProvider(cui);
            TreeNode node = tree.SelectSingleNode(nodeID, cui.PreferredCultureCode);

            if (node != null)
            {
                usCombination.DocumentID = node.DocumentID;
                usCombination.PageTemplateID = node.GetUsedPageTemplateId();
            }

            usCombination.ReloadData(true);
        }

        testName = mvInfo.MVTestName;

        ucGraphType.ProcessChartSelectors(false);

        // Enables/disables radio buttons (based on UI elements)
        CurrentUserInfo ui = CMSContext.CurrentUser;

        if (!RequestHelper.IsPostBack())
        {
            if (!ui.IsGlobalAdministrator)
            {
                rbCount.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "MVTestConversionCount");
                rbRate.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "MVTestConversionRate");
                rbValue.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "MVTestConversionValue");
                rbCombinations.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "ConversionsByCombination");

                bool checkedButton = false;

                // Check first enabled button 
                foreach (Control ctrl in pnlRadioButtons.Controls)
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
                    RedirectToAccessDenied(GetString("mvtest.noreportavaible"));
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

        // Set repors name
        String conversionCount = "mvtestconversioncount.yearreport;mvtestconversioncount.monthreport;mvtestconversioncount.weekreport;mvtestconversioncount.dayreport;mvtestconversioncount.hourreport";
        String conversionRate = "mvtestconversionrate.yearreport;mvtestconversionrate.monthreport;mvtestconversionrate.weekreport;mvtestconversionrate.dayreport;mvtestconversionrate.hourreport";
        String conversionValue = "mvtestconversionvalue.yearreport;mvtestconversionvalue.monthreport;mvtestconversionvalue.weekreport;mvtestconversionvalue.dayreport;mvtestconversionvalue.hourreport";
        String conversionCombinations = "mvtestconversionsbycombinations.yearreport;mvtestconversionsbycombinations.monthreport;mvtestconversionsbycombinations.weekreport;mvtestconversionsbycombinations.dayreport;mvtestconversionsbycombinations.hourreport";

        pnlCombination.Visible = false;
        // Set proper report name
        if (rbCount.Checked)
        {
            CheckWebAnalyticsUI("MVTestConversionCount");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionCount);
        }

        if (rbRate.Checked)
        {
            CheckWebAnalyticsUI("MVTestConversionRate");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionRate);
        }

        if (rbValue.Checked)
        {
            CheckWebAnalyticsUI("MVTestConversionValue");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionValue);
        }

        if (rbCombinations.Checked)
        {
            CheckWebAnalyticsUI("ConversionsByCombination");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionCombinations);
            pnlCombination.Visible = true;
        }

        // Conversion
        ucConversions.PostbackOnDropDownChange = true;
        ucConversions.MVTestName = testName;
        ucConversions.ReloadData(true);

        // Conversion
        String conversion = ValidationHelper.GetString(ucConversions.Value, String.Empty);
        if (conversion == ucConversions.AllRecordValue)
        {
            conversion = String.Empty;
        }

        // Combination
        String combination = ValidationHelper.GetString(usCombination.Value, String.Empty);
        if (combination == usCombination.AllRecordValue)
        {
            combination = String.Empty;
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
        dtp.Columns.Add("MVTestName", typeof(string));
        dtp.Columns.Add("ConversionName", typeof(string));
        dtp.Columns.Add("CombinationName", typeof(string));

        object[] parameters = new object[6];
        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "pageviews";
        parameters[3] = testName;
        parameters[4] = conversion;
        parameters[5] = combination;

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
        reportHeader.ManageDataCodeName = "mvtconversion;" + testName + ";%";

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