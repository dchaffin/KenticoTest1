using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

// Set edited object
[EditedObject("newsletter.issue", "issueid")]
[Title("CMSModules/CMS_Newsletter/senddraft.png", "newsletterissue_content.senddraft", "send_draft", HelpName = "helpTopic")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SendDraft : CMSToolsModalPage
{
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

        // Check permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "authorissues"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "authorissues");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get newsletter issue and check its existence
        IssueInfo issue = (IssueInfo)EditedObject;

        if (issue != null)
        {
            if (!RequestHelper.IsPostBack())
            {
                // Fill draft emails box
                NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(issue.IssueNewsletterID);
                EditedObject = newsletter;
                txtSendDraft.Text = newsletter.NewsletterDraftEmails;
            }
        }
        else
        {
            btnSend.Enabled = false;
        }
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
        // Check field for emptyness
        if (string.IsNullOrEmpty(txtSendDraft.Text) || (txtSendDraft.Text.Trim().Length == 0))
        {
            ShowError(GetString("newsletter.recipientsmissing"));
        }
        // Check e-mail address validity
        else if (!ValidationHelper.AreEmails(txtSendDraft.Text.Trim()))
        {
            ShowError(GetString("newsletter.wrongemailformat"));
        }
        else
        {
            // Send the draft e-mails
            IssueInfoProvider.SendIssue(((IssueInfo)EditedObject).IssueID, txtSendDraft.Text.Trim());

            // Close the dialog
            ScriptHelper.RegisterStartupScript(this, GetType(), "ClosePage", "if (CloseDialog) { CloseDialog(); }", true);
        }
    }
}