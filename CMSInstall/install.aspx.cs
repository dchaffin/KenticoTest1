using System;
using System.Collections;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.DatabaseHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.VirtualPathHelper;

using ProcessStatus = CMS.CMSImportExport.ProcessStatus;


#region "InstallInfo"

/// <summary>
/// Installation info.
/// </summary>
[Serializable]
public class InstallInfo
{
    #region "Variables"

    public const string SEPARATOR = "<#>";

    private const string LOG = "I" + SEPARATOR + SEPARATOR + SEPARATOR;

    // Deletion log
    private string mInstallLog = LOG;
    private string mScriptsFullPath = null;
    private string mConnectionString = null;
    private string mDBSchema = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Keep information about installation progress.
    /// </summary>
    public string InstallLog
    {
        get
        {
            return mInstallLog;
        }
        set
        {
            mInstallLog = value;
        }
    }


    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            return mConnectionString;
        }

        set
        {
            mConnectionString = value;
        }
    }


    /// <summary>
    /// Scripts full path.
    /// </summary>
    public string ScriptsFullPath
    {
        get
        {
            return mScriptsFullPath;
        }

        set
        {
            mScriptsFullPath = value;
        }
    }


    /// <summary>
    /// Database schema.
    /// </summary>
    public string DBSchema
    {
        get
        {
            return mDBSchema;
        }
        set
        {
            mDBSchema = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Constructor.
    /// </summary>
    public InstallInfo()
    {
    }


    /// <summary>
    /// Clear log.
    /// </summary>
    public void ClearLog()
    {
        InstallLog = LOG;
    }


    /// <summary>
    /// Gets limited progress log for callback.
    /// </summary>
    /// <param name="reqMessageLength">Requested message part length</param>
    /// <param name="reqErrorLength">Requested error part length</param>
    /// <param name="reqWarningLength">Requested warning part length</param>
    public string GetLimitedProgressLog(int reqMessageLength, int reqErrorLength, int reqWarningLength)
    {
        if (mInstallLog != null)
        {
            string[] parts = mInstallLog.Split(new string[] { SEPARATOR }, StringSplitOptions.None);

            if (parts.Length != 4)
            {
                return "F" + SEPARATOR + "Wrong internal log." + SEPARATOR + SEPARATOR;
            }

            string message = parts[1];
            string error = parts[2];
            string warning = parts[3];

            // Message part
            int messageLength = message.Length;
            if (reqMessageLength > messageLength)
            {
                reqMessageLength = messageLength;
            }

            // Error part
            int errorLength = error.Length;
            if (reqErrorLength > errorLength)
            {
                reqErrorLength = errorLength;
            }

            // Warning part
            int warningLength = warning.Length;
            if (reqWarningLength > warningLength)
            {
                reqWarningLength = warningLength;
            }

            return parts[0] + SEPARATOR + message.Substring(0, messageLength - reqMessageLength) + SEPARATOR + parts[2].Substring(0, errorLength - reqErrorLength) + SEPARATOR + parts[3].Substring(0, warningLength - reqWarningLength);
        }
        return "F" + SEPARATOR + "Internal error." + SEPARATOR + SEPARATOR;
    }

    #endregion
}

#endregion


public partial class CMSInstall_install : CMSPage, ICallbackEventHandler
{
    #region "Constants"

    private const string WWAG_KEY = "CMSWWAGInstallation";
    private const string COLLATION_CASE_INSENSITIVE = "SQL_Latin1_General_CP1_CI_AS";

    #endregion


    #region "Variables"

    private static Hashtable mInstallInfos = new Hashtable();
    private static Hashtable mManagers = new Hashtable();
    private string hostName = URLHelper.Url.Host.ToLowerCSafe();
    private static bool dbReady = false;
    private static bool writePermissions = true;
    private UserInfo mImportUser = null;
    private LocalizedButton mNextButton;
    private LocalizedButton mPreviousButton;
    private LocalizedButton mStartNextButton;

    #endregion


    #region "Properties"

    /// <summary>
    /// User for actions context
    /// </summary>
    protected UserInfo ImportUser
    {
        get
        {
            if (mImportUser == null)
            {
                mImportUser = UserInfoProvider.GetUserInfo("administrator");
                CMSActionContext.CurrentUser = mImportUser;
            }

            return mImportUser;
        }
    }


    /// <summary>
    /// Database is created.
    /// </summary>
    private bool DBCreated
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DBCreated"], false);
        }

        set
        {
            ViewState["DBCreated"] = value;
        }
    }


    /// <summary>
    /// Database is installed.
    /// </summary>
    private bool DBInstalled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DBInstalled"], false);
        }

        set
        {
            ViewState["DBInstalled"] = value;
        }
    }


    /// <summary>
    /// Process GUID.
    /// </summary>
    public Guid ProcessGUID
    {
        get
        {
            if (ViewState["ProcessGUID"] == null)
            {
                ViewState["ProcessGUID"] = Guid.NewGuid();
            }

            return ValidationHelper.GetGuid(ViewState["ProcessGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Install info.
    /// </summary>
    public InstallInfo InstallInfo
    {
        get
        {
            string key = "instInfos_" + ProcessGUID;
            if (mInstallInfos[key] == null)
            {
                InstallInfo instInfo = new InstallInfo();
                mInstallInfos[key] = instInfo;
            }
            return (InstallInfo)mInstallInfos[key];
        }
        set
        {
            string key = "instInfos_" + ProcessGUID;
            mInstallInfos[key] = value;
        }
    }


    /// <summary>
    /// Authentication type.
    /// </summary>
    public SQLServerAuthenticationModeEnum authenticationType
    {
        get
        {
            if (ViewState["authentication"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Connection information was lost!");
                }
            }
            return (SQLServerAuthenticationModeEnum)ViewState["authentication"];
        }
        set
        {
            ViewState["authentication"] = value;
        }
    }


    /// <summary>
    /// Database name.
    /// </summary>
    public string Database
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Database"], "");
        }
        set
        {
            ViewState["Database"] = value;
        }
    }


    /// <summary>
    /// Import manager.
    /// </summary>
    public ImportManager ImportManager
    {
        get
        {
            string key = "imManagers_" + ProcessGUID;
            if (mManagers[key] == null)
            {
                SiteImportSettings imSettings = new SiteImportSettings(ImportUser);
                imSettings.IsWebTemplate = true;
                imSettings.ImportType = ImportTypeEnum.All;
                imSettings.CopyFiles = false;
                imSettings.EnableSearchTasks = false;
                ImportManager im = new ImportManager(imSettings);
                mManagers[key] = im;
            }
            return (ImportManager)mManagers[key];
        }
        set
        {
            string key = "imManagers_" + ProcessGUID;
            mManagers[key] = value;
        }
    }


    /// <summary>
    /// New site domain.
    /// </summary>
    public string Domain
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Domain"], "");
        }

        set
        {
            ViewState["Domain"] = value;
        }
    }


    /// <summary>
    /// New site site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteName"], "");
        }

        set
        {
            ViewState["SiteName"] = value;
        }
    }


    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            if (ViewState["connString"] == null)
            {
                ViewState["connString"] = "";
            }
            return (string)ViewState["connString"];
        }

        set
        {
            ViewState["connString"] = value;
        }
    }


    /// <summary>
    /// Step index.
    /// </summary>
    public int StepIndex
    {
        get
        {
            if (ViewState["stepIndex"] == null)
            {
                ViewState["stepIndex"] = 1;
            }
            return (int)ViewState["stepIndex"];
        }

        set
        {
            ViewState["stepIndex"] = value;
        }
    }


    private string mResult
    {
        get
        {
            if (ViewState["result"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Information was lost!");
                }
            }
            return (string)ViewState["result"];
        }
        set
        {
            ViewState["result"] = value;
        }
    }


    private bool mDisplayLog
    {
        get
        {
            if (ViewState["displLog"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Information was lost!");
                }
                return false;
            }
            return (bool)ViewState["displLog"];
        }
        set
        {
            ViewState["displLog"] = value;
        }
    }


    /// <summary>
    /// Flag - indicate whether DB objects will be created.
    /// </summary>
    private bool CreateDBObjects
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CreateDBObjects"], true);
        }
        set
        {
            ViewState["CreateDBObjects"] = value;
        }
    }


    protected CMSAdminControls_UI_PageElements_Help StartHelp
    {
        get
        {
            return (CMSAdminControls_UI_PageElements_Help)wzdInstaller.Controls[0].Controls[2].Controls[0].Controls[0].Controls[1].FindControl("hlpContext");
        }
    }


    protected CMSAdminControls_UI_PageElements_Help Help
    {
        get
        {
            return (CMSAdminControls_UI_PageElements_Help)wzdInstaller.Controls[0].Controls[2].Controls[0].Controls[2].Controls[1].FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Previous step index.
    /// </summary>
    private int PreviousStep
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PreviousStep"], 0);
        }
        set
        {
            ViewState["PreviousStep"] = value;
        }
    }

    /// <summary>
    /// Current step index.
    /// </summary>
    private int ActualStep
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ActualStep"], 0);
        }
        set
        {
            ViewState["ActualStep"] = value;
        }
    }


    private int StepOperation
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["StepOperation"], 0);
        }
        set
        {
            ViewState["StepOperation"] = value;
        }
    }


    /// <summary>
    ///  User password.
    /// </summary>
    private string Password
    {
        get
        {
            return Convert.ToString(ViewState["install.password"]);
        }
        set
        {
            ViewState["install.password"] = value;
        }
    }

    #endregion


    #region "Step wizard buttons"

    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get
        {
            return mPreviousButton ?? (mPreviousButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepPrevButton") as LocalizedButton);
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            return mNextButton ?? (mNextButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepNextButton") as LocalizedButton);
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton StartNextButton
    {
        get
        {
            return mStartNextButton ?? (mStartNextButton = wzdInstaller.FindControl("StartNavigationTemplateContainerID").FindControl("startStepNavigation").FindControl("StepNextButton") as LocalizedButton);
        }
    }

    #endregion


    protected void Page_Load(Object sender, EventArgs e)
    {
        // Disable CSS minification
        CSSHelper.MinifyCurrentRequest = false;
        ScriptHelper.MinifyCurrentRequestScripts = false;

        // Register script for pendingCallbacks repair
        // Cannot use ScriptHelper.FixPendingCallbacks as during installation the DB is not available
        ScriptManager.RegisterClientScriptInclude(this, GetType(), "cms.js", URLHelper.ResolveUrl("~/CMSScripts/cms.js"));
        ScriptManager.RegisterClientScriptBlock(this, GetType(), "fixPendingCallbacks", "WebForm_CallbackComplete = WebForm_CallbackComplete_SyncFixed", true);

        SetBrowserClass(false, false);

        if (!RequestHelper.IsCallback())
        {
            ucAsyncControl.OnFinished += worker_OnFinished;
            ucDBAsyncControl.OnFinished += workerDB_OnFinished;
            databaseDialog.ServerName = userServer.ServerName;

            // Javascript functions
            string jsFunctions =
                "var iMessageText = '';\n" +
                "var iErrorText = '';\n" +
                "var iWarningText = '';\n" +
                "var getBusy = false; \n" +
                "function GetInstallState(argument)\n" +
                "{ if (getBusy) return; getBusy = true; setTimeout(\"getBusy = false;\", 2000); if(window.Activity){window.Activity();} var arg = argument + ';' + iMessageText.length + ';' + iErrorText.length + ';' + iWarningText.length; return " + Page.ClientScript.GetCallbackEventReference(this, "arg", "SetInstallStateMssg", "arg", true) + " } \n";

            jsFunctions +=
                "function SetInstallStateMssg(rValue, context)\n" +
                "{\n" +
                "   getBusy = false; \n" +
                "   if (rValue != '')\n" +
                "   {\n" +
                "       var args = context.split(';');\n" +
                "       var values = rValue.split('" + AbstractImportExportSettings.SEPARATOR + "');\n" +
                "       var messageElement = document.getElementById('lblProgress');\n" +
                "       var errorElement = document.getElementById('" + errorPanel.ErrorLabelClientID + "');\n" +
                "       var warningElement = document.getElementById('" + warningPanel.WarningLabelClientID + "');\n" +
                "       var messageText = iMessageText;\n" +
                "       messageText = values[1] + messageText.substring(messageText.length - args[2]);\n" +
                "       if(messageText.length > iMessageText.length){ iMessageText = messageElement.innerHTML = messageText; }\n" +
                "       var errorText = iErrorText;\n" +
                "       errorText = values[2] + errorText.substring(errorText.length - args[3]);\n" +
                "       if(errorText.length > iErrorText.length){ iErrorText = errorElement.innerHTML = errorText; }\n" +
                "       var warningText = iWarningText;\n" +
                "       warningText = values[3] + warningText.substring(warningText.length - args[4]);\n" +
                "       if(warningText.length > iWarningText.length){ iWarningText = warningElement.innerHTML = warningText; }\n" +
                "       if((values == '') || (values[0] == 'F'))\n" +
                "       {\n" +
                "           StopInstallStateTimer();\n" +
                "           if(values[2] != '')\n" +
                "           {\n" +
                "               BTN_Disable('" + NextButton.ClientID + "');\n" +
                "               BTN_Enable('" + PreviousButton.ClientID + "');\n" +
                "           }\n" +
                "           else\n" +
                "           {\n" +
                "               BTN_Disable('" + NextButton.ClientID + "');\n" +
                "               BTN_Disable('" + PreviousButton.ClientID + "');\n" +
                "           }\n" +
                "       }\n" +
                "   }\n" +
                "}\n";

            // JS for advanced options link
            jsFunctions += "function ShowHideElement(elemid, show) { \n" +
                           " var elem = document.getElementById(elemid); \n" +
                           " if (elem) { \n" +
                           "   if (show=='1') { elem.style.display = ''; } else { elem.style.display = 'none'; } \n" +
                           " } \n" +
                           " } \n" +
                           " function AdvancedOptions(state) { \n" +
                           "   var elem = document.getElementById('" + hdnAdvanced.ClientID + "'); \n" +
                           "   if (elem) { \n " +
                           "      if (state=='1' || state=='?' && (elem.value == '' || elem.value == '0')) { elem.value = '1'; } else { elem.value = '0'; } \n" +
                           "       ShowHideElement('" + databaseDialog.SchemaClientID + "', elem.value); ShowHideElement('" + databaseDialog.SchemaLabelClientID + "', elem.value); \n" +
                           "       var label = document.getElementById('" + databaseDialog.AdvancedLabelClientID + "'); \n" +
                           "       if (label) { " +
                           "         if (elem.value == '1') { label.innerHTML = " + ScriptHelper.GetString(ResHelper.GetFileString("install.HideAdvancedOptions")) + "; } \n" +
                           "         else { label.innerHTML = " + ScriptHelper.GetString(ResHelper.GetFileString("install.ShowAdvancedOptions")) + "; } \n" +
                           "       } \n" +
                           "   } \n " +
                           " } \n"
                ;


            // Register the script to perform get flags for showing buttons retrieval callback
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InstallFunctions", ScriptHelper.GetScript(jsFunctions));

            StartHelp.Tooltip = ResHelper.GetFileString("install.tooltip");
            StartHelp.TopicName = "DBInstall_Step1";
            StartHelp.IconUrl = GetImageUrl("Others/LogonForm/HelpButton.png"); Response.Cache.SetNoStore();
            Help.Tooltip = ResHelper.GetFileString("install.tooltip");
            Help.IconUrl = GetImageUrl("Others/LogonForm/HelpButton.png");

            btnPermissionTest.Click += btnPermissionTest_Click;
            btnPermissionSkip.Click += btnPermissionSkip_Click;
            btnPermissionContinue.Click += btnPermissionContinue_Click;

            // If the connection string is set, redirect
            if (!RequestHelper.IsPostBack())
            {
                if (CMSAppBase.ConnectionAvailable)
                {
                    URLHelper.Redirect("~/default.aspx");
                }

                bool checkPermission = QueryHelper.GetBoolean("checkpermission", true);
                bool testAgain = QueryHelper.GetBoolean("testagain", false);

                string dir = HttpContext.Current.Server.MapPath("~/");

                // Do not test write permissions in WWAG mode
                if (!ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
                {
                    if (!DirectoryHelper.CheckPermissions(dir) && checkPermission)
                    {
                        writePermissions = false;
                        pnlWizard.Visible = false;
                        imgHeader.Visible = false;
                        pnlPermission.Visible = true;
                        pnlButtons.Visible = true;

                        lblPermission.Text = string.Format(ResHelper.GetFileString("Install.lblPermission"), WindowsIdentity.GetCurrent().Name, dir);
                        btnPermissionSkip.Text = ResHelper.GetFileString("Install.btnPermissionSkip");
                        btnPermissionTest.Text = ResHelper.GetFileString("Install.btnPermissionTest");

                        // Show troubleshoot link
                        errorPanel.DisplayError("Install.ErrorPermissions", "DiskPermissions");
                        return;
                    }

                    if (testAgain)
                    {
                        pnlWizard.Visible = false;
                        imgHeader.Visible = false;
                        pnlPermission.Visible = false;
                        pnlButtons.Visible = false;
                        pnlPermissionSuccess.Visible = true;
                        lblPermissionSuccess.Text = ResHelper.GetFileString("Install.lblPermissionSuccess");
                        btnPermissionContinue.Text = ResHelper.GetFileString("Install.btnPermissionContinue");
                        writePermissions = true;
                        return;
                    }
                }
            }

            pnlWizard.Visible = true;
            pnlPermission.Visible = false;
            pnlButtons.Visible = false;

            if (!RequestHelper.IsPostBack())
            {
                if ((HttpContext.Current != null) && !ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSWWAGInstallation"], false))
                {
                    userServer.ServerName = HTTPHelper.MachineName;
                }
                authenticationType = SQLServerAuthenticationModeEnum.SQLServerAuthentication;

                wzdInstaller.ActiveStepIndex = 0;
            }
            else
            {
                if (Password == null)
                {
                    Password = userServer.DBPassword;
                }
            }

            // Load the strings
            mDisplayLog = false;


            ltlAdvanced.Text = ScriptHelper.GetScript(" AdvancedOptions('" + (hdnAdvanced.Value == "1" ? "1" : "0") + "'); ");

            lblCompleted.Text = ResHelper.GetFileString("Install.DBSetupOK");
            lblMediumTrustInfo.Text = ResHelper.GetFileString("Install.MediumTrustInfo");

            ltlScript.Text = ScriptHelper.GetScript(
                "function NextStep(btnNext,elementDiv)\n" +
                "{\n" +
                "   btnNext.disabled=true;\n" +
                "   try{BTN_Disable('" + PreviousButton.ClientID + "');}catch(err){}\n" +
                ClientScript.GetPostBackEventReference(btnHiddenNext, null) +
                "}\n" +
                "function PrevStep(btnPrev,elementDiv)\n" +
                "{" +
                "   btnPrev.disabled=true;\n" +
                "   try{BTN_Disable('" + NextButton.ClientID + "');}catch(err){}\n" +
                ClientScript.GetPostBackEventReference(btnHiddenBack, null) +
                "}\n"
                );
            mResult = "";

            // Sets connection string panel
            lblConnectionString.Text = ResHelper.GetFileString("Install.lblConnectionString");
            wzdInstaller.StartNextButtonText = ResHelper.GetFileString("general.next") + " >";
            wzdInstaller.FinishCompleteButtonText = ResHelper.GetFileString("Install.Finish");
            wzdInstaller.FinishPreviousButtonText = ResHelper.GetFileString("Install.BackStep");
            wzdInstaller.StepNextButtonText = ResHelper.GetFileString("general.next") + " >";
            wzdInstaller.StepPreviousButtonText = ResHelper.GetFileString("Install.BackStep");

            // Show WWAG dialog instead of license dialog (if running in WWAG mode)
            if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
            {
                ucLicenseDialog.Visible = false;
                ucWagDialog.Visible = true;
            }
        }

        // Set the active step as 1 if connection string already initialized
        if (!RequestHelper.IsPostBack() && SqlHelperClass.IsConnectionStringInitialized)
        {
            wzdInstaller.ActiveStepIndex = 1;
            databaseDialog.UseExistingChecked = true;
        }

        NextButton.Attributes.Remove("disabled");
        PreviousButton.Attributes.Remove("disabled");

        wzdInstaller.ActiveStepChanged += wzdInstaller_ActiveStepChanged;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (dbReady || CMSAppBase.ConnectionAvailable)
        {
            ucSiteCreationDialog.StopProcessing = false;
            ucSiteCreationDialog.ReloadData();
        }

        // Display the log if result filled
        if (mDisplayLog)
        {
            logPanel.DisplayLog(mResult);
        }

        InitializeHeader(wzdInstaller.ActiveStepIndex);
        EnsureDefaultButton();

        if (!RequestHelper.IsPostBack())
        {
            ltlInstallScript.Text += ScriptHelper.GetScript("var imgSelected = new Image(); imgSelected.src = '" + GetImageUrl("/Others/Install/SiteSelector.png") + "';");
        }

        PreviousButton.Visible = !SqlHelperClass.IsConnectionStringInitialized && (wzdInstaller.ActiveStepIndex != 0) && (wzdInstaller.ActiveStepIndex != 4) ||
            (wzdInstaller.ActiveStepIndex == 6);
    }


    private void wzdInstaller_ActiveStepChanged(object sender, EventArgs e)
    {
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 1:
                break;
            // Finish step
            case 7:
                // Set current user default culture of the site
                CMSContext.PreferredCultureCode = SettingsKeyProvider.GetStringValue(SiteName + ".CMSDefaultCultureCode");

                // Initialize virtual path provider
                VirtualPathHelper.Init(this);

                // Check medium trust
                if (!SettingsKeyProvider.UsingVirtualPathProvider)
                {
                    btnWebSite.Text = ResHelper.GetFileString("Install.lnkMediumTrust");
                    lblMediumTrustInfo.Visible = true;
                }
                else
                {
                    btnWebSite.Text = ResHelper.GetFileString("Install.lnkWebsite");
                }
                break;
        }
    }


    private void btnPermissionContinue_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(URLHelper.Url.GetLeftPart(UriPartial.Path));
    }


    private void btnPermissionSkip_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(URLHelper.Url.GetLeftPart(UriPartial.Path) + "?checkpermission=0");
    }


    private void btnPermissionTest_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(URLHelper.Url.GetLeftPart(UriPartial.Path) + "?testagain=1");
    }


    protected void btnWebSite_onClick(object sender, EventArgs e)
    {
        if (!SettingsKeyProvider.UsingVirtualPathProvider)
        {
            CMSContext.AuthenticateUser("administrator", false);
            URLHelper.Redirect("~/cmssitemanager/default.aspx?section=administration");
        }
        else
        {
            URLHelper.Redirect(ResolveUrl("~/default.aspx"));
        }
    }


    protected void btnHiddenBack_onClick(object sender, EventArgs e)
    {
        StepOperation = -1;
        if ((wzdInstaller.ActiveStepIndex == 8) || (wzdInstaller.ActiveStepIndex == 3))
        {
            StepIndex = 2;
            wzdInstaller.ActiveStepIndex = 1;
        }
        else
        {
            StepIndex--;
            wzdInstaller.ActiveStepIndex--;
        }
    }


    protected void btnHiddenNext_onClick(object sender, EventArgs e)
    {
        StepOperation = 1;
        StepIndex++;

        switch (wzdInstaller.ActiveStepIndex)
        {
            case 0:
                Password = userServer.DBPassword;

                // Set the authentication type
                authenticationType = userServer.WindowsAuthenticationChecked ? SQLServerAuthenticationModeEnum.WindowsAuthentication : SQLServerAuthenticationModeEnum.SQLServerAuthentication;

                // Check the server name
                if (userServer.ServerName == "")
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorServerEmpty"));
                    return;
                }
                // Check if it is possible to connect to the database
                string res = ConnectionHelper.TestConnection(authenticationType, userServer.ServerName, "", userServer.DBUsername, Password);
                if (!string.IsNullOrEmpty(res))
                {
                    HandleError(res, "Install.ErrorSqlTroubleshoot", "SQLError");
                    return;
                }
                else
                {
                    wzdInstaller.ActiveStepIndex = 1;
                }
                databaseDialog.AuthenticationType = authenticationType;
                databaseDialog.Password = Password;
                databaseDialog.Username = userServer.DBUsername;
                databaseDialog.ServerName = userServer.ServerName;
                break;

            case 1:
            case 8:
                // Get database name
                Database = TextHelper.LimitLength((databaseDialog.CreateNewChecked ? databaseDialog.NewDatabaseName : databaseDialog.ExistingDatabaseName), 100);

                // Set up the connection string
                if (SqlHelperClass.IsConnectionStringInitialized)
                {
                    ConnectionString = SqlHelperClass.ConnectionString;
                }
                else
                {
                    ConnectionString = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, 240, false);
                }

                if (Database == "")
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorDBNameEmpty"));
                    return;
                }

                // Check if existing DB has the same version as currently installed CMS
                if (databaseDialog.UseExistingChecked && !databaseDialog.CreateDatabaseObjects)
                {
                    string dbVersion = null;
                    try
                    {
                        dbVersion = CMSDatabaseHelper.GetDatabaseVersion(ConnectionString);
                    }
                    catch
                    {
                    }

                    if (String.IsNullOrEmpty(dbVersion))
                    {
                        // Unable to get DB version => DB objects missing
                        HandleError(ResHelper.GetFileString("Install.DBObjectsMissing"));
                        return;
                    }

                    if (dbVersion != CMSContext.SYSTEM_VERSION)
                    {
                        // Get wrong version number
                        HandleError(ResHelper.GetFileString("Install.WrongDBVersion"));
                        return;
                    }
                }

                string dbCollation = null;
                string dbSchema = null;
                if (hdnAdvanced.Value == "1")
                {
                    dbSchema = databaseDialog.SchemaText;
                }

                InstallInfo.DBSchema = dbSchema;

                // Use existing database
                if (databaseDialog.UseExistingChecked)
                {
                    // Check if DB exists
                    if (!ConnectionHelper.DatabaseExists(ConnectionString))
                    {
                        HandleError(string.Format(ResHelper.GetFileString("Install.ErrorDatabseDoesntExist"), Database));
                        return;
                    }

                    // Set default salt for so the default data being imported are signed correctly
                    if (string.IsNullOrEmpty(ValidationHelper.GetDefaultHashStringSalt()))
                    {
                        ValidationHelper.HashStringSalt = ConnectionString;
                    }

                    // Check if DB schema exists
                    if (!CMSDatabaseHelper.CheckIfSchemaExist(ConnectionString, dbSchema))
                    {
                        HandleError(string.Format(ResHelper.GetFileString("Install.ErrorDatabseSchemaDoesnotExist"), dbSchema, CMSDatabaseHelper.GetCurrentDefaultSchema(ConnectionString)));
                        return;
                    }

                    // Check if DB is in correct version

                    // Get collation of existing DB
                    string collation = ConnectionHelper.GetDatabaseCollation(ConnectionString);
                    dbCollation = collation;
                    ConnectionHelper.DatabaseCollation = collation;

                    if (wzdInstaller.ActiveStepIndex != 8)
                    {
                        // Check target database collation (ask the user if it is not fully supported)
                        if (CMSString.Compare(dbCollation, COLLATION_CASE_INSENSITIVE, true) != 0)
                        {
                            lblCollation.Text = ResHelper.GetFileString("install.databasecollation");
                            rbLeaveCollation.Text = string.Format(ResHelper.GetFileString("install.leavecollation"), collation);
                            rbChangeCollationCI.Text = string.Format(ResHelper.GetFileString("install.changecollation"), COLLATION_CASE_INSENSITIVE);
                            wzdInstaller.ActiveStepIndex = 8;
                            return;
                        }
                    }
                    else
                    {
                        // Change database collation
                        if (!rbLeaveCollation.Checked)
                        {
                            if (rbChangeCollationCI.Checked)
                            {
                                ConnectionHelper.ChangeDatabaseCollation(ConnectionString, Database, COLLATION_CASE_INSENSITIVE);
                            }
                        }
                    }
                }
                else
                {
                    // Create a new database
                    if (!CreateDatabase(dbCollation))
                    {
                        HandleError(string.Format(ResHelper.GetFileString("Install.ErrorCreateDB"), databaseDialog.NewDatabaseName));
                        return;
                    }
                    else
                    {
                        databaseDialog.ExistingDatabaseName = databaseDialog.NewDatabaseName;
                        databaseDialog.CreateNewChecked = false;
                        databaseDialog.UseExistingChecked = true;
                    }
                }

                if ((!AzureHelper.IsRunningOnAzure && writePermissions) || SqlHelperClass.IsConnectionStringInitialized)
                {
                    if (databaseDialog.CreateDatabaseObjects)
                    {
                        if (DBInstalled && DBCreated)
                        {
                            ucDBAsyncControl.RaiseFinished(this, EventArgs.Empty);
                        }
                        else
                        {
                            // Run SQL installation
                            RunSQLInstallation(dbSchema);
                        }
                    }
                    else
                    {
                        CreateDBObjects = false;

                        // Set connection string
                        if (SettingsHelper.SetConnectionString(SqlHelper.ConnectionStringName, ConnectionString))
                        {
                            // Set the application connection string
                            SetAppConnectionString();

                            // If this is installation to existing BD and objects are not created
                            // Check if license key for current domain is present
                            LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(hostName);
                            wzdInstaller.ActiveStepIndex = (lki == null) ? 4 : 5;
                            ucLicenseDialog.SetLicenseExpired();
                        }
                        else
                        {
                            string connStringDisplay = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, 240, true);
                            wzdInstaller.ActiveStepIndex = 2;
                            string message = ResHelper.GetFileString("Install.ConnectionStringError") + " <br/><br/><strong>&lt;add name=\"CMSConnectionString\" connectionString=\"" + connStringDisplay + "\"/&gt;</strong><br/><br/>";
                            lblErrorConnMessage.Text = message;

                            // Show troubleshoot link
                            errorPanel.DisplayError("Install.ErrorPermissions", "DiskPermissions");
                        }
                    }
                }
                else
                {
                    wzdInstaller.ActiveStepIndex = 2;
                    string message = string.Empty;
                    if (AzureHelper.IsRunningOnAzure)
                    {
                        string connStringValue = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, "English", 240, true, true);
                        string connString = "&lt;add name=\"CMSConnectionString\" connectionString=\"" + connStringValue + "\"/&gt;";
                        string appSetting = "&lt;Setting name=\"CMSConnectionString\" value=\"" + connStringValue + "\"/&gt;";
                        message = string.Format(ResHelper.GetFileString("Install.ConnectionStringAzure"), connString, appSetting);
                    }
                    else
                    {
                        string connString = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, 240, true);
                        message = ResHelper.GetFileString("Install.ConnectionStringError") + " <br/><br/><strong>&lt;add name=\"CMSConnectionString\" connectionString=\"" + connString + "\"/&gt;</strong><br/><br/>";

                        // Show troubleshoot link
                        errorPanel.DisplayError("Install.ErrorPermissions", "DiskPermissions");
                    }

                    lblErrorConnMessage.Text = message;
                }

                break;

            // After DB install
            case 3:
                break;

            // After connection string save error
            case 2:
                //// Restart application to ensure connection string update
                //try
                //{
                //    // Try to restart applicatin by unload app domain
                //    HttpRuntime.UnloadAppDomain();
                //}
                //catch
                //{
                //}

                // If connectionstrings don't match
                if ((SettingsHelper.ConnectionStrings[SqlHelper.ConnectionStringName] == null) ||
                    (SettingsHelper.ConnectionStrings[SqlHelper.ConnectionStringName].ConnectionString == null) ||
                    (SettingsHelper.ConnectionStrings[SqlHelper.ConnectionStringName].ConnectionString.Trim() == "") ||
                    (SettingsHelper.ConnectionStrings[SqlHelper.ConnectionStringName].ConnectionString != ConnectionString))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorAddConnString"));
                    return;
                }
                else
                {
                    if (CreateDBObjects)
                    {
                        if (DBInstalled)
                        {
                            ucDBAsyncControl.RaiseFinished(this, EventArgs.Empty);
                        }
                        else
                        {
                            // Run SQL installation
                            RunSQLInstallation(null);
                        }
                    }
                    else
                    {
                        // If this is installation to existing DB and objects are not created
                        if ((hostName != "localhost") && (hostName != "127.0.0.1"))
                        {
                            wzdInstaller.ActiveStepIndex = 4;
                        }
                        else
                        {
                            wzdInstaller.ActiveStepIndex = 5;
                        }
                    }
                }
                break;

            // After license entering
            case 4:
                try
                {
                    if (ucLicenseDialog.Visible)
                    {
                        ucLicenseDialog.SetLicenseKey();
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                    else
                    {
                        if (ucWagDialog.ProcessRegistration(ConnectionString))
                        {
                            wzdInstaller.ActiveStepIndex = 5;
                        }
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex.Message);
                    return;
                }
                break;

            // Site creation
            case 5:
                switch (ucSiteCreationDialog.CreationType)
                {
                    case CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.Template:
                        {
                            if (ucSiteCreationDialog.TemplateName == "")
                            {
                                HandleError(ResHelper.GetFileString("install.notemplate"));
                                return;
                            }

                            // Settings preparation
                            SiteImportSettings settings = new SiteImportSettings(ImportUser);
                            settings.IsWebTemplate = true;
                            settings.ImportType = ImportTypeEnum.All;
                            settings.CopyFiles = false;
                            settings.EnableSearchTasks = false;

                            if (HttpContext.Current != null)
                            {
                                const string www = "www.";
                                if (hostName.StartsWithCSafe(www))
                                {
                                    hostName = hostName.Remove(0, www.Length);
                                }

                                if (!URLHelper.Url.IsDefaultPort)
                                {
                                    hostName += ":" + URLHelper.Url.Port;
                                }

                                settings.SiteDomain = hostName;
                                Domain = hostName;
                            }

                            // Create site
                            WebTemplateInfo ti = WebTemplateInfoProvider.GetWebTemplateInfo(ucSiteCreationDialog.TemplateName);
                            if (ti == null)
                            {
                                HandleError("[Install]: Template not found.");
                                return;
                            }

                            settings.SiteName = ti.WebTemplateName;
                            settings.SiteDisplayName = ti.WebTemplateDisplayName;

                            if (HttpContext.Current != null)
                            {
                                string path = HttpContext.Current.Server.MapPath(ti.WebTemplateFileName);
                                if (File.Exists(path + "\\template.zip"))
                                {                                    // Template from zip file
                                    path += "\\" + ZipStorageProvider.GetZipFileName("template.zip");
                                    settings.TemporaryFilesPath = path;
                                    settings.SourceFilePath = path;
                                    settings.TemporaryFilesCreated = true;
                                }
                                else
                                {
                                    settings.SourceFilePath = path;
                                }
                                settings.WebsitePath = HttpContext.Current.Server.MapPath("~/");
                            }

                            settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_SITE, true);
                            settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_TEMPORARY_FILES, false);

                            SiteName = settings.SiteName;

                            // Init the Mimetype helper (required for the Import)
                            MimeTypeHelper.LoadMimeTypes();

                            // Import the site asynchronously
                            ImportManager.Settings = settings;

                            ucAsyncControl.RunAsync(ImportManager.Import, WindowsIdentity.GetCurrent());
                            NextButton.Attributes.Add("disabled", "true");
                            PreviousButton.Attributes.Add("disabled", "true");
                            wzdInstaller.ActiveStepIndex = 6;

                            ltlInstallScript.Text = ScriptHelper.GetScript("StartInstallStateTimer('IM');");
                        }
                        break;

                    // Else redirect to the site import
                    case CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.ExistingSite:
                        {
                            CMSContext.AuthenticateUser("administrator", false);
                            URLHelper.Redirect("~/cmssitemanager/default.aspx?section=sites&action=import");
                        }
                        break;

                    // Else redirect to the new site wizard
                    case CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.NewSiteWizard:
                        {
                            CMSContext.AuthenticateUser("administrator", false);
                            URLHelper.Redirect("~/cmssitemanager/default.aspx?section=sites&action=new");
                        }
                        break;
                }
                break;

            default:
                wzdInstaller.ActiveStepIndex++;
                break;
        }
    }


    /// <summary>
    /// Runs SQL installation scripts
    /// </summary>
    /// <param name="dbSchema">Database schema</param>
    private void RunSQLInstallation(string dbSchema)
    {
        // Setup the installation
        InstallInfo.ScriptsFullPath = CMSDatabaseHelper.GetSQLInstallPath();
        InstallInfo.ConnectionString = ConnectionString;

        // Set default salt for so the default data being imported are signed correctly
        if (string.IsNullOrEmpty(ValidationHelper.GetDefaultHashStringSalt()))
        {
            ValidationHelper.HashStringSalt = ConnectionString;
        }

        if (dbSchema != null)
        {
            InstallInfo.DBSchema = dbSchema;
        }

        InstallInfo.ClearLog();

        // Start the installation process
        ucDBAsyncControl.RunAsync(InstallDatabase, WindowsIdentity.GetCurrent());

        NextButton.Attributes.Add("disabled", "true");
        PreviousButton.Attributes.Add("disabled", "true");
        wzdInstaller.ActiveStepIndex = 3;

        ltlInstallScript.Text = ScriptHelper.GetScript("StartInstallStateTimer('DB');");
    }


    private void worker_OnFinished(object sender, EventArgs e)
    {
        DBCreated = true;

        // If the import finished without error
        if ((ImportManager.ImportStatus != ProcessStatus.Error) && (ImportManager.ImportStatus != ProcessStatus.Restarted))
        {
            wzdInstaller.ActiveStepIndex = 7;
        }
        else
        {
            string log = ImportManager.Settings.ProgressLog;
            string[] messages = log.Split(new string[] { InstallInfo.SEPARATOR }, StringSplitOptions.None);
            errorPanel.ErrorLabelText = messages[2];
            siteProgress.ProgressText = messages[1];
            NextButton.Enabled = false;
        }
    }


    private void workerDB_OnFinished(object sender, EventArgs e)
    {
        CreateDBObjects = databaseDialog.CreateDatabaseObjects;

        DBInstalled = true;

        // Try to set connection string into db only if not running on Azure
        bool setConnectionString = !AzureHelper.IsRunningOnAzure && writePermissions;

        // Set connection string
        if (SqlHelperClass.IsConnectionStringInitialized || (setConnectionString && SettingsHelper.SetConnectionString(SqlHelper.ConnectionStringName, ConnectionString)))
        {
            if (SqlHelperClass.IsConnectionStringInitialized)
            {
                CMSAppBase.ReInit();
            }

            // Set the application connection string
            SetAppConnectionString();

            // If this is installation to existing BD and objects are not created
            // Add license keys
            bool licensesAdded = true;

            if (CreateDBObjects && (ucSiteCreationDialog.CreationType != CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.ExistingSite))
            {
                licensesAdded = AddTrialLicenseKeys(ConnectionString);
            }

            if (licensesAdded)
            {
                if ((hostName != "localhost") && (hostName != "127.0.0.1"))
                {
                    // Check if license key for current domain is present
                    LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(hostName);
                    wzdInstaller.ActiveStepIndex = lki == null ? 4 : 5;
                }
                else
                {
                    wzdInstaller.ActiveStepIndex = 5;
                }
            }
            else
            {
                wzdInstaller.ActiveStepIndex = 4;
                ucLicenseDialog.SetLicenseExpired();
            }

            // Request meta file
            try
            {
                WebClient client = new WebClient();
                string url = URLHelper.Url.Scheme + "://" + URLHelper.Url.Host + URLHelper.ApplicationPath.TrimEnd('/') + "/CMSPages/GetMetaFile.aspx";
                client.DownloadData(url);
                client.Dispose();
            }
            catch
            {
            }
        }
        else
        {
            string message = string.Empty;
            if (AzureHelper.IsRunningOnAzure)
            {
                string connStringValue = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, "English", 240, true, true);
                string connString = "&lt;add name=\"CMSConnectionString\" connectionString=\"" + connStringValue + "\"/&gt;";
                string appSetting = "&lt;Setting name=\"CMSConnectionString\" value=\"" + connStringValue + "\"/&gt;";
                message = string.Format(ResHelper.GetFileString("Install.ConnectionStringAzure"), connString, appSetting);
            }
            else
            {
                string connString = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, 240, true);
                message = ResHelper.GetFileString("Install.ConnectionStringError") + " <br/><br/><strong>&lt;add name=\"CMSConnectionString\" connectionString=\"" + connString + "\"/&gt;</strong><br/><br/>";

                // Show troubleshoot link
                errorPanel.DisplayError("Install.ErrorPermissions", "DiskPermissions");
            }

            wzdInstaller.ActiveStepIndex = 2;
            lblErrorConnMessage.Text = message;
        }
    }


    /// <summary>
    /// Sets the application connection string and initializes the application
    /// </summary>
    private void SetAppConnectionString()
    {
        SqlHelperClass.ConnectionString = ConnectionString;
        dbReady = true;

        // Set property indicating that db objects are installed
        SqlHelper.IsDatabaseAvailable = true;

        CMSContext.Init();
    }


    protected void wzdInstaller_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        --StepIndex;
        wzdInstaller.ActiveStepIndex -= 1;
    }


    /// <summary>
    /// Adds trial license keys to DB. No license is added when running in web application gallery mode.
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    private bool AddTrialLicenseKeys(string connectionString)
    {
        // Skip creation of trial licence keys if running in WWAG mode
        if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
        {
            return false;
        }

        string licenseKey = ValidationHelper.GetString(SettingsHelper.AppSettings["CMSTrialKey"], "");
        if (licenseKey != "")
        {
            return CMSDatabaseHelper.AddTrialLicenseKeys(connectionString, licenseKey, true, false);
        }
        else
        {
            errorPanel.ErrorLabelText = ResHelper.GetFileString("Install.ErrorTrialLicense");
        }

        return false;
    }


    /// <summary>
    /// Initialize wizard header
    /// </summary>
    /// <param name="index">Step index</param>
    private void InitializeHeader(int index)
    {
        Help.Visible = true;
        StartHelp.Visible = true;

        string imgName = null;

        lblHeader.Text = ResHelper.GetFileString("Install.Step") + " - ";

        switch (index)
        {
            // SQL server and authentication mode
            case 0:
                imgName = "header_connection.png";
                StartHelp.TopicName = Help.TopicName = "DBInstall_StepConnection";
                lblHeader.Text += ResHelper.GetFileString("Install.Step0");
                break;
            // Database
            case 1:
                imgName = "header_db.png";
                StartHelp.TopicName = Help.TopicName = "DBInstall_StepDB";
                lblHeader.Text += ResHelper.GetFileString("Install.Step1");
                break;
            // web.config permissions
            case 2:
                imgName = "header_db.png";
                StartHelp.Visible = Help.Visible = false;
                lblHeader.Text += ResHelper.GetFileString("Install.Step3");
                break;
            // Database creation log
            case 3:
                imgName = "header_db.png";
                StartHelp.Visible = Help.Visible = false;
                lblHeader.Text += ResHelper.GetFileString("Install.Step2");
                break;
            // License import
            case 4:
                imgName = "header_db.png";
                StartHelp.TopicName = Help.TopicName = "DBInstall_StepLicense";
                lblHeader.Text += ResHelper.GetFileString("Install.Step4");
                break;
            // Starter site selction
            case 5:
                imgName = "header_site.png";
                StartHelp.TopicName = Help.TopicName = "DBInstall_StepSite";
                lblHeader.Text += ResHelper.GetFileString("Install.Step5");
                break;
            // Import log
            case 6:
                imgName = "header_site.png";
                StartHelp.Visible = Help.Visible = false;
                lblHeader.Text += ResHelper.GetFileString("Install.Step6");
                break;
            // Finish step
            case 7:
                imgName = "header_finish.png";
                StartHelp.TopicName = Help.TopicName = "DBInstall_StepFinish";
                lblHeader.Text += ResHelper.GetFileString("Install.Step7");
                break;

            case 8:
                imgName = "header_db.png";
                StartHelp.TopicName = Help.TopicName = "DBInstall_StepCollation";
                lblHeader.Text += ResHelper.GetFileString("Install.Step8");
                break;
        }

        // Calculate step number
        if (PreviousStep == index)
        {
            StepOperation = 0;
        }
        ActualStep += StepOperation;
        lblHeader.Text = string.Format(lblHeader.Text, ActualStep + 1);
        PreviousStep = index;

        imgHeader.ImageUrl = GetImageUrl("Others/Install/" + imgName);
        imgHeader.AlternateText = "Header";
    }



    private void EnsureDefaultButton()
    {
        if (wzdInstaller.ActiveStep != null)
        {
            if (wzdInstaller.ActiveStep.StepType == WizardStepType.Start)
            {
                Page.Form.DefaultButton = StartNextButton.UniqueID;
            }
            else
            {
                Page.Form.DefaultButton = NextButton.UniqueID;
            }
        }
    }


    #region "Installation methods"

    public bool CreateDatabase(string collation)
    {
        try
        {
            string message = ResHelper.GetFileString("Installer.LogCreatingDatabase") + " " + databaseDialog.NewDatabaseName;
            AddResult(message);
            LogProgressState(LogStatusEnum.Info, message);

            string connectionString = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, "", userServer.DBUsername, Password, 240, false);

            // Use default collation, if none specified
            if (String.IsNullOrEmpty(collation))
            {
                collation = ConnectionHelper.DatabaseCollation;
            }

            if (!DBCreated)
            {
                CMSDatabaseHelper.CreateDatabase(databaseDialog.NewDatabaseName, connectionString, collation);
            }

            return true;
        }
        catch (Exception ex)
        {
            mDisplayLog = true;
            string message = ResHelper.GetFileString("Intaller.LogErrorCreateDB") + " " + ex.Message;
            AddResult(message);
            LogProgressState(LogStatusEnum.Error, message);
        }
        return false;
    }


    /// <summary>
    /// Logs message to install log.
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="type">Type of message ("E" - error, "I" - info)</param>
    /// <param name="ignore">Set to true if error can be ignored</param>
    public void Log(string message, CMSDatabaseHelper.MessageType type, ref bool ignore)
    {
        AddResult(message);
        if (type == CMSDatabaseHelper.MessageType.Error)
        {
            LogProgressState(LogStatusEnum.Error, message);
        }
        if (type == CMSDatabaseHelper.MessageType.Info)
        {
            LogProgressState(LogStatusEnum.Info, message);
        }
    }


    /// <summary>
    /// Installs database (table structure + default data).
    /// </summary>
    /// <param name="parameter">Async action param</param>
    private void InstallDatabase(object parameter)
    {
        if (!DBInstalled && !CMSDatabaseHelper.InstallDatabase(InstallInfo.ConnectionString, InstallInfo.ScriptsFullPath,
                                                               ResHelper.GetFileString("Installer.LogErrorCreateDBObjects"), ResHelper.GetFileString("Installer.LogErrorDefaultData"), Log,
                                                               InstallInfo.DBSchema))
        {
            throw new Exception("[InstallDatabase]: Error during database creation.");
        }
        else
        {
            LogProgressState(LogStatusEnum.Finish, ResHelper.GetFileString("Installer.DBInstallFinished"));
        }
    }

    #endregion


    #region "Error handling methods"

    protected void HandleError(string message, WizardNavigationEventArgs e)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        errorPanel.ErrorLabelText = message;
        e.Cancel = true;
    }


    protected void HandleError(string message)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        errorPanel.ErrorLabelText = message;
    }


    protected void HandleError(string message, string resourceString, string topic)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        errorPanel.ErrorLabelText = message;
        errorPanel.DisplayError(resourceString, topic);
    }

    #endregion


    #region "Logging methods"

    /// <summary>
    /// Appends the result string to the result message.
    /// </summary>
    /// <param name="result">String to append</param>
    public void AddResult(string result)
    {
        mResult = result + "\n" + mResult;
    }


    /// <summary>
    /// Logs progress state.
    /// </summary>
    /// <param name="type">Type of the message</param>
    /// <param name="message">Message to be logged</param>
    public void LogProgressState(LogStatusEnum type, string message)
    {
        string[] status = InstallInfo.InstallLog.Split(new string[] { InstallInfo.SEPARATOR }, StringSplitOptions.None);

        // Wrong format of the internal status
        if (status.Length != 4)
        {
            InstallInfo.InstallLog = "F" + InstallInfo.SEPARATOR + "Wrong internal log." + InstallInfo.SEPARATOR + InstallInfo.SEPARATOR;
        }

        switch (type)
        {
            case LogStatusEnum.Info:
                status[0] = "I";
                status[1] = message + "<br />" + status[1];
                break;

            case LogStatusEnum.Error:
                status[0] = "F";
                status[2] += "<strong>" + ResHelper.GetFileString("Global.ErrorSign") + "</strong>" + message + "<br />";
                break;

            case LogStatusEnum.Warning:
                status[3] += "<strong>" + ResHelper.GetFileString("Global.Warning") + "</strong>" + message + "<br />";
                break;

            case LogStatusEnum.Finish:
                status[0] = "F";
                status[1] = "<strong>" + message + "</strong><br /><br />" + status[1];
                break;
        }

        InstallInfo.InstallLog = status[0] + InstallInfo.SEPARATOR + status[1] + InstallInfo.SEPARATOR + status[2] + InstallInfo.SEPARATOR + status[3];
    }

    #endregion


    #region "ICallbackEventHandler Members"

    public string GetCallbackResult()
    {
        return hdnState.Value;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        // Get arguments
        string[] args = eventArgument.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        bool cancel = ValidationHelper.GetBoolean(args[0], false);
        bool import = (args[1] == "IM");
        int messageLength = 0;
        int errorLength = 0;
        int warningLength = 0;

        if (args.Length == 5)
        {
            messageLength = ValidationHelper.GetInteger(args[2], 0);
            errorLength = ValidationHelper.GetInteger(args[3], 0);
            warningLength = ValidationHelper.GetInteger(args[4], 0);
        }

        if (import)
        {
            try
            {
                // Cancel
                if (cancel)
                {
                    ImportManager.Settings.Cancel();
                }

                hdnState.Value = ImportManager.Settings.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
            catch
            {
                ImportManager.Settings.LogProgressState(LogStatusEnum.Finish, ResHelper.GetFileString("SiteImport.Applicationrestarted"));
                hdnState.Value = ImportManager.Settings.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
        }
        else
        {
            try
            {
                hdnState.Value = InstallInfo.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
            catch
            {
                LogProgressState(LogStatusEnum.Finish, ResHelper.GetFileString("SiteImport.Applicationrestarted"));
                hdnState.Value = InstallInfo.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
        }
    }

    #endregion
}