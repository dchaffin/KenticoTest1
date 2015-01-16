using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_Newsletters_CMSPages_Unsubscribe : CMSPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get data from query string
        Guid subscriberGuid = QueryHelper.GetGuid("subscriberguid", Guid.Empty);
        Guid newsletterGuid = QueryHelper.GetGuid("newsletterguid", Guid.Empty);
        string subscriptionHash = QueryHelper.GetString("subscriptionhash", string.Empty);
        Guid issueGuid = QueryHelper.GetGuid("issueGuid", Guid.Empty);
        int issueID = QueryHelper.GetInteger("issueid", 0);
        int contactId = QueryHelper.GetInteger("contactid", 0);
        bool unsubscribed = false;

        string requestTime = QueryHelper.GetString("datetime", string.Empty);
        DateTime datetime = DateTimeHelper.ZERO_TIME;

        // Get date and time
        if (!string.IsNullOrEmpty(requestTime))
        {
            try
            {
                datetime = DateTime.ParseExact(requestTime, SecurityHelper.EMAIL_CONFIRMATION_DATETIME_FORMAT, null);
            }
            catch
            {
                ShowError(GetString("newsletter.unsubscribefailed"));
                return;
            }
        }

        // Get site ID
        int siteId = 0;
        if (CMSContext.CurrentSite != null)
        {
            siteId = CMSContext.CurrentSiteID;
        }

        if ((subscriberGuid != Guid.Empty) && (newsletterGuid != Guid.Empty) && (siteId != 0))
        {
            SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(subscriberGuid, siteId);
            if (subscriber == null)
            {
                ShowError(GetString("Unsubscribe.SubscriberDoesNotExist"));
                return;
            }
            // Show error message if subscriber type is 'Role'
            if (!string.IsNullOrEmpty(subscriber.SubscriberType) && subscriber.SubscriberType.EqualsCSafe(SiteObjectType.ROLE, true))
            {
                ShowError(GetString("Unsubscribe.CannotUnsubscribeRole"));
                return;
            }

            NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterGuid, siteId);
            if (newsletter == null)
            {
                ShowError(GetString("Unsubscribe.NewsletterDoesNotExist"));
                return;
            }

            // Check if subscriber with given GUID is subscribed to specified newsletter
            if (SubscriberInfoProvider.IsSubscribed(subscriber.SubscriberID, newsletter.NewsletterID))
            {
                bool isSubscribed = true;

                if (string.IsNullOrEmpty(subscriber.SubscriberType) || !subscriber.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true))
                {
                    // Unsubscribe action
                    SubscriberInfoProvider.Unsubscribe(subscriber.SubscriberID, newsletter.NewsletterID);
                }
                else
                {
                    // Check if the contact group member has unsubscription activity for the specified newsletter
                    isSubscribed = (contactId > 0) && !ModuleCommands.OnlineMarketingIsContactUnsubscribed(contactId, newsletter.NewsletterID, siteId);
                }

                if (isSubscribed)
                {
                    // Log newsletter unsubscription activity
                    LogActivity(subscriber, 0, newsletter.NewsletterID, issueID, contactId);

                    // Display confirmation
                    ShowInformation(GetString("Unsubscribe.Unsubscribed"));
                    unsubscribed = true;
                }
                else
                {
                    // Contact group member is already unsubscribed
                    ShowError(GetString("Unsubscribe.NotSubscribed"));
                }
            }
            else
            {
                ShowError(GetString("Unsubscribe.NotSubscribed"));
            }
        }
        // Check if subscription approval hash is supplied
        else if (!string.IsNullOrEmpty(subscriptionHash))
        {
            SubscriberNewsletterInfo sni = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriptionHash);
            // Check if hash is valid
            if ((sni != null) && sni.SubscriptionEnabled)
            {
                SubscriberInfoProvider.ApprovalResult result = SubscriberInfoProvider.Unsubscribe(subscriptionHash, true, CMSContext.CurrentSiteName, datetime);

                switch (result)
                {
                    // Approving subscription was successful
                    case SubscriberInfoProvider.ApprovalResult.Success:
                        bool isSubscribed = true;

                        // Get subscriber
                        SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(sni.SubscriberID);
                        if ((subscriber != null) && !string.IsNullOrEmpty(subscriber.SubscriberType) && subscriber.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true))
                        {
                            // Check if the contact group member has unsubscription activity for the specified newsletter
                            isSubscribed = (contactId > 0) && !ModuleCommands.OnlineMarketingIsContactUnsubscribed(contactId, sni.NewsletterID, siteId);
                        }

                        if (isSubscribed)
                        {
                            // Log newsletter unsubscription activity
                            LogActivity(null, sni.SubscriberID, sni.NewsletterID, issueID, contactId);

                            // Display confirmation
                            ShowInformation(GetString("Unsubscribe.Unsubscribed"));
                            unsubscribed = true;
                        }
                        else
                        {
                            // Contact group member is already unsubscribed
                            ShowError(GetString("Unsubscribe.NotSubscribed"));
                        }
                        break;

                    // Subscription was already approved
                    case SubscriberInfoProvider.ApprovalResult.Failed:
                        ShowError(GetString("newsletter.unsubscribefailed"));
                        break;

                    case SubscriberInfoProvider.ApprovalResult.TimeExceeded:
                        ShowError(GetString("newsletter.approval_timeexceeded"));
                        break;

                    // Subscription not found
                    default:
                    case SubscriberInfoProvider.ApprovalResult.NotFound:
                        ShowError(GetString("Unsubscribe.NotSubscribed"));
                        break;
                }
            }
            else
            {
                ShowError(GetString("Unsubscribe.NotSubscribed"));
            }
        }
        else
        {
            if (subscriberGuid == Guid.Empty)
            {
                ShowError(GetString("Unsubscribe.SubscriberDoesNotExist"));
            }
            if (newsletterGuid == Guid.Empty)
            {
                ShowError(GetString("Unsubscribe.NewsletterDoesNotExist"));
            }
        }

        // Increase unsubscribed count
        if (unsubscribed)
        {
            // If Issue ID was provided
            if (issueID > 0)
            {
                IssueInfoProvider.Unsubscribe(issueID);
                return;
            }
            // Otherwise try using the Issue GUID
            if (issueGuid != Guid.Empty)
            {
                IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueGuid, siteId);
                if (issue == null)
                {
                    return;
                }

                IssueInfoProvider.Unsubscribe(issue.IssueID);
            }
        }
    }


    /// <summary>
    /// Logs activity for unsubscribing.
    /// </summary>
    /// <param name="sb">Subscriber (optional - can be null if subscriber ID is used)</param>
    /// <param name="subscriberId">Subscriber ID (optional - can be zero if subscriber object is used)</param>
    /// <param name="newsletterId">Newsletter ID</param>
    /// <param name="issueId">Issue ID</param>
    /// <param name="contactId">Contact ID is present if the mail is sent to a contact or a contact group</param>
    private void LogActivity(SubscriberInfo sb, int subscriberId, int newsletterId, int issueId, int contactId)
    {
        bool isFromContactGroup = !string.IsNullOrEmpty(sb.SubscriberType) && sb.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true);
        if (contactId <= 0)
        {
            contactId = ActivityTrackingHelper.GetContactID(sb);
        }

        if (sb == null)
        {
            sb = SubscriberInfoProvider.GetSubscriberInfo(subscriberId);
        }

        if (contactId > 0)
        {
            NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);

            Activity activity = new ActivityNewsletterUnsubscribing(sb, news, CMSContext.ActivityEnvironmentVariables);
            if (activity.Data != null)
            {
                activity.Data.ContactID = contactId;
                activity.Data.ItemDetailID = issueId;
                activity.Data.Value = isFromContactGroup ? "contactgroup(" + sb.SubscriberRelatedID + ")" : null;
                activity.Log();
            }
        }
    }

    #endregion
}