using System;
using System.Data.SqlTypes;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.Scheduler;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Configuration : CMSNewsletterNewslettersPage
{
    #region "Variables"

    /// <summary>
    /// Newsletter ID.
    /// </summary>
    protected int newsletterId = 0;


    /// <summary>
    /// It is true if edited newsletter is dynamic.
    /// </summary>
    protected bool isDynamic = false;


    /// <summary>
    /// Determines if newsletter tracking is enabled.
    /// </summary>
    private bool? mTrackingEnabled = null;

    private bool TrackingEnabled
    {
        get
        {
            if (mTrackingEnabled == null)
            {
                mTrackingEnabled = NewsletterHelper.IsTrackingAvailable();
            }

            return (bool)mTrackingEnabled;
        }
    }


    /// <summary>
    /// Determines if Online Marketing is enabled.
    /// </summary>
    private bool? mOnlineMarketingEnabled = null;

    private bool OnlineMarketingEnabled
    {
        get
        {
            if (mOnlineMarketingEnabled == null)
            {
                mOnlineMarketingEnabled = NewsletterHelper.OnlineMarketingAvailable(CMSContext.CurrentSiteName);
            }

            return (bool)mOnlineMarketingEnabled;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get newsletter ID from query string
        newsletterId = QueryHelper.GetInteger("newsletterid", 0);
        if (newsletterId == 0)
        {
            return;
        }

        // Set validation messages
        rfvNewsletterDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvNewsletterName.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptyName");
        rfvNewsletterSenderName.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptySenderName");
        rfvNewsletterSenderEmail.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptySenderEmail");

        // Set form groups texts
        pnlGeneral.GroupingText = GetString("general.general");
        pnlTemplate.GroupingText = GetString("newsletter_configuration.templatebased");
        pnlDynamic.GroupingText = GetString("newsletter_configuration.dynamic");
        pnlOM.GroupingText = GetString("onlinemarketing.general");
        pnlDoubleOptIn.GroupingText = GetString("newsletter_configuration.optin");

        // Register save button
        CurrentMaster.HeaderActions.AddAction(new SaveAction(this));
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Load newsletter configuration
        LoadData();
    }


    protected void LoadData()
    {
        // Get newsletter object and check if exists
        NewsletterInfo newsletterObj = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
        EditedObject = newsletterObj;

        // Initialize template selectors
        int siteId = newsletterObj.NewsletterSiteID;
        string whereTemplate = "TemplateType='{0}' AND TemplateSiteID=" + siteId;

        subscriptionTemplate.WhereCondition = String.Format(whereTemplate, EmailTemplateType.Subscription);
        unsubscriptionTemplate.WhereCondition = String.Format(whereTemplate, EmailTemplateType.Unsubscription);
        optInSelector.WhereCondition = String.Format(whereTemplate, EmailTemplateType.DoubleOptIn);

        issueTemplate.WhereCondition = String.Format(whereTemplate, EmailTemplateType.Issue);
        issueTemplate.WhereCondition = SqlHelperClass.AddWhereCondition(issueTemplate.WhereCondition, String.Format("TemplateID={0}", newsletterObj.NewsletterTemplateID), "OR");

        // Check if the newsletter is dynamic and adjust config dialog
        isDynamic = newsletterObj.NewsletterType.EqualsCSafe(NewsletterType.Dynamic, StringComparison.InvariantCultureIgnoreCase);

        // Display template/dynamic based newsletter config and online marketing config
        plcDynamic.Visible = isDynamic;
        plcTemplate.Visible = !isDynamic;
        plcTracking.Visible = TrackingEnabled;
        plcOM.Visible = OnlineMarketingEnabled;

        if (!RequestHelper.IsPostBack())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                // If user was redirected from newsletter_new.aspx, display the 'Changes were saved' message
                ShowChangesSaved();
            }

            // Fill config dialog with newsletter data
            GetNewsletterValues(newsletterObj);

            if (!isDynamic)
            {
                // Initialize issue template selector
                issueTemplate.Value = newsletterObj.NewsletterTemplateID.ToString();
            }
            else
            {
                // Check if dynamic newsletter subject is empty
                bool subjectEmpty = string.IsNullOrEmpty(newsletterObj.NewsletterDynamicSubject);
                radPageTitle.Checked = subjectEmpty;
                radFollowing.Checked = !subjectEmpty;
                txtSubject.Enabled = radFollowing.Checked;

                if (!subjectEmpty)
                {
                    txtSubject.Text = newsletterObj.NewsletterDynamicSubject;
                }

                txtNewsletterDynamicURL.Text = newsletterObj.NewsletterDynamicURL;

                TaskInfo task = TaskInfoProvider.GetTaskInfo(newsletterObj.NewsletterDynamicScheduledTaskID);
                if (task != null)
                {
                    chkSchedule.Checked = true;
                    schedulerInterval.Visible = true;
                    schedulerInterval.ScheduleInterval = task.TaskInterval;
                }
                else
                {
                    chkSchedule.Checked = false;
                    schedulerInterval.Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Save button action.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                {
                    SaveData();
                }
                break;
        }
    }


    /// <summary>
    /// Saves configuration changes.
    /// </summary>
    protected void SaveData()
    {
        // Check "configure" permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "configure"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "configure");
        }

        string scheduledInterval = null;
        if (isDynamic && chkSchedule.Checked)
        {
            // Get scheduled interval for dynamic newsletter
            scheduledInterval = schedulerInterval.ScheduleInterval;
        }

        string errorMessage = ValidateNewsletterValues();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        NewsletterInfo newsletterObj = NewsletterInfoProvider.GetNewsletterInfo(txtNewsletterName.Text.Trim(), CMSContext.CurrentSiteID);

        // Newsletter's code name must be unique
        if (newsletterObj != null && newsletterObj.NewsletterID != newsletterId)
        {
            ShowError(GetString("Newsletter_Edit.NewsletterNameExists"));
            return;
        }

        if (newsletterObj == null)
        {
            newsletterObj = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
        }

        SetNewsletterValues(newsletterObj);

        // Check if subscription template was selected
        int subscriptionTemplateValue = ValidationHelper.GetInteger(subscriptionTemplate.Value, 0);
        if (subscriptionTemplateValue == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoSubscriptionTemplateSelected"));
            return;
        }
        newsletterObj.NewsletterSubscriptionTemplateID = subscriptionTemplateValue;

        // Check if double opt-in template was selected
        if (chkEnableOptIn.Checked)
        {
            int optInTemplateValue = ValidationHelper.GetInteger(optInSelector.Value, 0);
            if (optInTemplateValue == 0)
            {
                ShowError(GetString("Newsletter_Edit.NoOptInTemplateSelected"));
                return;
            }
            newsletterObj.NewsletterOptInTemplateID = optInTemplateValue;
        }
        else
        {
            newsletterObj.NewsletterOptInTemplateID = 0;
        }

        // Check if unsubscription template was selected
        int unsubscriptionTemplateValue = ValidationHelper.GetInteger(unsubscriptionTemplate.Value, 0);
        if (unsubscriptionTemplateValue == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoUnsubscriptionTemplateSelected"));
            return;
        }
        newsletterObj.NewsletterUnsubscriptionTemplateID = unsubscriptionTemplateValue;

        // ID of scheduled task which should be deleted
        int deleteScheduledTaskId = 0;

        if (isDynamic)
        {
            newsletterObj.NewsletterType = NewsletterType.Dynamic;
            newsletterObj.NewsletterDynamicSubject = radFollowing.Checked ? txtSubject.Text : string.Empty;

            if ((String.IsNullOrEmpty(txtNewsletterDynamicURL.Text.Trim())))
            {
                // Dynamic URL cannot be empty
                ShowError(GetString("menuitem_edit.errorpageurlempty"));
                return;
            }

            newsletterObj.NewsletterDynamicURL = txtNewsletterDynamicURL.Text.Trim();

            if (chkSchedule.Checked)
            {
                // Set info for scheduled task
                TaskInfo task = GetDynamicNewsletterTask(newsletterObj);

                if (!schedulerInterval.CheckOneDayMinimum())
                {
                    // If problem occurred while setting schedule interval
                    ShowError(GetString("Newsletter_Edit.NoDaySelected"));
                    return;
                }

                if (!IsValidDate(SchedulingHelper.DecodeInterval(scheduledInterval).StartTime))
                {
                    ShowError(GetString("Newsletter.IncorrectDate"));
                    return;
                }

                task.TaskInterval = scheduledInterval;

                task.TaskNextRunTime = SchedulingHelper.GetNextTime(task.TaskInterval, new DateTime(), new DateTime());
                task.TaskDisplayName = GetString("DynamicNewsletter.TaskName") + newsletterObj.NewsletterDisplayName;
                task.TaskName = "DynamicNewsletter_" + newsletterObj.NewsletterName;
                // Set task for processing in external service
                task.TaskAllowExternalService = true;
                task.TaskUseExternalService = (SchedulingHelper.UseExternalService && NewsletterHelper.UseExternalServiceForDynamicNewsletters(CMSContext.CurrentSiteName));
                TaskInfoProvider.SetTaskInfo(task);
                newsletterObj.NewsletterDynamicScheduledTaskID = task.TaskID;
            }
            else
            {
                if (newsletterObj.NewsletterDynamicScheduledTaskID > 0)
                {
                    // Store task ID for deletion
                    deleteScheduledTaskId = newsletterObj.NewsletterDynamicScheduledTaskID;
                }
                newsletterObj.NewsletterDynamicScheduledTaskID = 0;
                schedulerInterval.Visible = false;
            }
        }
        else
        {
            newsletterObj.NewsletterType = NewsletterType.TemplateBased;

            // Check if issue template was selected
            int issueTemplateValue = ValidationHelper.GetInteger(issueTemplate.Value, 0);
            if (issueTemplateValue == 0)
            {
                ShowError(GetString("Newsletter_Edit.NoEmailTemplateSelected"));
                return;
            }
            newsletterObj.NewsletterTemplateID = issueTemplateValue;
        }

        // Save changes to DB
        NewsletterInfoProvider.SetNewsletterInfo(newsletterObj);
        if (deleteScheduledTaskId > 0)
        {
            // Delete scheduled task if schedule mail-outs were unchecked
            TaskInfoProvider.DeleteTaskInfo(deleteScheduledTaskId);
        }

        ShowChangesSaved();

        // Refresh header with display name
        ScriptHelper.RefreshTabHeader(Page, GetString("Newsletter_Header.Configuration"));
    }


    protected void radSubject_CheckedChanged(object sender, EventArgs e)
    {
        txtSubject.Enabled = radFollowing.Checked;
    }


    protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
    {
        // Show/hide scheduler
        schedulerInterval.Visible = chkSchedule.Checked;
    }


    protected void chkEnableOptIn_CheckedChanged(object sender, EventArgs e)
    {
        // Show/hide double opt-in options
        plcOptIn.Visible = chkEnableOptIn.Checked;
    }


    /// <summary>
    /// Initializes config form.
    /// </summary>
    /// <param name="newsletter">Newsletter object</param>
    private void GetNewsletterValues(NewsletterInfo newsletter)
    {
        txtNewsletterDisplayName.Text = newsletter.NewsletterDisplayName;
        txtNewsletterName.Text = newsletter.NewsletterName;
        txtNewsletterSenderName.Text = newsletter.NewsletterSenderName;
        txtNewsletterSenderEmail.Text = newsletter.NewsletterSenderEmail;
        txtNewsletterBaseUrl.Text = newsletter.NewsletterBaseUrl;
        txtNewsletterUnsubscribeUrl.Text = newsletter.NewsletterUnsubscribeUrl;
        txtDraftEmails.Text = newsletter.NewsletterDraftEmails;
        chkUseEmailQueue.Checked = newsletter.NewsletterUseEmailQueue;
        chkEnableResending.Checked = newsletter.NewsletterEnableResending;

        subscriptionTemplate.Value = newsletter.NewsletterSubscriptionTemplateID.ToString();
        unsubscriptionTemplate.Value = newsletter.NewsletterUnsubscriptionTemplateID.ToString();

        if (TrackingEnabled)
        {
            chkTrackOpenedEmails.Checked = newsletter.NewsletterTrackOpenEmails;
            chkTrackClickedLinks.Checked = newsletter.NewsletterTrackClickedLinks;
        }

        if (OnlineMarketingEnabled)
        {
            chkLogActivity.Checked = newsletter.NewsletterLogActivity;
        }

        chkEnableOptIn.Checked = plcOptIn.Visible = newsletter.NewsletterEnableOptIn;
        optInSelector.Value = newsletter.NewsletterOptInTemplateID;
        txtOptInURL.Text = newsletter.NewsletterOptInApprovalURL;
        chkSendOptInConfirmation.Checked = newsletter.NewsletterSendOptInConfirmation;
    }


    /// <summary>
    /// Validates newsletter config form.
    /// </summary>
    /// <returns>Returns error message in case of an error</returns>
    private string ValidateNewsletterValues()
    {
        return new Validator()
            .NotEmpty(txtNewsletterDisplayName.Text, GetString("general.requiresdisplayname"))
            .NotEmpty(txtNewsletterName.Text, GetString("Newsletter_Edit.ErrorEmptyName"))
            .NotEmpty(txtNewsletterSenderName.Text, GetString("Newsletter_Edit.ErrorEmptySenderName"))
            .NotEmpty(txtNewsletterSenderEmail.Text, GetString("Newsletter_Edit.ErrorEmptySenderEmail"))
            .IsEmail(txtNewsletterSenderEmail.Text.Trim(), GetString("Newsletter_Edit.ErrorEmailFormat"))
            .IsCodeName(txtNewsletterName.Text, GetString("general.invalidcodename"))
            .Result;
    }


    /// <summary>
    /// Sets newsletter object from config form data.
    /// </summary>
    /// <param name="newsletterObj">Newsletter object</param>
    private void SetNewsletterValues(NewsletterInfo newsletterObj)
    {
        newsletterObj.NewsletterDisplayName = txtNewsletterDisplayName.Text.Trim();
        newsletterObj.NewsletterName = txtNewsletterName.Text.Trim();
        newsletterObj.NewsletterSenderName = txtNewsletterSenderName.Text.Trim();
        newsletterObj.NewsletterSenderEmail = txtNewsletterSenderEmail.Text.Trim();
        newsletterObj.NewsletterBaseUrl = txtNewsletterBaseUrl.Text.Trim();
        newsletterObj.NewsletterUnsubscribeUrl = txtNewsletterUnsubscribeUrl.Text.Trim();
        newsletterObj.NewsletterDraftEmails = txtDraftEmails.Text;
        newsletterObj.NewsletterUseEmailQueue = chkUseEmailQueue.Checked;
        newsletterObj.NewsletterEnableResending = chkEnableResending.Checked;
        newsletterObj.NewsletterTrackOpenEmails = TrackingEnabled && chkTrackOpenedEmails.Checked;
        newsletterObj.NewsletterTrackClickedLinks = TrackingEnabled && chkTrackClickedLinks.Checked;
        newsletterObj.NewsletterLogActivity = OnlineMarketingEnabled && chkLogActivity.Checked;
        newsletterObj.NewsletterEnableOptIn = chkEnableOptIn.Checked && (ValidationHelper.GetInteger(optInSelector.Value, 0) > 0);
        newsletterObj.NewsletterOptInApprovalURL = txtOptInURL.Text.Trim();
        newsletterObj.NewsletterSendOptInConfirmation = chkSendOptInConfirmation.Checked;
    }


    /// <summary>
    /// Returns existing or new task info object.
    /// </summary>
    /// <param name="newsletterObj">Newsletter object</param>
    private static TaskInfo GetDynamicNewsletterTask(NewsletterInfo newsletterObj)
    {
        return TaskInfoProvider.GetTaskInfo(newsletterObj.NewsletterDynamicScheduledTaskID) ?? CreateDynamicNewsletterTask(newsletterObj);
    }


    /// <summary>
    /// Creates new task info object for given newsletter.
    /// </summary>
    /// <param name="newsletterObj">Newsletter object</param>
    private static TaskInfo CreateDynamicNewsletterTask(NewsletterInfo newsletterObj)
    {
        return new TaskInfo()
                   {
                       TaskAssemblyName = "CMS.Newsletter",
                       TaskClass = "CMS.Newsletter.DynamicNewsletterSender",
                       TaskEnabled = true,
                       TaskLastResult = string.Empty,
                       TaskSiteID = CMSContext.CurrentSiteID,
                       TaskData = newsletterObj.NewsletterGUID.ToString(),
                       TaskType = ScheduledTaskTypeEnum.System
                   };
    }


    /// <summary>
    /// Checks if given date is valid.
    /// </summary>
    /// <param name="date">Date</param>
    private static bool IsValidDate(DateTime date)
    {
        return (date > SqlDateTime.MinValue.Value) && (date < SqlDateTime.MaxValue.Value);
    }

    #endregion
}