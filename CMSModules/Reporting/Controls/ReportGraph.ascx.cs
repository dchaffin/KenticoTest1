using System;
using System.Data;
using System.Web.UI;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.WebAnalytics;

public partial class CMSModules_Reporting_Controls_ReportGraph : AbstractReportControl
{
    #region "Variables"

    private string mReportGraphName = "";
    private ReportGraphInfo mReportGraphInfo;
    private string mParameter = String.Empty;
    private bool registerWidthScript = false;
    private string mWidth = String.Empty;
    private int mHeight = 0;
    private ReportInfo report = null;

    /// <summary>
    /// Indicates whether exception was thrown during data loading
    /// </summary>
    private bool errorOccurred = false;

    /// <summary>
    /// Store info object loaded from DB - prevents from double load.
    /// </summary>
    private ReportGraphInfo mGraphInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns graph info from DB or from memory.
    /// </summary>
    private ReportGraphInfo ReportGraphInfo
    {
        get
        {
            // This is used for preview or direct set of info without using DB
            if (GraphInfo != null)
            {
                return GraphInfo;
            }

            // If graph info is not set already
            return mGraphInfo ?? (mGraphInfo = ReportGraphInfoProvider.GetReportGraphInfo(Parameter));
        }
    }


    /// <summary>
    /// Graph name - prevent using viewstate  (problems with displayreportcontrol and postback).
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
    /// Report graph connection string
    /// </summary>
    public override string ConnectionString
    {
        get
        {
            String graphConn = (ReportGraphInfo == null) ? String.Empty : ReportGraphInfo.GraphConnectionString;
            if (String.IsNullOrEmpty(graphConn))
            {
                return (report == null) ? String.Empty : report.ReportConnectionString; ;
            }

            return graphConn;
        }
    }


    /// <summary>
    /// If true, graph is render for email purpose.
    /// </summary>
    public override bool EmailMode
    {
        get
        {
            return base.EmailMode;
        }
        set
        {
            graphDiv.Visible = !value;
            pnlImage.Visible = value;
            base.EmailMode = value;
        }
    }


    /// <summary>
    /// Graph name.
    /// </summary>
    private string ReportGraphName
    {
        get
        {
            return mReportGraphName;
        }
        set
        {
            mReportGraphName = value;
        }
    }


    /// <summary>
    /// If set graphinfo will not be loaded from database.
    /// </summary>
    public ReportGraphInfo GraphInfo
    {
        get
        {
            return mReportGraphInfo;
        }
        set
        {
            mReportGraphInfo = value;
        }
    }


    /// <summary>
    /// Returns true if graph belongs to report.
    /// </summary>
    /// <param name="report">Report to validate</param>
    public override bool IsValid(ReportInfo report)
    {
        ReportGraphInfo rgi = ReportGraphInfo;
        // Test validity
        if ((report != null) && (rgi != null) && (report.ReportID == rgi.GraphReportID))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets or sets width of graph. If graph contains '%', width is relative to max possible width.
    /// </summary>
    public String Width
    {
        get
        {
            return mWidth;
        }
        set
        {
            mWidth = value;
            if (mWidth.Contains("%"))
            {
                string pureWidth = mWidth.Replace('%', ' ');
                int val = ValidationHelper.GetInteger(pureWidth.Trim(), 0);
                if (val != 0)
                {
                    GraphImageWidth = val;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets height of graph.
    /// </summary>
    public int Height
    {
        get
        {
            return mHeight;
        }
        set
        {
            mHeight = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        ItemType = ReportItemType.Graph;

        imgInfo.ImageUrl = GetImageUrl("/Objects/Reporting_ReportGraph/object.png");

        // Hide refresh button
        btnRefresh.Style.Add("display", "none");

        // If hidden field contains information about width - add to request for multiple charts on one page
        int width = ValidationHelper.GetInteger(Request.Params[hdnValues.UniqueID], 0);
        if (URLHelper.IsPostback() && (width != 0))
        {
            // Fix the position to page with slider
            ComputedWidth = width - 17;

            if (BrowserHelper.IsIE7())
            {
                ComputedWidth -= 7;
            }
        }

        base.OnLoad(e);
    }


    /// <summary>
    /// Changes the width of graph - depends on percent sets.
    /// </summary>
    private int ModifyGraphInfo()
    {
        return (int)((ComputedWidth) * ((float)GraphImageWidth / 100));
    }


    /// <summary>
    /// OnPreRender handler - register progress script.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (ReportGraphInfo != null)
        {
            // Register context menu for export - if allowed
            EnableExport = (EnableExport && ValidationHelper.GetBoolean(ReportGraphInfo.GraphSettings["ExportEnabled"], false));
            RegisterContextMenu(menuCont);

            RegisterSubscriptionScript(ReportGraphInfo.GraphReportID, "graphid", ReportGraphInfo.GraphID, menuCont);


            if (registerWidthScript)
            {
                string script = ControlsHelper.GetPostBackEventReference(btnRefresh, "refresh");
                // Count width of div and postback
                ScriptHelper.RegisterStartupScript(this, typeof(string), "ReportGraphReloader",
                                                   ScriptHelper.GetScript(@" document.body.onload = function () {var graphDivObject = document.getElementById('" + graphDiv.ClientID + @"'); 
                                            document.getElementById('" + hdnValues.ClientID + "').value = graphDivObject.offsetWidth;" + script + "};"));
            }
        }

        if ((report != null) && !errorOccurred)
        {
            ProcessExport(ValidationHelper.GetCodeName(report.ReportDisplayName));
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        // If percent Width is Set and reloadonPrerender == false  - it means first run .. ad postback scripts and exit
        if ((GraphImageWidth != 0) && (ComputedWidth == 0))
        {
            string argument = Request.Params["__EVENTARGUMENT"];
            string target = Request.Params["__EVENTTARGET"];

            // Check for empty (invisible) div to prevent neverending loop
            // If refresh postback and still no computedwidth - display graph with default width
            if (!((target == btnRefresh.UniqueID) && (argument == "refresh")))
            {
                registerWidthScript = true;
                ucChart.Visible = false;
                RequestStockHelper.Add("CMSGraphAutoWidth", true);
                return;
            }
        }

        RequestStockHelper.Add("CMSGraphAutoWidth", false);
        registerWidthScript = false;

        // ReportGraphName is set from AbstractReportControl parameter
        ReportGraphName = Parameter;

        // Preview
        if (GraphInfo != null)
        {
            GetReportGraph(GraphInfo);
        }
        else
        {
            ReportGraphInfo rgi = ReportGraphInfo;
            // If saved report guid is empty ==> create "live" graph, else load saved graph
            if (SavedReportGuid == Guid.Empty)
            {
                // Get graph info object
                if (rgi != null)
                {
                    GetReportGraph(rgi);
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "[ReportGraph.ascx] Report graph '" + ReportGraphName + "' not found.";
                    EventLogProvider ev = new EventLogProvider();
                    ev.LogEvent("Report graph", "E", new Exception("Report graph '" + ReportGraphName + "' not found."));
                }
            }
            else
            {
                if (rgi != null)
                {
                    int correctWidth = 0;
                    if (ComputedWidth != 0)
                    {
                        correctWidth = ModifyGraphInfo();
                    }

                    rgi.GraphTitle = ResHelper.LocalizeString(rgi.GraphTitle);
                    rgi.GraphXAxisTitle = ResHelper.LocalizeString(rgi.GraphXAxisTitle);
                    rgi.GraphYAxisTitle = ResHelper.LocalizeString(rgi.GraphYAxisTitle);

                    QueryIsStoredProcedure = rgi.GraphQueryIsStoredProcedure;
                    QueryText = ResolveMacros(rgi.GraphQuery);

                    // Load data, generate image
                    ReportGraph rg = new ReportGraph();

                    // Save image
                    SavedGraphInfo sgi = new SavedGraphInfo();

                    string noRecordText = CMSContext.ResolveMacros(ValidationHelper.GetString(rgi.GraphSettings["QueryNoRecordText"], String.Empty));

                    try
                    {
                        rg.CorrectWidth = correctWidth;
                        DataSet ds = LoadData();
                        if (!DataHelper.DataSourceIsEmpty(ds) || !String.IsNullOrEmpty(noRecordText))
                        {
                            sgi.SavedGraphBinary = rg.CreateChart(rgi, LoadData(), ContextResolver, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogProvider ev = new EventLogProvider();
                        ev.LogEvent("Report graph", "E", ex);

                        byte[] invalidGraph = rg.CreateChart(rgi, null, ContextResolver, true);
                        sgi.SavedGraphBinary = invalidGraph;
                    }

                    if (sgi.SavedGraphBinary != null)
                    {
                        sgi.SavedGraphGUID = SavedReportGuid;
                        sgi.SavedGraphMimeType = "image/png";
                        sgi.SavedGraphSavedReportID = SavedReportID;

                        SavedGraphInfoProvider.SetSavedGraphInfo(sgi);

                        // Create graph image
                        imgGraph.Visible = true;
                        ucChart.Visible = false;
                        imgGraph.ImageUrl = ResolveUrl("~/CMSModules/Reporting/CMSPages/GetReportGraph.aspx") + "?graphguid=" + SavedReportGuid.ToString();
                    }
                    else
                    {
                        Visible = false;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Returns report graph.
    /// </summary>
    private void GetReportGraph(ReportGraphInfo reportGraph)
    {
        Visible = true;
        ucChart.Visible = true;

        int correctWidth = 0;
        if (ComputedWidth != 0)
        {
            correctWidth = ModifyGraphInfo();
        }

        if (Width != String.Empty)
        {
            int graphWidth = ValidationHelper.GetInteger(Width, 0);
            if (graphWidth != 0)
            {
                reportGraph.GraphWidth = graphWidth;
            }
        }

        if (Height != 0)
        {
            reportGraph.GraphHeight = Height;
        }

        ReportGraph graph = new ReportGraph();
        graph.ChartControl = ucChart;

        report = ReportInfoProvider.GetReportInfo(reportGraph.GraphReportID);
        if (report == null)
        {
            return;
        }

        // Check graph security settings
        if (report.ReportAccess != ReportAccessEnum.All)
        {
            if (!CMSContext.CurrentUser.IsAuthenticated())
            {
                Visible = false;
                return;
            }
        }

        // Set default parametrs directly if not set
        if (ReportParameters == null)
        {
            // Load ReportInfo            
            if (report != null)
            {
                FormInfo fi = new FormInfo(report.ReportParameters);

                // Get datarow with required columns
                ReportParameters = fi.GetDataRow(false);
                fi.LoadDefaultValues(ReportParameters, true);
            }
        }

        // If used via widget - this function ensure showing specific interval from actual time (f.e. last 6 weeks)
        ApplyTimeParameters();

        // Only use base parameters in case of stored procedure
        if (QueryIsStoredProcedure)
        {
            AllParameters = SpecialFunctions.ConvertDataRowToParams(ReportParameters, null);
        }

        ContextResolver resolver = CMSContext.CurrentResolver.CreateContextChild();

        errorOccurred = false;

        // Create graph image                    
        try
        {
            // Resolve parameters in query
            resolver.SourceParameters = AllParameters.ToArray();

            // Resolve dynamic data macros
            if (DynamicMacros != null)
            {
                for (int i = 0; i <= DynamicMacros.GetUpperBound(0); i++)
                {
                    resolver.AddDynamicParameter(DynamicMacros[i, 0], DynamicMacros[i, 1]);
                }
            }

            // Prepare query attributes
            QueryText = resolver.ResolveMacros(reportGraph.GraphQuery);
            QueryIsStoredProcedure = reportGraph.GraphQueryIsStoredProcedure;

            // LoadData
            DataSet dsGraphData = LoadData();

            // Empty dataset, and flag not send empty dataset is set
            if (SendOnlyNonEmptyDataSource && EmailMode && DataHelper.DataSourceIsEmpty(dsGraphData))
            {
                Visible = false;
                return;
            }

            // Test if dataset is empty            
            if (DataHelper.DataSourceIsEmpty(dsGraphData))
            {
                string noRecordText = CMSContext.ResolveMacros(ValidationHelper.GetString(reportGraph.GraphSettings["QueryNoRecordText"], String.Empty));
                if (noRecordText != String.Empty)
                {
                    ltlEmail.Text = noRecordText;
                    lblInfo.Text = noRecordText;
                    ucChart.Visible = false;
                    menuCont.MenuID = String.Empty;
                    EnableExport = false;
                    plcInfo.Visible = true;
                }
                else
                {
                    Visible = false;
                }
            }
            else
            {
                // Create Chart                                
                graph.CorrectWidth = correctWidth;
                if (EmailMode)
                {
                    byte[] data = graph.CreateChart(reportGraph, dsGraphData, resolver, true);
                    ltlEmail.Text = "##InlineImage##" + reportGraph.GraphName + "##InlineImage##";
                    ReportSubscriptionSender.AddToRequest(report.ReportName, "g" + reportGraph.GraphName, data);
                }
                else
                {
                    graph.CreateChart(reportGraph, dsGraphData, resolver);
                }
            }

            // Check if subscription is allowed
            EnableSubscription = (EnableSubscription && ValidationHelper.GetBoolean(ReportGraphInfo.GraphSettings["SubscriptionEnabled"], true) && report.ReportEnableSubscription);
            if (EmailMode && !EnableSubscription)
            {
                this.Visible = false;
                return;
            }
        }
        catch (Exception ex)
        {
            EventLogProvider ev = new EventLogProvider();
            ev.LogEvent("Get report graph", "E", ex);
            graph.CorrectWidth = correctWidth;
            graph.CreateInvalidDataGraph(reportGraph, "Reporting.Graph.InvalidDataGraph", false);
            errorOccurred = true;
        }
    }


    protected override void Render(HtmlTextWriter writer)
    {
        try
        {
            base.Render(writer);
        }
        catch (Exception e)
        {
            EventLogProvider.LogException("Get report graph", "E", e);
            lblError.Visible = true;
            lblError.Text = GetString("Reporting.Graph.InvalidDataGraph");
            lblError.RenderControl(writer);
        }
    }

    #endregion
}