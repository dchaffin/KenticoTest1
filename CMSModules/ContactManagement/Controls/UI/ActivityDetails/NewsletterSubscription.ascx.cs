using System;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_NewsletterSubscription : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || !ModuleEntry.IsModuleLoaded(ModuleEntry.NEWSLETTER))
        {
            return false;
        }

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.NEWSLETTER_SUBSCRIBING:
            case PredefinedActivityType.NEWSLETTER_UNSUBSCRIBING:
                break;
            default:
                return false;
        }

        // Get newsletter name
        int nesletterId = ai.ActivityItemID;
        GeneralizedInfo iinfo = ModuleCommands.NewsletterGetNewsletterInfo(nesletterId);
        if (iinfo != null)
        {
            string subject = ValidationHelper.GetString(iinfo.GetValue("NewsletterDisplayName"), null);
            ucDetails.AddRow("om.activitydetails.newsletter", subject);
        }

        // Get issue subject
        int issueId = ai.ActivityItemDetailID;
        iinfo = ModuleCommands.NewsletterGetNewsletterIssueInfo(issueId);
        if (iinfo != null)
        {
            string subject = ValidationHelper.GetString(iinfo.GetValue("IssueSubject"), null);
            ucDetails.AddRow("om.activitydetails.newsletterissue", MacroResolver.RemoveSecurityParameters(subject, true, null));
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}