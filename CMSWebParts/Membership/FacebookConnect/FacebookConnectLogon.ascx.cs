using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.MembershipProvider;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSWebParts_Membership_FacebookConnect_FacebookConnectLogon : CMSAbstractWebPart
{
    #region "Constants"

    protected const string SESSION_NAME_USERDATA = "facebookid";
    protected const string CONFIRMATION_URLPARAMETER = "confirmed";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets sign in text.
    /// </summary>
    public string SignInText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SignInText"), "");
        }
        set
        {
            SetValue("SignInText", value);
        }
    }

    /// <summary>
    /// Specifies whether to show faces underneath the Login button.
    /// </summary>
    public bool ShowFaces
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFaces"), false);
        }
        set
        {
            SetValue("ShowFaces", value);
        }
    }

    
    /// <summary>
    /// Returns "show-faces" HTML attribute for FB button according to current setting.
    /// </summary>
    public string ShowFacesAttr
    {
        get
        {
            return String.Format(" show-faces=\"{0}\"", ShowFaces.ToString().ToLowerCSafe());
        }
    }


    /// <summary>
    /// The width of the plugin in pixels. Default width: 200px.
    /// </summary>
    public int ButtonWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ButtonWidth"), 200);
        }
        set
        {
            SetValue("ButtonWidth", value);
        }
    }


    /// <summary>
    /// Returns "width" HTML attribute for FB button according to current setting.
    /// </summary>
    public string WidthAttr
    {
        get
        {
            return String.Format(" width=\"{0}\"", (ButtonWidth <= 0?200:ButtonWidth));
        }
    }


    /// <summary>
    /// The maximum number of rows of profile pictures to display. Default value: 1. 
    /// </summary>
    public int MaxRows
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRows"), 1);
        }
        set
        {
            SetValue("MaxRows", value);
        }
    }


    /// <summary>
    /// Returns "max-rows" HTML attribute for FB button according to current setting.
    /// </summary>
    public string MaxRowsAttr
    {
        get
        {
            return String.Format(" max-rows=\"{0}\"", (MaxRows < 0 ? 0 : MaxRows));
        }
    }


    /// <summary>
    /// A comma separated list of extended permissions. By default the Login button prompts users for
    /// their public information. If your application needs to access other parts of the user's profile that
    /// may be private, your application can request extended permissions. A complete list of extended permissions can be
    /// found here: http://developers.facebook.com/docs/authentication/permissions/
    /// </summary>
    public string Scope
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Scope"), null);
        }
        set
        {
            SetValue("Scope", value);
        }
    }


    /// <summary>
    /// Returns "scope" HTML attribute for FB button according to current setting.
    /// </summary>
    public string ScopeAttr
    {
        get
        {
            if (String.IsNullOrEmpty(Scope))
            {
                return null;
            }
            return String.Format(" scope=\"{0}\"", Scope);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to show sign out link.
    /// </summary>
    public bool ShowSignOut
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSignOut"), false);
        }
        set
        {
            SetValue("ShowSignOut", value);
        }
    }


    /// <summary>
    /// Gets or sets sign out text.
    /// </summary>
    public string SignOutText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SignOutText"), "");
        }
        set
        {
            SetValue("SignOutText", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that buttons will be used instead of links.
    /// </summary>
    public bool ShowAsButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAsButton"), false);
        }
        set
        {
            SetValue("ShowAsButton", value);
        }
    }


    /// <summary>
    /// Gets or sets sign out button image URL.
    /// </summary>
    public string SignOutImageURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SignOutImageURL"), null);
        }
        set
        {
            SetValue("SignOutImageURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), "");
        }
        set
        {
            if ((value != null) && (value.Length > 400))
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful registration.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether after successful registration is 
    /// notification email sent to the administrator.
    /// </summary>
    public bool NotifyAdministrator
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("NotifyAdministrator"), false);
        }
        set
        {
            SetValue("NotifyAdministrator", value);
        }
    }


    /// <summary>
    /// Gets the sender email (from).
    /// </summary>
    private string FromAddress
    {
        get
        {
            return SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSNoreplyEmailAddress");
        }
    }


    /// <summary>
    /// Gets the recipient email (to).
    /// </summary>
    private string ToAddress
    {
        get
        {
            return SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSAdminEmailAddress");
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            if (QueryHelper.GetInteger("logout", 0) > 0)
            {
                // Sign out from CMS
                CMSContext.LogoutUser();

                CMSContext.CurrentUser = null;
                Response.Cache.SetNoStore();
                URLHelper.Redirect(URLHelper.RemoveParameterFromUrl(URLHelper.CurrentURL, "logout"));
                return;
            }

            string currentSiteName = CMSContext.CurrentSiteName;
            if (!String.IsNullOrEmpty(currentSiteName) && SettingsKeyProvider.GetBoolValue(currentSiteName + ".CMSEnableFacebookConnect"))
            {
                // Check Facebook Connect settings
                if (!FacebookConnectHelper.FacebookIsAvailable(currentSiteName))
                {
                    // Display warning message in "Design mode"
                    if (DisplayMessage())
                    {
                        return;
                    }

                    Visible = false;
                    return;
                }

                // Try to retrieve return URL from query
                string returnUrl = QueryHelper.GetString("returnurl", null);

                // Init Facebook Connect
                if (Page is ContentPage)
                {
                    // Adding XML namespace
                    ((ContentPage)Page).XmlNamespace = FacebookConnectHelper.GetFacebookXmlNamespace();
                }

                // Init FB connect
                string initscr = FacebookConnectHelper.GetFacebookInitScriptForSite(currentSiteName) + " " + FacebookConnectHelper.GetFacebookLoginHandlerScript();
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "FBConnectLogon" + ClientID, initscr);
                // Return URL
                string currentUrl = URLHelper.AddParameterToUrl(URLHelper.CurrentURL, "logout", "1");
                string additionalScript = "window.location.href=" + ScriptHelper.GetString(URLHelper.GetAbsoluteUrl(currentUrl)) + "; return false;";
                // Logout script for FB connect
                string logoutScript = FacebookConnectHelper.GetFacebookLogoutScriptForSignOut(URLHelper.CurrentURL, FacebookConnectHelper.GetFacebookApiKey(currentSiteName), additionalScript);

                // Validate FB access token against FB server
                string facebookUserId = null;
                bool facebookCookiesValid = false;
                string confirmToken = QueryHelper.GetString(CONFIRMATION_URLPARAMETER, null);
                if (!String.IsNullOrEmpty(confirmToken))
                {
                    facebookCookiesValid = FacebookConnectHelper.ValidateFBAccessToken(confirmToken, out facebookUserId);
                }

                // If user is already authenticated 
                if (CMSContext.CurrentUser.IsAuthenticated())
                {
                    // Is user logged in using Facebook Connect?
                    if ((CMSContext.CurrentUser.UserSettings != null) && String.IsNullOrEmpty(CMSContext.CurrentUser.UserSettings.UserFacebookID))
                    {
                        // no, user is not logged in by Facebook Connect
                        logoutScript = additionalScript;
                    }

                    // Hide Facebook Connect button
                    plcFBButton.Visible = false;

                    // If signout should be visible and user has FacebookID registered
                    if (ShowSignOut && !String.IsNullOrEmpty(CMSContext.CurrentUser.UserSettings.UserFacebookID))
                    {
                        // If only text is set use text/button link
                        if (!String.IsNullOrEmpty(SignOutText))
                        {
                            // Button link
                            if (ShowAsButton)
                            {
                                btnSignOut.OnClientClick = logoutScript;
                                btnSignOut.Text = SignOutText;
                                btnSignOut.Visible = true;
                            }
                            // Text link
                            else
                            {
                                lnkSignOutLink.Text = SignOutText;
                                lnkSignOutLink.Visible = true;
                                lnkSignOutLink.Attributes.Add("onclick", logoutScript);
                                lnkSignOutLink.Attributes.Add("style", "cursor:pointer;");
                            }
                        }
                        // Image link
                        else
                        {
                            string signOutImageUrl = SignOutImageURL;
                            // Use default image if none is specified
                            if (String.IsNullOrEmpty(signOutImageUrl))
                            {
                                signOutImageUrl = GetImageUrl("Others/FacebookConnect/signout.gif");
                            }
                            imgSignOut.ImageUrl = ResolveUrl(signOutImageUrl);
                            imgSignOut.Visible = true;
                            imgSignOut.AlternateText = GetString("webparts_membership_signoutbutton.signout");
                            lnkSignOutImageBtn.Visible = true;
                            lnkSignOutImageBtn.Attributes.Add("onclick", logoutScript);
                            lnkSignOutImageBtn.Attributes.Add("style", "cursor:pointer;");
                        }
                    }
                    else
                    {
                        Visible = false;
                    }
                }
                // Sign In
                else
                {
                    if (facebookCookiesValid)
                    {
                        if (!String.IsNullOrEmpty(facebookUserId))
                        {
                            UserInfo ui = UserInfoProvider.GetUserInfoByFacebookConnectID(facebookUserId);
                            // Claimed Facebook ID is in DB
                            if (ui != null)
                            {
                                // Login existing user
                                if ((ui != null) && ui.Enabled)
                                {
                                    // Ban IP addresses which are blocked for login
                                    BannedIPInfoProvider.CheckIPandRedirect(currentSiteName, BanControlEnum.Login);

                                    // Create autentification cookie
                                    AuthenticationHelper.SetAuthCookieWithUserData(ui.UserName, true, Session.Timeout, new string[] { "facebooklogon" });
                                    UserInfoProvider.SetPreferredCultures(ui);

                                    int contactId = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                                    Activity activity = new ActivityUserLogin(contactId, ui,CMSContext.CurrentDocument, CMSContext.ActivityEnvironmentVariables);
                                    activity.Log();

                                    // Redirect user
                                    if (String.IsNullOrEmpty(returnUrl))
                                    {
                                        returnUrl = URLHelper.RemoveParameterFromUrl(URLHelper.CurrentURL, CONFIRMATION_URLPARAMETER);
                                    }

                                    URLHelper.Redirect(returnUrl);
                                }
                                // Otherwise is user disabled
                                else
                                {
                                    lblError.Text = GetString("membership.userdisabled");
                                    lblError.Visible = true;
                                }
                            }
                            // Claimed Facebook ID not found  = save new user
                            else
                            {
                                // Check whether additional user info page is set
                                string additionalInfoPage = SettingsKeyProvider.GetStringValue(currentSiteName + ".CMSRequiredFacebookPage").Trim();

                                // No page set, user can be created
                                if (String.IsNullOrEmpty(additionalInfoPage))
                                {
                                    // Register new user
                                    string error = null;
                                    ui = AuthenticationHelper.AuthenticateFacebookConnectUser(facebookUserId, currentSiteName, false, true, ref error);

                                    // If user was found or successfuly created
                                    if (ui != null)
                                    {
                                        // If user is enabled
                                        if (ui.Enabled)
                                        {
                                            // Create authentification cookie
                                            AuthenticationHelper.SetAuthCookieWithUserData(ui.UserName, true, Session.Timeout, new string[] { "facebooklogon" });

                                            int contactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                                            Activity activityLogin = new ActivityUserLogin(contactID, ui, CMSContext.CurrentDocument, CMSContext.ActivityEnvironmentVariables);
                                            activityLogin.Log();
                                        }

                                        // Send registration e-mails
                                        // E-mail confirmation is not required as user already provided confirmation by successful login using Facebook connect
                                        AuthenticationHelper.SendRegistrationEmails(ui, null, null, false, false);

                                        // Notify administrator
                                        if (NotifyAdministrator && !String.IsNullOrEmpty(FromAddress) && !String.IsNullOrEmpty(ToAddress))
                                        {
                                            AuthenticationHelper.NotifyAdministrator(ui, FromAddress, ToAddress);
                                        }

                                        // Log registration into analytics
                                        AuthenticationHelper.TrackUserRegistration(TrackConversionName, ConversionValue, currentSiteName, ui);

                                        Activity activity = new ActivityRegistration(ui, CMSContext.CurrentDocument, CMSContext.ActivityEnvironmentVariables);
                                        if (activity.Data != null)
                                        {
                                            activity.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                                            activity.Log();
                                        }
                                    }
                                    // Redirect when authentication was succesfull
                                    if (String.IsNullOrEmpty(error))
                                    {
                                        if (!String.IsNullOrEmpty(returnUrl))
                                        {
                                            URLHelper.Redirect(URLHelper.GetAbsoluteUrl(returnUrl));
                                        }
                                        else
                                        {
                                            URLHelper.Redirect(URLHelper.RemoveParameterFromUrl(URLHelper.CurrentURL, CONFIRMATION_URLPARAMETER));
                                        }
                                    }
                                    // Display error otherwise
                                    else
                                    {
                                        lblError.Text = error;
                                        lblError.Visible = true;
                                    }
                                }
                                // Additional information page is set
                                else
                                {
                                    // Store user object in session for additional info page
                                    SessionHelper.SetValue(SESSION_NAME_USERDATA, facebookUserId);

                                    // Redirect to additional info page
                                    string targetURL = URLHelper.GetAbsoluteUrl(additionalInfoPage);

                                    if (!String.IsNullOrEmpty(returnUrl))
                                    {
                                        // Add return URL to parameter
                                        targetURL = URLHelper.AddParameterToUrl(targetURL, "returnurl", HttpUtility.UrlEncode(returnUrl));
                                    }
                                    URLHelper.Redirect(targetURL);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Show warning message in "Design mode"
                Visible = DisplayMessage();
            }
        }
    }


    /// <summary>
    /// Displays warning message in "Design mode".
    /// </summary>
    private bool DisplayMessage()
    {
        // Error label is displayed in Design mode when Facebook Connect is disabled
        if (CMSContext.ViewMode == ViewModeEnum.Design)
        {
            StringBuilder parameter = new StringBuilder();
            parameter.Append(GetString("header.sitemanager") + " -> ");
            parameter.Append(GetString("settingscategory.cmssettings") + " -> ");
            parameter.Append(GetString("settingscategory.socialnetworks") + " -> ");
            parameter.Append(GetString("settingscategory.cmsfacebookconnect"));
            if (CMSContext.CurrentUser.UserSiteManagerAdmin)
            {
                // Make it link for SiteManager Admin
                parameter.Insert(0, "<a href=\"" + URLHelper.GetAbsoluteUrl("~/CMSSiteManager/default.aspx?section=settings") + "\" target=\"_top\">");
                parameter.Append("</a>");
            }

            lblError.Text = String.Format(GetString("mem.facebook.disabled"), parameter.ToString());
            lblError.Visible = true;
            plcFBButton.Visible = false;
            imgSignOut.Visible = false;
            lnkSignOutImageBtn.Visible = false;
            lnkSignOutLink.Visible = false;
        }

        return lblError.Visible;
    }

    #endregion
}