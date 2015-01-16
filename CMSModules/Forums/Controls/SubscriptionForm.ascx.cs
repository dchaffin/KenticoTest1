using System;
using System.Collections.Generic;
using System.Web;

using CMS.CMSHelper;
using CMS.Forums;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.WebAnalytics;
using CMS.ExtendedControls;

public partial class CMSModules_Forums_Controls_SubscriptionForm : ForumViewer
{
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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        #region "Resources"

        // Set string values
        rfvEmail.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.emailErrorMsg");

        // Regular expression to validate email (e-mail is not required)
        rfvEmail.ValidationExpression = @"^([\w0-9_-]+(\.[\w0-9_-]+)*@[\w0-9_-]+(\.[\w0-9_-]+)+)*$";
        rfvEmailRequired.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.emailRequireErrorMsg");

        btnOk.Text = GetString("General.Ok");
        btnCancel.Text = GetString("General.Cancel");

        #endregion


        // Pre-fill user email
        if (!RequestHelper.IsPostBack())
        {
            txtEmail.Text = CMSContext.CurrentUser.Email;
        }
    }


    /// <summary>
    /// OK click handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(CMSContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            ShowError(GetString("General.BannedIP"));
            return;
        }

        // Check input fields
        string result = new Validator().NotEmpty(txtEmail.Text, rfvEmailRequired.ErrorMessage).IsRegularExp(txtEmail.Text, @"^([\w0-9_-]+(\.[\w0-9_-]+)*@[\w0-9_-]+(\.[\w0-9_-]+)+)*$", rfvEmail.ErrorMessage).Result;

        if (result == "")
        {
            // For selected forum and only if subscription is enabled
            if ((ForumContext.CurrentForum != null) && ((ForumContext.CurrentState == ForumStateEnum.SubscribeToPost) || (ForumContext.CurrentState == ForumStateEnum.NewSubscription)))
            {
                // Check permissions
                if (!IsAvailable(ForumContext.CurrentForum, ForumActionType.SubscribeToForum))
                {
                    ShowError(GetString("ForumNewPost.PermissionDenied"));
                    return;
                }

                // Create new subscription
                ForumSubscriptionInfo fsi = new ForumSubscriptionInfo();
                fsi.SubscriptionForumID = ForumContext.CurrentForum.ForumID;
                fsi.SubscriptionEmail = HTMLHelper.HTMLEncode(txtEmail.Text.Trim());
                fsi.SubscriptionGUID = Guid.NewGuid();

                if (ForumContext.CurrentSubscribeThread != null)
                {
                    fsi.SubscriptionPostID = ForumContext.CurrentSubscribeThread.PostId;
                }

                if (CMSContext.CurrentUser != null)
                {
                    fsi.SubscriptionUserID = CMSContext.CurrentUser.UserID;
                }

                // Check whether user is not subscribed
                if (ForumSubscriptionInfoProvider.IsSubscribed(txtEmail.Text.Trim(), fsi.SubscriptionForumID, fsi.SubscriptionPostID))
                {
                    ShowError(GetString("ForumSubscibe.SubscriptionExists"));
                    return;
                }

                ForumSubscriptionInfoProvider.Subscribe(fsi, DateTime.Now, true, true);

                if (fsi.SubscriptionApproved)
                {
                    ShowConfirmation(GetString("blog.subscription.beensubscribed"));
                    LogSubscriptionActivity(fsi, ForumContext.CurrentForum);
                }
                else
                {
                    string confirmation = GetString("general.subscribed.doubleoptin");
                    int optInInterval = ForumGroupInfoProvider.DoubleOptInInterval(CMSContext.CurrentSiteName);
                    if (optInInterval > 0)
                    {
                        confirmation += "<br />" + string.Format(GetString("general.subscription_timeintervalwarning"), optInInterval);
                    }
                    ShowConfirmation(confirmation);
                }

            }
        }

        URLHelper.Redirect(ClearURL());
    }


    /// <summary>
    /// Cancel click handler.
    /// </summary>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(ClearURL());
    }


    /// <summary>
    /// Logs activity.
    /// </summary>
    /// <param name="bci">Forum subscription</param>
    private void LogSubscriptionActivity(ForumSubscriptionInfo fsi, ForumInfo fi)
    {
        if ((fi == null) || !fi.ForumLogActivity)
        {
            return;
        }

        Activity activity = new ActivitySubscriptionForumPost(fi, fsi, CMSContext.CurrentDocument, CMSContext.ActivityEnvironmentVariables);
        activity.Log();
    }
}