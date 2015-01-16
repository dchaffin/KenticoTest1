using System;

using CMS.CMSHelper;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.SettingsProvider;
using CMS.EventLog;

[Title("CMSModules/CMS_WebAnalytics/Details/datamanagement.png", "com.reportsui.datagenerator", "sampledata")]
public partial class CMSModules_Reporting_Tools_Ecommerce_SampleData_Generator : CMSEcommerceReportsPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        if (!CurrentUser.IsGlobalAdministrator)
        {
            RedirectToAccessDenied(GetString("security.accesspage.onlyglobaladmin"));
        }

        // Generate JavaScript prompt
        btnGenerate.OnClientClick = GenerateConfirmation("com.generator.generateconfirm");
        btnDelete.OnClientClick = GenerateConfirmation("com.generator.deleteconfirm");

        // Display info message
        ShowInformation(GetString("com.generator.mainpericope"));
    }

    #endregion


    #region "Form Events"

    protected void btnGenerateClick(object sender, EventArgs e)
    {
        GenerateData();
    }


    protected void btnDeleteClick(object sender, EventArgs e)
    {
        DeleteData();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Generates sample data.
    /// </summary>
    private void GenerateData()
    {
        try
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("siteId", CMSContext.CurrentSiteID);

            ConnectionHelper.ExecuteQuery("ecommerce.order.generateSampleData", parameters);

            // Log successful attempt
            EventLogProvider eventLog = new EventLogProvider();
            eventLog.LogEvent("I", DateTime.Now, "E-commerce data generator", "DATAGENERATED", CMSContext.CurrentSiteID);
            ShowConfirmation(GetString("com.reports.datagenerated"));
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Reports", "Generate", ex);
            ShowError(GetString("com.reports.operationFailed"));
        }
    }


    /// <summary>
    /// Deletes sample data.
    /// </summary>
    private void DeleteData()
    {
        try
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("siteId", CMSContext.CurrentSiteID);

            ConnectionHelper.ExecuteQuery("ecommerce.order.deleteSampleData", parameters);

            // Log successful attempt
            EventLogProvider eventLog = new EventLogProvider();
            eventLog.LogEvent("I", DateTime.Now, "E-commerce data generator", "DATADELETED", CMSContext.CurrentSiteID);
            ShowConfirmation(GetString("com.reports.datadeleted"));
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Reports", "Delete", ex);
            ShowError(GetString("com.reports.operationFailed"));
        }
    }


    /// <summary>
    /// Generates JavaScript confirm pop-up.
    /// </summary>
    /// <param name="resourceString">Resource String to be shown</param>
    private string GenerateConfirmation(string resourceString)
    {
        return "return confirm(\"" + GetString(resourceString) + "\")";
    }

    #endregion
}