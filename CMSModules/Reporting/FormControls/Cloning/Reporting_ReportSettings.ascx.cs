using System;
using System.Collections;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Reporting;
using CMS.UIControls;

public partial class CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Returns close script which should be run when cloning is sucessfully done.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            string script = String.Empty;
            string refreshLink = URLHelper.ResolveUrl("~/CMSModules/Reporting/Tools/ReportCategory_Tree.aspx?saved=1&reload=true");
            if (QueryHelper.Contains("reloadall"))
            {
                script += "wopener.parent.frames['reportcategorytree'].location.href ='" + refreshLink + "&reportid={0}';";
            }
            else
            {
                script += "wopener.parent.parent.frames['reportcategorytree'].location.href ='" + refreshLink + "&categoryid=" + categorySelector.Value + "';";
            }
            script += "CloseDialog();";
            return script;
        }
    }


    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return ReportingObjectType.REPORTSAVEDREPORT + ";" + ReportingObjectType.REPORTSUBSCRIPTION;
        }
    }

    #endregion


    #region "Methods"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        lblCategory.ToolTip = GetString("clonning.settings.report.category.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            ReportInfo ri = InfoToClone as ReportInfo;
            if (ri != null)
            {
                categorySelector.Value = ri.ReportCategoryID.ToString();
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ReportingObjectType.REPORT + ".categoryid"] = categorySelector.Value;
        result[ReportingObjectType.REPORT + ".savedreports"] = chkCloneSavedReports.Checked;
        result[ReportingObjectType.REPORT + ".subscriptions"] = chkSubscriptions.Checked;
        return result;
    }

    #endregion
}