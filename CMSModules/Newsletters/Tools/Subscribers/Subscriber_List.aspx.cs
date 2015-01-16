using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

// Title
[Title("Objects/Newsletter_Subscriber/object.png", "newsletters.subscribers", "CMS_Newsletter_Subscribers", HelpName = "helpTopic")]
public partial class CMSModules_Newsletters_Tools_Subscribers_Subscriber_List : CMSNewsletterSubscribersPage
{
    #region "Variables"

    private int mBounceLimit;


    private bool mBounceInfoAvailable;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        SiteInfo currentSite = CMSContext.CurrentSite;
        mBounceLimit = NewsletterHelper.BouncedEmailsLimit(currentSite.SiteName);
        mBounceInfoAvailable = NewsletterHelper.MonitorBouncedEmails(currentSite.SiteName);

        // Add subscriber link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("Subscriber_List.NewItemCaption");
        actions[0, 3] = ResolveUrl("Subscriber_New.aspx");
        actions[0, 5] = GetImageUrl("Objects/Newsletter_Subscriber/add.png");

        CurrentMaster.HeaderActions.Actions = actions;

        // Initialize unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        UniGrid.WhereCondition = "SubscriberSiteID = " + currentSite.SiteID;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide columns with bounced emails if bounce info is not available
        UniGrid.NamedColumns["blocked"].Visible =
            UniGrid.NamedColumns["bounces"].Visible = mBounceInfoAvailable;

        if (UniGrid.RowsCount > 0)
        {
            int i = 0;
            DataView view = (DataView)UniGrid.GridView.DataSource;
            foreach (DataRow row in view.Table.Rows)
            {
                // Hide object menu for other than normal subscribers
                if (ValidationHelper.GetString(DataHelper.GetDataRowValue(row, "SubscriberType"), "") != "")
                {
                    if ((UniGrid.GridView.Rows[i].Cells.Count > 0) && (UniGrid.GridView.Rows[i].Cells[0].Controls.Count > 4)
                        && (UniGrid.GridView.Rows[i].Cells[0].Controls[4] is ContextMenuContainer))
                    {
                        UniGrid.GridView.Rows[i].Cells[0].Controls[4].Visible = false;
                    }
                }

                i++;
            }
        }
    }


    /// <summary>
    /// Unigrid external databound event handler.
    /// </summary>
    protected object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "block":
                return SetBlockAction(sender, ((parameter as GridViewRow).DataItem) as DataRowView);

            case "unblock":
                return SetUnblockAction(sender, ((parameter as GridViewRow).DataItem) as DataRowView);

            case "email":
                return GetEmail(parameter as DataRowView);

            case "blocked":
                return GetBlocked(parameter as DataRowView);

            case "bounces":
                return GetBounces(parameter as DataRowView);

            default:
                return null;
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
                // Edit subscriber
            case "edit":
                Edit(actionArgument);
                break;

                // Delete subscriber
            case "delete":
                Delete(actionArgument);
                break;

                // Block subscriber
            case "block":
                Block(actionArgument);
                break;

                // Un-block subscriber
            case "unblock":
                Unblock(actionArgument);
                break;
        }
    }


    /// <summary>
    /// Displays/hides block action button in unigrid.
    /// </summary>
    private object SetBlockAction(object sender, DataRowView rowView)
    {
        int bounces = GetBouncesFromRow(rowView);

        (sender as ImageButton).Visible = mBounceInfoAvailable && !IsRoleORContactGroupSubscriber(rowView)
            && ((mBounceLimit > 0 && bounces < mBounceLimit) || (mBounceLimit == 0 && bounces < int.MaxValue));

        return null;
    }


    /// <summary>
    /// Displays/hides un-block action button in unigrid.
    /// </summary>
    private object SetUnblockAction(object sender, DataRowView rowView)
    {
        int bounces = GetBouncesFromRow(rowView);

        (sender as ImageButton).Visible = mBounceInfoAvailable && !IsRoleORContactGroupSubscriber(rowView)
            && ((mBounceLimit > 0 && bounces >= mBounceLimit) || (mBounceLimit == 0 && bounces == int.MaxValue));

        return null;
    }


    /// <summary>
    /// Returns subscriber's e-mail address.
    /// </summary>
    private object GetEmail(DataRowView rowView)
    {
        // Try to get subscriber email
        string email = ValidationHelper.GetString(rowView.Row["SubscriberEmail"], string.Empty);
        if (string.IsNullOrEmpty(email))
        {
            // Try to get user email
            email = ValidationHelper.GetString(rowView.Row["Email"], string.Empty);
        }

        if (string.IsNullOrEmpty(email) && ValidationHelper.GetString(rowView.Row["SubscriberType"], string.Empty).EqualsCSafe(PredefinedObjectType.CONTACT))
        {
            // Add the field transformation control that handles the translation
            var tr = new ObjectTransformation("om.contact", ValidationHelper.GetInteger(rowView.Row["SubscriberRelatedID"], 0));
            tr.Transformation = "ContactEmail";

            return tr;
        }

        return email;
    }


    /// <summary>
    /// Returns colored yes/no or nothing according to subscriber's blocked info.
    /// </summary>
    private string GetBlocked(DataRowView rowView)
    {
        // Do not handle if bounce email monitoring is not available
        if (!mBounceInfoAvailable)
        {
            return null;
        }

        // If bounce limit is not a natural number, then the feature is considered disabled
        if (mBounceLimit < 0)
        {
            return UniGridFunctions.ColoredSpanYesNoReversed(false);
        }

        if (IsRoleORContactGroupSubscriber(rowView))
        {
            return null;
        }

        int bounces = GetBouncesFromRow(rowView);

        return UniGridFunctions.ColoredSpanYesNoReversed((mBounceLimit > 0 && bounces >= mBounceLimit) || (mBounceLimit == 0 && bounces == int.MaxValue));
    }


    /// <summary>
    /// Returns number of bounces or nothing according to subscriber's bounce info.
    /// </summary>
    private string GetBounces(DataRowView rowView)
    {
        // Do not handle if bounce email monitoring is not available
        if (!mBounceInfoAvailable)
        {
            return null;
        }

        int bounces = GetBouncesFromRow(rowView);

        if (bounces == 0 || bounces == int.MaxValue || IsRoleORContactGroupSubscriber(rowView))
        {
            return null;
        }

        return bounces.ToString();
    }


    /// <summary>
    /// Edit selected subscriber.
    /// </summary>
    private void Edit(object actionArgument)
    {
        URLHelper.Redirect("Subscriber_Frameset.aspx?subscriberid=" + ValidationHelper.GetString(actionArgument, string.Empty));
    }


    /// <summary>
    /// Delete selected subscriber.
    /// </summary>
    private void Delete(object actionArgument)
    {
        CheckAuthorization();
        SubscriberInfoProvider.DeleteSubscriberInfo(ValidationHelper.GetInteger(actionArgument, 0));
    }


    /// <summary>
    /// Block selected subscriber.
    /// </summary>
    private void Block(object actionArgument)
    {
        CheckAuthorization();
        SubscriberInfoProvider.BlockSubscriber(ValidationHelper.GetInteger(actionArgument, 0));
    }


    /// <summary>
    /// Un-block selected subscriber.
    /// </summary>
    private void Unblock(object actionArgument)
    {
        CheckAuthorization();
        SubscriberInfoProvider.UnblockSubscriber(ValidationHelper.GetInteger(actionArgument, 0));
    }


    /// <summary>
    /// Checks if the user has permission to manage subscribers.
    /// </summary>
    private static void CheckAuthorization()
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "managesubscribers");
        }
    }


    /// <summary>
    /// Returns if type of the subscriber is "cms.role" or "om.contactgroup".
    /// </summary>
    private static bool IsRoleORContactGroupSubscriber(DataRowView rowView)
    {
        string type = ValidationHelper.GetString(DataHelper.GetDataRowValue(rowView.Row, "SubscriberType"), string.Empty);
        return (type.EqualsCSafe(SiteObjectType.ROLE, true) || type.EqualsCSafe("om.contactgroup", true));
    }


    /// <summary>
    /// Returns number of bounces of the subscriber.
    /// </summary>
    private static int GetBouncesFromRow(DataRowView rowView)
    {
        return ValidationHelper.GetInteger(DataHelper.GetDataRowValue(rowView.Row, "SubscriberBounces"), 0);
    }

    #endregion
}