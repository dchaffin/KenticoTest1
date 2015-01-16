using System;
using System.Security.Principal;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.DatabaseHelper;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.URLRewritingEngine;
using CMS.WebFarmSync;
using CMS.ExtendedControls;

public partial class CMSModules_System_Controls_System : CMSAdminControl
{
    #region "Variables"

    private static DateTime mLastRPS = DateTime.MinValue;

    private static long mLastPageRequests;
    private static long mLastNonPageRequests;
    private static long mLastSystemPageRequests;
    private static long mLastPageNotFoundRequests;
    private static long mLastGetFileRequests;

    private static double mRPSPageRequests = -1;
    private static double mRPSNonPageRequests = -1;
    private static double mRPSSystemPageRequests = -1;
    private static double mRPSPageNotFoundRequests = -1;
    private static double mRPSGetFileRequests = -1;

    private bool isSeparatedDB;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the refresh interval (in seconds) for the page. If set to 0 then no refresh.
    /// </summary>
    public int RefreshInterval
    {
        get
        {
            return (int)(timRefresh.Interval / 1000);
        }
        set
        {
            if (value == 0)
            {
                timRefresh.Enabled = false;
            }
            else
            {
                timRefresh.Enabled = true;
                timRefresh.Interval = value * 1000;
                if (drpRefresh.Items.FindByValue(value.ToString()) != null)
                {
                    drpRefresh.SelectedValue = value.ToString();
                }
            }
        }
    }

    #endregion


    #region "Page Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        timRefresh.Interval = 1000 * ValidationHelper.GetInteger(drpRefresh.SelectedValue, 0);
        isSeparatedDB = !String.IsNullOrEmpty(DBSeparationHelper.ConnStringSeparate);

        // Check permissions            
        RaiseOnCheckPermissions("READ", this);

        if (StopProcessing)
        {
            return;
        }

        // Get values from counters
        long totalSystemRequests = RequestHelper.TotalSystemPageRequests.GetValue(null);
        long totalPageRequests = RequestHelper.TotalPageRequests.GetValue(null);
        long totalPageNotFoundRequests = RequestHelper.TotalPageNotFoundRequests.GetValue(null);
        long totalNonPageRequests = RequestHelper.TotalNonPageRequests.GetValue(null);
        long totalGetFileRequests = RequestHelper.TotalGetFileRequests.GetValue(null);

        // Reevaluate RPS
        if (mLastRPS != DateTime.MinValue)
        {
            double seconds = DateTime.Now.Subtract(mLastRPS).TotalSeconds;
            if ((seconds < 3) && (seconds > 0))
            {
                mRPSSystemPageRequests = (totalSystemRequests - mLastSystemPageRequests) / seconds;
                mRPSPageRequests = (totalPageRequests - mLastPageRequests) / seconds;
                mRPSPageNotFoundRequests = (totalPageNotFoundRequests - mLastPageNotFoundRequests) / seconds;
                mRPSNonPageRequests = (totalNonPageRequests - mLastNonPageRequests) / seconds;
                mRPSGetFileRequests = (totalGetFileRequests - mLastGetFileRequests) / seconds;
            }
            else
            {
                mRPSGetFileRequests = -1;
                mRPSNonPageRequests = -1;
                mRPSPageNotFoundRequests = -1;
                mRPSPageRequests = -1;
                mRPSSystemPageRequests = -1;
            }
        }

        mLastRPS = DateTime.Now;

        // Update last values
        mLastGetFileRequests = totalGetFileRequests;
        mLastNonPageRequests = totalNonPageRequests;
        mLastPageNotFoundRequests = totalPageNotFoundRequests;
        mLastPageRequests = totalPageRequests;
        mLastSystemPageRequests = totalSystemRequests;

        // Do not count this page with async postback
        if (RequestHelper.IsAsyncPostback())
        {
            RequestHelper.TotalSystemPageRequests.Decrement(null);
        }

        pnlSystemInfo.GroupingText = GetString("Administration-System.SystemInfo");
        pnlSystemTime.GroupingText = GetString("Administration-System.SystemTimeInfo");
        pnlDatabaseInfo.GroupingText = GetString("Administration-System.DatabaseInfo");
        lblServerName.Text = GetString("Administration-System.ServerName");
        lblServerVersion.Text = GetString("Administration-System.ServerVersion");
        lblDBName.Text = GetString("Administration-System.DatabaseName");
        lblDBSize.Text = GetString("Administration-System.DatabaseSize");
        lblMachineName.Text = GetString("Administration-System.MachineName");
        lblMachineNameValue.Text = HTTPHelper.MachineName;
        lblAspAccount.Text = GetString("Administration-System.Account");
        lblValueAspAccount.Text = WindowsIdentity.GetCurrent().Name;
        lblPool.Text = GetString("Administration-System.Pool");
        lblValuePool.Text = GetString("General.NA");
        plcSeparatedName.Visible = isSeparatedDB;
        plcSeparatedServerName.Visible = isSeparatedDB;
        plcSeparatedSize.Visible = isSeparatedDB;
        plcSeparatedVersion.Visible = isSeparatedDB;
        plcSeparatedSpace.Visible = isSeparatedDB;
        plcSeparatedHeader.Visible = isSeparatedDB;

        try
        {
            lblValuePool.Text = SystemHelper.GetApplicationPoolName();
        }
        catch
        {
        }

        // Get application trust level
        lblTrustLevel.Text = GetString("Administration-System.TrustLevel");
        lblValueTrustLevel.Text = GetString("General.NA");

        try
        {
            lblValueTrustLevel.Text = SystemHelper.CurrentTrustLevel.ToString();
        }
        catch
        {
        }

        lblAspVersion.Text = GetString("Administration-System.Version");
        lblValueAspVersion.Text = Environment.Version.ToString();

        pnlMemory.GroupingText = GetString("Administration-System.MemoryStatistics");

        lblAlocatedMemory.Text = GetString("Administration-System.Memory");
        lblPeakMemory.Text = GetString("Administration-System.PeakMemory");
        lblVirtualMemory.Text = GetString("Administration-System.VirtualMemory");
        lblPhysicalMemory.Text = GetString("Administration-System.PhysicalMemory");
        lblIP.Text = GetString("Administration-System.IP");

        pnlPageViews.GroupingText = GetString("Administration-System.PageViews");
        lblPageViewsValues.Text = GetString("Administration-System.PageViewsValues");

        lblPages.Text = GetString("Administration-System.Pages");
        lblSystemPages.Text = GetString("Administration-System.SystemPages");
        lblNonPages.Text = GetString("Administration-System.NonPages");
        lblGetFilePages.Text = GetString("Administration-System.GetFilePages");
        lblPagesNotFound.Text = GetString("Administration-System.PagesNotFound");
        lblPending.Text = GetString("Administration-System.Pending");

        pnlCache.GroupingText = GetString("Administration-System.CacheStatistics");
        lblCacheExpired.Text = GetString("Administration-System.CacheExpired");
        lblCacheRemoved.Text = GetString("Administration-System.CacheRemoved");
        lblCacheUnderused.Text = GetString("Administration-System.CacheUnderused");
        lblCacheItems.Text = GetString("Administration-System.CacheItems");
        lblCacheDependency.Text = GetString("Administration-System.CacheDependency");

        pnlGC.GroupingText = GetString("Administration-System.GC");

        btnClear.Text = GetString("Administration-System.btnClear");
        btnClearCache.Text = GetString("Administration-System.btnClearCache");
        btnRestart.Text = GetString("Administration-System.btnRestart");
        btnRestartWebfarm.Text = GetString("Administration-System.btnRestartWebfarm");
        btnClearCounters.Text = GetString("Administration-System.btnClearCounters");
        btnRestartServices.Text = GetString("Administration-System.btnRestartServices");

        // Hide button if wefarms are not enabled or disallow restarting webfarms for Windows Azure
        if (!WebSyncHelperClass.WebFarmEnabled || AzureHelper.IsRunningOnAzure)
        {
            btnRestartWebfarm.Visible = false;
        }

        // Hide the web farm restart button if this web farm server is not enabled
        if (!RequestHelper.IsPostBack())
        {
            WebFarmServerInfo webFarmServerObj = WebFarmServerInfoProvider.GetWebFarmServerInfo(WebSyncHelperClass.ServerName);
            if ((webFarmServerObj != null) && (!webFarmServerObj.ServerEnabled))
            {
                btnRestartWebfarm.Visible = false;
            }
        }

        // Hide restart service button if services folder doesn't exist
        btnRestartServices.Visible = SystemHelper.IsFullTrustLevel && WinServiceHelper.ServicesAvailable();

        // Hide the performance counters clear button if the health monitoring is not enabled or feature is not available
        btnClearCounters.Visible = SystemHelper.IsFullTrustLevel && LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.HealthMonitoring);

        LoadData();

        if (!RequestHelper.IsPostBack())
        {
            switch (QueryHelper.GetString("lastaction", String.Empty).ToLowerCSafe())
            {
                case "restarted":
                    ShowConfirmation(GetString("Administration-System.RestartSuccess"));
                    break;

                case "webfarmrestarted":
                    if (ValidationHelper.ValidateHash("WebfarmRestarted", QueryHelper.GetString("restartedhash", String.Empty)))
                    {
                        ShowConfirmation(GetString("Administration-System.WebframRestarted"));
                        // Restart other servers - create webfarm task for application restart
                        WebSyncHelperClass.CreateTask(WebFarmTaskTypeEnum.RestartApplication, "RestartApplication", "", null);
                    }
                    else
                    {
                        ShowError(GetString("general.actiondenied"));
                    }
                    break;

                case "countercleared":
                    ShowConfirmation(GetString("Administration-System.CountersCleared"));
                    break;

                case "winservicesrestarted":
                    ShowConfirmation(GetString("Administration-System.WinServicesRestarted"));
                    break;
            }
        }

        lblRunTime.Text = GetString("Administration.System.RunTime");
        lblServerTime.Text = GetString("Administration.System.ServerTime");

        // Remove miliseconds from the end of the time string
        string timeSpan = (DateTime.Now - CMSAppBase.ApplicationStart).ToString();
        int index = timeSpan.LastIndexOfCSafe('.');
        if (index >= 0)
        {
            timeSpan = timeSpan.Remove(index);
        }

        // Display application run time
        lblRunTimeValue.Text = timeSpan;
        lblServerTimeValue.Text = Convert.ToString(DateTime.Now) + " " + TimeZoneHelper.GetGMTStringOffset(TimeZoneHelper.ServerTimeZone);

        lblIPValue.Text = HTTPHelper.UserHostAddress;
    }

    #endregion


    #region "Page Events"

    /// <summary>
    /// Clear unused memory.
    /// </summary>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Collect the memory
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Log event
        EventLogProvider eventLog = new EventLogProvider();
        eventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "System", "CLEARMEM", null, GetString("Administration-System.ClearSuccess"));

        ShowConfirmation(GetString("Administration-System.ClearSuccess"));

        LoadData();
    }


    /// <summary>
    /// Restart application.
    /// </summary>
    protected void btnRestart_Click(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (RestartApplication())
        {
            if (AzureHelper.IsRunningOnAzure)
            {
                // Log event
                EventLogProvider eventLog = new EventLogProvider();
                eventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "System", "ENDAPP", null, GetString("admin.system.restartazure"));
                ShowConfirmation(GetString("admin.system.restartazure"));
            }
            else
            {
                // Log event
                EventLogProvider eventLog = new EventLogProvider();
                eventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "System", "ENDAPP", null, GetString("Administration-System.RestartSuccess"));

                string url = URLHelper.UpdateParameterInUrl(URLRewriter.CurrentURL, "lastaction", "Restarted");
                URLHelper.Redirect(url);
            }
        }
        else
        {
            ShowError(GetString("System.Restart.Failed"));
        }
    }


    /// <summary>
    /// Restart webfarm.
    /// </summary>
    protected void btnRestartWebfarm_Click(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Restart current server
        RestartApplication();

        // Log event
        EventLogProvider eventLog = new EventLogProvider();
        eventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "System", "RESTARTWEBFARM", null, GetString("Administration-System.WebframRestarted"));

        string url = URLHelper.UpdateParameterInUrl(URLRewriter.CurrentURL, "lastaction", "WebfarmRestarted");
        url = URLHelper.UpdateParameterInUrl(url, "restartedhash", ValidationHelper.GetHashString("WebfarmRestarted"));
        URLHelper.Redirect(url);
    }


    /// <summary>
    /// Clear counters.
    /// </summary>
    protected void btnClearCounters_Click(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        try
        {
            // Reset values of health monitoring counters
            HealthMonitoringManager.ResetCounters();
            // Clear application counters
            HealthMonitoringLogHelper.ClearApplicationCounters();

            // Log event
            EventLogProvider eventLog = new EventLogProvider();
            eventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "System", "CLEARCOUNTERS", null, GetString("Administration-System.CountersCleared"));

            string url = URLHelper.UpdateParameterInUrl(URLRewriter.CurrentURL, "lastaction", "CounterCleared");
            URLHelper.Redirect(url);
        }
        catch (Exception ex)
        {
            LogAndShowError("System", "CLEARCOUNTERS", ex);
        }
    }


    /// <summary>
    /// Restart windows services.
    /// </summary>
    protected void btnRestartServices_Click(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Resets values of counters
        try
        {
            WinServiceHelper.RestartService(null);

            // Log event
            EventLogProvider eventLog = new EventLogProvider();
            eventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "System", "RESTARTWINSERVICES", null, GetString("Administration-System.WinServicesRestarted"));
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("WinServiceHelper", "RestartService", ex);
        }

        string url = URLHelper.UpdateParameterInUrl(URLRewriter.CurrentURL, "lastaction", "WinServicesRestarted");
        URLHelper.Redirect(url);
    }


    protected void btnClearCache_Click(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Clear the cache
        CacheHelper.ClearCache(null, true);
        Functions.ClearHashtables();

        // Drop the routes
        CMSMvcHandler.DropAllRoutes();

        // Disposes all zip files
        ZipStorageProvider.DisposeAll();

        // Collect the memory
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Log event
        EventLogProvider eventLog = new EventLogProvider();
        eventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "System", "CLEARCACHE", null, GetString("Administration-System.ClearCacheSuccess"));

        ShowConfirmation(GetString("Administration-System.ClearCacheSuccess"));

        LoadData();
    }

    #endregion


    #region "Other Methods"

    /// <summary>
    /// Loads the data.
    /// </summary>
    protected void LoadData()
    {
        lblValueAlocatedMemory.Text = DataHelper.GetSizeString(GC.GetTotalMemory(false));

        lblValueVirtualMemory.Text = "N/A";
        lblValuePhysicalMemory.Text = "N/A";
        lblValuePeakMemory.Text = "N/A";

        // Process memory
        try
        {
            lblValueVirtualMemory.Text = DataHelper.GetSizeString(SystemHelper.GetVirtualMemorySize());
            lblValuePhysicalMemory.Text = DataHelper.GetSizeString(SystemHelper.GetWorkingSetSize());
            lblValuePeakMemory.Text = DataHelper.GetSizeString(SystemHelper.GetPeakWorkingSetSize());
        }
        catch
        {
        }

        lblValuePages.Text = GetViewValues(RequestHelper.TotalPageRequests.GetValue(null), mRPSPageRequests);
        lblValuePagesNotFound.Text = GetViewValues(RequestHelper.TotalPageNotFoundRequests.GetValue(null), mRPSPageNotFoundRequests);
        lblValueSystemPages.Text = GetViewValues(RequestHelper.TotalSystemPageRequests.GetValue(null), mRPSSystemPageRequests);
        lblValueNonPages.Text = GetViewValues(RequestHelper.TotalNonPageRequests.GetValue(null), mRPSNonPageRequests);
        lblValueGetFilePages.Text = GetViewValues(RequestHelper.TotalGetFileRequests.GetValue(null), mRPSGetFileRequests);

        long pending = RequestHelper.PendingRequests.GetValue(null);
        if (pending > 1)
        {
            // Current request does not count as pending at the time of display
            pending--;
        }
        if (pending < 0)
        {
            pending = 0;
        }

        lblValuePending.Text = pending.ToString();

        lblCacheItemsValue.Text = Cache.Count.ToString();
        lblCacheExpiredValue.Text = CacheHelper.Expired.GetValue(null).ToString();
        lblCacheRemovedValue.Text = CacheHelper.Removed.GetValue(null).ToString();
        lblCacheUnderusedValue.Text = CacheHelper.Underused.GetValue(null).ToString();
        lblCacheDependencyValue.Text = CacheHelper.DependencyChanged.GetValue(null).ToString();

        // GC collections
        try
        {
            plcGC.Controls.Clear();

            int generations = GC.MaxGeneration;
            for (int i = 0; i <= generations; i++)
            {
                int count = GC.CollectionCount(i);
                string genString = "<tr><td style=\"white-space: nowrap; width: 200px;\">" + GetString("GC.Generation") + " " + i.ToString() + ":</td><td>" + count.ToString() + "</td></tr>";

                plcGC.Controls.Add(new LiteralControl(genString));
            }
        }
        catch
        {
        }

        TableManager tm = new TableManager(null);

        lblDBNameValue.Text = tm.DatabaseName;
        lblServerVersionValue.Text = tm.DatabaseServerVersion;

        // DB information       
        if (!RequestHelper.IsPostBack())
        {
            lblDBSizeValue.Text = tm.DatabaseSize;
            lblServerNameValue.Text = tm.DatabaseServerName;

            // Check if DB is separated
            bool separated = !String.IsNullOrEmpty(DBSeparationHelper.ConnStringSeparate);
            if (separated)
            {
                tm = new TableManager(DBSeparationHelper.OM_CONNECTIONSTRING);

                lblSeparatedServerName.Text = tm.DatabaseServerName;
                lblSeparatedVersion.Text = tm.DatabaseServerVersion;
                lblSeparatedName.Text = tm.DatabaseName;
                lblSeparatedSize.Text = tm.DatabaseSize;
            }
        }
    }


    /// <summary>
    /// Gets the values string for the page views.
    /// </summary>
    private string GetViewValues(long total, double rps)
    {
        return total.ToString() + ((rps >= 1) ? " (" + Math.Floor(rps).ToString() + " RPS)" : "");
    }


    /// <summary>
    /// Check user settings if site manager is not explicitly denied for current user. 
    /// If it is user is redirected to access denied page.
    /// </summary>        
    private void CheckAccessToSiteManager()
    {
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        if (currentUser != null)
        {
            if (currentUser.UserSiteManagerDisabled)
            {
                RedirectToAccessDenied(ResHelper.GetString("accessdeniedtopage.sitemanagerdenied"));
            }
        }
    }


    /// <summary>
    /// Checks if the user is global administrator (can access the administration (development) page.
    /// </summary>
    private void CheckGlobalAdministrator()
    {
        if (!RequestHelper.IsAsyncPostback())
        {
            CurrentUserInfo currentUser = CMSContext.CurrentUser;
            if (currentUser == null)
            {
                URLHelper.Redirect(mAccessDeniedPage);
            }
            else if (!currentUser.UserSiteManagerAdmin)
            {
                URLHelper.Redirect(mAccessDeniedPage);
            }
        }
    }


    /// <summary>
    /// Tries to restart application and returns if restart was successful.
    /// </summary>    
    private bool RestartApplication()
    {
        // Restart azure instance
        if (AzureHelper.IsRunningOnAzure)
        {
            AzureHelper.RestartAzureInstance();
        }
        // Restart classic application
        else
        {
            try
            {
                // Try to restart application by unload app domain
                HttpRuntime.UnloadAppDomain();
            }
            catch
            {
                try
                {
                    // Try to restart application by changing web.config file
                    File.SetLastWriteTimeUtc(Request.PhysicalApplicationPath + "\\web.config", DateTime.UtcNow);
                }
                catch
                {
                    return false;
                }
            }
        }
        return true;
    }

    #endregion
}