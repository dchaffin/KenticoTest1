using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading;
using System.Xml;
using System.Data;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.FormEngine;
using CMS.Reporting;
using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.SettingsProvider;
using CMS.WebAnalytics;
using CMS.Scheduler;
using CMS.IO;
using CMS.EmailEngine;
using CMS.SiteProvider;
using CMS.EventLog;

using CultureInfo = System.Globalization.CultureInfo;


public partial class CMSModules_Reporting_Controls_SubscriptionEdit : CMSAdminControl
{
    #region "Variables"

    ReportInfo ri = null;
    ReportSubscriptionInfo rsi = null;
    DataRow drParameters = null;
    bool mSimpleMode = false;
    bool checkLast = true;
    int graphID = QueryHelper.GetInteger("graphID", 0);
    int tableID = QueryHelper.GetInteger("tableID", 0);
    int valueID = QueryHelper.GetInteger("valueID", 0);
    String intervalStr = QueryHelper.GetString("interval", "none");

    #endregion


    #region "Properties"

    /// <summary>
    /// Get report info object
    /// </summary>
    public ReportInfo Report
    {
        get
        {
            return ri;
        }
    }


    /// <summary>
    /// Indicates if control is in simple mode. This mode is displayed in Web Analytics.
    /// </summary>
    public bool SimpleMode
    {
        get
        {
            return mSimpleMode;
        }
        set
        {
            mSimpleMode = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        UseCMSDeskAccessDeniedPage = !IsLiveSite;
        CurrentUserInfo cui = CMSContext.CurrentUser;
        bool haveModify = cui.IsAuthorizedPerResource("cms.reporting", "modify");
        if (!(cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || haveModify))
        {
            RedirectToAccessDenied("cms.reporting", "Subscribe");
        }

        pnlBasic.GroupingText = GetString("reportsubscription.settings");
        pnlParameters.GroupingText = GetString("reportsubscription.parameters.title");

        // Init validator
        rfvEmail.Text = GetString("om.contact.enteremail");
        rfvSubject.Text = GetString("bizformgeneral.emptyemailsubject");

        rsi = ReportSubscriptionInfoProvider.GetReportSubscriptionInfo(QueryHelper.GetInteger("SubscriptionID", 0));

        int reportID = QueryHelper.GetInteger("ReportID", 0);
        String reportName = QueryHelper.GetString("reportName", String.Empty);

        if (reportID != 0)
        {
            ri = ReportInfoProvider.GetReportInfo(reportID);
        }
        else if (!String.IsNullOrEmpty(reportName))
        {
            ri = ReportInfoProvider.GetReportInfo(reportName);
        }
        else
        {
            if (rsi != null)
            {
                // If no report specified, get it from subscription
                ri = ReportInfoProvider.GetReportInfo(rsi.ReportSubscriptionReportID);
            }
        }

        if ((rsi != null) && !haveModify && !SimpleMode)
        {
            if (rsi.ReportSubscriptionUserID != cui.UserID)
            {
                RedirectToAccessDenied(GetString("reportsubscription.onlymodifyusersallowed"));
            }
        }

        if (ri == null)
        {
            return;
        }

        // Set interval control for subscription mode
        ucInterval.DisplayStartTime = false;
        ucInterval.DisplayOnce = false;
        ucInterval.DisplaySecond = false;

        if (!RequestHelper.IsPostBack())
        {
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.hours"), "hour"));
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.days"), "day"));
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.weeks"), "week"));
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.months"), "month"));
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.years"), "year"));
        }

        if (SimpleMode)
        {
            LoadAdHoc();
        }
        else
        {
            LoadData();
        }

        // Show save changes after new object created
        if (!RequestHelper.IsPostBack() && (QueryHelper.GetBoolean("saved", false) == true))
        {
            ShowChangesSaved();
        }

        base.OnInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        rbAll.Text = GetString("reportsubscription.available");
        rbTime.Text = GetString("reportsubscription.time");

        // Register show/hide JS for limit date checkbox
        String script = @"function disableLast(disable) { 
                                document.getElementById ('" + txtLast.ClientID + @"').disabled = disable; 
                                document.getElementById ('" + drpLast.ClientID + @"').disabled = disable; 
                           }
                        ";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "TypeDisable", ScriptHelper.GetScript(script));

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (RequestHelper.IsPostBack())
        {
            checkLast = rbTime.Checked;

            // Interval and last are disabled (no value posted from client), recount their init values
            if (rbAll.Checked)
            {
                SetTimeControls();
            }
        }

        rbTime.Checked = checkLast;
        rbAll.Checked = !checkLast;
        drpLast.Enabled = checkLast;
        txtLast.Enabled = checkLast;

        base.OnPreRender(e);
    }


    /// <summary>
    /// Saves the control. Returns false, if error eccured.
    /// </summary>
    public bool Save()
    {
        // Validates input data
        String error = ValidateData();
        if (!String.IsNullOrEmpty(error))
        {
            ShowError(error);
            return false;
        }

        if (ri != null)
        {
            bool isNew = false;
            if (rsi == null)
            {
                // Create new object for new subscription
                rsi = new ReportSubscriptionInfo();
                rsi.ReportSubscriptionUserID = CMSContext.CurrentUser.UserID;
                rsi.ReportSubscriptionSiteID = CMSContext.CurrentSiteID;
                rsi.ReportSubscriptionSettings["ReportInterval"] = intervalStr;
                isNew = true;
            }

            if (!SimpleMode)
            {
                // Save basic form & validates basic form data
                if (!formElem.SaveData(null))
                {
                    return false;
                }

                // Store all parameters in basic form to string XML representation
                drParameters = formElem.DataRow;
                rsi.ReportSubscriptionValueID = 0;
                rsi.ReportSubscriptionTableID = 0;
                rsi.ReportSubscriptionGraphID = 0;

                // If subscription is not for whole report, store item ID 
                String drpValue = drpItems.SelectedValue;
                if (drpValue != "all")
                {
                    int id = ValidationHelper.GetInteger(drpValue.Substring(1), 0);
                    if (drpValue.Contains('g'))
                    {
                        rsi.ReportSubscriptionGraphID = id;
                    }
                    if (drpValue.Contains('t'))
                    {
                        rsi.ReportSubscriptionTableID = id;
                    }
                    if (drpValue.Contains('v'))
                    {
                        rsi.ReportSubscriptionValueID = id;
                    }
                }
            }
            else
            {
                rsi.ReportSubscriptionGraphID = graphID;
                rsi.ReportSubscriptionTableID = tableID;
                rsi.ReportSubscriptionValueID = valueID;
            }

            if (drParameters != null)
            {
                // Find special 'from' and 'to' parameters. 
                DataColumn dcFrom = drParameters.Table.Columns["fromdate"];
                DataColumn dcTo = drParameters.Table.Columns["todate"];

                if (rbTime.Checked)
                {
                    if (dcTo != null)
                    {
                        // Convert column from datetime to string to enable store macros
                        drParameters.Table.Columns.Remove(dcTo.ColumnName);
                        drParameters.Table.Columns.Add(dcTo.ColumnName, typeof(String));

                        // Add current date time macro
                        drParameters[dcTo.ColumnName] = "{%CurrentDateTime%}";
                    }

                    // Create right macro datetime command based on given interval.
                    String command = String.Empty;
                    switch (drpLast.SelectedValue)
                    {
                        case "hour":
                            command = "AddHours";
                            break;

                        case "day":
                            command = "AddDays";
                            break;

                        case "week":
                            command = "AddWeeks";
                            break;

                        case "month":
                            command = "AddMonths";
                            break;

                        case "year":
                            command = "AddYears";
                            break;
                    }

                    // Create todate macro
                    int last = ValidationHelper.GetInteger(txtLast.Text.Trim(), 0);
                    String dateCommand = String.Format("{{%CurrentDateTime.{0}({1})%}}", command, last * (-1));

                    // Convert fromdate to string
                    if (dcFrom != null)
                    {
                        drParameters.Table.Columns.Remove(dcFrom.ColumnName);
                        drParameters.Table.Columns.Add(dcFrom.ColumnName, typeof(String));
                        drParameters[dcFrom.ColumnName] = dateCommand;
                    }
                }
                else
                {
                    // Empty fromdate and todate for uncheck limit date
                    if (dcFrom != null)
                    {
                        drParameters[dcFrom.ColumnName] = DBNull.Value;
                    }

                    if (dcTo != null)
                    {
                        drParameters[dcTo.ColumnName] = DBNull.Value;
                    }
                }

                // Write parameters to XML string representation
                rsi.ReportSubscriptionParameters = ReportHelper.WriteParametersToXml(drParameters);
            }

            String email = txtEmail.Text.Trim();
            bool emailChanged = rsi.ReportSubscriptionEmail != email;

            rsi.ReportSubscriptionEnabled = chkEnabled.Checked;
            rsi.ReportSubscriptionReportID = ri.ReportID;
            rsi.ReportSubscriptionInterval = ucInterval.ScheduleInterval;
            rsi.ReportSubscriptionEmail = email;
            rsi.ReportSubscriptionSubject = txtSubject.Text;
            rsi.ReportSubscriptionCondition = ucMacroEditor.Text;
            rsi.ReportSubscriptionOnlyNonEmpty = chkNonEmpty.Checked;
            rsi.ReportSubscriptionNextPostDate = SchedulingHelper.GetNextTime(ucInterval.ScheduleInterval, DateTimeHelper.ZERO_TIME, DateTimeHelper.ZERO_TIME);

            ReportSubscriptionInfoProvider.SetReportSubscriptionInfo(rsi);

            // Check whether email changed (applies for new subscription also)
            if (emailChanged)
            {
                String siteName = CMSContext.CurrentSiteName;

                EmailTemplateInfo eti = EmailTemplateProvider.GetEmailTemplate("Reporting_Subscription_information", siteName);
                if (eti != null)
                {
                    // Send information email
                    EmailMessage em = new EmailMessage();
                    em.EmailFormat = EmailFormatEnum.Default;
                    em.From = String.IsNullOrEmpty(eti.TemplateFrom) ? EmailHelper.Settings.NotificationsSenderAdress(siteName) : eti.TemplateFrom; ;
                    em.Recipients = email;
                    em.Subject = eti.TemplateSubject;
                    em.BccRecipients = eti.TemplateBcc;
                    em.CcRecipients = eti.TemplateCc;

                    MacroResolver resolver = ReportSubscriptionSender.CreateSubscriptionMacroResolver(ri, rsi, CMSContext.CurrentSite, em.Recipients);
                    em.Body = resolver.ResolveMacros(eti.TemplateText);
                    em.PlainTextBody = resolver.ResolveMacros(eti.TemplatePlainText);

                    MetaFileInfoProvider.ResolveMetaFileImages(em, eti.TemplateID, EmailObjectType.EMAILTEMPLATE, MetaFileInfoProvider.OBJECT_CATEGORY_TEMPLATE);

                    EmailSender.SendEmail(siteName, em);
                }
            }

            // For new item and advanced mode redirect to store ID in query string
            if ((isNew) && (!SimpleMode))
            {
                URLHelper.Redirect(URLHelper.CurrentURL + "&saved=1&subscriptionid=" + rsi.ReportSubscriptionID);
            }

            ShowChangesSaved();
            return true;
        }

        return false;
    }


    /// <summary>
    /// Sets time controls (dropdown with interval and textbox with interval value). Returns true if time controls are to be hided.
    /// </summary>
    private bool SetTimeControls()
    {
        HitsIntervalEnum interval = HitsIntervalEnumFunctions.StringToHitsConversion(intervalStr);

        DateTime from = DateTimeHelper.ZERO_TIME;
        DateTime to = DateTimeHelper.ZERO_TIME;

        DataColumn dcFrom = null;
        DataColumn dcTo = null;

        if (drParameters != null)
        {
            // Load fromdate and todate from report parameters (passed from query string)
            dcFrom = drParameters.Table.Columns["FromDate"];
            dcTo = drParameters.Table.Columns["ToDate"];
            if (dcFrom != null)
            {
                from = ValidationHelper.GetDateTime(drParameters["FromDate"], DateTimeHelper.ZERO_TIME);
            }

            if (dcTo != null)
            {
                to = ValidationHelper.GetDateTime(drParameters["ToDate"], DateTimeHelper.ZERO_TIME);
            }
        }

        // If one contains zero time, set all time radio button. In such situation, report can maintain unlimited fromdate or todate.
        if ((from == DateTimeHelper.ZERO_TIME) || (to == DateTimeHelper.ZERO_TIME))
        {
            checkLast = false;
        }

        // If one is not set, hide limitdata panel 
        if ((dcFrom == null) || (dcTo == null))
        {
            ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_DAY;
            return true;
        }

        int diff = 0;
        bool noAddToDiff = false;

        // If interval is not known, but 'from' and 'to' is set (f.e. preview, webpart,..) - compute interval from date values
        if (interval == HitsIntervalEnum.None)
        {
            String sFrom = ValidationHelper.GetString(drParameters["FromDate"], String.Empty).ToLowerCSafe();
            String sTo = ValidationHelper.GetString(drParameters["ToDate"], String.Empty).ToLowerCSafe();
            checkLast = true;

            if (ValidationHelper.IsMacro(sFrom) && ValidationHelper.IsMacro(sTo))
            {
                if (sFrom.Contains("addhours"))
                {
                    interval = HitsIntervalEnum.Hour;
                }
                else
                    if (sFrom.Contains("adddays"))
                    {
                        interval = HitsIntervalEnum.Day;
                    }
                    else
                        if (sFrom.Contains("addweeks"))
                        {
                            interval = HitsIntervalEnum.Week;
                        }
                        else
                            if (sFrom.Contains("addmonths"))
                            {
                                interval = HitsIntervalEnum.Month;
                            }
                            else
                                if (sFrom.Contains("addyears"))
                                {
                                    interval = HitsIntervalEnum.Year;
                                }

                to = DateTime.Now;
                from = ValidationHelper.GetDateTime(CMSContext.ResolveMacros(sFrom), DateTime.Now);
                noAddToDiff = true;
            }
            else
                if ((from != DateTimeHelper.ZERO_TIME) && (to != DateTimeHelper.ZERO_TIME))
                {
                    // Set interval as greatest possible interval (365+ days -> years, 30+days->months ,...)
                    diff = (int)(to - from).TotalDays;
                    if (diff >= 365)
                    {
                        interval = HitsIntervalEnum.Year;
                    }
                    else if (diff >= 30)
                    {
                        interval = HitsIntervalEnum.Month;
                    }
                    else if (diff >= 7)
                    {
                        interval = HitsIntervalEnum.Week;
                    }
                    else if (diff >= 1)
                    {
                        interval = HitsIntervalEnum.Day;
                    }
                    else
                    {
                        interval = HitsIntervalEnum.Hour;
                    }
                }
        }

        // Set default period and diff based on interval
        switch (interval)
        {
            case HitsIntervalEnum.Year:
                diff = to.Year - from.Year;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_MONTH;
                break;

            case HitsIntervalEnum.Month:
                diff = ((to.Year - from.Year) * 12) + to.Month - from.Month;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_MONTH;
                break;

            case HitsIntervalEnum.Week:
                diff = (int)(to - from).TotalDays / 7;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_WEEK;
                break;

            case HitsIntervalEnum.Day:
                diff = (int)(to - from).TotalDays;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_DAY;
                break;

            case HitsIntervalEnum.Hour:
                diff = (int)(to - from).TotalHours;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_HOUR;
                break;

            case HitsIntervalEnum.None:
                checkLast = false;
                break;
        }

        // Add current 
        if (!noAddToDiff)
        {
            diff++;
        }

        if (interval != HitsIntervalEnum.None)
        {
            drpLast.SelectedValue = HitsIntervalEnumFunctions.HitsConversionToString(interval);
        }

        if (!checkLast)
        {
            // Defaul settings for no time
            ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_DAY;
        }

        if (diff != 0)
        {
            txtLast.Text = diff.ToString();
        }

        return false;
    }


    /// <summary>
    /// Load data for simple mode
    /// </summary>
    private void LoadAdHoc()
    {
        // Hide controls for advanced mode
        pnlParametersEnvelope.Visible = false;
        pnlSubscription.Visible = false;
        pnlInfo.Visible = true;
        pnlEnabled.Visible = false;


        if (ri != null)
        {
            GenerateInfoText();

            // Parse parameters from query string
            IFormatProvider culture = DateTimeHelper.DefaultIFormatProvider;
            IFormatProvider currentCulture = new CultureInfo(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);

            // Get report parameters
            string parameters = QueryHelper.GetString("parameters", "");
            drParameters = ReportHelper.GetReportParameters(ri, parameters, AnalyticsHelper.PARAM_SEMICOLON, culture, currentCulture);

            if (!RequestHelper.IsPostBack())
            {
                // Create subject from report name
                txtSubject.Text = ri.ReportDisplayName;

                // Fill dropdown with intervals
                txtEmail.Text = CMSContext.CurrentUser.Email;

                ucInterval.ScheduleInterval = String.Empty;
                chkNonEmpty.Checked = true;
                chkEnabled.Checked = true;

                pnlFromToParams.Visible = !SetTimeControls();
            }
        }
    }


    /// <summary>
    /// Generaters info label
    /// </summary>
    private void GenerateInfoText()
    {
        String type = String.Empty;
        if (graphID != 0)
        {
            type = GetString("reporting.graph");
        }
        if (tableID != 0)
        {
            type = GetString("reporting.table");
        }
        if (valueID != 0)
        {
            type = GetString("reporting.value");
        }

        if (type != String.Empty)
        {
            lblInfo.Text = String.Format(GetString("reportsubscription.infotitleitem"), type, ri.ReportDisplayName);
        }
        else
        {
            lblInfo.Text = GetString("reportsubscription.infotitle") + " <b>" + ri.ReportDisplayName + "</b>.";
        }
    }


    /// <summary>
    /// Loads data for advanced mode
    /// </summary>
    private void LoadData()
    {
        if (!RequestHelper.IsPostBack())
        {
            if (rsi != null)
            {
                // Initial settings
                ucInterval.ScheduleInterval = rsi.ReportSubscriptionInterval;
                chkEnabled.Checked = rsi.ReportSubscriptionEnabled;
                txtEmail.Text = rsi.ReportSubscriptionEmail;
                txtSubject.Text = rsi.ReportSubscriptionSubject;
                chkNonEmpty.Checked = rsi.ReportSubscriptionOnlyNonEmpty;
                ucMacroEditor.Text = rsi.ReportSubscriptionCondition;
            }
            else
            {
                // New item
                chkEnabled.Checked = true;
                txtEmail.Text = CMSContext.CurrentUser.Email;
                txtSubject.Text = ri.ReportDisplayName;
                chkNonEmpty.Checked = true;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_DAY;
            }

            FillItems();
        }

        if ((ri != null) && !String.IsNullOrEmpty(ri.ReportParameters) && (ri.ReportParameters != "<form></form>"))
        {
            // Convert field "DateFrom" and "DateTo" to string to be able to display macros
            FormInfo fi = new FormInfo(ri.ReportParameters);
            FormFieldInfo ffi = fi.GetFormField("FromDate");

            if (ffi != null)
            {
                ffi.DataType = FormFieldDataTypeEnum.Text;
                ffi.FieldType = FormFieldControlTypeEnum.CalendarControl;
                ffi.Settings["AllowMacros"] = true;
                ffi.Size = 400;
                ffi.AllowEmpty = true;
                ffi.Visible = false;
            }

            ffi = fi.GetFormField("ToDate");
            if (ffi != null)
            {
                ffi.DataType = FormFieldDataTypeEnum.Text;
                ffi.FieldType = FormFieldControlTypeEnum.CalendarControl;
                ffi.Settings["AllowMacros"] = true;
                ffi.Size = 400;
                ffi.AllowEmpty = true;
                ffi.Visible = false;
            }

            // Get datarow from forminfo (it contains report parameters passed by querystring)
            DataRow dr = fi.GetDataRow();

            // Load default values
            fi.LoadDefaultValues(dr, true);

            // Display basic form, only if any item is visible
            bool display = false;
            foreach (FormFieldInfo field in fi.ItemsList)
            {
                if (field.Visible)
                {
                    display = true;
                    break;
                }
            }

            pnlParametersEnvelope.Visible = display;

            // Loop through all nodes is subscription's XML parameters and replace report's value with current subscription's ones.
            if (rsi != null)
            {
                if (!String.IsNullOrEmpty(rsi.ReportSubscriptionParameters))
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(rsi.ReportSubscriptionParameters);
                    foreach (DataColumn col in dr.Table.Columns)
                    {
                        XmlNode node = xml.SelectSingleNode("/Root/" + col.ColumnName);
                        if (node != null)
                        {
                            // In case of different data types use try catch block
                            try
                            {
                                dr[col.ColumnName] = DataHelper.ConvertValue(node.InnerText, col.DataType);
                            }
                            catch
                            { }
                        }
                    }
                }
            }

            drParameters = dr;
            pnlFromToParams.Visible = !SetTimeControls();

            // Set basic form parameters
            formElem.DataRow = dr;
            formElem.SubmitButton.Visible = false;
            formElem.SubmitButton.RegisterHeaderAction = false;
            formElem.FormInformation = fi;
            formElem.SiteName = CMSContext.CurrentSiteName;
            formElem.ShowPrivateFields = true;
            formElem.Mode = FormModeEnum.Update;
            formElem.Visible = true;
        }
        else
        {
            pnlParametersEnvelope.Visible = false;
            pnlFromToParams.Visible = false;
        }
    }


    /// <summary>
    /// Fill items dropdown
    /// </summary>
    private void FillItems()
    {
        drpItems.Items.Add(new ListItem(GetString("reportsubscription.wholereport"), "all"));

        // Fill graphs
        DataSet ds = ReportGraphInfoProvider.GetGraphs("GraphReportID=" + ri.ReportID, String.Empty);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String displayName = ValidationHelper.GetString(dr["GraphDisplayName"], String.Empty);
                String id = ValidationHelper.GetString(dr["GraphID"], String.Empty);
                drpItems.Items.Add(new ListItem(String.Format("{0} ({1})", displayName, GetString("reporting.graph")), "g" + id));
            }
        }

        // Fill tables
        ds = ReportTableInfoProvider.GetTables("TableReportID=" + ri.ReportID, String.Empty);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String displayName = ValidationHelper.GetString(dr["TableDisplayName"], String.Empty);
                String id = ValidationHelper.GetString(dr["TableID"], String.Empty);
                drpItems.Items.Add(new ListItem(String.Format("{0} ({1})", displayName, GetString("reporting.table")), "t" + id));
            }
        }

        // Fill values
        ds = ReportValueInfoProvider.GetValues("ValueReportID=" + ri.ReportID, String.Empty);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String displayName = ValidationHelper.GetString(dr["ValueDisplayName"], String.Empty);
                String id = ValidationHelper.GetString(dr["ValueID"], String.Empty);
                drpItems.Items.Add(new ListItem(String.Format("{0} ({1})", displayName, GetString("reporting.value")), "v" + id));
            }
        }

        // Select value in dropdown based on non empty key in object
        if (rsi != null)
        {
            if (rsi.ReportSubscriptionGraphID != 0)
            {
                drpItems.SelectedValue = "g" + rsi.ReportSubscriptionGraphID;
            }

            if (rsi.ReportSubscriptionTableID != 0)
            {
                drpItems.SelectedValue = "t" + rsi.ReportSubscriptionTableID;
            }

            if (rsi.ReportSubscriptionValueID != 0)
            {
                drpItems.SelectedValue = "v" + rsi.ReportSubscriptionValueID;
            }
        }
    }


    /// <summary>
    /// Validates input data
    /// </summary>
    protected String ValidateData()
    {
        String error = String.Empty;
        String email = txtEmail.Text.Trim();

        // Check for valid email
        if ((email == String.Empty) || (!ValidationHelper.IsEmail(email)))
        {
            error = GetString("om.contact.enteremail"); ;
        }

        // Check for non empty subject
        if (txtSubject.Text.Trim() == String.Empty)
        {
            error = GetString("bizformgeneral.emptyemailsubject");
        }

        // Check for non empty interval
        if (String.IsNullOrEmpty(ucInterval.ScheduleInterval))
        {
            error = GetString("task_edit.emptyinterval");
        }

        // For simple mode check for non empty 'limit date' value
        if (SimpleMode)
        {
            int last = ValidationHelper.GetInteger(txtLast.Text.Trim(), 0);
            if ((last == 0) && rbTime.Checked)
            {
                error = GetString("reportsubscription.positivenumber");
            }
        }

        return error;
    }

    #endregion
}
