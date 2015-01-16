using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.MembershipProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.EventLog;
using CMS.Controls.Configuration;

public partial class CMSAdminControls_UI_Header : CMSUserControl
{
    #region "Variables & Constants"

    protected int logoColspan = 5;

    #endregion


    #region "Properties"

    /// <summary>
    /// Determines whether the header control is used for API examples section.
    /// </summary>
    public bool IsAPIExampleHeader
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckUserImpersonate();
        CheckTrial();

        // Facebook Connect sign out
        if (CMSContext.CurrentUser.IsAuthenticated())
        {
            if (QueryHelper.GetInteger("logout", 0) > 0)
            {
                // Stop processing for all controls during logout
                ucUsers.StopProcessing = true;
                siteSelector.StopProcessing = true;
                ucUICultures.StopProcessing = true;
                btnSignOut_Click(this, EventArgs.Empty);
                return;
            }
        }

        // Display the techPreview info if there is a key in the web.config
        pnlTechPreview.Visible = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSUseTechnicalPreview"], false);

        InitializeVersion();

        lnkTestingMode.Text = GetString("cmstesting.headerlink");
        lnkTestingMode.Visible = SettingsKeyProvider.TestingMode;
        lnkTestingMode.NavigateUrl = "~/CMSPages/GetTestingModeReport.aspx";

        lblUserInfo.Text = HTMLHelper.HTMLEncode(CMSContext.CurrentUser.FullName);

        pnlBody.CssClass = IsAPIExampleHeader ? "APIExamplesHeader" : (IsCMSDesk ? "CMSDeskHeader" : "SMHeader");

        tabControl.Visible = !IsCMSDesk && !IsAPIExampleHeader || IsCMSDesk;

        lblSignOut.Text = GetString("signoutbutton.signout");

        lnkCMSDesk.Text = GetString("Header.CMSDesk");
        lnkCMSDesk.NavigateUrl = EnsureViewModeParam("~/CMSDesk/default.aspx", "returnviewmode");
        lnkCMSDesk.Target = "_parent";

        lnkCMSSiteManager.Text = GetString("Header.SiteManager");
        lnkCMSSiteManager.NavigateUrl = EnsureViewModeParam("~/CMSSiteManager/default.aspx", "returnviewmode");
        lnkCMSSiteManager.Target = "_parent";

        string eventLogText = GetString("administration.ui.eventlog");
        lnkLog.NavigateUrl = ResolveUrl("~/CMSModules/EventLog/EventLog.aspx");
        lnkLog.ToolTip = eventLogText;
        imgLog.ImageUrl = GetImageUrl("Objects/CMS_EventLog/list.png");
        imgLog.AlternateText = eventLogText;

        string debugText = GetString("administration-system.debug");
        lnkDebug.NavigateUrl = ResolveUrl("~/CMSModules/System/Debug/System_DebugFrameset.aspx");
        lnkDebug.ToolTip = debugText;
        imgDebug.ImageUrl = GetImageUrl("CMSModules/CMS_System/debug.png");
        imgDebug.AlternateText = debugText;

        pwdExpiration.ShowChangePasswordLink = true;
        pwdExpiration.ExpirationTextBefore = GetString("passwordexpiration.expired");
        pwdExpiration.ExpirationWarningTextBefore = string.Format(GetString("passwordexpiration.willexpire"), CMSContext.CurrentUser.UserPasswordExpiration);

        // Displays windows azure and EMS icons
        if (AzureHelper.IsRunningOnAzure && SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSShowAzureLogo"))
        {
            imgWindowsAzure.Visible = true;
            imgWindowsAzure.ImageUrl = GetImageUrl("General/IconWindowsAzure.png");
            pnlExtraIcons.Visible = true;
        }
        if (LicenseHelper.CurrentEdition == ProductEditionEnum.EnterpriseMarketingSolution)
        {
            imgEnterpriseSolution.Visible = true;
            imgEnterpriseSolution.ImageUrl = GetImageUrl("General/IconEnterpriseSolution.png");
            pnlExtraIcons.Visible = true;
        }
        if (!CMSContext.CurrentUser.UserSiteManagerAdmin)
        {
            plcLog.Visible = false;
            plcDebug.Visible = false;
        }
        string redirectURL = null;

        string scHideWarning = @"
function HideWarning() {
  var panel = document.getElementById('" + pnlPwdExp.ClientID + @"');
  if(panel)
  {
    panel.style.display = ""none"";
    window.top.layouts[0].resizeAll(); 
  }
}";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "HideHeaderWarning", scHideWarning, true);

        SiteInfo si = CMSContext.CurrentSite;
        if ((si != null) && (!si.SiteIsOffline))
        {
            // Initialize variables from query string 
            int nodeId = QueryHelper.GetInteger("nodeid", 0);
            string culture = QueryHelper.GetText("culture", null);
            string liveSiteUrl = "~";

            // Set url to node from which CMSDesk was opened
            if (IsCMSDesk && (nodeId > 0) && !String.IsNullOrEmpty(culture))
            {
                TreeProvider treeProvider = new TreeProvider(CMSContext.CurrentUser);
                TreeNode node = treeProvider.SelectSingleNode(nodeId, culture, false, false);
                if (node != null)
                {
                    liveSiteUrl = CMSContext.GetUrl(node.NodeAliasPath, node.DocumentUrlPath);
                }
            }
            // Resolve Url and add live site view mode
            liveSiteUrl = ResolveUrl(liveSiteUrl);
            liveSiteUrl = URLHelper.AddParameterToUrl(liveSiteUrl, "viewmode", "livesite");
            liveSiteUrl = EnsureViewModeParam(liveSiteUrl, "viewmode");
            lnkLiveSite.Text = ResHelper.GetString("Header.LiveSite");
            lnkLiveSite.NavigateUrl = liveSiteUrl;
            lnkLiveSite.Target = "_parent";
        }

        if (IsCMSDesk)
        {
            // Make 'Site manager' link visible for global administrators
            CurrentUserInfo ui = CMSContext.CurrentUser;
            if (ui.UserSettings != null)
            {
                plcSiteManagerRedirect.Visible = ui.UserSiteManagerAdmin;
            }

            // Site selector settings
            siteSelector.DropDownSingleSelect.CssClass = "HeaderSiteDrop";
            siteSelector.UpdatePanel.RenderMode = UpdatePanelRenderMode.Inline;
            siteSelector.AllowAll = false;
            siteSelector.UniSelector.OnSelectionChanged += SiteSelector_OnSelectionChanged;
            siteSelector.UniSelector.OnBeforeClientChanged = "if (!CheckChanges()) { this.value = this.originalValue; return false; }";
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.OnlyRunningSites = true;

            if (!RequestHelper.IsPostBack())
            {
                siteSelector.Value = CMSContext.CurrentSiteID;
            }

            // Show only assigned sites for not global admins
            if (!CMSContext.CurrentUser.IsGlobalAdministrator)
            {
                siteSelector.UserId = CMSContext.CurrentUser.UserID;
            }

            StringBuilder script = new StringBuilder();
            script.Append(
                @"function SetActivePage() {}
  function SiteRedirect(url) { parent.location = url; }
  function CheckChanges() {
    try { if (!parent.frames['cmsdesktop'].CheckChanges()) return false; } catch (ex) { }
    return true;
  }
");
            ScriptHelper.RegisterStartupScript(this, typeof(string), "headerScript", ScriptHelper.GetScript(script.ToString()));

            lnkLogo.NavigateUrl = "~/CMSDesk/default.aspx";
            lnkLogo.Target = "_parent";

            redirectURL = URLHelper.CurrentURL;

            tabControl.UrlTarget = "cmsdesktop";
            tabControl.QueryParameterName = "section";
            tabControl.ModuleName = "CMS.Desk";
            tabControl.ModuleAvailabilityForSiteRequired = true;
            tabControl.OnTabCreated += tabControl_OnTabCreated;

            if (RequestHelper.IsWindowsAuthentication())
            {
                pnlSignOut.Visible = false;
                pnlRight.CssClass += " HeaderWithoutSignOut";
            }
            else
            {
                // Init Facebook Connect and join logout script to sign out button
                string logoutScript = FacebookConnectHelper.FacebookConnectInitForSignOut(CMSContext.CurrentSiteName, ltlFBConnectScript);
                if (!String.IsNullOrEmpty(logoutScript))
                {
                    // If Facebook Connect initialized include 'CheckChanges()' to logout script
                    logoutScript = "if (CheckChanges()) { " + logoutScript + " } return false; ";
                }
                else
                {
                    // If Facebook Connect not initialized just return 'CheckChanges()' script
                    logoutScript = "return CheckChanges();";
                }
                lnkSignOut.OnClientClick = logoutScript;
            }
        }
        else
        {
            plcSiteSelector.Visible = false;
            pnlSeparator.Visible = true;

            lblUserInfo.Text = HTMLHelper.HTMLEncode(CMSContext.CurrentUser.FullName);
            plcCMSDeskRedirect.Visible = true;
            plcSiteManagerRedirect.Visible = IsAPIExampleHeader;

            lnkTestingMode.Text = GetString("cmstesting.headerlink");
            lnkTestingMode.Visible = SettingsKeyProvider.TestingMode;
            lnkTestingMode.NavigateUrl = "~/CMSPages/GetTestingModeReport.aspx";

            string url = null;
            if (IsAPIExampleHeader)
            {
                url = "~/CMSAPIExamples/default.aspx";
            }
            else
            {
                url = "~/CMSSiteManager/default.aspx";
            }
            lnkLogo.NavigateUrl = url;
            lnkLogo.Target = "_parent";

            redirectURL = "~/CMSMessages/Redirect.aspx?frame=top&url=" + url;

            tabControl.OnTabCreated += new UITabs.TabCreatedEventHandler(tabControl_OnTabCreated);
            tabControl.UrlTarget = "cmsdesktop";
            tabControl.QueryParameterName = "section";
            tabControl.ModuleName = "CMS.SiteManager";
            tabControl.UseClientScript = true;

            if (RequestHelper.IsWindowsAuthentication())
            {
                pnlSignOut.Visible = false;
                pnlRight.CssClass += " HeaderWithoutSignOut";
            }
            else
            {
                lblSignOut.Text = GetString("signoutbutton.signout");
                lnkSignOut.OnClientClick = FacebookConnectHelper.FacebookConnectInitForSignOut(CMSContext.CurrentSiteName, ltlFBConnectScript);
            }
        }

        CheckUICultureChange(redirectURL);

        if (ModuleEntry.IsModuleRegistered("CMS.Chat") && ModuleEntry.IsModuleLoaded("CMS.Chat") && (CMSContext.CurrentSiteID > 0)) 
        {
            CMSUserControl supportChatControl = Page.LoadUserControl("~/CMSModules/Chat/Controls/SupportChatHeader.ascx") as CMSUserControl;
            if (supportChatControl != null)
            {
                plcSupportChatControl.Controls.Add(supportChatControl);
                pnlSupportChat.Visible = true;
            }
        }
    }


    private void InitializeVersion()
    {
        string version = "v";

        if (SettingsKeyProvider.DevelopmentMode)
        {
            version += CMSContext.FullSystemSuffixVersion;
        }
        else
        {
            if (ValidationHelper.GetInteger(CMSContext.HotfixVersion, 0) > 0)
            {
                version += CMSContext.HotfixedSystemSuffixVersion;
            }
            else
            {
                version += CMSContext.GeneralVersionSuffix;
            }
            lblVersion.ToolTip = CMSContext.FullSystemSuffixVersion.Trim();
        }
        lblVersion.Text = version.Trim();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        logoColspan += Convert.ToInt32(plcLiveSite.Visible)
                       + Convert.ToInt32(plcCMSDeskRedirect.Visible)
                       + Convert.ToInt32(plcSiteManagerRedirect.Visible)
                       + Convert.ToInt32(plcSiteSelector.Visible)
                       + Convert.ToInt32(pnlUsers.Visible)
                       + Convert.ToInt32(pnlSupportChat.Visible)
                       + Convert.ToInt32(pnlCultures.Visible)
                       + Convert.ToInt32(plcLog.Visible)
                       + Convert.ToInt32(plcDebug.Visible);

        pnlPwdExp.Visible = pwdExpiration.Visible;
    }

    #endregion


    #region "Methods"

    private void CheckTrial()
    {
        string info = null;

        if (LicenseHelper.ApplicationExpires != DateTime.MinValue)
        {
            int appExpiration = LicenseHelper.ApplicationExpires.Subtract(DateTime.Now).Days;

            // Application expires
            if (CMSContext.SYSTEM_VERSION_SUFFIX.Contains("BETA"))
            {
                if (appExpiration <= 0)
                {
                    info = GetString("Beta.AppExpired");
                }
                else
                {
                    info = string.Format(GetString("Beta.AppExpiresIn"), appExpiration);
                }
            }
            else
            {
                if (appExpiration <= 0)
                {
                    info = string.Format(GetString("Preview.AppExpired"), CMSContext.SYSTEM_VERSION_SUFFIX);
                }
                else
                {
                    info = string.Format(GetString("Preview.AppExpiresIn"), CMSContext.SYSTEM_VERSION_SUFFIX, appExpiration);
                }
            }
        }
        // Check the license key for trial version
        else if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), string.Empty) != string.Empty)
        {
            LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(URLHelper.GetCurrentDomain());
            if ((lki != null) && (lki.Key.Length == LicenseKeyInfo.TRIAL_KEY_LENGTH) && (lki.ExpirationDateReal != LicenseKeyInfo.TIME_UNLIMITED_LICENSE))
            {
                int expiration = lki.ExpirationDateReal.Subtract(DateTime.Now).Days;
                // Trial version expiration date
                if (expiration <= 0)
                {
                    info = GetString("Trial.Expired");
                }
                else
                {
                    info = string.Format(GetString("Trial.ExpiresIn"), expiration);
                }
            }

            if (lki != null)
            {
                // Check the number of users for free edition
                if (lki.Edition == ProductEditionEnum.Free)
                {
                    UserInfoProvider.LicenseController();
                }
            }
        }

        ltlText.Text = info;
        pnlTrial.Visible = !string.IsNullOrEmpty(ltlText.Text);
    }


    /// <summary>
    /// Check if UI changes was made.
    /// </summary>
    private void CheckUICultureChange(string redirectUrl)
    {
        DataSet ds = UICultureInfoProvider.GetUICultures(String.Empty, String.Empty, 0, "COUNT (UICultureID)");

        // Show selector only if there are more UI cultures than one
        if (!DataHelper.DataSourceIsEmpty(ds) && (ValidationHelper.GetInteger(ds.Tables[0].Rows[0][0], 0) > 1))
        {
            ucUICultures.ButtonImage = string.Empty;

            ucUICultures.DialogLink.ImageUrl = GetImageUrl("Objects/CMS_Site/list.png");
            ucUICultures.DialogLink.ToolTip = GetString("uicultures.change");

            string cultureName = ValidationHelper.GetString(ucUICultures.Value, String.Empty);
            if (cultureName != string.Empty)
            {
                CMSContext.CurrentUser.PreferredUICultureCode = cultureName;
                UserInfoProvider.SetUserInfo(CMSContext.CurrentUser);

                // Set selected UI culture and refresh all pages
                CultureHelper.SetPreferredUICulture(cultureName);

                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    URLHelper.Redirect(redirectUrl);
                }
            }
        }
        else
        {
            pnlCultures.Visible = false;
        }
    }


    /// <summary>
    /// Check for user impersonate.
    /// </summary>
    private void CheckUserImpersonate()
    {
        CurrentUserInfo user = CMSContext.CurrentUser;

        string originalUserName = "";

        if (RequestHelper.IsFormsAuthentication())
        {
            originalUserName = ValidationHelper.GetString(SessionHelper.GetValue("OriginalUserName"), "");
        }
        else
        {
            originalUserName = ValidationHelper.GetString(SessionHelper.GetValue("ImpersonateUserName"), "");
        }

        // Show impersonate button for global admin only or impersonated user
        if ((user.IsGlobalAdministrator) || (!String.IsNullOrEmpty(originalUserName)))
        {
            // Set context menu for impersonate
            imgImpersonate.ImageUrl = GetImageUrl("Design/Backgrounds/ArrowWhite.png");
            menuCont.MenuControlPath = "~/CMSAdminControls/ContextMenus/UserImpersonateMenu.ascx";
            menuCont.MenuID = ClientID + "m_impersonate_context_menu";
            menuCont.ParentElementClientID = ClientID;
            menuCont.Parameter = "''";
            menuCont.RenderAsTag = HtmlTextWriterTag.A;
            menuCont.MouseButton = MouseButtonEnum.Both;
            menuCont.VerticalPosition = VerticalPositionEnum.Bottom;
            menuCont.HorizontalPosition = HorizontalPositionEnum.Left;

            pnlUsers.Visible = true;

            // Hide button for cancel impersonation
            btnHiddenImpersonate.Style.Add("display", "none");

            if (IsCMSDesk)
            {
                // Show users from current site only
                ucUsers.WhereCondition = "UserID IN (SELECT UserID FROM CMS_UserSite WHERE (SiteID = " + CMSContext.CurrentSiteID + ") AND (UserIsGlobalAdministrator = 0)) AND (UserID != " + CMSContext.CurrentUser.UserID + ") AND (UserName != N'public')";
            }
            else
            {
                // Show all users except global administrators
                ucUsers.WhereCondition = "UserID IN (SELECT UserID FROM CMS_UserSite WHERE (UserIsGlobalAdministrator = 0)) AND (UserID != " + CMSContext.CurrentUser.UserID + ") AND (UserName != N'public')";
            }

            // Script for open uniselector modal dialog
            string impersonateScript = "function userImpersonateShowDialog () {US_SelectionDialog_" + ucUsers.ClientID + "()}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ImpersonateContextMenu", ScriptHelper.GetScript(impersonateScript));


            string userName = HttpUtility.UrlDecode(ValidationHelper.GetString(ucUsers.Value, String.Empty));
            if (userName != String.Empty)
            {
                // Get selected user info
                UserInfo iui = UserInfoProvider.GetUserInfo(userName);
                if (!iui.IsGlobalAdministrator)
                {
                    CMSContext.CurrentUser.UserImpersonate(iui);
                }
            }

            // Set script for cancel impersonation only if original user name is specified
            if (!String.IsNullOrEmpty(originalUserName))
            {
                string script = "function CancelImpersonation() {document.getElementById('" + btnHiddenImpersonate.ClientID + "').click();return false;}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CancelImpersonation", ScriptHelper.GetScript(script));
            }
        }
    }


    /// <summary>
    /// Ensures that the given url will be extended by returnviewmode parameter.
    /// </summary>
    /// <param name="url">The URL</param>
    /// <param name="paramName">The parameter name which the viewmode parameter will be assigned to</param>
    private string EnsureViewModeParam(string url, string paramName)
    {
        if (QueryHelper.Contains("returnviewmode"))
        {
            // Set the viewmode according to the "returnviewmode" parameter if set
            url = URLHelper.AddParameterToUrl(url, paramName, QueryHelper.GetString("returnviewmode", "livesite"));
        }

        return url;
    }

    #endregion


    #region "Control events"

    protected void SiteSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Create URL
        int siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
        if (si != null)
        {
            string domain = si.DomainName.TrimEnd('/');
            string url = "~" + VirtualContext.CurrentURLPrefix + "/CMSDesk/default.aspx";

            if (domain.Contains("/"))
            {
                // Resolve application path
                url = url.Substring(1);
            }

            url = URLHelper.GetAbsoluteUrl(url, domain, null, null);

            // Check if single sign-on is turned on
            if (SettingsKeyProvider.GetBoolValue("CMSAutomaticallySignInUser"))
            {
                url = AuthenticationHelper.GetUserAuthenticationUrl(CMSContext.CurrentUser, url);
            }
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "selectSite", ScriptHelper.GetScript("SiteRedirect('" + url + "');"));
        }
    }


    protected void btnSignOut_Click(object sender, EventArgs e)
    {
        // Usual sign out
        string signOutUrl = URLHelper.ApplicationPath.TrimEnd('/') + "/default.aspx";

        // LiveID sign out URL is set if this LiveID session
        CMSPage.SignOut(signOutUrl);
    }


    protected void btnHiddenImpersonate_Click(object sender, EventArgs e)
    {
        string originalUserName = ValidationHelper.GetString(SessionHelper.GetValue("OriginalUserName"), "");
        if (RequestHelper.IsFormsAuthentication())
        {
            UserInfo ui = UserInfoProvider.GetUserInfo(originalUserName);
            CMSContext.CurrentUser.UserImpersonate(ui);
        }
        else
        {
            SessionHelper.SetValue("ImpersonateUserName", null);
            SessionHelper.SetValue("OriginalUserName", null);

            CMSContext.CurrentUser.Invalidate(false);

            // Log event
            EventLogProvider log = new EventLogProvider();
            log.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "Administration", "Impersonate", 0, null, 0, null, null, "User " + originalUserName + " has returned to his account.", CMSContext.CurrentSiteID, URLHelper.CurrentURL);

            URLHelper.Redirect(URLHelper.CurrentURL);
        }
    }


    private TabItem tabControl_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Hide Online marketing tab if license is not EMS
        if (element.ElementName.ToLowerCSafe() == "onlinemarketing")
        {
            if (LicenseHelper.CurrentEdition != ProductEditionEnum.EnterpriseMarketingSolution)
            {
                return null;
            }
        }
        return tab;
    }

    #endregion
}
