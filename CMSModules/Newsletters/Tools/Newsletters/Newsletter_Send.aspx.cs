using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Send : CMSNewsletterNewslettersPage
{
    #region "Variables"

    private int newsletterId;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        newsletterId = QueryHelper.GetInteger("newsletterid", 0);
        if (newsletterId == 0)
        {
            return;
        }

        sendElem.NewsletterID = newsletterId;
        btnSend.Enabled = true;

        if (btnSend.Action != null)
        {
            btnSend.Action.Text = GetString("general.send");
            btnSend.Action.SmallImageUrl = GetImageUrl("CMSModules/CMS_Newsletter/send.png");
        }
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
        // Check permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "authorissues"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "authorissues");
        }

        // Generate new issue
        try
        {
            int issueId = EmailQueueManager.GenerateDynamicIssue(newsletterId);
            if (issueId <= 0)
            {
                return;
            }

            // Set ID of generated issue
            sendElem.IssueID = issueId;

            // Send the issue according to selected options
            if (sendElem.SendIssue())
            {
                ShowConfirmation(GetString("Newsletter_Send.SuccessfullySent"));
            }
            else
            {
                ShowError(sendElem.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ShowError(GetString("Newsletter_Send.ErrorSent") + ex.Message);
        }
    }

    #endregion
}