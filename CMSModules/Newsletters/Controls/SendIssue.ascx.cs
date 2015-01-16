using System;
using System.Data;
using System.Data.SqlTypes;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Controls_SendIssue : CMSAdminControl
{
    #region "Properties"

    /// <summary>
    /// ID of newsletter issue that should be sent, required for template based newsletters.
    /// </summary>
    public int IssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Newsletter ID, required for dynamic newsletters.
    /// </summary>
    public int NewsletterID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Schedule newsletter mail-out' option should be displayed.
    /// </summary>
    public bool ShowScheduler
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Send draft' option should be displayed.
    /// </summary>
    public bool ShowSendDraft
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Send later' option should be displayed.
    /// </summary>
    public bool ShowSendLater
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || ((IssueID <= 0) && (NewsletterID <= 0)))
        {
            return;
        }

        if (!RequestHelper.IsPostBack())
        {
            int newsletterId = NewsletterID;
            if (newsletterId <= 0)
            {
                IssueInfo issue = IssueInfoProvider.GetIssueInfo(IssueID);
                if (issue != null)
                {
                    newsletterId = issue.IssueNewsletterID;
                }
                else
                {
                    StopProcessing = true;
                    return;
                }
            }

            // Fill draft emails box
            NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
            if (newsletter != null)
            {
                txtSendDraft.Text = newsletter.NewsletterDraftEmails;
            }
            else
            {
                StopProcessing = true;
            }
        }

        calendarControl.DateTimeTextBox.CssClass = "EditingFormCalendarTextBox";
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Show/hide specific send options
        plcSendScheduled.Visible = ShowScheduler;
        plcSendDraft.Visible = ShowSendDraft;
        plcSendLater.Visible = ShowSendLater;
    }


    protected void radGroupSend_CheckedChanged(object sender, EventArgs e)
    {
        calendarControl.Enabled = radSchedule.Checked;
        txtSendDraft.Enabled = radSendDraft.Checked;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Sends the issue according to sending options.
    /// </summary>
    public bool SendIssue()
    {
        bool result = radSendLater.Checked;

        if (IssueID > 0)
        {
            // Depending on action chosen, send the issue
            if (radSendNow.Checked)
            {
                result = SendNow();
            }
            else if (radSchedule.Checked)
            {
                result = SendScheduled();
            }
            else if (radSendDraft.Checked)
            {
                result = SendDraft();
            }
        }

        return result;
    }


    /// <summary>
    /// Sends issue immediately.
    /// </summary>
    private bool SendNow()
    {
        RemovePreviousTaskAndSendIssue(IssueID, DateTime.Now);
        return true;
    }


    /// <summary>
    /// Schedules mail-out of the issue to the future.
    /// </summary>
    private bool SendScheduled()
    {
        if (IsValidDate(calendarControl.SelectedDateTime))
        {
            RemovePreviousTaskAndSendIssue(IssueID, calendarControl.SelectedDateTime);
        }
        else
        {
            ErrorMessage = GetString("newsletter.incorrectdate");
        }

        return String.IsNullOrEmpty(ErrorMessage);
    }


    /// <summary>
    /// Sends issue to testing email addresses.
    /// </summary>
    /// <returns>True when everything went ok</returns>
    private bool SendDraft()
    {
        string draftEmails = txtSendDraft.Text.Trim();

        if (String.IsNullOrEmpty(draftEmails))
        {
            ErrorMessage = GetString("newsletter.recipientsmissing");
        }
        else if (!ValidationHelper.AreEmails(draftEmails))
        {
            ErrorMessage = GetString("newsletter.wrongemailformat");
        }
        else
        {
            IssueInfoProvider.SendIssue(IssueID, draftEmails);
        }

        return String.IsNullOrEmpty(ErrorMessage);
    }

    
    /// <summary>
    /// Removes all scheduled tasks for the given issue.
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    /// <param name="when">DateTime when to send</param>
    private void RemovePreviousTaskAndSendIssue(int issueId, DateTime when)
    {
        IssueInfo issue = IssueInfoProvider.GetIssueInfo(IssueID);
        if (issue != null)
        {
            // Remove all previously scheduled tasks (if any)
            IssueInfoProvider.DeleteMailoutTask(issue.IssueGUID, issue.IssueSiteID);
            // Schedule new task for new mailout time
            IssueInfoProvider.SendIssue(issue, when);
        }
    }


    /// <summary>
    /// Decides about DateTime validity.
    /// </summary>
    /// <param name="date">DateTime to valid</param>
    /// <returns>True when DateTime is valid</returns>
    private static bool IsValidDate(DateTime date)
    {
        return (date > SqlDateTime.MinValue.Value) && (date < SqlDateTime.MaxValue.Value);
    }

    #endregion
}