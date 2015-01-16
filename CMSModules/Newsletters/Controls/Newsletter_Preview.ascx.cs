using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Controls_Newsletter_Preview : CMSUserControl
{
    #region "Constants"

    // Maximal number of subscribers for preview
    private const int MAX_PREVIEW_SUBSCRIBERS = 20;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get Issue object
        IssueInfo issue = (IssueInfo)CMSContext.EditedObject;
        if ((issue == null) || (issue.IssueSiteID != CMSContext.CurrentSiteID))
        {
            return;
        }

        // Get newsletter object
        NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(issue.IssueNewsletterID);

        string subject = null;
        IssueHelper ih = new IssueHelper();
        string siteName = CMSContext.CurrentSiteName;

        InitGUI();

        // Get specific number of subscribers subscribed to the newsletter
        string where = "SubscriberID IN (SELECT SubscriberID FROM Newsletter_SubscriberNewsletter WHERE NewsletterID=" + issue.IssueNewsletterID + " AND (SubscriptionApproved=1 OR SubscriptionApproved IS NULL) AND (SubscriptionEnabled=1 OR SubscriptionEnabled IS NULL))";
        DataSet subscribers = SubscriberInfoProvider.GetSubscribers(where, null, MAX_PREVIEW_SUBSCRIBERS, null);

        string script;
        if (!DataHelper.DataSourceIsEmpty(subscribers))
        {
            // Limit max subscribers count to number of rows
            int maxCount = subscribers.Tables[0].Rows.Count;

            // Generate javascript based on subscribers
            script = string.Format(
@"var currentSubscriberIndex = 0;
var newsletterIssueId ={0};
var guid = new Array({1});
var email = new Array({1});
var subject = new Array({1});
var subscribers = new Array(guid, email);", issue.IssueID, maxCount);

            SubscriberInfo subscriber = null;
            SubscriberInfo sbMember = null;
            SortedDictionary<int, SubscriberInfo> subMembers = null;
            string infoLine = null;

            for (int i = 0; i < maxCount; i++)
            {
                // Get subscriber
                subscriber = new SubscriberInfo(subscribers.Tables[0].Rows[i]);
                if (subscriber != null)
                {
                    // Insert subscriber GUID
                    script = string.Format("{0} guid[{1}] = '{2}'; \n ", script, i, subscriber.SubscriberGUID);

                    // Get subscriber's member (different for user, role or contact group subscribers)
                    subMembers = SubscriberInfoProvider.GetSubscribers(subscriber, 1, 0, issue.IssueNewsletterID);
                    if ((subMembers != null) && (subMembers.Count > 0))
                    {
                        foreach (KeyValuePair<int, SubscriberInfo> item in subMembers)
                        {
                            // Get 1st subscriber's member
                            sbMember = item.Value;
                            if (sbMember != null)
                            {
                                // Create information line
                                infoLine = ScriptHelper.GetString(sbMember.SubscriberEmail, false);

                                // Add info about subscriber type
                                if (sbMember.SubscriberType.EqualsCSafe(SiteObjectType.USER, true))
                                {
                                    infoLine = string.Format("{0} ({1})", infoLine, GetString("objecttype.cms_user").ToLowerCSafe());
                                }
                                else if (sbMember.SubscriberType.EqualsCSafe(SiteObjectType.ROLE, true))
                                {
                                    infoLine = string.Format("{0} ({1})", infoLine, GetString("objecttype.cms_role").ToLowerCSafe());
                                }
                                else if (sbMember.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true))
                                {
                                    infoLine = string.Format("{0} ({1})", infoLine, GetString("objecttype.om_contactgroup").ToLowerCSafe());
                                }
                                else if (sbMember.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACT, true))
                                {
                                    infoLine = string.Format("{0} ({1})", infoLine, GetString("objecttype.om_contact").ToLowerCSafe());
                                }

                                script = string.Format("{0}email[{1}] = '{2}'; \n ", script, i, infoLine);

                                // Resolve dynamic field macros ({%FirstName%}, {%LastName%}, {%Email%})
                                if (ih.LoadDynamicFields(sbMember, news, null, issue, true, siteName, null, null, null))
                                {
                                    subject = ih.ResolveDynamicFieldMacros(issue.IssueSubject);
                                }

                                // Create resolved subject
                                script = string.Format("{0}subject[{1}] = {2}; \n ", script, i, ScriptHelper.GetString(HTMLHelper.HTMLEncode(subject)));
                            }
                        }
                    }
                    else
                    {
                        script = string.Format("{0}email[{1}] = '{2}'; \n ", script, i, "(N/A)");

                        // Resolve dynamic field macros (newsletter and issue)
                        if (ih.LoadDynamicFields(null, news, null, issue, true, siteName, null, null, null))
                        {
                            subject = ih.ResolveDynamicFieldMacros(issue.IssueSubject);
                        }

                        // Create resolved subject
                        script = string.Format("{0}subject[{1}] = {2}; \n ", script, i, ScriptHelper.GetString(HTMLHelper.HTMLEncode(subject)));
                    }
                }
            }
        }
        else
        {
            // Resolve dynamic field macros ({%FirstName%}, {%LastName%}, {%Email%}) in subject
            if (ih.LoadDynamicFields(null, news, null, issue, true, siteName, null, null, null))
            {
                subject = ih.ResolveDynamicFieldMacros(issue.IssueSubject);
            }

            // Generate void javascript 
            script = string.Format(
@"var currentSubscriberIndex = 0;
var newsletterIssueId ={0};
var guid = new Array(1);
var email = new Array(1);
var subject = new Array(1);
subject[0] = '{1}';
var subscribers = new Array(guid, email);
guid[0] = 0;
email[0] = '(N/A)';", issue.IssueID, subject);
        }
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PreviewData", ScriptHelper.GetScript(script));

        if (!RequestHelper.IsPostBack())
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "LoadPreview" + ClientID, ScriptHelper.GetScript("pageLoad();"));
        }
    }


    /// <summary>
    /// GUI initialization.
    /// </summary>
    protected void InitGUI()
    {
        lnkNext.Text = GetString("general.next") + " >";
        lblNext.Text = GetString("general.next") + " >";
        lnkPrevious.Text = "< " + GetString("general.back");
        lblPrevious.Text = "< " + GetString("general.back");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        string elemString = "var lblEmail = '" + lblEmail.ClientID + "';\n" +
                            "var lblPrev = '" + lblPrevious.ClientID + "';\n" +
                            "var lnkPrev = '" + lnkPrevious.ClientID + "';\n" +
                            "var lblNext = '" + lblNext.ClientID + "';\n" +
                            "var lnkNext = '" + lnkNext.ClientID + "';\n" +
                            "var lblSubj = '" + lblSubjectValue.ClientID + "';\n";

        // Register client IDs of the elements
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PreviewElems", ScriptHelper.GetScript(elemString));
    }
}