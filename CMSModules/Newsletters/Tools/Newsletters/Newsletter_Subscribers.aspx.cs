using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Subscribers : CMSNewsletterNewslettersPage
{
    #region "Variables"

    private const string SELECT = "SELECT";
    private const string UNSUBSCRIBE = "UNSUBSCRIBE";
    private const string SUBSCRIBE = "SUBSCRIBE";
    private const string APPROVE = "APPROVE";
    private const string REJECT = "REJECT";
    private const string REMOVE = "REMOVE";


    protected int newsletterId;
    private int mBounceLimit;
    private bool mBounceInfoAvailable;

    /// <summary>
    /// Contact group selector
    /// </summary>
    private FormEngineUserControl cgSelector = null;


    /// <summary>
    /// Contact selector
    /// </summary>
    private FormEngineUserControl cSelector = null;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        CurrentMaster.ActionsViewstateEnabled = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(this);

        CurrentMaster.DisplayActionsPanel = true;
        chkRequireOptIn.CheckedChanged += chkRequireOptIn_CheckedChanged;

        string siteName = CMSContext.CurrentSiteName;
        mBounceLimit = NewsletterHelper.BouncedEmailsLimit(siteName);
        mBounceInfoAvailable = NewsletterHelper.MonitorBouncedEmails(siteName);

        newsletterId = QueryHelper.GetInteger("newsletterid", 0);
        NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
        EditedObject = newsletter;

        // Check if newsletter enables double opt-in
        if ((newsletter != null) && !newsletter.NewsletterEnableOptIn)
        {
            chkRequireOptIn.Visible = false;
        }

        if (!RequestHelper.IsPostBack())
        {
            chkSendConfirmation.Checked = false;
        }

        // Initialize unigrid
        UniGridSubscribers.WhereCondition = "NewsletterID = " + newsletterId;
        UniGridSubscribers.OnAction += UniGridSubscribers_OnAction;
        UniGridSubscribers.OnExternalDataBound += UniGridSubscribers_OnExternalDataBound;

        // Initialize selectors and mass actions
        SetupSelectors();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide columns with bounced emails if bounce info is not available
        UniGridSubscribers.NamedColumns["blocked"].Visible =
            UniGridSubscribers.NamedColumns["bounces"].Visible = mBounceInfoAvailable;

        pnlActions.Visible = !DataHelper.DataSourceIsEmpty(UniGridSubscribers.GridView.DataSource);
    }


    /// <summary>
    /// Configures selectors.
    /// </summary>
    private void SetupSelectors()
    {
        // Setup role selector
        selectRole.CurrentSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        selectRole.CurrentSelector.OnItemsSelected += RolesSelector_OnItemsSelected;
        selectRole.CurrentSelector.ReturnColumnName = "RoleID";
        selectRole.CurrentSelector.ButtonImage = GetImageUrl("Objects/CMS_Role/add.png");
        selectRole.CurrentSelector.DialogLink.CssClass = "MenuItemEdit Pad";
        selectRole.ShowSiteFilter = false;
        selectRole.CurrentSelector.ResourcePrefix = "addroles";
        selectRole.CurrentSelector.DialogButton.CssClass = "LongButton";
        selectRole.IsLiveSite = false;
        selectRole.UseCodeNameForSelection = false;
        selectRole.GlobalRoles = false;

        // Setup user selector
        selectUser.SelectionMode = SelectionModeEnum.MultipleButton;
        selectUser.UniSelector.OnItemsSelected += UserSelector_OnItemsSelected;
        selectUser.UniSelector.ReturnColumnName = "UserID";
        selectUser.UniSelector.DisplayNameFormat = "{%FullName%} ({%Email%})";
        selectUser.UniSelector.ButtonImage = GetImageUrl("Objects/CMS_User/add.png");
        selectUser.UniSelector.AdditionalSearchColumns = "UserName, Email";
        selectUser.UniSelector.DialogLink.CssClass = "MenuItemEdit Pad";
        selectUser.ShowSiteFilter = false;
        selectUser.ResourcePrefix = "newsletteraddusers";
        selectUser.UniSelector.DialogButton.CssClass = "LongButton";
        selectUser.IsLiveSite = false;

        // Setup subscriber selector
        selectSubscriber.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        selectSubscriber.UniSelector.OnItemsSelected += SubscriberSelector_OnItemsSelected;
        selectSubscriber.UniSelector.ReturnColumnName = "SubscriberID";
        selectSubscriber.UniSelector.ButtonImage = GetImageUrl("Objects/Newsletter_Subscriber/add.png");
        selectSubscriber.UniSelector.DialogLink.CssClass = "MenuItemEdit Pad";
        selectSubscriber.ShowSiteFilter = false;
        selectSubscriber.UniSelector.DialogButton.CssClass = "LongButton";
        selectSubscriber.IsLiveSite = false;
        selectSubscriber.UniSelector.RemoveMultipleCommas = true;

        // Setup contact group and contact selectors
        if (ModuleEntry.IsModuleLoaded(ModuleEntry.ONLINEMARKETING) && LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.ContactManagement))
        {
            plcSelectCG.Controls.Clear();

            // Check read permission for contact groups
            if (CMSContext.CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ReadContactGroups"))
            {
                // Load selector control and initialize it
                cgSelector = (FormEngineUserControl)Page.LoadUserControl("~/CMSModules/ContactManagement/FormControls/ContactGroupSelector.ascx");
                if (cgSelector != null)
                {
                    cgSelector.ID = "selectCG";
                    cgSelector.ShortID = "scg";
                    // Get inner uniselector control
                    UniSelector selector = (UniSelector)cgSelector.GetValue("uniselector");
                    if (selector != null)
                    {
                        // Bind an event handler on 'items selected' event
                        selector.OnItemsSelected += CGSelector_OnItemsSelected;
                        selector.ResourcePrefix = "contactgroupsubscriber";
                    }
                    // Insert selector to the header
                    plcSelectCG.Controls.Add(cgSelector);
                }
            }

            // Check read permission for contacts
            if (CMSContext.CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ReadContacts"))
            {
                // Load selector control and initialize it
                cSelector = (FormEngineUserControl)Page.LoadUserControl("~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx");
                if (cSelector != null)
                {
                    cSelector.ID = "slContact";
                    cSelector.ShortID = "sc";
                    // Set where condition to filter contacts with email addresses
                    cSelector.SetValue("wherecondition", "NOT (ContactEmail IS NULL OR ContactEmail LIKE '')");
                    // Set site ID
                    cSelector.SetValue("siteid", CMSContext.CurrentSiteID);
                    // Get inner uniselector control
                    UniSelector selector = (UniSelector)cSelector.GetValue("uniselector");
                    if (selector != null)
                    {
                        // Bind an event handler on 'items selected' event
                        selector.OnItemsSelected += ContactSelector_OnItemsSelected;
                        selector.SelectionMode = SelectionModeEnum.MultipleButton;
                        selector.ResourcePrefix = "contactsubscriber";
                        selector.ButtonImage = GetImageUrl("/Objects/OM_Contact/add.png");
                        selector.DisplayNameFormat = "{%ContactFirstName%} {%ContactLastName%} ({%ContactEmail%})";
                        selector.AdditionalSearchColumns = "ContactFirstName,ContactMiddleName,ContactEmail";
                    }
                    // Insert selector to the header
                    plcSelectCG.Controls.Add(cSelector);
                }
            }
        }

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


    /// <summary>
    /// Unigrid external databound event handler.
    /// </summary>
    protected object UniGridSubscribers_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        bool approved;
        switch (sourceName)
        {
            case "subscribe":
                ((ImageButton)sender).Visible = !ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionEnabled"], true);
                break;

            case "unsubscribe":
                ((ImageButton)sender).Visible = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionEnabled"], true);
                break;
                
            case "block":
                return SetBlockAction(sender, ((parameter as GridViewRow).DataItem) as DataRowView);

            case "unblock":
                return SetUnblockAction(sender, ((parameter as GridViewRow).DataItem) as DataRowView);

            case "email":
                return GetEmail(parameter as DataRowView);

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

            case "blocked":
                return GetBlocked(parameter as DataRowView);

            case "bounces":
                return GetBounces(parameter as DataRowView);
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridSubscribers_OnAction(string actionName, object actionArgument)
    {
        // Check 'manage subscribers' permission
        CheckAuthorization();

        int subscriberId = ValidationHelper.GetInteger(actionArgument, 0);

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

            // Delete subscription
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

            // Block selected subscriber
            case "block":
                SubscriberInfoProvider.BlockSubscriber(subscriberId);
                break;

            // Un-block selected subscriber
            case "unblock":
                SubscriberInfoProvider.UnblockSubscriber(subscriberId);
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
    /// Checkbox 'Require double opt-in' state changed.
    /// </summary>
    protected void chkRequireOptIn_CheckedChanged(object sender, EventArgs e)
    {
        if (chkRequireOptIn.Checked)
        {
            chkSendConfirmation.Enabled = false;
            chkSendConfirmation.Checked = false;
        }
        else
        {
            chkSendConfirmation.Enabled = true;
        }
    }


    /// <summary>
    /// Roles control items changed event.
    /// </summary>
    protected void RolesSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        int roleID = 0;
        string[] newItems = null;
        SubscriberInfo sb = null;
        RoleInfo ri = null;
        int siteId = CMSContext.CurrentSiteID;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(selectRole.Value, null);

        // Get added items
        newItems = newValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (newItems != null)
        {
            foreach (string item in newItems)
            {
                // Check limited number of subscribers
                if (!SubscriberInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Subscribers, VersionActionEnum.Insert))
                {
                    ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                    break;
                }

                roleID = ValidationHelper.GetInteger(item, 0);

                // Get subscriber
                sb = SubscriberInfoProvider.GetSubscriberInfo(SiteObjectType.ROLE, roleID, siteId);
                if (sb == null)
                {
                    // Get role info and copy display name to new subscriber
                    ri = RoleInfoProvider.GetRoleInfo(roleID);
                    if ((ri == null) || (ri.SiteID != siteId))
                    {
                        continue;
                    }

                    // Create new subscriber of role type
                    sb = new SubscriberInfo();
                    sb.SubscriberFirstName = ri.DisplayName;
                    // Full name consists of "role " and role display name
                    sb.SubscriberFullName = string.Concat("Role '", ri.DisplayName, "'");
                    sb.SubscriberSiteID = siteId;
                    sb.SubscriberType = SiteObjectType.ROLE;
                    sb.SubscriberRelatedID = roleID;
                    SubscriberInfoProvider.SetSubscriberInfo(sb);
                }

                // If subscriber exists and is not subscribed, subscribe him
                if ((sb != null) && (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId)))
                {
                    try
                    {
                        SubscriberInfoProvider.Subscribe(sb.SubscriberID, newsletterId, DateTime.Now, chkSendConfirmation.Checked, false);
                    }
                    catch
                    {
                    }
                }
            }
        }

        selectRole.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// User control items changed event.
    /// </summary>
    protected void UserSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        int userID = 0;
        string[] newItems = null;
        SubscriberInfo sb = null;
        UserInfo ui = null;
        int siteId = CMSContext.CurrentSiteID;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(selectUser.Value, null);

        newItems = newValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (newItems != null)
        {
            foreach (string item in newItems)
            {
                // Check limited number of subscribers
                if (!SubscriberInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Subscribers, VersionActionEnum.Insert))
                {
                    ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                    break;
                }

                userID = ValidationHelper.GetInteger(item, 0);

                // Get subscriber
                sb = SubscriberInfoProvider.GetSubscriberInfo(SiteObjectType.USER, userID, siteId);
                if (sb == null)
                {
                    // Get user info
                    ui = UserInfoProvider.GetUserInfo(userID);
                    if (ui == null)
                    {
                        continue;
                    }

                    // Create new subscriber of user type
                    sb = new SubscriberInfo();
                    sb.SubscriberFirstName = ui.FullName;
                    sb.SubscriberFullName = "User '" + ui.FullName + "'";
                    sb.SubscriberSiteID = siteId;
                    sb.SubscriberType = SiteObjectType.USER;
                    sb.SubscriberRelatedID = userID;
                    SubscriberInfoProvider.SetSubscriberInfo(sb);
                }

                // If subscriber exists and is not subscribed, subscribe him
                if ((sb != null) && (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId)))
                {
                    try
                    {
                        SubscriberInfoProvider.Subscribe(sb.SubscriberID, newsletterId, DateTime.Now, chkSendConfirmation.Checked, chkRequireOptIn.Checked);
                    }
                    catch
                    {
                    }
                }
            }
        }

        selectUser.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Subscriber control items changed event.
    /// </summary>
    protected void SubscriberSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        int subscriberID = 0;
        string[] newItems = null;
        SubscriberInfo sb = null;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(selectSubscriber.Value, null);

        newItems = newValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (newItems != null)
        {
            // Add all new items to site
            foreach (string item in newItems)
            {
                subscriberID = ValidationHelper.GetInteger(item, 0);

                // Get subscriber
                sb = SubscriberInfoProvider.GetSubscriberInfo(subscriberID);

                // If subscriber exists and is not subscribed, subscribe him
                if ((sb != null) && (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId)))
                {
                    try
                    {
                        SubscriberInfoProvider.Subscribe(sb.SubscriberID, newsletterId, DateTime.Now, chkSendConfirmation.Checked, chkRequireOptIn.Checked);
                    }
                    catch
                    {
                    }
                }
            }
        }

        selectSubscriber.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Contact group items selected event handler.
    /// </summary>
    protected void CGSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        if (cgSelector == null)
        {
            return;
        }
        int groupID = 0;
        string[] newItems = null;
        SubscriberInfo sb = null;
        int siteId = CMSContext.CurrentSiteID;
        string cgType = "om.contactgroup";
        string cgName = null;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(cgSelector.Value, null);

        // Get added items
        newItems = newValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (newItems != null)
        {
            foreach (string item in newItems)
            {
                // Check limited number of subscribers
                if (!SubscriberInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Subscribers, VersionActionEnum.Insert))
                {
                    ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                    break;
                }

                // Get group ID
                groupID = ValidationHelper.GetInteger(item, 0);

                // Try to get subscriber
                sb = SubscriberInfoProvider.GetSubscriberInfo(cgType, groupID, siteId);
                if (sb == null)
                {
                    // Get contact group display name
                    cgName = ModuleCommands.OnlineMarketingGetContactGroupName(groupID);
                    if (string.IsNullOrEmpty(cgName))
                    {
                        continue;
                    }

                    // Create new subscriber of contact group type
                    sb = new SubscriberInfo();
                    sb.SubscriberFirstName = cgName;
                    // Full name consists of "contact group " and display name
                    sb.SubscriberFullName = string.Concat("Contact group '", cgName, "'");
                    sb.SubscriberSiteID = siteId;
                    sb.SubscriberType = cgType;
                    sb.SubscriberRelatedID = groupID;
                    SubscriberInfoProvider.SetSubscriberInfo(sb);
                }

                // If subscriber exists and is not subscribed, subscribe him
                if ((sb != null) && (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId)))
                {
                    try
                    {
                        SubscriberInfoProvider.Subscribe(sb.SubscriberID, newsletterId, DateTime.Now, chkSendConfirmation.Checked, false);
                    }
                    catch
                    {
                    }
                }
            }
        }

        cgSelector.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Contact items selected event handler.
    /// </summary>
    protected void ContactSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        if (cSelector == null)
        {
            return;
        }
        int contactID = 0;
        string[] newItems = null;
        SubscriberInfo sb = null;
        int siteId = CMSContext.CurrentSiteID;
        string cType = PredefinedObjectType.CONTACT;
        DataSet contactData = null;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(cSelector.Value, null);

        // Get added items
        newItems = newValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (newItems != null)
        {
            foreach (string item in newItems)
            {
                // Check limited number of subscribers
                if (!SubscriberInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Subscribers, VersionActionEnum.Insert))
                {
                    ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                    break;
                }

                // Get contact ID
                contactID = ValidationHelper.GetInteger(item, 0);

                // Try to get subscriber
                sb = SubscriberInfoProvider.GetSubscriberInfo(cType, contactID, siteId);
                if (sb == null)
                {
                    // Get contact's info
                    contactData = ModuleCommands.OnlineMarketingGetContactForNewsletters(contactID, "ContactFirstName,ContactMiddleName,ContactLastName");
                    if (DataHelper.DataSourceIsEmpty(contactData))
                    {
                        continue;
                    }

                    string firstName = ValidationHelper.GetString(contactData.Tables[0].Rows[0]["ContactFirstName"], string.Empty);
                    string lastName = ValidationHelper.GetString(contactData.Tables[0].Rows[0]["ContactLastName"], string.Empty);
                    string middleName = ValidationHelper.GetString(contactData.Tables[0].Rows[0]["ContactMiddleName"], string.Empty);

                    // Create new subscriber of contact type
                    sb = new SubscriberInfo();
                    sb.SubscriberFirstName = firstName;
                    sb.SubscriberLastName = lastName;
                    sb.SubscriberFullName = string.Format("Contact '{0}{1}{2}'", string.IsNullOrEmpty(firstName) ? string.Empty : firstName + " ", string.IsNullOrEmpty(middleName) ? string.Empty : middleName + " ", lastName);
                    sb.SubscriberSiteID = siteId;
                    sb.SubscriberType = cType;
                    sb.SubscriberRelatedID = contactID;
                    SubscriberInfoProvider.SetSubscriberInfo(sb);
                }

                // If subscriber exists and is not subscribed, subscribe him
                if ((sb != null) && (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId)))
                {
                    try
                    {
                        SubscriberInfoProvider.Subscribe(sb.SubscriberID, newsletterId, DateTime.Now, chkSendConfirmation.Checked, chkRequireOptIn.Checked);
                    }
                    catch
                    {
                    }
                }
            }
        }

        cSelector.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
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


    /// <summary>
    /// Handles multiple selector actions.
    /// </summary>
    protected void btnOk_Clicked(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        if (drpActions.SelectedValue != SELECT)
        {
            // Go through all selected items
            List<string> list = UniGridSubscribers.SelectedItems;
            if (list.Count > 0)
            {
                foreach (string subscriberId in list)
                {
                    var subscriberInt = ValidationHelper.GetInteger(subscriberId, 0);
                    var sni = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriberInt, newsletterId);
                    if (sni != null)
                    {
                        switch (drpActions.SelectedValue)
                        {
                            // Subscribe
                            case SUBSCRIBE:
                                SubscriberInfoProvider.RenewSubscription(subscriberInt, newsletterId, chkSendConfirmation.Checked);
                                break;

                            // Unsubscribe
                            case UNSUBSCRIBE:
                                SubscriberInfoProvider.Unsubscribe(sni, chkSendConfirmation.Checked);
                                break;

                            // Remove subscription
                            case REMOVE:
                                SubscriberInfoProvider.DeleteSubscription(subscriberInt, newsletterId, chkSendConfirmation.Checked);
                                break;

                            // Approve subscription
                            case APPROVE:
                                if (!sni.SubscriptionApproved)
                                {
                                    sni.SubscriptionApproved = true;
                                    sni.SubscriptionApprovedWhen = DateTime.Now;
                                    SubscriberNewsletterInfoProvider.SetSubscriberNewsletterInfo(sni);
                                }
                                break;

                            // Reject subscription
                            case REJECT:
                                if (sni.SubscriptionApproved)
                                {
                                    sni.SubscriptionApproved = false;
                                    sni.SubscriptionApprovedWhen = DateTime.MinValue;
                                    SubscriberNewsletterInfoProvider.SetSubscriberNewsletterInfo(sni);
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                ltlScript.Text += ScriptHelper.GetAlertScript(GetString("general.noitems"));
            }
        }
        UniGridSubscribers.ResetSelection();
        UniGridSubscribers.ReloadData();
    }

    #endregion
}