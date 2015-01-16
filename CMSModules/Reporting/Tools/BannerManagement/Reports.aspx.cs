using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Reporting_Tools_BannerManagement_Reports : CMSBannerManagementEditPage
{
    #region "Private fields"

    private bool paramsLoaded = false;
    private bool isSaved;

    #endregion


    #region "Properties"

    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!isSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        string objectType = "";

        switch (QueryHelper.GetString("parameterName", "").ToLowerCSafe())
        {
            // It is banner
            case "bannerid":
                objectType = SiteObjectType.BANNER;
                break;
            // It is banner category
            case "bannercategoryid":
                objectType = SiteObjectType.BANNERCATEGORY;
                break;
            default:
                RedirectToInformation(GetString("bannermanagement.error.internal"));
                break;
        }

        // Get the ID
        int id = QueryHelper.GetInteger("parameterValue", 0);
        
        SetEditedObject(BaseAbstractInfoProvider.GetInfoById(objectType, id), "");
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.InfoText = GetString("WebAnalytics.Disabled");
        ucDisabledModule.ParentPanel = pnlDisabled;

        reportHeader.ActionPerformed += new CommandEventHandler(reportHeader_ActionPerformed);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        LoadAttributes();

        ucDisplayReport.ReloadData(false);
    }

    #endregion


    #region "Private methods"

    private void reportHeader_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                LoadAttributes();

                isSaved = true;

                // Save report
                if (ucDisplayReport.SaveReport() > 0)
                {
                    ShowConfirmation(String.Format(GetString("Ecommerce_Report.ReportSavedTo"), ucDisplayReport.ReportName + " - " + DateTime.Now.ToString()));
                }

                isSaved = false;
                break;
        }
    }


    private void LoadAttributes()
    {
        if (paramsLoaded)
        {
            return;
        }

        string reportsCodeName = QueryHelper.GetString("reportCodeName", String.Empty);


        if (!reportsCodeName.Contains(";"))
        {
            ucDisplayReport.ReportName = reportsCodeName;
            ucGraphTypePeriod.GraphTypeVisible = false;
        }
        else
        {
            ucDisplayReport.ReportName = ucGraphTypePeriod.GetReportName(reportsCodeName);
        }

        ucGraphTypePeriod.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable dtp = new DataTable();

        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add(QueryHelper.GetString("parameterName", String.Empty), typeof(int));

        object[] parameters = new object[3];

        parameters[0] = ucGraphTypePeriod.From;
        parameters[1] = ucGraphTypePeriod.To;
        parameters[2] = QueryHelper.GetInteger("parameterValue", 0);

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();

        ucDisplayReport.LoadFormParameters = false;
        ucDisplayReport.DisplayFilter = false;
        ucDisplayReport.ReportParameters = dtp.Rows[0];
        ucDisplayReport.UseExternalReload = true;
        ucDisplayReport.UseProgressIndicator = true;

        reportHeader.ReportName = ucDisplayReport.ReportName;
        reportHeader.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphTypePeriod.SelectedInterval;
        reportHeader.DisplayManageData = false;

        paramsLoaded = true;
    }

    #endregion
}