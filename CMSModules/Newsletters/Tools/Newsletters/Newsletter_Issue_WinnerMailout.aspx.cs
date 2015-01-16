using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

// Set edited object
[EditedObject("newsletter.issue", "issueid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_WinnerMailout : CMSNewsletterNewslettersPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Newsletter/winner24.png", false);
        CurrentMaster.Title.TitleText = GetString("newsletter_winnermailout.title");
        CurrentMaster.Title.ShowFullScreenButton = false;

        RegisterModalPageScripts();
        RegisterEscScript();
        // Register script for refreshing parent page
        ScriptHelper.RegisterStartupScript(Page, GetType(), "RefreshParent", "function RefreshPage() {if((wopener)&&(wopener.RefreshPage!=null)){wopener.RefreshPage();}}", true);

        btnSend.Click += new EventHandler(btnSend_Click);

        // Show variant name in info message
        string variantName = GetString("general.na");
        IssueInfo winner = (IssueInfo)EditedObject;

        if (winner != null)
        {
            // Prevent accessing issues from sites other than current site
            if (winner.IssueSiteID != CMSContext.CurrentSiteID)
            {
                RedirectToResourceNotAvailableOnSite("Issue with ID " + winner.IssueID);
            }

            variantName = HTMLHelper.HTMLEncode(winner.IssueVariantName);
        }
        lblInfo.Text = String.Format(GetString("newsletter_winnermailout.question"), variantName);
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
        // Validate date/time (blank date/time textbox is allowed)
        if ((dtpMailout.SelectedDateTime == DateTimeHelper.ZERO_TIME) && !String.IsNullOrEmpty(dtpMailout.DateTimeTextBox.Text.Trim()))
        {
            ShowError(GetString("newsletterissue_send.invaliddatetime"));
            return;
        }

        IssueInfo winner = (IssueInfo)EditedObject;
        int parentIssueId = winner.IssueVariantOfIssueID;

        // Get A/B test info
        ABTestInfo abi = ABTestInfoProvider.GetABTestInfoForIssue(parentIssueId);
        if (abi != null)
        {
            IssueInfo issue = IssueInfoProvider.GetIssueInfo(parentIssueId);

            // Check if winner was selected and sent
            if (abi.TestWinnerIssueID != 0)
            {
                if ((issue.IssueStatus == IssueStatusEnum.Finished) || (issue.IssueStatus == IssueStatusEnum.Sending))
                {
                    // Winner was already sent
                    CloseDialogAndRefreshParent();
                    return;
                }
            }

            // Update A/B test info and winner selection task (if exist)
            abi.TestWinnerOption = ABTestWinnerSelectionEnum.Manual;
            IssueHelper.EnsureWinnerSelectionTask(abi, issue, false);

            abi.TestSelectWinnerAfter = 0;
            abi.TestWinnerSelected = DateTime.Now;
            abi.TestWinnerIssueID = winner.IssueID;
            ABTestInfoProvider.SetABTestInfo(abi);

            if ((issue != null) && (winner != null))
            {
                // Copy data from winner to parent
                IssueHelper.CopyIssueProperties(winner, issue, "issuesubject;issuetext;issuetemplateid;issueshowinnewsletterarchive;issuesendername;issuesenderemail");
                IssueInfoProvider.SetIssueInfo(issue);

                // Remove previous scheduled task of this issue
                IssueInfoProvider.DeleteMailoutTask(issue.IssueGUID, issue.IssueSiteID);

                DateTime mailoutTime = dtpMailout.SelectedDateTime;
                if (mailoutTime < DateTime.Now)
                {                    
                    // Send parent issue now
                    IssueInfoProvider.SendIssue(parentIssueId, DateTime.Now);
                }
                else
                {
                    // Send winner at specified time
                    IssueInfoProvider.SendIssue(parentIssueId, mailoutTime);
                }
            }
        }

        // Close dialog and refresh parent page
        CloseDialogAndRefreshParent();
    }


    private void CloseDialogAndRefreshParent()
    {
        ScriptHelper.RegisterStartupScript(this, GetType(), "ClosePage", "RefreshPage(); setTimeout('CloseDialog()',200);", true);
    }
}