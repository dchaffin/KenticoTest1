using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.EmailEngine;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Title("CMSModules/CMS_EmailQueue/module.png", "newsletters.emailqueue", "CMS_Newsletter_EmailQueue", HelpName = "helpTopic")]
[Security(Resource = "CMS.Newsletter", UIElements = "EmailQueue")]
public partial class CMSModules_Newsletters_Tools_EmailQueue_NewsletterEmailQueue : CMSNewsletterPage
{
    #region "Private variables"

    private int siteId;

    private bool emailsEnabled = false;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        siteId = CMSContext.CurrentSiteID;

        emailsEnabled = EmailHelper.Settings.EmailsEnabled(CMSContext.CurrentSiteName);
        emailsEnabled |= SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSGenerateNewsletters");

        // Display disabled information
        if (!emailsEnabled)
        {
            ShowWarning(GetString("NewsletterEmailQueue_List.EmailsDisabled"), null, null);
        }

        // Initialize unigrid
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.WhereCondition = "EmailSiteID = @SiteID";

        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@SiteID", siteId);

        gridElem.QueryParameters = parameters;

        InitializeActionMenu();
    }

    #endregion


    #region "Unigrid events"

    /// <summary>
    /// Handles Unigrid's OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "subject":
                return TextHelper.LimitLength(HTMLHelper.HTMLEncode(parameter.ToString()), 50);

            case "result":
                return TextHelper.LimitLength(HTMLHelper.HTMLEncode(parameter.ToString()), 50);

            case "subjecttooltip":
            case "resulttooltip":
                return parameter.ToString().Replace("\r\n", "<br />").Replace("\n", "<br />");
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "configure"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "configure");
        }

        switch (actionName.ToLowerCSafe())
        {
            case "resend":
                // Resend an issue from the queue
                EmailQueueManager.ResendEmail(Convert.ToInt32(actionArgument));
                break;

            case "delete":
                // Delete EmailQueueItem object from database
                EmailQueueManager.DeleteEmailQueueItem(Convert.ToInt32(actionArgument));
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes action menu.
    /// </summary>
    protected void InitializeActionMenu()
    {
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionsList.Clear();

        // Resend all failed
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("NewsletterEmailQueue_List.ResendAllFailed"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("NewsletterEmailQueue_List.ResendAllFailedConfirmationMessage")) + ")) return false;",
            CommandName = "resendallfailed"
        });

        // Resend all
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("NewsletterEmailQueue_List.ResendAll"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("EmailQueue.ResendAllConfirmation")) + ")) return false;",
            CommandName = "resendall"
        });

        // Delete all failed
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("NewsletterEmailQueue_List.DeleteAllFailed"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("NewsletterEmailQueue_List.DeleteAllFailedConfirmationMessage")) + ")) return false;",
            CommandName = "deleteallfailed"
        });

        // Delete all
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("NewsletterEmailQueue_List.DeleteAll"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("NewsletterEmailQueue_List.DeleteAllConfirmationMessage")) + ")) return false;",
            CommandName = "deleteall"
        });

        // Refresh
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("general.refresh"),
            ImageUrl = GetImageUrl("CMSModules/CMS_EmailQueue/refresh.png"),
            CommandName = "refresh"
        });

        actions.ActionPerformed += HeaderActions_ActionPerformed;
        actions.PreRender += HeaderActions_PreRender;
    }

    #endregion


    #region "Header action events"

    protected void HeaderActions_PreRender(object sender, EventArgs e)
    {
        bool enabled = (gridElem.GridView.Rows.Count > 0);
        bool resending = enabled && (ThreadEmailSender.SendingThreads <= 0) && emailsEnabled;

        HeaderActions actions = CurrentMaster.HeaderActions;

        if (actions.ActionsList.Count > 4)
        {
            // Resend all failed
            actions.ActionsList[0].ImageUrl = resending ? GetImageUrl("CMSModules/CMS_EmailQueue/resendallfailed.png") : GetImageUrl("CMSModules/CMS_EmailQueue/resendallfailed_disabled.png");
            actions.ActionsList[0].Enabled = resending;

            // Resend all
            actions.ActionsList[1].ImageUrl = resending ? GetImageUrl("CMSModules/CMS_EmailQueue/resendselected.png") : GetImageUrl("CMSModules/CMS_EmailQueue/resendselected_disabled.png");
            actions.ActionsList[1].Enabled = resending;

            // Delete all failed
            actions.ActionsList[2].ImageUrl = enabled ? GetImageUrl("CMSModules/CMS_EmailQueue/resendall.png") : GetImageUrl("CMSModules/CMS_EmailQueue/resendall_disabled.png");
            actions.ActionsList[2].Enabled = enabled;

            // Delete all
            actions.ActionsList[3].ImageUrl = enabled ? GetImageUrl("CMSModules/CMS_EmailQueue/deleteallfailed.png") : GetImageUrl("CMSModules/CMS_EmailQueue/deleteallfailed_disabled.png");
            actions.ActionsList[3].Enabled = enabled;
        }
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check user permission (for complex operations only)
        if (e.CommandName != "refresh")
        {
            if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "configure"))
            {
                RedirectToCMSDeskAccessDenied("cms.newsletter", "configure");
            }
        }

        switch (e.CommandName.ToLowerCSafe())
        {
            case "resendall":
                EmailQueueManager.SendAllEmails(true, true, 0);
                gridElem.ReloadData();
                lblText.Text = "<strong>" + GetString("EmailQueue.SendingEmails") + "</strong>";
                break;

            case "resendallfailed":
                EmailQueueManager.SendAllEmails(true, false, 0);
                gridElem.ReloadData();
                lblText.Text = "<strong>" + GetString("EmailQueue.SendingEmails") + "</strong>";
                break;

            case "deleteall":
                EmailQueueManager.DeleteAllEmailQueueItems(siteId);
                gridElem.ReloadData();
                break;

            case "deleteallfailed":
                EmailQueueManager.DeleteFailedEmailQueueItems(siteId);
                gridElem.ReloadData();
                break;

            case "refresh":
                gridElem.ReloadData();
                break;
        }
    }

    #endregion
}