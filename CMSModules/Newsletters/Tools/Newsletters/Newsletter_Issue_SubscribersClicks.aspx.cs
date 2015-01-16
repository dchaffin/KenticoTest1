using System;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

/// <summary>
/// Displays a table of subscribers who clicked a link in a specified issue.
/// </summary>
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SubscribersClicks : CMSToolsModalPage
{
    #region "Variables"

    private int linkId;

    // Default page size 15
    private const int PAGESIZE = 15;

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Check the license
        if (!string.IsNullOrEmpty(DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), string.Empty)))
        {
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.Newsletters);
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Newsletter", CMSContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Newsletter");
        }

        CurrentUserInfo user = CMSContext.CurrentUser;

        // Check permissions for CMS Desk -> Tools -> Newsletter
        if (!user.IsAuthorizedPerUIElement("CMS.Tools", "Newsletter"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Tools", "Newsletter");
        }

        // Check 'NewsletterRead' permission
        if (!user.IsAuthorizedPerResource("CMS.Newsletter", "Read"))
        {
            RedirectToCMSDeskAccessDenied("CMS.Newsletter", "Read");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("newsletter_issue_subscribersclicks.title");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Newsletter/ViewParticipants.png");

        linkId = QueryHelper.GetInteger("linkid", 0);
        if (linkId == 0)
        {
            RequestHelper.EndResponse();
        }

        LinkInfo link = LinkInfoProvider.GetLinkInfo(linkId);
        EditedObject = link;

        IssueInfo issue = IssueInfoProvider.GetIssueInfo(link.LinkIssueID);
        EditedObject = issue;

        // Prevent accessing issues from sites other than current site
        if (issue.IssueSiteID != CMSContext.CurrentSiteID)
        {
            RedirectToResourceNotAvailableOnSite("Issue with ID " + link.LinkIssueID);
        }

        QueryDataParameters parameters = null;
        string where = string.Empty;

        // Link's issue is the main A/B test issue
        if (issue.IssueIsABTest && !issue.IssueIsVariant)
        {
            // Prepare base where condition
            where = "LinkID IN (" + link.LinkID.ToString();

            // Get A/B test and its winner issue ID
            ABTestInfo test = ABTestInfoProvider.GetABTestInfoForIssue(issue.IssueID);
            if (test != null)
            {
                // Get ID of the same link from winner issue
                InfoDataSet<LinkInfo> winnerLink = LinkInfoProvider.GetLinks(string.Format("LinkIssueID={0} AND LinkTarget=N'{1}' AND LinkDescription=N'{2}'", test.TestWinnerIssueID, link.LinkTarget, link.LinkDescription), null, 1, "LinkID");
                if (!DataHelper.DataSourceIsEmpty(winnerLink))
                {
                    string winnerLinkID = ValidationHelper.GetString(DataHelper.GetDataRowValue(winnerLink.Tables[0].Rows[0], "LinkID"), string.Empty);
                    if (!string.IsNullOrEmpty(winnerLinkID))
                    {
                        // Add link ID of winner issue link
                        where += "," + winnerLinkID;
                    }
                }
            }

            // Close where condition
            where += ")";
        }
        else
        {
            // Filter by Link ID (from querystring)
            parameters = new QueryDataParameters();
            parameters.Add("@LinkID", linkId);
        }

        UniGrid.QueryParameters = parameters;
        UniGrid.WhereCondition = SqlHelperClass.AddWhereCondition(fltOpenedBy.WhereCondition, where);
        UniGrid.Pager.DefaultPageSize = PAGESIZE;
        UniGrid.Pager.ShowPageSize = false;
        UniGrid.FilterLimit = 1;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = (DataRowView)parameter;
        int subscriberId = 0;

        switch (sourceName)
        {
            case "name":
                subscriberId = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "SubscriberID"), 0);
                string name = null;
                if (subscriberId == 0)
                {
                    // Get full name for contact group member (contact)
                    name = ValidationHelper.GetString(DataHelper.GetDataRowValue(row.Row, "SubscriberFullName"), string.Empty);

                    // Return encoded name
                    return HTMLHelper.HTMLEncode(name);
                }
                else
                {
                    // Add the field transformation control that handles the translation
                    var tr = new ObjectTransformation("newsletter.subscriber", subscriberId);
                    tr.Transformation = "SubscriberFullName";

                    return tr;
                }

            case "email":
                subscriberId = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "SubscriberID"), 0);
                string email = null;
                if (subscriberId == 0)
                {
                    // Get email for contact group member (contact)
                    email = ValidationHelper.GetString(DataHelper.GetDataRowValue(row.Row, "SubscriberEmail"), string.Empty);
                }
                else
                {
                    SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(subscriberId);
                    if (subscriber != null)
                    {
                        if (subscriber.SubscriberType == null)
                        {
                            // Get email for classic subscriber
                            email = subscriber.SubscriberEmail;
                        }
                        else
                        {
                            switch (subscriber.SubscriberType)
                            {
                                case PredefinedObjectType.USER:
                                    UserInfo user = UserInfoProvider.GetUserInfo(subscriber.SubscriberRelatedID);
                                    if (user != null)
                                    {
                                        // Get email for user subscriber
                                        email = user.Email;
                                    }
                                    break;
                                case PredefinedObjectType.CONTACT:
                                    DataSet ds = ModuleCommands.OnlineMarketingGetContactForNewsletters(subscriber.SubscriberRelatedID, "ContactEmail");
                                    if (!DataHelper.DataSourceIsEmpty(ds))
                                    {
                                        // Get email from contact subscriber
                                        email = ValidationHelper.GetString(ds.Tables[0].Rows[0]["ContactEmail"], string.Empty);
                                    }
                                    break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(email))
                {
                    // Return encoded email
                    email = HTMLHelper.HTMLEncode(email);
                }

                return email;

            default:
                return parameter;
        }
    }

    #endregion
}