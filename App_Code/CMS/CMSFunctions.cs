using System;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.OutputFilter;
using CMS.SettingsProvider;
using CMS.WebAnalytics;
using CMS.WebFarmSyncHelper;
using CMS.IO;

using IOExceptions = System.IO;
using CMS.VirtualPathHelper;
using CMS.WebFarmSync;

/// <summary>
/// Global CMS Functions.
/// </summary>
public static class CMSFunctions
{
    #region "Variables"

    public static bool mAsyncInit = true;

    /// <summary>
    /// If true, at least one debug has the logging to file enabled.
    /// </summary>
    private static bool? mAnyDebugLogToFileEnabled = null;

    /// <summary>
    /// Folder containing folder structure and zip files which defines automatically handled zipped folders
    /// </summary>
    private const string ZIPPED_FILES_FOLDER = "~/CMSZipped";

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns true if any debug is enabled.
    /// </summary>
    public static bool AnyDebugEnabled
    {
        get
        {
            return CMSContext.AnyDebugEnabled;
        }
        set
        {
            CMSContext.AnyDebugEnabled = value;
        }
    }


    /// <summary>
    /// Returns true if any debug has the logging to file enabled.
    /// </summary>
    public static bool AnyDebugLogToFileEnabled
    {
        get
        {
            if (mAnyDebugLogToFileEnabled == null)
            {
                mAnyDebugLogToFileEnabled =
                    SqlHelperClass.LogQueries ||
                    CacheHelper.LogCache ||
                    OutputHelper.LogOutputToFile ||
                    SecurityHelper.LogSecurity ||
                    MacroResolver.LogMacros ||
                    File.LogFiles ||
                    WebSyncHelperClass.LogWebFarm ||
                    RequestHelper.LogRequests ||
                    AnalyticsHelper.LogAnalytics;
            }

            return mAnyDebugLogToFileEnabled.Value;
        }
        set
        {
            mAnyDebugLogToFileEnabled = value;
        }
    }


    /// <summary>
    /// Returns true if any LiveDebug is enabled.
    /// </summary>
    public static bool AnyLiveDebugEnabled
    {
        get
        {
            return CMSContext.AnyLiveDebugEnabled;
        }
        set
        {
            CMSContext.AnyLiveDebugEnabled = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Searches the request logs for a particular log by the request GUID
    /// </summary>
    /// <param name="guid">Request GUID</param>
    public static RequestLog FindRequestLog(Guid guid)
    {
        // Find the root log
        RequestLog log = FindRequestLog(
            guid,
            SqlHelperClass.LastLogs,
            CacheHelper.LastLogs,
            File.LastLogs,
            CMSControlsHelper.LastLogs,
            SecurityHelper.LastLogs,
            OutputHelper.LastLogs,
            RequestHelper.LastLogs
        );

        return log;
    }


    /// <summary>
    /// Finds any log in the given lists.
    /// </summary>
    /// <param name="lists">Array of the available log lists</param>
    /// <param name="guid">Request GUID</param>
    private static RequestLog FindRequestLog(Guid guid, params IEnumerable<RequestLog>[] lists)
    {
        foreach (var list in lists)
        {
            RequestLog log = RequestLog.FindByGUID(list, guid);
            if (log != null)
            {
                return log;
            }
        }

        return null;
    }



    /// <summary>
    /// Sets all the debug settings to null and causes them to be reloaded.
    /// </summary>
    public static void ResetDebugSettings()
    {
        mAnyDebugLogToFileEnabled = null;
        SqlHelperClass.ResetDebugSettings();
        AnalyticsHelper.ResetDebugSettings();
        RequestHelper.ResetDebugSettings();
        WebSyncHelperClass.ResetDebugSettings();
        DebugHelper.ResetDebugSettings();
        File.ResetDebugSettings();
        SecurityHelper.ResetDebugSettings();
        MacroResolver.ResetDebugSettings();
        CacheHelper.ResetDebugSettings();
        OutputHelper.ResetDebugSettings();
        CMSControlsHelper.ResetDebugSettings();
        CMSContext.ResetDebugSettings();
    }


    /// <summary>
    /// Initializes the web farm environment
    /// </summary>
    public static void InitWebFarm()
    {
        // Delete memory synchronization tasks
        WebSyncHelper.DeleteMemorySynchronizationTasks();

        string serverDisplayName = string.Empty;
        string serverUrl = string.Empty;
        bool createServer = false;

        // Create web farm server if running on Azure
        if (AzureHelper.IsRunningOnAzure)
        {
            // Set web farm server name
            WebSyncHelperClass.ServerName = ValidationHelper.GetCodeName(AzureHelper.CurrentInstanceID);
            serverDisplayName = AzureHelper.CurrentInstanceID;
            serverUrl = AzureHelper.CurrentInternalEndpoint;

            createServer = true;
        }
        // Get dynamic name for server name  
        else if (WebSyncHelperClass.GenerateWebFarmServers)
        {
            WebSyncHelperClass.WebFarmEnabled = true;
            serverUrl = HTTPHelper.UserHostAddress + SettingsKeyProvider.ApplicationPath.TrimEnd('/') + WebSyncHelperClass.WebFarmUpdaterPage;
            createServer = true;

            if (string.IsNullOrEmpty(WebSyncHelperClass.ServerName))
            {
                serverDisplayName = WebFarmServerInfoProvider.GetAutomaticServerName();
                WebSyncHelperClass.ServerName = ValidationHelper.GetCodeName(serverDisplayName);            
            }
            else
            {
                serverDisplayName = WebSyncHelperClass.ServerName;
            }            
        }        

        // Create new web farm server if is needed
        if (createServer)
        {
            string serverName = WebSyncHelperClass.ServerName;

            // Create web farm server
            WebFarmServerInfo wfsi = WebFarmServerInfoProvider.GetWebFarmServerInfo(serverName);
            if (wfsi == null)
            {
                wfsi = new WebFarmServerInfo();
            }

            wfsi.ServerName = serverName;
            wfsi.ServerEnabled = true;
            wfsi.ServerDisplayName = serverDisplayName;
            wfsi.ServerURL = serverUrl;

            WebFarmServerInfoProvider.SetWebFarmServerInfo(wfsi);
        }
    }
    

    /// <summary>
    /// PreInitializes the system.
    /// </summary>
    public static void PreInit()
    {
        CMSModuleLoader.PreInitializeModules();
    }


    /// <summary>
    /// Initializes the system.
    /// </summary>
    public static void Init()
    {
        // Init CMS environment
        CMSContext.Init(mAsyncInit);

        // Register the events
        RegisterEvents();

        // Register module methods and transformation methods to macro resolver
        CMSModuleLoader.InitializeModules();

        MacroMethods.RegisterMethods();
        CMSMacroMethods.RegisterMethods();
        TransformationMacroMethods.RegisterMethods();

        Functions.RegisterUniGridTransformations();
    }


    /// <summary>
    /// Waits until the initialization is completed.
    /// </summary>
    public static void WaitForInitialization()
    {
        if (mAsyncInit)
        {
            // Wait until all modules are ready
            while (!CMSContext.ModulesReady)
            {
                Thread.Sleep(20);
            }
        }
    }


    /// <summary>
    /// Registers the events.
    /// </summary>
    private static void RegisterEvents()
    {
        // Register watchers
        try
        {
            RegisterWatchers();
        }
        catch
        {
            // This code causes issues in medium trust
            EventLogProvider ep = new EventLogProvider();
            ep.LogEvent(EventLogProvider.EVENT_TYPE_WARNING, DateTime.Now, "WebFarm", "FullTrustRequest", 0, null, 0, null, null, "Web farm notify watcher wasn't initialized. It is available only under full trust level.", 0, null);
        }
    }


    /// <summary>
    /// Initializes file system watchers
    /// </summary>
    private static void RegisterWatchers()
    {
        // Init file system watchers
        if (!AzureHelper.IsRunningOnAzure)
        {
            if (!SystemHelper.IsFullTrustLevel)
            {
                EventLogProvider ep = new EventLogProvider();
                ep.LogEvent(EventLogProvider.EVENT_TYPE_WARNING, DateTime.Now, "WebFarm", "FullTrustRequest", 0, null, 0, null, null, "Web farm notify watcher wasn't initialized. It is available only under full trust level.", 0, null);
            }
            else
            {
                WebSyncHelper.NotifyWatcher.Changed += NotifyWatcher_Changed;
            }
        }
    }


    /// <summary>
    /// Clears the system cache.
    /// </summary>
    public static void ClearCache()
    {
        CacheHelper.ClearCache(null, true);

        // Collect the memory
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }


    /// <summary>
    /// Handles changed event of file system watcher.
    /// </summary>
    /// <param name="sender">File system watcher</param>
    /// <param name="e">File system event argument</param>
    private static void NotifyWatcher_Changed(object sender, IOExceptions.FileSystemEventArgs e)
    {
        try
        {
            // Temporarily disable raising events because event OnChange is called twice when file is changed
            WebSyncHelper.NotifyWatcher.EnableRaisingEvents = false;

            // Process web farm tasks
            WebSyncHelper.ProcessMyTasks();
        }
        catch (Exception ex)
        {
            // Log exception
            EventLogProvider.LogException("FileSystemWatcher", "Changed", ex);
        }
        finally
        {
            // Enable raising events
            WebSyncHelper.NotifyWatcher.EnableRaisingEvents = true;
        }
    }

    #endregion


    #region "Storage methods"

    /// <summary>
    /// Initializes the project file system
    /// </summary>
    public static void InitStorage()
    {
        // Init GetCachedSection event for access to the cache
        SystemMethods.InitGetCachedSection(CacheHelper.GetCachedSection);

        MapZippedImages();
        MapZippedFolders();
        RegisterVirtualFolders();
    }


    /// <summary>
    /// Registers the folders handled by virtual path provider
    /// </summary>
    private static void RegisterVirtualFolders()
    {
        VirtualPathHelper.RegisterVirtualPath("~/CMSWebParts/", false);
        VirtualPathHelper.RegisterVirtualPath("~/CMSFormControls/", false);
        VirtualPathHelper.RegisterVirtualPath("~/CMSPages/", false);
    }


    /// <summary>
    /// Maps the zipped images folder
    /// </summary>
    private static void MapZippedImages()
    {
        // Register mapped paths
        if (StorageHelper.UseZippedResources)
        {
            Path.RegisterMappedZippedFolder("~/App_Themes/Default/Images", "Images.zip");
        }
        else if (SettingsKeyProvider.DevelopmentMode)
        {
            // Map files to the zipped images folder in development mode
            Path.RegisterMappedPath("~/App_Themes/Default/Images", "~/App_Themes/Default/ZippedImages");
        }
    }


    /// <summary>
    /// Maps the automatic zipped folders
    /// </summary>
    private static void MapZippedFolders()
    {
        // Map all zipped content
        string zippedPath = StorageHelper.GetFullFilePhysicalPath(ZIPPED_FILES_FOLDER, null);
        if (Directory.Exists(zippedPath))
        {
            DirectoryInfo dir = DirectoryInfo.New(zippedPath);
            string subFolder = null;

            // Register zipped files
            RegisterZippedFiles(dir, subFolder);
        }
    }


    /// <summary>
    /// Registers zipped folders based on the 
    /// </summary>
    /// <param name="dir">Directory info</param>
    /// <param name="subFolder">Current subfolder</param>
    private static void RegisterZippedFiles(DirectoryInfo dir, string subFolder)
    {
        // Register zipped files
        FileInfo[] files = dir.GetFiles("*.zip");

        foreach (FileInfo zippedFile in files)
        {
            // Map the corresponding folder to the zip file
            string fileName = zippedFile.Name;
            string folderName = Path.GetFileNameWithoutExtension(fileName);

            string sourcePath = "~/" + subFolder + folderName;
            string targetPath = null;

            if (StorageHelper.UseZippedResources)
            {
                targetPath = ZIPPED_FILES_FOLDER + "/" + subFolder + ZipStorageProvider.GetZipFileName(fileName);
            }
            else
            {
                targetPath = ZIPPED_FILES_FOLDER + "/" + subFolder + folderName;
            }

            Path.RegisterMappedPath(sourcePath, targetPath);
        }

        // Register zipped subfolders
        DirectoryInfo[] subDirs = dir.GetDirectories();

        foreach (DirectoryInfo subDir in subDirs)
        {
            subFolder += subDir.Name + "/";

            RegisterZippedFiles(subDir, subFolder);
        }
    }

    #endregion
}