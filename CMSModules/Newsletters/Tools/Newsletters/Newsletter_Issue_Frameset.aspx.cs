using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Frameset : CMSNewsletterNewslettersPage
{
    protected string issueContentUrl = string.Empty;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get issue object
        int issueId = QueryHelper.GetInteger("issueid", 0);
        IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueId);

        if (issue != null)
        {
            // Get newsletter object
            NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(issue.IssueNewsletterID);
            if (news != null)
            {
                if (news.NewsletterType == NewsletterType.Dynamic)
                {
                    // Only send page is allowed for dynamic newsletters
                    issueContentUrl = "Newsletter_Issue_Send.aspx?issueid=" + issueId.ToString();
                }
                else
                {
                    issueContentUrl = "Newsletter_Issue_Edit.aspx?issueid=" + issueId.ToString();
                }
            }
        }

        if (string.IsNullOrEmpty(issueContentUrl))
        {
            issueContentUrl = "Newsletter_Issue_Edit.aspx";
        }
    }
}