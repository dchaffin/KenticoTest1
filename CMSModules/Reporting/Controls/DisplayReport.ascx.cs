using System;
using System.Data;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.Reporting;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Reporting_Controls_DisplayReport : AbstractReportControl, IDisplayReport
{
    #region "Variables"

    private string mReportName = "";
    private string mReportHTML = "";
    private bool mDisplayFilter = true; // DisplayFilter property
    private bool mDisplayFilterResult = true; // internal display filter based on DisplayFilter and visible elements
    private bool mSaveMode = false;
    private bool mLoadFormParameters = true;
    private ReportInfo mReportInfo = null;
    private int mSavedReportId = 0;
    private ArrayList mReportControls = null;
    private bool wasInit = false;
    private bool wasPreRender = false;
    private bool mCheckInnerControls = true;
    private bool mUseExternalReload = false;
    private bool mUseProgressIndicator = false;
    private bool isSave = false;
    private static Regex mInlineImageRegex = null;
    private bool reportLoaded = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Regex object for search inline images in report.
    /// </summary>
    private static Regex InlineImageRegex
    {
        get
        {
            if (mInlineImageRegex == null)
            {
                mInlineImageRegex = RegexHelper.GetRegex("<img\\s*src=\"\\S*fileguid=(\\S*)\" />");
            }

            return mInlineImageRegex;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether progress indicator should be used.
    /// </summary>
    public bool UseProgressIndicator
    {
        get
        {
            return mUseProgressIndicator;
        }
        set
        {
            mUseProgressIndicator = value;
        }
    }


    /// <summary>
    /// Report name.
    /// </summary>
    public ReportInfo ReportInfo
    {
        get
        {
            if ((mReportInfo == null) && (!String.IsNullOrEmpty(ReportName)))
            {
                mReportInfo = ReportInfoProvider.GetReportInfo(ReportName);
            }
            return mReportInfo;
        }
        set
        {
            mReportInfo = value;
        }
    }


    /// <summary>
    /// Content panel.
    /// </summary>
    public Panel ReportPanel
    {
        get
        {
            return pnlContent;
        }
    }


    /// <summary>
    /// Graph possible width of control.
    /// </summary>
    public int AreaMaxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["AreaMaxWidth"], 0);
        }
        set
        {
            ViewState["AreaMaxWidth"] = value;
        }
    }


    /// <summary>
    /// If true, reload is not called from this control.
    /// </summary>
    public bool UseExternalReload
    {
        get
        {
            return mUseExternalReload;
        }
        set
        {
            mUseExternalReload = value;
        }
    }


    /// <summary>
    /// BasicForm object.
    /// </summary>
    public BasicForm ParametersForm
    {
        get
        {
            return formElem;
        }
    }


    /// <summary>
    /// Returns report display name
    /// </summary>
    public string ReportDisplayName
    {
        get
        {
            if (ReportInfo != null)
            {
                return ReportInfo.ReportDisplayName;
            }

            return null;
        }
    }


    /// <summary>
    /// Report name.
    /// </summary>
    public string ReportName
    {
        get
        {
            return mReportName;
        }
        set
        {
            mReportName = value;
        }
    }


    /// <summary>
    /// Indicates if check inner sub controls.
    /// </summary>
    public bool CheckInnerControls
    {
        get
        {
            return mCheckInnerControls;
        }
        set
        {
            mCheckInnerControls = value;
        }
    }


    /// <summary>
    /// ReportHTML.
    /// </summary>
    public string ReportHTML
    {
        get
        {
            return mReportHTML;
        }
        set
        {
            mReportHTML = value;
        }
    }


    /// <summary>
    /// Display filter.
    /// </summary>
    public bool DisplayFilter
    {
        get
        {
            return mDisplayFilter;
        }
        set
        {
            mDisplayFilter = value;
        }
    }


    /// <summary>
    /// Body CSS class.
    /// </summary>
    public string BodyCssClass
    {
        get
        {
            return pnlContent.CssClass;
        }
        set
        {
            pnlContent.CssClass = value;
        }
    }


    /// <summary>
    /// Form CSS class.
    /// </summary>
    public string FormCssClass
    {
        get
        {
            return formElem.CssClass;
        }
        set
        {
            formElem.CssClass = value;
        }
    }


    /// <summary>
    /// Assigned if parameters will be loaded automaticcaly.
    /// </summary>
    public bool LoadFormParameters
    {
        get
        {
            return mLoadFormParameters;
        }
        set
        {
            mLoadFormParameters = value;
        }
    }


    /// <summary>
    /// Load always default values.
    /// </summary>
    public bool ForceLoadDefaultValues
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ForceLoadDefaultValues"], false);
        }
        set
        {
            ViewState["ForceLoadDefaultValues"] = value;
        }
    }


    /// <summary>
    /// Reloads parameters even if were already inited.
    /// </summary>
    public bool IgnoreWasInit
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IgnoreWasInit"], false);
        }
        set
        {
            ViewState["IgnoreWasInit"] = value;
        }
    }


    /// <summary>
    /// Child report controls.
    /// </summary>
    public ArrayList ReportControls
    {
        get
        {
            if (mReportControls == null)
            {
                mReportControls = new ArrayList();

                // Collect the controls
                foreach (Control c in pnlContent.Controls)
                {
                    if (c is AbstractReportControl)
                    {
                        mReportControls.Add(c);
                    }
                }
            }

            return mReportControls;
        }
    }

    #endregion


    /// <summary>
    /// Load parameters from data form.
    /// </summary>
    protected void LoadParameters()
    {
        if (ReportInfo != null)
        {
            // Set the form
            FormInfo fi = new FormInfo(ReportInfo.ReportParameters);
            // Get datarow with required columns
            DataRow dr = fi.GetDataRow();
            if (!RequestHelper.IsPostBack() || ForceLoadDefaultValues || !DisplayFilter)
            {
                fi.LoadDefaultValues(dr, true);
            }
            else
            {
                if ((!ForceLoadDefaultValues) && (formElem.DataRow != null))
                {
                    dr = formElem.DataRow;
                }
            }

            // show filter - based on DisplayFilter and number of visible elements
            mDisplayFilterResult = DisplayFilter && (fi.GetFormElements(true, false).Any());

            formElem.DataRow = dr;
            formElem.SubmitButton.Visible = mDisplayFilterResult;
            formElem.SubmitButton.RegisterHeaderAction = false;
            formElem.FormInformation = fi;
            formElem.SubmitButton.Text = GetString("Report_View.btnUpdate");
            formElem.SiteName = CMSContext.CurrentSiteName;
            formElem.ShowPrivateFields = true;
            formElem.Mode = FormModeEnum.Insert;
            formElem.Visible = mDisplayFilterResult;
        }
    }


    /// <summary>
    /// OnInit.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Load parameters from data form
        LoadParameters();

        // Set wasInit value
        wasInit = true;

        // Get handler to OnAfter save
        formElem.OnAfterSave += formElem_OnAfterSave;
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (RenderCssClasses)
        {
            formElem.SubmitButton.CssClass = "SubmitButton";
        }

        bool preloaderRequired = ValidationHelper.GetBoolean(RequestStockHelper.GetItem("CMSGraphAutoWidth"), false);
        if (UseProgressIndicator && (!preloaderRequired || RequestHelper.IsPostBack()))
        {
            ScriptHelper.RegisterProgress(Page);
        }

        wasPreRender = true;

        if (RequestHelper.IsPostBack() && (!UseExternalReload || DisplayFilter))
        {
            try
            {
                formElem.SaveData(null);
            }
            catch
            {
            }
        }

        if (!reportLoaded)
        {
            ReloadData(true);
        }


        // Disable CSS class for filter if filter isn't visible
        if ((mDisplayFilterResult == false) || (formElem == null) ||
            (ReportInfo == null) || (ReportInfo.ReportParameters == "") ||
            (ReportInfo.ReportParameters.ToLowerCSafe() == "<form></form>"))
        {
            formElem.CssClass = "";
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// OnAfterSave handler.
    /// </summary>
    private void formElem_OnAfterSave(object sender, EventArgs e)
    {
        // Only want to react to SaveData()
        if (wasPreRender)
        {
            bool tmpForceLoad = ForceLoadDefaultValues;
            ForceLoadDefaultValues = false;
            ReloadData(false);
            ForceLoadDefaultValues = tmpForceLoad;
        }
        else
        {
            // Set report parameters for parent control - used for print   
            if (mLoadFormParameters)
            {
                ReportParameters = formElem.DataRow;
            }
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        pnlContent.Controls.Clear();
        mReportControls = null;

        // Report info must exists
        if (ReportInfo != null)
        {
            // Check permissions for report
            if (ReportInfo.ReportAccess != ReportAccessEnum.All)
            {
                if (!CMSContext.CurrentUser.IsAuthenticated())
                {
                    Visible = false;
                    return;
                }
            }

            // If ReloadData is called before Init, load parameters
            if (!wasInit || IgnoreWasInit)
            {
                LoadParameters();
            }

            // Set parameters only if it is allowed
            if (mLoadFormParameters)
            {
                ReportParameters = formElem.DataRow;
            }

            // Clear resolver because it can contains old parameters values
            ClearResolver();

            // Build report HTML
            mReportHTML = ReportInfo.ReportLayout;
            mReportHTML = ResolveMacros(mReportHTML);
            mReportHTML = HTMLHelper.ResolveUrls(mReportHTML, null);

            // For emails - resolve metafile images as inline attachments
            if (EmailMode)
            {
                // Find all inline images
                MatchCollection coll = InlineImageRegex.Matches(mReportHTML);
                foreach (Match m in coll)
                {
                    Guid guid = ValidationHelper.GetGuid(m.Groups[1], Guid.Empty);
                    if (guid != Guid.Empty)
                    {
                        // For all found images, find according metafile
                        MetaFileInfo mfi = MetaFileInfoProvider.GetMetaFileInfo(guid, null, true);
                        if (mfi != null)
                        {
                            // If metafile found add binary representation to inline attachments
                            ReportSubscriptionSender.AddToRequest(ReportName, "g" + guid, mfi.MetaFileBinary);
                            mReportHTML = mReportHTML.Replace(m.Value, "##InlineImage##" + guid + "##InlineImage##");
                        }
                    }
                }
            }

            // Add the content
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(new LiteralControl(mReportHTML));

            ControlsHelper.ResolveDynamicControls(pnlContent);

            // Null GraphImageWidh if no graph present to enable load alone tables,values,etc 
            bool containsGraph = mReportHTML.Contains("%%control:ReportGraph?");
            if (!containsGraph)
            {
                GraphImageWidth = 0;
            }

            // Indicates if any item in the report is visible
            bool itemVisible = false;

            // Init the child controls
            foreach (AbstractReportControl ctrl in ReportControls)
            {
                if ((CheckInnerControls) && (!ctrl.IsValid(ReportInfo)))
                {
                    ctrl.Visible = false;
                    continue;
                }

                ctrl.RenderCssClasses = RenderCssClasses;
                ctrl.ReportParameters = ReportParameters;
                ctrl.ContextParameters = ContextParameters;
                ctrl.AllParameters = AllParameters;
                ctrl.SavedReportGuid = Guid.Empty;
                ctrl.GraphImageWidth = GraphImageWidth;
                ctrl.DynamicMacros = DynamicMacros;
                ctrl.TableFirstColumnWidth = TableFirstColumnWidth;
                ctrl.SelectedInterval = SelectedInterval;
                ctrl.RenderCssClasses = RenderCssClasses;
                ctrl.SendOnlyNonEmptyDataSource = SendOnlyNonEmptyDataSource;
                ctrl.EmailMode = EmailMode;
                ctrl.ReportSubscriptionSiteID = ReportSubscriptionSiteID;

                // Do no generate export context menu for saved graph
                ctrl.EnableExport = !isSave && EnableExport;

                if (AreaMaxWidth != 0)
                {
                    ctrl.ComputedWidth = AreaMaxWidth;
                }

                // In save mode must be defined new Guid for graph image and saved reeport id
                if (mSaveMode)
                {
                    ctrl.SavedReportGuid = Guid.NewGuid();
                    ctrl.SavedReportID = mSavedReportId;
                }

                if (formElem != null && formElem.ValidateData())
                {
                    ctrl.ReloadData(forceLoad);
                }

                if (ctrl.ComputedWidth != 0)
                {
                    AreaMaxWidth = ctrl.ComputedWidth;
                }

                itemVisible = itemVisible || ctrl.Visible;
            }

            if (!itemVisible && EmailMode)
            {
                Visible = false;
            }

            reportLoaded = true;

            // Display/hide the filtering form
            formElem.Visible = mDisplayFilterResult;
        }
    }


    /// <summary>
    /// Returns true if report is loaded
    /// </summary>
    public bool IsReportLoaded()
    {
        return (ReportInfo != null);
    }


    /// <summary>
    /// Saves the report - Returns the saved report ID or 0 if some error was occurred or don't have permissions to this report.
    /// </summary>    
    public int SaveReport()
    {
        // Report info must exists
        if (ReportInfo != null)
        {
            // Check permissions for report
            if (ReportInfo.ReportAccess != ReportAccessEnum.All)
            {
                if (!CMSContext.CurrentUser.IsAuthenticated())
                {
                    Visible = false;
                    return 0;
                }
            }
            try
            {
                // Save saved report info object
                SavedReportInfo sri = new SavedReportInfo();
                sri.SavedReportDate = DateTime.Now;
                sri.SavedReportGUID = Guid.NewGuid();
                sri.SavedReportHTML = "";
                sri.SavedReportReportID = ReportInfo.ReportID;
                sri.SavedReportCreatedByUserID = CMSContext.CurrentUser.UserID;
                sri.SavedReportParameters = ReportInfo.ReportParameters;

                string name = ReportInfo.ReportDisplayName;
                string timeStamp = " - " + DateTime.Now.ToString();
                if (name.Length + timeStamp.Length > 200)
                {
                    name = name.Substring(0, name.Length - (name.Length + timeStamp.Length - 200));
                }
                sri.SavedReportTitle = name + timeStamp;
                SavedReportInfoProvider.SetSavedReportInfo(sri);

                // Render control to get HTML code
                mSavedReportId = sri.SavedReportID;
                mSaveMode = true;

                formElem.SaveData("");

                isSave = true;
                ReloadData(true);
                isSave = false;

                // Render panel with controls and save it to string
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                Html32TextWriter writer2 = new Html32TextWriter(sw);
                pnlContent.RenderControl(writer2);

                // Save HTML to the saved report
                sri.SavedReportHTML = ResolveMacros(sb.ToString());
                sri.SavedReportHTML = HTMLHelper.UnResolveUrls(sri.SavedReportHTML, ResolveUrl("~/"));
                SavedReportInfoProvider.SetSavedReportInfo(sri);

                mSaveMode = false;

                // When no 'read' for reporting - reload data with mSaveMode set to false. This will prevent GetReportGraph.aspx to stop rendering graph.
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.reporting", "read"))
                {
                    ReloadData(true);
                }

                return sri.SavedReportID;
            }
            catch
            {
            }
        }
        return 0;
    }


    /// <summary>
    /// Renders control to String representation
    /// </summary>
    /// <param name="siteID">This SiteID is used in report query instead of default CMSContext one</param>
    public String RenderToString(int siteID)
    {
        // Change siteID from context to subscription id
        foreach (DataParameter par in AllParameters)
        {
            if (par.Name == "CMSContextCurrentSiteID")
            {
                par.Value = siteID;
                break;
            }
        }

        // Reload data
        isSave = true;
        ReloadData(true);
        isSave = false;

        // If control is not visible - subscription (report's or all items) is not allowed or no data found
        if (!Visible)
        {
            return String.Empty;
        }

        // Render to string
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        Html32TextWriter writer2 = new Html32TextWriter(sw);
        pnlContent.RenderControl(writer2);

        return sb.ToString();
    }
}