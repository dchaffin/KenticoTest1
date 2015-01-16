using System;
using System.Collections.Generic;

using CMS.Controls.Configuration;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("Objects/Newsletter_Subscriber/object.png", "newsletters.subscribers", "general_tab2", HelpName = "helpTopic")]
public partial class CMSModules_Newsletters_Tools_Subscribers_Subscriber_Header : CMSNewsletterSubscribersPage
{
    #region "Variables"

    protected int subscriberId;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        string currentSubscriber = GetString("Subscriber_Edit.NewItemCaption");

        // Get subscriber id from querystring        
        subscriberId = QueryHelper.GetInteger("subscriberid", 0);

        // Get subscriber object
        SubscriberInfo subscriberObj = SubscriberInfoProvider.GetSubscriberInfo(subscriberId);
        if (subscriberObj != null)
        {
            if (!DataHelper.IsEmpty(subscriberObj.SubscriberEmail))
            {
                // Get e-mail for breadcrumbs
                currentSubscriber = subscriberObj.SubscriberEmail;
            }
            else
            {
                string type = null;
                switch (subscriberObj.SubscriberType)
                {
                    case SiteObjectType.USER:
                        type = GetString("objecttype.cms_user");
                        break;
                    case SiteObjectType.ROLE:
                        type = GetString("objecttype.cms_role");
                        break;
                    case PredefinedObjectType.CONTACTGROUP:
                        type = GetString("objecttype.om_contactgroup");
                        break;
                    case PredefinedObjectType.CONTACT:
                        type = GetString("objecttype.om_contact");
                        break;
                }

                // Get first, last names and type for breadcrumbs
                currentSubscriber = string.Format("{0} ({1})", string.Concat(subscriberObj.SubscriberFirstName, " ", subscriberObj.SubscriberLastName), type.ToLowerCSafe());
            }
        }

        // Initializes page title
        string[,] breadcrumbs = new string[2,3];
        breadcrumbs[0, 0] = GetString("Subscriber_Edit.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Newsletters/Tools/Subscribers/Subscriber_List.aspx";
        breadcrumbs[0, 2] = "newslettersContent";
        breadcrumbs[1, 0] = currentSubscriber;

        CurrentMaster.Title.Breadcrumbs = breadcrumbs;

        // Initialize tabs
        InitalizeMenu(subscriberObj);
    }


    /// <summary>
    /// Initializes user edit menu.
    /// </summary>
    protected void InitalizeMenu(SubscriberInfo subscriber)
    {
        if (subscriber == null)
        {
            return;
        }

        string subscriptionUrl = "Subscriber_Subscriptions.aspx?subscriberid=" + subscriberId;
        List<TabItem> tabItems = new List<TabItem>();
        
        // Display General tab only to standard subscribers ...
        if (string.IsNullOrEmpty(subscriber.SubscriberType) && subscriber.SubscriberRelatedID == 0)
        {
            // Add General tab
            tabItems.Add(new TabItem()
            {
                Text = GetString("general.general"),
                OnClientClick = "SetHelpTopic('helpTopic', 'general_tab2');",
                RedirectUrl = "Subscriber_General.aspx?subscriberid=" + subscriberId
            });
        }
        else
        {
            // ... other subscriber types have Subscriptions tab as the first one
            CurrentMaster.Title.HelpTopicName = "subscriptions_tab";

            ScriptHelper.RegisterStartupScript(this, typeof(string), "SelectSubscriptionTab", ScriptHelper.GetScript("SelTab(0, 'subscriberContent', '" + subscriptionUrl + "');"));
        }

        // Add Subscription tab
        tabItems.Add(new TabItem()
        {
            Text = GetString("Subscriber_Edit.Subscription"),
            OnClientClick = "SetHelpTopic('helpTopic', 'subscriptions_tab');",
            RedirectUrl = subscriptionUrl
        });

        // Display Users tab for role subscribers
        if (subscriber.SubscriberType == SiteObjectType.ROLE)
        {
            tabItems.Add(new TabItem()
            {
                Text = GetString("Subscriber_Edit.Users"),
                OnClientClick = "SetHelpTopic('helpTopic', 'subscriberusers_tab');",
                RedirectUrl = "Subscriber_Users.aspx?roleid=" + subscriber.SubscriberRelatedID + "&subscriberid=" + subscriberId
            });
        }
        // Display Contacts tab for contact group subscribers
        else if (subscriber.SubscriberType == PredefinedObjectType.CONTACTGROUP && ModuleEntry.IsModuleLoaded(ModuleEntry.ONLINEMARKETING) && LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.ContactManagement))
        {
            tabItems.Add(new TabItem()
            {
                Text = GetString("om.contact.list"),
                OnClientClick = "SetHelpTopic('helpTopic', 'subscribercontacts_tab');",
                RedirectUrl = "../../../ContactManagement/Pages/Tools/Contact/Subscriber_Contacts.aspx?groupid=" + subscriber.SubscriberRelatedID + "&subscriberid=" + subscriberId
            });
        }

        CurrentMaster.Tabs.TabItems = tabItems;
        CurrentMaster.Tabs.UrlTarget = "subscriberContent";
    }

    #endregion
}