using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.URLRewritingEngine;
using CMS.WebAnalytics;

public partial class CMSWebParts_Membership_FacebookConnect_FacebookUsersRequiredData : CMSAbstractWebPart
{
    #region "Constants"

    protected const string SESSION_NAME_USERDATA = "facebookid";

    #endregion


    #region "Private variables"

    private string facebookUserId = null;
    private string mDefaultTargetUrl = String.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether email to user should be sent.
    /// </summary>
    public bool SendWelcomeEmail
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendWelcomeEmail"), true);
        }
        set
        {
            SetValue("SendWelcomeEmail", value);
        }
    }


    /// <summary>
    /// Gets or sets registration approval page URL.
    /// </summary>
    public string ApprovalPage
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ApprovalPage"), "");
        }
        set
        {
            SetValue("ApprovalPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the sender email (from).
    /// </summary>
    public string FromAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FromAddress"), SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSNoreplyEmailAddress"));
        }
        set
        {
            SetValue("FromAddress", value);
        }
    }


    /// <summary>
    /// Gets or sets the recipient email (to).
    /// </summary>
    public string ToAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ToAddress"), SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSAdminEmailAddress"));
        }
        set
        {
            SetValue("ToAddress", value);
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
    /// Gets or sets the message which is displayed after successful registration.
    /// </summary>
    public string DisplayMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayMessage"), "");
        }
        set
        {
            SetValue("DisplayMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the value which enables abitity of new user to set password.
    /// </summary>
    public bool AllowFormsAuthentication
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowFormsAuthentication"), false);
        }
        set
        {
            SetValue("AllowFormsAuthentication", value);
            plcPasswordNew.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which enables abitity join liveid with existing account.
    /// </summary>
    public bool AllowExistingUser
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowExistingUser"), true);
        }
        set
        {
            SetValue("AllowExistingUser", value);
            plcPasswordNew.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the default target url (redirection when the user is logged in).
    /// </summary>
    public string DefaultTargetUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultTargetUrl"), mDefaultTargetUrl);
        }
        set
        {
            SetValue("DefaultTargetUrl", value);
            mDefaultTargetUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines the behaviour for no Facebook users.
    /// </summary>
    public bool HideForNoFacebookID
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideForNoFacebookUserID"), true);
        }
        set
        {
            SetValue("HideForNoFacebookUserID", value);
        }
    }

    #endregion


    #region "Conversion properties"

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
        if (!StopProcessing)
        {
            string currentSiteName = CMSContext.CurrentSiteName;

            // Get Facebook Connect settings
            string apiKey = SettingsKeyProvider.GetStringValue(currentSiteName + ".CMSFacebookConnectApiKey");
            string secret = SettingsKeyProvider.GetStringValue(currentSiteName + ".CMSFacebookApplicationSecret");

            if (SettingsKeyProvider.GetBoolValue(currentSiteName + ".CMSEnableFacebookConnect") &&
                !String.IsNullOrEmpty(apiKey) && !String.IsNullOrEmpty(secret))
            {
                // Hide webpart if user is authenticated
                if (CMSContext.CurrentUser.IsAuthenticated())
                {
                    Visible = false;
                    return;
                }

                plcPasswordNew.Visible = AllowFormsAuthentication;
                pnlExistingUser.Visible = AllowExistingUser;

                facebookUserId = ValidationHelper.GetString(SessionHelper.GetValue(SESSION_NAME_USERDATA), null);

                // There is no Facebook user ID stored in session - hide all
                if (String.IsNullOrEmpty(facebookUserId) && HideForNoFacebookID)
                {
                    Visible = false;
                }
            }
            else
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
                    plcError.Visible = true;
                    plcContent.Visible = false;
                }
                else
                {
                    Visible = false;
                }
            }
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Handles btnOkExist click, joins existing user with liveid token.
    /// </summary>
    protected void btnOkExist_Click(object sender, EventArgs e)
    {
        // Live user must be retrieved from session
        if (!String.IsNullOrEmpty(facebookUserId))
        {
            if (!String.IsNullOrEmpty(txtUserName.Text))
            {
                // Try to authenticate user
                UserInfo ui = AuthenticationHelper.AuthenticateUser(txtUserName.Text, txtPassword.Text, CMSContext.CurrentSiteName);

                // Check banned IPs
                BannedIPInfoProvider.CheckIPandRedirect(CMSContext.CurrentSiteName, BanControlEnum.Login);

                if (ui != null)
                {
                    // Add Facebook Connect user ID token to user
                    ui.UserSettings.UserFacebookID = facebookUserId;
                    UserInfoProvider.SetUserInfo(ui);

                    // Set authentication cookie and redirect to page
                    SetAuthCookieAndRedirect(ui);
                }
                else // Invalid credentials
                {
                    lblError.Text = GetString("Login_FailureText");
                    plcError.Visible = true;
                }
            }
            else // User did not fill the form
            {
                lblError.Text = GetString("mem.facebook.fillloginform");
                plcError.Visible = true;
            }
        }
    }


    /// <summary>
    /// Handles btnOkNew click, creates new user and joins it with liveid token.
    /// </summary>
    protected void btnOkNew_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(facebookUserId))
        {
            string password = passStrength.Text;
            string currentSiteName = CMSContext.CurrentSiteName;

            // Validate entered values
            string errorMessage = new Validator().IsRegularExp(txtUserNameNew.Text, "^([a-zA-Z0-9_\\-\\.@]+)$", GetString("mem.facebook.fillcorrectusername"))
                .IsEmail(txtEmail.Text, GetString("mem.facebook.fillvalidemail")).Result;

            // If password is enabled to set, check it
            if (plcPasswordNew.Visible && (String.IsNullOrEmpty(errorMessage)))
            {
                if (String.IsNullOrEmpty(password))
                {
                    errorMessage = GetString("mem.facebook.specifyyourpass");
                }
                else if (password != txtConfirmPassword.Text.Trim())
                {
                    errorMessage = GetString("webparts_membership_registrationform.passwordonotmatch");
                }

                // Check policy
                if (!passStrength.IsValid())
                {
                    errorMessage = AuthenticationHelper.GetPolicyViolationMessage(CMSContext.CurrentSiteName);
                }
            }

            // Check whether email is unique if it is required
            if ((String.IsNullOrEmpty(errorMessage)) && !UserInfoProvider.IsEmailUnique(txtEmail.Text.Trim(), currentSiteName, 0))
            {
                errorMessage = GetString("UserInfo.EmailAlreadyExist");
            }

            // Check reserved names
            if ((String.IsNullOrEmpty(errorMessage)) && UserInfoProvider.NameIsReserved(currentSiteName, txtUserNameNew.Text.Trim()))
            {
                errorMessage = GetString("Webparts_Membership_RegistrationForm.UserNameReserved").Replace("%%name%%", HTMLHelper.HTMLEncode(txtUserNameNew.Text.Trim()));
            }

            if (String.IsNullOrEmpty(errorMessage))
            {
                // Check if user with given username already exists
                UserInfo ui = UserInfoProvider.GetUserInfo(txtUserNameNew.Text.Trim());

                // User with given username is already registered
                if (ui != null)
                {
                    plcError.Visible = true;
                    lblError.Text = GetString("mem.openid.usernameregistered");
                }
                else
                {
                    // Register new user
                    string error = DisplayMessage;
                    ui = AuthenticationHelper.AuthenticateFacebookConnectUser(facebookUserId, currentSiteName, true, false, ref error);
                    DisplayMessage = error;

                    if (ui != null)
                    {
                        // Set additional information
                        ui.UserName = ui.UserNickName = txtUserNameNew.Text.Trim();
                        ui.Email = txtEmail.Text;

                        // Set password
                        if (plcPasswordNew.Visible)
                        {
                            UserInfoProvider.SetPassword(ui, password);

                            // If user can choose password then is not considered external(external user can't login in common way)
                            ui.IsExternal = false;
                        }

                        UserInfoProvider.SetUserInfo(ui);

                        // Remove live user object from session, won't be needed
                        SessionHelper.Remove(SESSION_NAME_USERDATA);

                        // Send registration e-mails
                        AuthenticationHelper.SendRegistrationEmails(ui, ApprovalPage, password, true, SendWelcomeEmail);

                        // Notify administrator
                        bool requiresConfirmation = SettingsKeyProvider.GetBoolValue(currentSiteName + ".CMSRegistrationEmailConfirmation");
                        if (!requiresConfirmation && NotifyAdministrator && (FromAddress != String.Empty) && (ToAddress != String.Empty))
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

                        // Set authentication cookie and redirect to page
                        SetAuthCookieAndRedirect(ui);

                        // Display error message
                        if (!String.IsNullOrEmpty(DisplayMessage))
                        {
                            lblInfo.Visible = true;
                            lblInfo.Text = DisplayMessage;
                            plcForm.Visible = false;
                        }
                        else
                        {
                            URLHelper.Redirect(ResolveUrl("~/Default.aspx"));
                        }
                    }
                }
            }
            else
            {
                lblError.Text = errorMessage;
                plcError.Visible = true;
            }
        }
    }


    /// <summary>
    /// Helper method, set authentication cookie and redirect to return URL or default page.
    /// </summary>
    /// <param name="ui">User info</param>
    /// <param name="user">Windows live user</param>
    private void SetAuthCookieAndRedirect(UserInfo ui)
    {
        // Create autentification cookie
        if (ui.Enabled)
        {
            AuthenticationHelper.SetAuthCookieWithUserData(ui.UserName, true, Session.Timeout, new string[] { "facebooklogin" });

            // Log activity
            int contactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
            Activity activityLogin = new ActivityUserLogin(contactID, ui, CMSContext.CurrentDocument, CMSContext.ActivityEnvironmentVariables);
            activityLogin.Log();

            string returnUrl = QueryHelper.GetString("returnurl", null);

            // Redirect to ReturnURL
            if (!String.IsNullOrEmpty(returnUrl))
            {
                URLHelper.Redirect(ResolveUrl(HttpUtility.UrlDecode(returnUrl)));
            }
            // Redirect to default page
            else if (!String.IsNullOrEmpty(DefaultTargetUrl))
            {
                URLHelper.Redirect(ResolveUrl(DefaultTargetUrl));
            }
            // Otherwise refresh current page
            else
            {
                URLHelper.Redirect(URLRewriter.CurrentURL);
            }
        }
    }

    #endregion
}