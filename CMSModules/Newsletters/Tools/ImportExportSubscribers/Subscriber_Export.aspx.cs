using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

[Title("CMSModules/CMS_Newsletter/exportsubscriber.png", "newsletters.exportsubscribers", "CMS_Newsletter_ExportSubscribers", HelpName = "helpTopic")]
[Security(Resource = "CMS.Newsletter", UIElements = "ExportSubscribers")]
public partial class CMSModules_Newsletters_Tools_ImportExportSubscribers_Subscriber_Export : CMSNewsletterPage
{
    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize newsletter selector
        usNewsletters.WhereCondition = "NewsletterSiteID = " + CMSContext.CurrentSiteID;

        // Initialize update panel progress
        pnlUpdate.ShowProgress = true;
        string text = GetString("general.exporting") + "...";
        pnlUpdate.ProgressHTML = "<div class=\"UP\"><img src=\"" + UIHelper.GetImageUrl(Page, "Design/Preloaders/preload16.gif") + "\" alt=\"" + text + "\" tooltip=\"" + text + "\"  /><span>" + text + "</span></div>";

        // Initialize radio button list items text
        rblExportList.Items[0].Text = GetString("newsletter.allsubscribers");
        rblExportList.Items[1].Text = GetString("general.approved");
        rblExportList.Items[2].Text = GetString("general.notapproved");

        if (!RequestHelper.IsPostBack())
        {
            rblExportList.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Handles export button click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void btnExport_Click(object sender, EventArgs e)
    {
        // Check "manage subscribers" permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "managesubscribers");
        }

        // Get selected newsletters
        List<int> newsletterIds = new List<int>();
        string values = ValidationHelper.GetString(usNewsletters.Value, null);
        if (!String.IsNullOrEmpty(values))
        {
            string[] newItems = values.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                foreach (string item in newItems)
                {
                    newsletterIds.Add(ValidationHelper.GetInteger(item, 0));
                }
            }
        }

        // Export subscribers
        string subscribers = null;
        if (CMSContext.CurrentSite != null)
        {
            subscribers = SubscriberInfoProvider.ExportSubscribersFromSite(newsletterIds, CMSContext.CurrentSiteID, true, ValidationHelper.GetInteger(rblExportList.SelectedValue, 0));
        }

        // No subscribers exported
        if (string.IsNullOrEmpty(subscribers))
        {
            ShowInformation(GetString("Subscriber_Export.noSubscribersExported"));
            txtExportSub.Enabled = false;
        }
        else
        {
            ShowInformation(GetString("Subscriber_Export.subscribersExported"));
            txtExportSub.Enabled = true;
        }

        txtExportSub.Text = subscribers;
    }
}