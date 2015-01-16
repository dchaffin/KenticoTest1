using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

[RegisterTitle("general.reports")]
public partial class CMSModules_Content_CMSDesk_OnlineMarketing_Reports : CMSAnalyticsContentPage
{
    private IDisplayReport ucDisplayReport;


    protected override void OnPreRender(EventArgs e)
    {
        CurrentUserInfo ui = CMSContext.CurrentUser;
        if (!ui.IsAuthorizedPerUIElement("CMS.Content", "Reports"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Reports");
        }

        // Check read for web analytics
        if (!ui.IsAuthorizedPerResource("cms.webanalytics", "Read"))
        {
            RedirectToAccessDenied(String.Format(GetString("general.permissionresource"), "Read", "Web analytics"));
        }

        // Set disabled module info
        ucDisabledModule.InfoText= GetString("WebAnalytics.Disabled");
        ucDisabledModule.ParentPanel = pnlDisabled;

        ucDisplayReport = this.LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx") as IDisplayReport;
        pnlContent.Controls.Add((Control)ucDisplayReport);

        ucGraphType.ProcessChartSelectors(false);
        CurrentMaster.PanelContent.CssClass = String.Empty;
        UIHelper.AllowUpdateProgress = false;

        // General report data
        ucDisplayReport.ReportName = rbContent.Checked ? "pagereports.content" : "pagereports.Traffic";
        ucDisplayReport.LoadFormParameters = false;
        ucDisplayReport.DisplayFilter = false;
        ucDisplayReport.GraphImageWidth = 100;
        ucDisplayReport.IgnoreWasInit = true;
        ucDisplayReport.TableFirstColumnWidth = Unit.Percentage(30);
        ucDisplayReport.UseExternalReload = true;
        ucDisplayReport.UseProgressIndicator = true;

        ucDisplayReport.SetDefaultDynamicMacros((int)ucGraphType.SelectedInterval);
        
        EditedObject = Node;

        // Resolve report macros 
        DataTable dtp = new DataTable();
        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CodeName", typeof(string));
        dtp.Columns.Add("NodeID", typeof(int));
        dtp.Columns.Add("CultureCode", typeof(string));

        object[] parameters = new object[5];
        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "pageviews";
        parameters[3] = Node.NodeID;
        parameters[4] = Node.DocumentCulture;

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();
        ucDisplayReport.ReportParameters = dtp.Rows[0];
        ucDisplayReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(ucGraphType.SelectedInterval);

        ucDisplayReport.ReloadData(true);

        base.OnPreRender(e);
    }
}