using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

// Set edited object and context help
[EditedObject("newsletter.issue", "issueid")]
[Title(null, null, "edit_tab", HelpName = "helpTopic")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Header : CMSNewsletterNewslettersPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get newsletter issue ID from querystring
        IssueInfo issue = EditedObject as IssueInfo;
        if (issue != null)
        {
            InitalizeMenu(issue);
        }
    }


    /// <summary>
    /// Initializes header menu.
    /// </summary>
    /// <param name="issue">Issue object</param>
    protected void InitalizeMenu(IssueInfo issue)
    {
        // Get newsletter
        NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(issue.IssueNewsletterID);
        if (news == null)
        {
            return;
        }

        // Initialize breadcrumbs
        CurrentPage.InitBreadcrumbs(2);
        CurrentPage.SetBreadcrumb(0, GetString("Newsletter_Header.Issues"), "~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_List.aspx?newsletterid=" + news.NewsletterID, "_parent", null);
        CurrentPage.SetBreadcrumb(1, MacroResolver.RemoveSecurityParameters(issue.IssueSubject, true, null), null, null, null);

        InitTabs("newsletterIssueContent");

        // Show only 'Send' tab for dynamic newsletter
        if (news.NewsletterType == NewsletterType.Dynamic)
        {
            SetTab(0, GetString("Newsletter_Issue_Header.Send"), "Newsletter_Issue_Send.aspx?issueid=" + issue.IssueID, "SetHelpTopic('helpTopic', 'send_tab');");

            // Set proper context help page
            SetHelp("send_tab", "helpTopic");
        }
        else
        {
            SetTab(0, GetString("General.Content"), "Newsletter_Issue_Edit.aspx?issueid=" + issue.IssueID, "SetHelpTopic('helpTopic', 'edit_tab');");

            // Display send page if the issue is A/B test or the issue may be re-sent or the issue has 'Idle' status
            bool displaySendPage = issue.IssueIsABTest || news.NewsletterEnableResending || (issue.IssueStatus == IssueStatusEnum.Idle);

            // Show 'Send' tab only to authorized users and if send page is allowed
            if (CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "authorissues") && displaySendPage)
            {
                SetTab(1, GetString("Newsletter_Issue_Header.Send"), "Newsletter_Issue_Send.aspx?issueid=" + issue.IssueID, issue.IssueIsABTest ? "SetHelpTopic('helpTopic', 'newsletterab_send');" : "SetHelpTopic('helpTopic', 'send_tab');");
            }
        }
    }
}