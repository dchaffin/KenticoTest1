using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Tools_Subscribers_Subscriber_Subscriptions : CMSNewsletterSubscribersPage
{
    #region "Constants and variables"

    private const string SELECT = "SELECT";
    private const string UNSUBSCRIBE = "UNSUBSCRIBE";
    private const string SUBSCRIBE = "SUBSCRIBE";
    private const string APPROVE = "APPROVE";
    private const string REJECT = "REJECT";
    private const string REMOVE = "REMOVE";
    protected int subscriberId = 0;
    protected bool isRoleORContactGroup = false;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        subscriberId = QueryHelper.GetInteger("subscriberid", 0);
        // Get subscriber by its ID and check its existence
        SubscriberInfo subscriberObj = SubscriberInfoProvider.GetSubscriberInfo(subscriberId);
        EditedObject = subscriberObj;

        // Check if it is role or contact group subscriber
        isRoleORContactGroup = (subscriberObj.SubscriberRelatedID > 0) && (subscriberObj.SubscriberType != null) &&
                               (subscriberObj.SubscriberType.EqualsCSafe(SiteObjectType.ROLE, true) ||
                                subscriberObj.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true));

        // Initialize page controls
        chkRequireOptIn.TextAlign = TextAlign.Right;
        chkSendConfirmation.TextAlign = TextAlign.Right;

        // Initialize uniselector for newsletters
        selectNewsletter.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        selectNewsletter.UniSelector.OnItemsSelected += new EventHandler(UniSelector_OnItemsSelected);
        selectNewsletter.UniSelector.ReturnColumnName = "NewsletterID";
        selectNewsletter.UniSelector.ButtonImage = GetImageUrl("Objects/Newsletter_Newsletter/add.png");
        selectNewsletter.UniSelector.DialogLink.CssClass = "MenuItemEdit";
        selectNewsletter.ShowSiteFilter = false;
        selectNewsletter.ResourcePrefix = "newsletterselect";
        selectNewsletter.UniSelector.DialogButton.CssClass = "LongButton";
        selectNewsletter.IsLiveSite = false;

        // Initialize unigrid
        unigridNewsletters.WhereCondition = "SubscriberID = " + subscriberId.ToString();
        unigridNewsletters.OnAction += new OnActionEventHandler(unigridNewsletters_OnAction);
        unigridNewsletters.OnExternalDataBound += new OnExternalDataBoundEventHandler(unigridNewsletters_OnExternalDataBound);


        // Initialize mass actions
        if (drpActions.Items.Count == 0)
        {
            drpActions.Items.Add(new ListItem(GetString("general.selectaction"), SELECT));
            drpActions.Items.Add(new ListItem(GetString("newsletter.unsubscribelink"), UNSUBSCRIBE));
            drpActions.Items.Add(new ListItem(GetString("newsletter.renewsubscription"), SUBSCRIBE));
            drpActions.Items.Add(new ListItem(GetString("newsletter.approvesubscription"), APPROVE));
            drpActions.Items.Add(new ListItem(GetString("newsletter.rejectsubscription"), REJECT));
            drpActions.Items.Add(new ListItem(GetString("newsletter.deletesubscription"), REMOVE));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display/hide mass action dialog under the unigrid
        pnlActions.Visible = !DataHelper.DataSourceIsEmpty(unigridNewsletters.GridView.DataSource);
        // Display/hide double opt-in option
        plcRequireOptIn.Visible = !isRoleORContactGroup;
    }


    /// <summary>
    /// Uniselector item selected event handler.
    /// </summary>
    protected void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "ManageSubscribers"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "ManageSubscribers");
        }

        // Get new items from selector
        string newValues = ValidationHelper.GetString(selectNewsletter.Value, null);
        string[] newItems = null;
        SubscriberNewsletterInfo subscription = null;
        int newsletterId = 0;

        newItems = newValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (newItems != null)
        {
            // Get all selected newsletters
            foreach (string item in newItems)
            {
                newsletterId = ValidationHelper.GetInteger(item, 0);

                // Get subscription
                subscription = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriberId, newsletterId);

                // If not already subscribed
                if (subscription == null)
                {
                    // Subscribe role
                    if (isRoleORContactGroup)
                    {
                        SubscriberInfoProvider.Subscribe(subscriberId, newsletterId, DateTime.Now, chkSendConfirmation.Checked, false);
                    }
                    // Subscribe users and subscribers
                    else
                    {
                        SubscriberInfoProvider.Subscribe(subscriberId, newsletterId, DateTime.Now, chkSendConfirmation.Checked, chkRequireOptIn.Checked);
                    }
                }
            }
        }

        selectNewsletter.Value = null;
        unigridNewsletters.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Unigrid databound handler.
    /// </summary>
    protected object unigridNewsletters_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        bool approved = false;
        switch (sourceName.ToLowerCSafe())
        {
            case "subscribe":
                ((ImageButton)sender).Visible = !ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionEnabled"], true);
                break;

            case "unsubscribe":
                ((ImageButton)sender).Visible = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionEnabled"], true);
                break;

            case "status":
                return GetSubscriptionStatus(parameter as DataRowView);


            case "approve":
                approved = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionApproved"], false);
                if (approved)
                {
                    ImageButton button = ((ImageButton)sender);
                    button.Visible = false;
                }
                break;

            case "reject":
                approved = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionApproved"], false);
                if (!approved)
                {
                    ImageButton button = ((ImageButton)sender);
                    button.Visible = false;
                }
                break;
        }

        return null;
    }


    /// <summary>
    /// Unigrid newsletters action handler.
    /// </summary>
    protected void unigridNewsletters_OnAction(string actionName, object actionArgument)
    {
        // Check 'configure' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "ManageSubscribers"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "ManageSubscribers");
        }

        int newsletterId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            // Renew subscription
            case "subscribe":
                SubscriberInfoProvider.RenewSubscription(subscriberId, newsletterId, chkSendConfirmation.Checked);
                break;

            // Unsubscribe selected subscriber
            case "unsubscribe":
                SubscriberInfoProvider.Unsubscribe(subscriberId, newsletterId, chkSendConfirmation.Checked);
                break;

            // Unsubscribe selected subscriber
            case "remove":
                SubscriberInfoProvider.DeleteSubscription(subscriberId, newsletterId, chkSendConfirmation.Checked);
                break;

            // Approve selected subscription
            case "approve":
                {
                    SubscriberNewsletterInfo subscriptionInfo = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriberId, newsletterId);
                    if ((subscriptionInfo != null) && (!subscriptionInfo.SubscriptionApproved))
                    {
                        subscriptionInfo.SubscriptionApproved = true;
                        subscriptionInfo.SubscriptionApprovedWhen = DateTime.Now;
                        SubscriberNewsletterInfoProvider.SetSubscriberNewsletterInfo(subscriptionInfo);
                    }
                }
                break;

            // Reject selected subscription
            case "reject":
                {
                    SubscriberNewsletterInfo subscriptionInfo = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriberId, newsletterId);
                    if ((subscriptionInfo != null) && (subscriptionInfo.SubscriptionApproved))
                    {
                        subscriptionInfo.SubscriptionApproved = false;
                        subscriptionInfo.SubscriptionApprovedWhen = DateTime.MinValue;
                        SubscriberNewsletterInfoProvider.SetSubscriberNewsletterInfo(subscriptionInfo);
                    }
                }
                break;
        }
    }


    /// <summary>
    /// Returns colored status of the subscription
    /// </summary>
    private string GetSubscriptionStatus(DataRowView rowView)
    {
        bool approved = ValidationHelper.GetBoolean(DataHelper.GetDataRowValue(rowView.Row, "SubscriptionApproved"), false);
        bool enabled = ValidationHelper.GetBoolean(DataHelper.GetDataRowValue(rowView.Row, "SubscriptionEnabled"), true);

        if (!approved && enabled)
        {
            return "<span class=\"StatusOrange\">" + ResHelper.GetString("administration.users_header.myapproval") + "</span>";
        }
        else if (approved && enabled)
        {
            return "<span class=\"StatusGreen\">" + ResHelper.GetString("general.approved") + "</span>";
        }
        else
        {
            return "<span class=\"StatusRed\">" + ResHelper.GetString("newsletterview.headerunsubscribed") + "</span>";
        }
    }


    /// <summary>
    /// Handles multiple selector actions.
    /// </summary>
    protected void btnOk_Clicked(object sender, EventArgs e)
    {
        // Check permissions
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "ManageSubscribers"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "ManageSubscribers");
        }
        // Return if no action was selected
        if (drpActions.SelectedValue.EqualsCSafe(SELECT, true))
        {
            return;
        }

        // Get selected items
        List<string> list = unigridNewsletters.SelectedItems;
        if (list.Count == 0)
        {
            ltlScript.Text += ScriptHelper.GetAlertScript(GetString("general.noitems"));
            return;
        }

        int newsletterId;
        SubscriberNewsletterInfo subscriptionInfo;
        foreach (string id in list)
        {
            newsletterId = ValidationHelper.GetInteger(id, 0);

            switch (drpActions.SelectedValue)
            {
                // Subscribe
                case SUBSCRIBE:
                    SubscriberInfoProvider.RenewSubscription(subscriberId, newsletterId, chkSendConfirmation.Checked);
                    break;

                // Unsubscribe
                case UNSUBSCRIBE:
                    SubscriberInfoProvider.Unsubscribe(subscriberId, newsletterId, chkSendConfirmation.Checked);
                    break;


                // Remove subscription
                case "REMOVE":
                    SubscriberInfoProvider.DeleteSubscription(subscriberId, newsletterId, chkSendConfirmation.Checked);
                    break;

                // Approve subscription
                case "APPROVE":
                    subscriptionInfo = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriberId, newsletterId);
                    if ((subscriptionInfo != null) && (!subscriptionInfo.SubscriptionApproved))
                    {
                        subscriptionInfo.SubscriptionApproved = true;
                        subscriptionInfo.SubscriptionApprovedWhen = DateTime.Now;
                        SubscriberNewsletterInfoProvider.SetSubscriberNewsletterInfo(subscriptionInfo);
                    }
                    break;

                // Reject subscription
                case "REJECT":
                    subscriptionInfo = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriberId, newsletterId);
                    if ((subscriptionInfo != null) && (subscriptionInfo.SubscriptionApproved))
                    {
                        subscriptionInfo.SubscriptionApproved = false;
                        subscriptionInfo.SubscriptionApprovedWhen = DateTime.MinValue;
                        SubscriberNewsletterInfoProvider.SetSubscriberNewsletterInfo(subscriptionInfo);
                    }
                    break;
            }
        }

        unigridNewsletters.ResetSelection();
        unigridNewsletters.ReloadData();
    }
}