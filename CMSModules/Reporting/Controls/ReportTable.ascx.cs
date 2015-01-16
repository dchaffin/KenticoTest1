using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.SettingsProvider;
using CMS.WebAnalytics;
using CMS.IO;
using CMS.EmailEngine;


public partial class CMSModules_Reporting_Controls_ReportTable : AbstractReportControl
{
    #region "Variables"

    private string mReportTableName = String.Empty;
    private GridView mGridTable = null;
    private ReportTableInfo mTableInfo = null;
    private string mParameter = string.Empty;
    private int mPageSize = 0;
    private bool? mEnablePaging = null;
    private ReportInfo ri = null;

    /// <summary>
    /// Indicates whether exception was thrown during data loading
    /// </summary>
    private bool errorOccurred = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the GridView object.
    /// </summary>
    protected GridView GridViewObject
    {
        get
        {
            if (mGridTable == null)
            {
                mGridTable = new GridView();
            }
            return mGridTable;
        }
    }


    /// <summary>
    /// Report table connection string
    /// </summary>
    public override string ConnectionString
    {
        get
        {
            String tableConn = (TableInfo == null) ? String.Empty : TableInfo.TableConnectionString;
            if (String.IsNullOrEmpty(tableConn))
            {
                return (ri == null) ? String.Empty : ri.ReportConnectionString; ;
            }

            return tableConn;
        }
    }


    /// <summary>
    /// Table name - prevent using viewstate  (problems with displayreportcontrol and postback).
    /// </summary>
    public override string Parameter
    {
        get
        {
            return mParameter;
        }
        set
        {
            mParameter = value;
        }
    }


    /// <summary>
    /// Table name.
    /// </summary>
    private string ReportTableName
    {
        get
        {
            return mReportTableName;
        }
        set
        {
            mReportTableName = value;
        }
    }


    /// <summary>
    /// Direct table info used by preview.
    /// </summary>
    public ReportTableInfo TableInfo
    {
        get
        {
            if (mTableInfo == null)
            {
                mTableInfo = ReportTableInfoProvider.GetReportTableInfo(Parameter);
            }
            return mTableInfo;
        }
        set
        {
            mTableInfo = value;
        }
    }


    /// <summary>
    /// Page size for paged tables
    /// </summary>
    public int PageSize
    {
        get
        {
            return mPageSize;
        }
        set
        {
            mPageSize = value;
        }
    }


    /// <summary>
    /// Enables/disables paging (if null report settings is used)
    /// </summary>
    public bool? EnablePaging
    {
        get
        {
            return mEnablePaging;
        }
        set
        {
            mEnablePaging = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Ensires teble info object.
    /// </summary>
    private void EnsureTableInfo()
    {
        ReportTableInfo rti = TableInfo;

        if (rti != null)
        {
            QueryIsStoredProcedure = rti.TableQueryIsStoredProcedure;

            // Resolve dynamic data macros
            if (DynamicMacros != null)
            {
                for (int i = 0; i <= DynamicMacros.GetUpperBound(0); i++)
                {
                    ContextResolver.AddDynamicParameter(DynamicMacros[i, 0], DynamicMacros[i, 1]);
                }
            }

            QueryText = ResolveMacros(rti.TableQuery);
            TableInfo = rti;
        }
    }


    /// <summary>
    /// Adds GridView to the controls collectio.
    /// </summary>
    private void LoadTableInfo()
    {
        EnsureTableInfo();

        if (TableInfo != null)
        {
            QueryIsStoredProcedure = TableInfo.TableQueryIsStoredProcedure;
            QueryText = ResolveMacros(TableInfo.TableQuery);

            // Enable paging has higher priority - if not set use report settings
            GridViewObject.AllowPaging = (!EmailMode && (EnablePaging.HasValue ? EnablePaging.Value : ValidationHelper.GetBoolean(TableInfo.TableSettings["enablepaging"], false)));

            if (GridViewObject.AllowPaging)
            {
                // Webpart - higher priority
                GridViewObject.PageSize = (PageSize > 0) ? PageSize : ValidationHelper.GetInteger(TableInfo.TableSettings["pagesize"], 10);
                GridViewObject.PagerSettings.Mode = (PagerButtons)ValidationHelper.GetInteger(TableInfo.TableSettings["pagemode"], (int)PagerButtons.Numeric);
                GridViewObject.PageIndexChanging += new GridViewPageEventHandler(GridViewObject_PageIndexChanging);
            }
            GridViewObject.AllowSorting = false;

            // Get SkinID from reportTable custom data
            string skinId = ValidationHelper.GetString(TableInfo.TableSettings["skinid"], "");
            if (skinId != String.Empty)
            {
                if (String.IsNullOrEmpty((GridViewObject.SkinID)))
                {
                    GridViewObject.SkinID = skinId;
                }
            }

            GridViewObject.ID = "reportGrid";

            // Add grid view control to the page
            plcGrid.Controls.Clear();
            plcGrid.Controls.Add(GridViewObject);

            if ((RenderCssClasses) && (String.IsNullOrEmpty(GridViewObject.SkinID)))
            {
                //Clear the css styles to eliminate control state
                GridViewObject.HeaderStyle.CssClass = "";
                GridViewObject.CssClass = "";
                GridViewObject.RowStyle.CssClass = "";
                GridViewObject.AlternatingRowStyle.CssClass = "";
            }
        }
    }


    /// <summary>
    /// Created grid view based on parameter from report table.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        imgInfo.ImageUrl = GetImageUrl("/Objects/Reporting_ReportTable/object.png");
        ItemType = ReportItemType.Table;

        // Set table name from inline parameter
        ReportTableName = Parameter;
        LoadTableInfo();
    }


    /// <summary>
    /// Returns true if graph belongs to report.
    /// </summary>
    /// <param name="report">Report to validate</param>
    public override bool IsValid(ReportInfo report)
    {
        ReportTableInfo rti = TableInfo;

        if ((report != null) && (rti != null) && (report.ReportID == rti.TableReportID))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        if ((GraphImageWidth != 0) && (ComputedWidth == 0))
        {
            // Graph width is computed no need to create graph
            return;
        }

        Visible = true;

        errorOccurred = false;

        try
        {
            ReportTableName = Parameter;

            EnsureTableInfo();
            EnsureChildControls();

            //Test security
            if (TableInfo != null)
            {
                ri = ReportInfoProvider.GetReportInfo(TableInfo.TableReportID);
                if (ri != null)
                {
                    if (ri.ReportAccess != ReportAccessEnum.All)
                    {
                        if (!CMSContext.CurrentUser.IsAuthenticated())
                        {
                            Visible = false;
                            return;
                        }
                    }

                    EnableSubscription = EnableSubscription && (ValidationHelper.GetBoolean(TableInfo.TableSettings["SubscriptionEnabled"], true) && ri.ReportEnableSubscription);
                    if (EmailMode && !EnableSubscription)
                    {
                        this.Visible = false;
                        return;
                    }

                    //Set default parametrs directly if not set
                    if (ReportParameters == null)
                    {
                        FormInfo fi = new FormInfo(ri.ReportParameters);
                        // Get datarow with required columns
                        ReportParameters = fi.GetDataRow(false);
                        fi.LoadDefaultValues(ReportParameters, true);
                    }

                    ApplyTimeParameters();
                }
            }

            // Only use base parameters in case of stored procedure
            if (QueryIsStoredProcedure)
            {
                AllParameters = SpecialFunctions.ConvertDataRowToParams(ReportParameters, null);
            }

            // Load data
            DataSet ds = LoadData();

            // If no data load, set empty dataset
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                if (EmailMode && SendOnlyNonEmptyDataSource)
                {
                    Visible = false;
                    return;
                }

                string noRecordText = ValidationHelper.GetString(TableInfo.TableSettings["QueryNoRecordText"], String.Empty);
                if (noRecordText != String.Empty)
                {
                    GridViewObject.Visible = false;
                    lblInfo.Text = CMSContext.ResolveMacros(noRecordText);
                    plcInfo.Visible = true;
                    EnableExport = false;
                    return;
                }

                if (!EmailMode)
                {
                    Visible = false;
                }
            }
            else
            {
                GridViewObject.Visible = true;
                // Resolve macros in column names
                int i = 0;
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    if (dc.ColumnName == "Column" + ((int)(i + 1)).ToString())
                    {
                        dc.ColumnName = ResolveMacros(ds.Tables[0].Rows[0][i].ToString());
                    }
                    else
                    {
                        dc.ColumnName = ResolveMacros(dc.ColumnName);
                    }
                    i++;
                }

                // Resolve macros in dataset
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    foreach (DataColumn dc in ds.Tables[0].Columns)
                    {
                        if (dc.DataType.FullName.ToLowerCSafe() == "system.string")
                        {
                            dr[dc.ColumnName] = ResolveMacros(ValidationHelper.GetString(dr[dc.ColumnName], ""));
                        }
                    }
                }

                if (EmailMode)
                {
                    // For some email formats, export data in csv format
                    EmailFormatEnum format = EmailHelper.GetEmailFormat(ReportSubscriptionSiteID);

                    if ((format == EmailFormatEnum.Both) || (format == EmailFormatEnum.PlainText))
                    {
                        using (MemoryStream ms = MemoryStream.New())
                        {
                            DataExportHelper deh = new DataExportHelper(ds);
                            byte[] data = deh.ExportToCSV(ds, 0, ms, true);
                            ReportSubscriptionSender.AddToRequest(ri.ReportName, "t" + TableInfo.TableName, data);
                        }
                    }

                    // For plain text email show table only as attachment
                    if (format == EmailFormatEnum.PlainText)
                    {
                        menuCont.Visible = false;
                        ltlEmail.Visible = true;
                        ltlEmail.Text = String.Format(GetString("reportsubscription.attachment"), TableInfo.TableName);
                        return;
                    }

                    GenerateTableForEmail(ds);
                    menuCont.Visible = false;
                    return;
                }
            }

            ApplyStyles();

            // Databind to gridview control
            GridViewObject.DataSource = ds;
            EnsurePageIndex();
            GridViewObject.DataBind();

            if ((TableFirstColumnWidth != Unit.Empty) && (GridViewObject.Rows.Count > 0))
            {
                GridViewObject.Rows[0].Cells[0].Width = TableFirstColumnWidth;
            }
        }
        catch (Exception ex)
        {
            // Display error message, if data load fail
            lblError.Visible = true;
            lblError.Text = "[ReportTable.ascx] Error loading the data: " + ex.Message;
            EventLogProvider ev = new EventLogProvider();
            ev.LogEvent("Report table", "E", ex);
            errorOccurred = true;
        }
    }


    /// <summary>
    /// Ensures the current page index with dependenco on request data du to different contol's life cycle.
    /// </summary>
    private void EnsurePageIndex()
    {
        if ((GridViewObject != null) && (GridViewObject.AllowPaging))
        {
            // Get current postback target
            string eventTarget = Request.Params["__EVENTTARGET"];

            // Handle paging manually because of lifecycle of the control
            if (CMSString.Compare(eventTarget, GridViewObject.UniqueID, true) == 0)
            {
                // Get the current page value
                string eventArg = ValidationHelper.GetString(Request.Params["__EVENTARGUMENT"], String.Empty);

                string[] args = eventArg.Split('$');
                if ((args.Length == 2) && (CMSString.Compare(args[0], "page", true) == 0))
                {
                    string pageValue = args[1];
                    int pageIndex = 0;
                    // Switch by page value  0,1.... first,last
                    switch (pageValue.ToLowerCSafe())
                    {
                        // Last item
                        case "last":
                            // Check whether page count is available
                            if (GridViewObject.PageCount > 0)
                            {
                                pageIndex = GridViewObject.PageCount - 1;
                            }
                            // if page count is not defined, try compute page count
                            else
                            {
                                DataSet ds = GridViewObject.DataSource as DataSet;
                                if (!DataHelper.DataSourceIsEmpty(ds))
                                {
                                    pageIndex = ds.Tables[0].Rows.Count / GridViewObject.PageSize;
                                }
                            }
                            break;

                        case "next":
                            pageIndex = GridViewObject.PageIndex + 1;
                            break;

                        case "prev":
                            pageIndex = GridViewObject.PageIndex - 1;
                            break;

                        // Page number
                        default:
                            pageIndex = ValidationHelper.GetInteger(pageValue, 1) - 1;
                            break;
                    }

                    GridViewObject.PageIndex = pageIndex;
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (TableInfo != null)
        {
            // Register context menu for export - if allowed
            EnableExport = (EnableExport && ValidationHelper.GetBoolean(TableInfo.TableSettings["ExportEnabled"], false));
            RegisterContextMenu(menuCont);
            RegisterSubscriptionScript(TableInfo.TableReportID, "tableid", TableInfo.TableID, menuCont);
        }

        // Export data
        if ((ri != null) && !errorOccurred)
        {
            ProcessExport(ValidationHelper.GetCodeName(ri.ReportDisplayName));
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Sets default grid styles and apply skinID (if any).
    /// </summary>
    private void ApplyStyles()
    {
        // Style frid view
        if (GridViewObject != null)
        {
            GridViewObject.CellPadding = 3;
            GridViewObject.GridLines = GridLines.Horizontal;
            GridViewObject.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

            if (RenderCssClasses && (String.IsNullOrEmpty(GridViewObject.SkinID) || EmailMode))
            {
                GridViewObject.HeaderStyle.CssClass = "UniGridHead";
                GridViewObject.HeaderStyle.Wrap = false;
                GridViewObject.CssClass = "UniGridGrid";
                GridViewObject.RowStyle.CssClass = "EvenRow";
                GridViewObject.AlternatingRowStyle.CssClass = "OddRow";
                GridViewObject.FooterStyle.CssClass = "UniGridHead";
                GridViewObject.PagerStyle.CssClass = "PagerRow";
            }
        }
    }


    /// <summary>
    /// Generates table for email
    /// </summary>
    /// <param name="ds">Dataset with table data</param>
    private void GenerateTableForEmail(DataSet ds)
    {
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            return;
        }

        ltlEmail.Visible = true;

        StringBuilder sb = new StringBuilder();

        // Generate header
        sb.Append(@"<table class=""UniGridGrid"" cellspacing=""0"" cellpadding=""3"" rules=""rows"" border=""1"" style=""border-collapse:collapse;"">
				<tr class=""UniGridHead"" align=""left"">");
        DataTable dt = ds.Tables[0];

        foreach (DataColumn dc in dt.Columns)
        {
            sb.Append("<th scope=\"col\">" + dc.ColumnName + "</th>");
        }

        sb.Append("</tr>");

        int rowNumber = 1;

        // Generate rows
        foreach (DataRow dr in dt.Rows)
        {
            rowNumber++;
            String cssClass = (rowNumber % 2 == 0) ? "EvenRow" : "OddRow";
            sb.Append("<tr class=" + cssClass + ">");

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append("<td>" + dr[i] + "</td>");
            }

            sb.Append("</tr>");
        }

        sb.Append("</table>");
        ltlEmail.Text = sb.ToString();
    }


    /// <summary>
    /// Handles paging on live site.
    /// </summary>
    protected void GridViewObject_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
    }

    #endregion


}