using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_Newsletters_Controls_SubscriptionApproval : CMSUserControl
{
    #region "Variables"

    private string mSuccessfulApprovalText = null;
    private string mUnsuccessfulApprovalText = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets successful approval text.
    /// </summary>
    public string SuccessfulApprovalText
    {
        get
        {
            return mSuccessfulApprovalText;
        }
        set
        {
            mSuccessfulApprovalText = value;
        }
    }


    /// <summary>
    /// Gets or sets unsuccessful approval text.
    /// </summary>
    public string UnsuccessfulApprovalText
    {
        get
        {
            return mUnsuccessfulApprovalText;
        }
        set
        {
            mUnsuccessfulApprovalText = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If StopProcessing flag is set, do nothing
        if (StopProcessing)
        {
            Visible = false;
            return;
        }

        string subscriptionHash = QueryHelper.GetString("subscriptionhash", string.Empty);
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
                lblInfo.Text = ResHelper.GetString("newsletter.approval_failed");
                return;
            }
        }

        if (string.IsNullOrEmpty(subscriptionHash))
        {
            Visible = false;
            return;
        }

        // Try to approve subscription
        SubscriberInfoProvider.ApprovalResult result = SubscriberInfoProvider.ApproveSubscription(subscriptionHash, false, CMSContext.CurrentSiteName, datetime);

        switch (result)
        {
            // Approving subscription was successful
            case SubscriberInfoProvider.ApprovalResult.Success:
                if (!String.IsNullOrEmpty(SuccessfulApprovalText))
                {
                    lblInfo.Text = SuccessfulApprovalText;
                }
                else
                {
                    lblInfo.Text = ResHelper.GetString("newsletter.successful_approval");
                }

                // Log newsletter subscription activity
                SubscriberNewsletterInfo sni = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriptionHash);
                if ((sni != null) && sni.SubscriptionEnabled)
                {
                    NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(sni.NewsletterID);
                    SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(sni.SubscriberID);
                    if (sb != null)
                    {
                        // Under what contacs this subscriber belogs to?
                        int contactId = ActivityTrackingHelper.GetContactID(sb);
                        if (contactId > 0)
                        {
                            Activity activity = new ActivityNewsletterSubscribing(sb, news, CMSContext.ActivityEnvironmentVariables);
                            if (activity.Data != null)
                            {
                                activity.Data.ContactID = contactId;
                                activity.Log();
                            }
                        }
                    }
                }
                break;

            // Subscription was already approved
            case SubscriberInfoProvider.ApprovalResult.Failed:
                if (!String.IsNullOrEmpty(UnsuccessfulApprovalText))
                {
                    lblInfo.Text = UnsuccessfulApprovalText;
                }
                else
                {
                    lblInfo.Text = ResHelper.GetString("newsletter.approval_failed");
                }
                break;

            case SubscriberInfoProvider.ApprovalResult.TimeExceeded:
                if (!String.IsNullOrEmpty(UnsuccessfulApprovalText))
                {
                    lblInfo.Text = UnsuccessfulApprovalText;
                }
                else
                {
                    lblInfo.Text = ResHelper.GetString("newsletter.approval_timeexceeded");
                }
                break;


            // Subscription not found
            default:
            case SubscriberInfoProvider.ApprovalResult.NotFound:
                if (!String.IsNullOrEmpty(UnsuccessfulApprovalText))
                {
                    lblInfo.Text = UnsuccessfulApprovalText;
                }
                else
                {
                    lblInfo.Text = ResHelper.GetString("newsletter.approval_invalid");
                }
                break;
        }
    }

    #endregion
}