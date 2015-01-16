using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_ShowPreview : CMSNewsletterNewslettersPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Guid subscriberGuid = QueryHelper.GetGuid("subscriberguid", Guid.Empty);
        int newsletterIssueId = QueryHelper.GetInteger("issueid", 0);

        // Get newsletter issue
        IssueInfo issue = IssueInfoProvider.GetIssueInfo(newsletterIssueId);
        if (issue == null)
        {
            return;
        }

        // Get subscriber
        SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(subscriberGuid, CMSContext.CurrentSiteID);

        // Get the newsletter
        NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(issue.IssueNewsletterID);
        if (news == null)
        {
            return;
        }

        // Get site default culture
        string culture = CultureHelper.GetDefaultCulture(CMSContext.CurrentSiteName);

        // Ensure preview in default site culture
        // Keep old culture
        CultureInfo oldUICulture = Thread.CurrentThread.CurrentUICulture;

        // Set current culture
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        string htmlPage = String.Empty;
        try
        {
            if (subscriber != null)
            {
                // Get subscriber's member (different for user or role subscribers)
                SortedDictionary<int, SubscriberInfo> subscribers = SubscriberInfoProvider.GetSubscribers(subscriber, 1, 0, news.NewsletterID);
                foreach (KeyValuePair<int, SubscriberInfo> item in subscribers)
                {
                    // Get 1st subscriber's member
                    SubscriberInfo sb = item.Value;

                    htmlPage = IssueInfoProvider.GetEmailBody(issue, news, null, sb, true, CMSContext.CurrentSiteName, null, null, null);
                }
            }

            if (string.IsNullOrEmpty(htmlPage))
            {
                htmlPage = IssueInfoProvider.GetEmailBody(issue, news, null, null, true, CMSContext.CurrentSiteName, null, null, null);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            // Set back to old culture
            Thread.CurrentThread.CurrentUICulture = oldUICulture;
        }

        Response.Clear();
        Response.Write(htmlPage);

        RequestHelper.EndResponse();
    }
}