using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Reporting_Tools_Report_List : CMSReportingPage
{
    #region "Constants"

    // Report macros
    private const string REP_TABLE_MACRO = "%%control:ReportTable?";
    private const string REP_GRAPH_MACRO = "%%control:ReportGraph?";
    private const string REP_HTMLGRAPH_MACRO = "%%control:ReportHtmlGraph?";
    private const string REP_VALUE_MACRO = "%%control:ReportValue?";

    #endregion


    #region "Variables"

    protected int categoryId = 0;

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        categoryId = QueryHelper.GetInteger("categoryid", 0);

        // Register script for refresh tree after delete/destroy
        string script = @"function RefreshAdditionalContent(){
                            if (parent.parent.frames['reportcategorytree'])
                            {
                                parent.parent.frames['reportcategorytree'].location.href = 'ReportCategory_tree.aspx?categoryid=" + categoryId + @"';
                            }
                        }";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshTree", ScriptHelper.GetScript(script));

        // Used for clone and delete calls
        int reportID = QueryHelper.GetInteger("reportid", 0);

        ReportCategoryInfo info = new ReportCategoryInfo();
        info = ReportCategoryInfoProvider.GetReportCategoryInfo(categoryId);
        if (info != null)
        {
            string categoryPath = info.CategoryPath;
            // Add the slash character at the end of the categoryPath
            if (!categoryPath.EndsWithCSafe("/"))
            {
                categoryPath += "/";
            }
            UniGrid.WhereCondition = "ObjectPath LIKE '" + SqlHelperClass.GetSafeQueryString(categoryPath, false) + "%' AND ObjectType = 'Report'";
        }

        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.ZeroRowsText = GetString("general.nodatafound");

        InitializeMasterPage();
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        int reportId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName)
        {
            case "delete":
                // Check 'Modify' permission
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
                {
                    RedirectToAccessDenied("cms.reporting", "Modify");
                }

                // Delete ReportInfo object from database
                ReportInfoProvider.DeleteReportInfo(reportId);

                // Refresh tree after delete
                ltlScript.Text += ScriptHelper.GetScript("RefreshAdditionalContent();");
                break;
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        Title = "Report list";
    }


    /// <summary>
    /// Increment counter at the end of string.
    /// </summary>
    /// <param name="s">String</param>
    /// <param name="lpar">Left parathenses</param>
    /// <param name="rpar">Right parathenses</param>
    /// <param name="lenghtLimit">Maximum length of output string</param>
    protected string Increment(string s, string lpar, string rpar, int lenghtLimit)
    {
        int i = 1;
        s = s.Trim();
        if ((rpar == String.Empty) || s.EndsWithCSafe(rpar))
        {
            int leftpar = s.LastIndexOfCSafe(lpar);
            if (lpar == rpar)
            {
                leftpar = s.LastIndexOfCSafe(lpar, leftpar - 1);
            }

            if (leftpar >= 0)
            {
                i = ValidationHelper.GetSafeInteger(s.Substring(leftpar + lpar.Length, s.Length - leftpar - lpar.Length - rpar.Length), 0);
                if (i > 0) // Remove parathenses only if parentheses found
                {
                    s = s.Remove(leftpar);
                }
                i++;
            }
        }

        string tail = lpar + i + rpar;
        if (s.Length + tail.Length > lenghtLimit)
        {
            s = s.Substring(0, s.Length - (s.Length + tail.Length - lenghtLimit));
        }
        s += tail;
        return s;
    }


    /// <summary>
    /// Replaces old macro with new macro in template string.
    /// </summary>
    /// <param name="template">Template</param>
    /// <param name="macro">Macro</param>
    /// <param name="oldValue">Old macro value</param>
    /// <param name="newValue">New macro value</param>
    /// <param name="oldReportName">Old report name</param>    
    /// <param name="newReportName">New report name</param>
    protected string ReplaceMacro(string template, string macro, string oldValue, string newValue, string oldReportName, string newReportName)
    {
        // Old name, old macro style, backward compatible
        string oldOrigValue = macro + oldValue + "%%";
        // Old macro name, but with full specification
        string oldFullValue = macro + oldReportName + "." + oldValue + "%%";

        newValue = macro + newReportName + "." + newValue + "%%";

        return template.Replace(oldOrigValue, newValue).Replace(oldFullValue, newValue);
    }

    #endregion
}