using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

/// <summary>
/// Displays a list of issues for a specified newsletter.
/// </summary>
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_List : CMSNewsletterNewslettersPage
{
    #region "Variables"

    private int newsletterId;
    private bool mBounceMonitoringEnabled;
    private bool mOnlineMarketingEnabled;
    private bool mTrackingEnabled;
    private bool mABTestEnabled;
    private NewsletterInfo mNewsletter;
    private bool contactGroupSubscribersExist;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        newsletterId = QueryHelper.GetInteger("newsletterid", 0);
        if (newsletterId == 0)
        {
            RequestHelper.EndResponse();
        }

        // Get newsletter object and check its existence
        mNewsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
        EditedObject = mNewsletter;

        string siteName = CMSContext.CurrentSiteName;
        mBounceMonitoringEnabled = NewsletterHelper.MonitorBouncedEmails(siteName);
        mOnlineMarketingEnabled = NewsletterHelper.OnlineMarketingAvailable(siteName);
        mTrackingEnabled = NewsletterHelper.IsTrackingAvailable();
        mABTestEnabled = NewsletterHelper.IsABTestingAvailable();

        ScriptHelper.RegisterDialogScript(this);

        string scriptBlock = string.Format(@"
            function RefreshPage() {{ document.location.replace(document.location); }}
            function NewItem(id) {{ modalDialog('{0}?newsletterid=' + id, 'NewsletterNewIssue', screen.availWidth - 10, screen.availHeight - 80); }}
            function ShowOpenedBy(id) {{ modalDialog('{1}?issueid=' + id, 'NewsletterIssueOpenedBy', '900px', '700px');  return false; }}
            function ViewClickedLinks(id) {{ modalDialog('{2}?issueid=' + id, 'NewsletterTrackedLinks', '900px', '700px'); return false; }}",
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_New.aspx"),
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_OpenedBy.aspx"),
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_TrackedLinks.aspx"));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Actions", scriptBlock, true);

        // Initialize unigrid
        UniGrid.WhereCondition = String.Format("IssueNewsletterID={0} AND IssueVariantOfIssueID IS NULL", newsletterId);
        UniGrid.ZeroRowsText = GetString("Newsletter_Issue_List.NoIssuesFound");
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;

        contactGroupSubscribersExist = !DataHelper.DataSourceIsEmpty(SubscriberNewsletterInfoProvider.GetSubscriptionsByType(PredefinedObjectType.CONTACTGROUP, newsletterId, null, 1, "SubscriberID"));

        // Initialize header actions
        InitHeaderActions();
    }


    protected void InitHeaderActions()
    {
        if (!mNewsletter.NewsletterType.EqualsCSafe(NewsletterType.Dynamic))
        {
            bool isAuthorized = CurrentUser.IsAuthorizedPerResource("CMS.Newsletter", "AuthorIssues");

            CurrentMaster.HeaderActions.AddAction(new HeaderAction()
            {
                ControlType = HeaderActionTypeEnum.LinkButton,
                Text = GetString("Newsletter_Issue_List.NewItemCaption"),
                Tooltip = GetString("Newsletter_Issue_List.NewItemCaption"),
                OnClientClick = string.Format("NewItem({0}); return false;", newsletterId),
                ImageUrl = GetImageUrl("Objects/Newsletter_Issue/add.png"),
                Enabled = isAuthorized
            });
        }
    }


    protected void UniGrid_OnBeforeDataReload()
    {
        // Hide opened emails if tracking is not available
        UniGrid.NamedColumns["openedemails"].Visible = mTrackingEnabled;

        // Hide bounced emails info if monitoring disabled or tracking is not available
        UniGrid.NamedColumns["bounces"].Visible = mBounceMonitoringEnabled;

        // Hide A/B test column for dynamic newsletters or if A/B testing is not available
        UniGrid.NamedColumns["isabtest"].Visible = mABTestEnabled && mNewsletter.NewsletterType.EqualsCSafe(NewsletterType.TemplateBased);
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="sourceName">Name of the source</param>
    /// <param name="parameter">The data row</param>
    /// <returns>Formatted value to be used in the UniGrid</returns>
    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "issuesubject":
                return HTMLHelper.HTMLEncode(MacroResolver.RemoveSecurityParameters(parameter.ToString(), true, null));

            case "issueopenedemails":
                return GetOpenedEmails(parameter as DataRowView);
            case "issuestatus":
                IssueStatusEnum status = IssueStatusEnum.Idle;
                if ((parameter != DBNull.Value) && (parameter != null))
                {
                    status = (IssueStatusEnum)parameter;
                }
                return GetString(String.Format("newsletterissuestatus." + Convert.ToString(status)));

            case "edit":
                if (sender is ImageButton)
                {
                    if (mNewsletter.NewsletterType.EqualsCSafe(NewsletterType.Dynamic) && !mNewsletter.NewsletterEnableResending)
                    {
                        // Hide 'edit' action if newsletter is dynamic and resending is disabled
                        ImageButton imageButton = sender as ImageButton;
                        imageButton.Style.Add(HtmlTextWriterStyle.Display, "none");
                    }
                }
                return sender;

            case "viewclickedlinks":
                if (sender is ImageButton)
                {
                    // Hide 'view clicked links' action if tracking is not available or if the issue has no tracked links
                    ImageButton imageButton = sender as ImageButton;
                    if (!mTrackingEnabled)
                    {
                        imageButton.Style.Add(HtmlTextWriterStyle.Display, "none");
                    }
                    else
                    {
                        GridViewRow gvr = parameter as GridViewRow;
                        if (gvr != null)
                        {
                            DataRowView drv = gvr.DataItem as DataRowView;
                            if (drv != null)
                            {
                                int issueId = ValidationHelper.GetInteger(drv["IssueID"], 0);
                                // Try to get one tracked link (only ID) of the issue
                                InfoDataSet<LinkInfo> links = LinkInfoProvider.GetLinks("LinkIssueID=" + issueId, null, 1, "LinkID");
                                if (DataHelper.DataSourceIsEmpty(links))
                                {
                                    imageButton.Style.Add(HtmlTextWriterStyle.Display, "none");
                                }
                            }
                        }
                    }
                }
                return sender;

            default:
                return parameter;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                DeleteIssue(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }


    /// <summary>
    /// Gets a clickable opened emails counter based on the values from datasource.
    /// </summary>
    /// <param name="rowView">A <see cref="DataRowView" /> that represents one row from UniGrid's source</param>
    /// <returns>A link with detailed statistics about opened emails</returns>
    private string GetOpenedEmails(DataRowView rowView)
    {
        // Get issue ID
        int issueId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueID"), 0);

        // Get opened emails count from issue record
        int openedEmails = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueOpenedEmails"), 0);
        if (mOnlineMarketingEnabled && contactGroupSubscribersExist)
        {
            // Get number of emails opened by contact group members
            openedEmails += OpenedEmailInfoProvider.GetContactGroupOpens(issueId);
        }

        // Add winner variant data if it is an A/B test and a winner has been selected
        if (ValidationHelper.GetBoolean(DataHelper.GetDataRowViewValue(rowView, "IssueIsABTest"), false))
        {
            openedEmails += GetWinnerVariantOpenes(issueId);
        }

        if (openedEmails > 0)
        {
            return string.Format(@"<a href=""#"" onclick=""ShowOpenedBy({0})"">{1}</a>", issueId, openedEmails);
        }
        else
        {
            return "0";
        }
    }


    /// <summary>
    /// Gets number of opened e-mails of winner variant issue.
    /// </summary>
    /// <param name="issueId">ID of parent issue</param>
    private int GetWinnerVariantOpenes(int issueId)
    {
        int openedEmails = 0;

        ABTestInfo test = ABTestInfoProvider.GetABTestInfoForIssue(issueId);
        if ((test != null) && (test.TestWinnerIssueID > 0))
        {
            IssueInfo winner = IssueInfoProvider.GetIssueInfo(test.TestWinnerIssueID);
            if (winner != null)
            {
                // Get opened emails count from winner issue
                openedEmails += winner.IssueOpenedEmails;

                if (mOnlineMarketingEnabled)
                {
                    // Get number of emails opened by contact group members
                    openedEmails += OpenedEmailInfoProvider.GetContactGroupOpens(winner.IssueID);
                }
            }
        }

        return openedEmails;
    }


    /// <summary>
    /// Deletes an issue specified by its ID (if authorized).
    /// </summary>
    /// <param name="issueId">Issue's ID</param>
    private static void DeleteIssue(int issueId)
    {
        // Delete issue from database (if authorized)
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "authorissues"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "authorissues");
        }


        IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueId);
        if (issue != null)
        {
            // Delete variant issues if any
            if (issue.IssueIsABTest && (issue.IssueVariantOfIssueID == 0))
            {
                // Get all variants (children) and delete their mailout tasks
                List<IssueABVariantItem> variants = IssueHelper.GetIssueVariants(issue, null);
                IssueInfo issueVar = null;
                if ((variants != null) && (variants.Count > 0))
                {
                    foreach (IssueABVariantItem variant in variants)
                    {
                        issueVar = IssueInfoProvider.GetIssueInfo(variant.IssueID);
                        if (issueVar != null)
                        {
                            IssueInfoProvider.DeleteMailoutTask(issueVar.IssueGUID, issueVar.IssueSiteID);
                        }
                    }
                }
            }
            // Delete issue
            IssueInfoProvider.DeleteMailoutTask(issue.IssueGUID, issue.IssueSiteID);
            IssueInfoProvider.DeleteIssueInfo(issue);
        }
    }

    #endregion
}