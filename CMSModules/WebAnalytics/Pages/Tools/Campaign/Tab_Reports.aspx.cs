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

public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Reports : CMSCampaignAndConversionPage
{
    #region "Variables"

    private bool isSaved = false;
    private bool reportLoaded = false;
    private string campaignName = String.Empty;
    private CampaignInfo campaignInfo;

    private IDisplayReport ucDisplayReport = null;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        reportHeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlContent.Controls.Add((Control)ucDisplayReport);

        CurrentMaster.PanelContent.CssClass = string.Empty;
        UIHelper.AllowUpdateProgress = false;
        ScriptHelper.RegisterDialogScript(Page);

        // Campaign Info
        int campaignID = QueryHelper.GetInteger("campaignID", 0);
        campaignInfo = CampaignInfoProvider.GetCampaignInfo(campaignID);
        if (campaignInfo == null)
        {
            return;
        }

        // Validate SiteID for non administrators
        if (!CMSContext.CurrentUser.IsGlobalAdministrator)
        {
            if (campaignInfo.CampaignSiteID != CMSContext.CurrentSiteID)
            {
                RedirectToAccessDenied(GetString("cmsmessages.accessdenied"));
            }
        }

        campaignName = campaignInfo.CampaignName;

        ScriptHelper.RegisterPrintDialogScript(this);
        ucGraphType.ProcessChartSelectors(false);

        // Enables/disables radio buttons (based on UI elements)
        CurrentUserInfo ui = CMSContext.CurrentUser;
        bool checkedButton = false;

        if (!RequestHelper.IsPostBack())
        {
            if (!ui.IsGlobalAdministrator)
            {
                rbCount.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "CampaignConversionCount");
                rbDetail.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "CampaignDetails");
                rbGoalCount.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "goals.numberofconversions");
                rbGoalValue.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "goals.totalvalueofconversions");
                rbGoalView.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "goals.numberofvisitors");
                rbValue.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "campaignsConversionValue");
                rbValuePerVisitor.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "valuepervisitor");
                rbViews.Enabled = ui.IsAuthorizedPerUIElement("cms.WebAnalytics", "campaign.overview");

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
                    RedirectToAccessDenied(GetString("campaign.noreportavaible"));
                }
            }
            else
            {
                // Admin check first radio button
                rbViews.Checked = true;
            }
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

        ucGraphType.ProcessChartSelectors(false);

        // Set reports name
        const string views = "campaign.yearreport;campaign.monthreport;campaign.weekreport;campaign.dayreport;campaign.hourreport";
        const string conversionCount = "campaignconversioncount.yearreport;campaignconversioncount.monthreport;campaignconversioncount.weekreport;campaignconversioncount.dayreport;campaignconversioncount.hourreport";
        const string conversionValue = "campaignconversionvalue.yearreport;campaignconversionvalue.monthreport;campaignconversionvalue.weekreport;campaignconversionvalue.dayreport;campaignconversionvalue.hourreport";
        const string details = "campaigns.singledetails";
        const string visitorsGoal = "goalsnumberofvisitors.yearreport;goalsnumberofvisitors.monthreport;goalsnumberofvisitors.weekreport;goalsnumberofvisitors.dayreport;goalsnumberofvisitors.hourreport";
        const string valuePerVisitor = "goalsvaluepervisit.yearreport;goalsvaluepervisit.monthreport;goalsvaluepervisit.weekreport;goalsvaluepervisit.dayreport;goalsvaluepervisit.hourreport";
        const string valueGoal = "goalsvalueofconversions.yearreport;goalsvalueofconversions.monthreport;goalsvalueofconversions.weekreport;goalsvalueofconversions.dayreport;goalsvalueofconversions.hourreport";
        const string countGoal = "goalsnumberofconversions.yearreport;goalsnumberofconversions.monthreport;goalsnumberofconversions.weekreport;goalsnumberofconversions.dayreport;goalsnumberofconversions.hourreport";

        String codeName = String.Empty;
        pnlConversions.Visible = false;
        ucGraphType.EnableDateTimePicker = true;

        if (rbViews.Checked)
        {
            CheckWebAnalyticsUI("campaign.overview");
            codeName = "campaign";
            ucDisplayReport.ReportName = ucGraphType.GetReportName(views);
        }

        if (rbCount.Checked)
        {
            CheckWebAnalyticsUI("CampaignConversionCount");
            pnlConversions.Visible = true;
            codeName = "campconversion";
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionCount);
        }

        if (rbValue.Checked)
        {
            CheckWebAnalyticsUI("campaignsConversionValue");
            pnlConversions.Visible = true;
            codeName = "campconversion";
            ucDisplayReport.ReportName = ucGraphType.GetReportName(conversionValue);
        }

        if (rbDetail.Checked)
        {
            CheckWebAnalyticsUI("CampaignDetails");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(details);
            ucGraphType.EnableDateTimePicker = false;
        }

        if (rbGoalView.Checked)
        {
            CheckWebAnalyticsUI("goals.numberofvisitors");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(visitorsGoal);
        }

        if (rbGoalCount.Checked)
        {
            CheckWebAnalyticsUI("goals.numberofconversions");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(countGoal);
        }

        if (rbGoalValue.Checked)
        {
            CheckWebAnalyticsUI("goals.totalvalueofconversions");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(valueGoal);
        }

        if (rbValuePerVisitor.Checked)
        {
            CheckWebAnalyticsUI("goals.valuepervisitor");
            ucDisplayReport.ReportName = ucGraphType.GetReportName(valuePerVisitor);
        }

        // Load conversions fit to campaign
        if ((pnlConversions.Visible) && (campaignInfo != null))
        {
            ucConversions.PostbackOnDropDownChange = true;
            if (!campaignInfo.CampaignUseAllConversions)
            {
                ucConversions.WhereCondition = "ConversionID  IN (SELECT ConversionID FROM Analytics_ConversionCampaign WHERE CampaignID =" + campaignInfo.CampaignID + ")";
            }

            ucConversions.WhereCondition = SqlHelperClass.AddWhereCondition(ucConversions.WhereCondition, "ConversionSiteID =" + CMSContext.CurrentSiteID);
            ucConversions.ReloadData(true);
        }

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

        // Resolve report macros 
        DataTable dtp = new DataTable();
        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CodeName", typeof(string));
        dtp.Columns.Add("CampaignName", typeof(string));
        dtp.Columns.Add("ConversionName", typeof(string));

        object[] parameters = new object[5];
        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = codeName;
        parameters[3] = campaignName;
        parameters[4] = conversion;

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();
        ucDisplayReport.ReportParameters = dtp.Rows[0];

        if (reload)
        {
            ucDisplayReport.ReloadData(true);
        }

        reportLoaded = true;
    }


    /// <summary>
    /// Handles lnkSave click event.
    /// </summary>
    protected void lnkSave_Click(object sender, EventArgs e)
    {
        Save();
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport(true);

        reportHeaderActions.ReportName = ucDisplayReport.ReportName;
        reportHeaderActions.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeaderActions.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeaderActions.ManageDataCodeName = "singlecampaign;" + campaignName;

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
            lblInfo.Visible = true;
            lblInfo.Text = String.Format(GetString("Analytics_Report.ReportSavedTo"), ucDisplayReport.ReportDisplayName + " - " + DateTime.Now.ToString());
        }

        isSaved = false;
    }

    #endregion
}