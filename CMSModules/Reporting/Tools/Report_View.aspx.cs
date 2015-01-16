using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.SettingsProvider;

public partial class CMSModules_Reporting_Tools_Report_View : CMSReportingPage
{
    #region "Variables"

    private int reportId = 0;
    private bool isSaved = false;

    #endregion


    #region "Properties"
    
    /// <summary>
    /// OnInit.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Ensure the script manager
        EnsureScriptManager();

        reportId = ValidationHelper.GetInteger(Request.QueryString["ReportId"], 0);
        ReportInfo ri = ReportInfoProvider.GetReportInfo(reportId);
        if (ri != null)
        {
            DisplayReport1.ReportName = ri.ReportName;
        }
    }

    #endregion


    #region "Methods" 

    /// <summary>
    /// VerifyRenderingInServerForm.
    /// </summary>
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
    /// Save click handler.
    /// </summary>
    protected void Save()
    {
        // Check 'SaveReports' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.reporting", "SaveReports"))
        {
            RedirectToAccessDenied("cms.reporting", "SaveReports");
        }

        if (!DisplayReport1.ParametersForm.ValidateData())
        {
            return;
        }

        isSaved = true;
        int savedReportId = DisplayReport1.SaveReport();

        if (savedReportId != 0)
        {
            URLHelper.Redirect("SavedReports/SavedReport_View.aspx?reportId=" + savedReportId.ToString() + "&view=1");
        }
    }


    /// <summary>
    /// Send to email click handler.
    /// </summary>
    protected void lnkSend_Click(object sender, EventArgs e)
    {
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);
        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// On PreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        DisplayReport1.ReloadData(false);
        reportHeader.ReportName = DisplayReport1.ReportName;
        reportHeader.ReportParameters = DisplayReport1.ReportParameters;
    }

    #endregion
}