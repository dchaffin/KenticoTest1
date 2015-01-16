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
/// Displays a table of issue openers (subscribers who have opened the email with the specified issue).
/// </summary>
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_OpenedBy : CMSToolsModalPage
{
    #region "Variables"
    
    // Default page size 15
    private const int PAGESIZE = 15;

    private bool isMainABTestIssue;

    private int issueId = 0;

    private IssueInfo winnerIssue = null;

    #endregion


    #region "Page events"

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
        CurrentMaster.Title.TitleText = GetString("newsletter_issue_openedby.title");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Newsletter/module.png");

        issueId = QueryHelper.GetInteger("issueid", 0);
        if (issueId == 0)
        {
            RequestHelper.EndResponse();
        }

        IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueId);
        EditedObject = issue;

        // Prevent accessing issues from sites other than current site
        if (issue.IssueSiteID != CMSContext.CurrentSiteID)
        {
            RedirectToResourceNotAvailableOnSite("Issue with ID " + issueId);
        }
        
        // Issue is the main A/B test issue
        isMainABTestIssue = issue.IssueIsABTest && !issue.IssueIsVariant;
        if (isMainABTestIssue)
        {
            // Initialize variant selector in the filter
            fltOpenedBy.IssueId = issue.IssueID;

            if (RequestHelper.IsPostBack())
            {
                // Get issue ID from variant selector
                issueId = fltOpenedBy.IssueId;
            }

            // Reset ID for main issue, grid will show data from main and winner variant issues
            if (issueId == issue.IssueID)
            {
                issueId = 0;
            }
        }

        QueryDataParameters parameters = null;
        if (issueId > 0)
        {
            parameters = new QueryDataParameters();
            // Filter by Issue ID (from querystring or variant selector in case of A/B test issue)
            parameters.Add("@IssueID", issueId);
        }

        UniGrid.QueryParameters = parameters;
        UniGrid.WhereCondition = fltOpenedBy.WhereCondition;
        UniGrid.Pager.DefaultPageSize = PAGESIZE;
        UniGrid.Pager.ShowPageSize = false;
        UniGrid.FilterLimit = 1;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
        UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
    }

    #endregion


    #region "Other methods"

    protected void UniGrid_OnBeforeDataReload()
    {
        // Display 'Variant name' column if data from all variants should be retrieved
        UniGrid.NamedColumns["variants"].Visible = isMainABTestIssue && (issueId < 0);
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = null;
        if (parameter is DataRowView)
        {
            row = (DataRowView)parameter;
        }
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

            case "variantname":
                if (!isMainABTestIssue)
                {
                    return null;
                }

                IssueInfo issue = IssueInfoProvider.GetIssueInfo(ValidationHelper.GetInteger(parameter, 0));
                string variantName = null;

                if (issue != null)
                {
                    if (!issue.IssueIsVariant)
                    {
                        // Get varinat name from the winner issue
                        if (winnerIssue == null)
                        {
                            ABTestInfo test = ABTestInfoProvider.GetABTestInfoForIssue(issue.IssueID);
                            if (test != null)
                            {
                                // Get A/B test winner variant
                                winnerIssue = IssueInfoProvider.GetIssueInfo(test.TestWinnerIssueID);
                            }
                        }

                        if (winnerIssue != null)
                        {
                            // Get variant name
                            variantName = winnerIssue.IssueVariantName;
                        }
                    }
                    else
                    {
                        // Get variant name
                        variantName = issue.IssueVariantName;
                    }
                }

                return variantName;

            default:
                return parameter;
        }
    }

    #endregion
}