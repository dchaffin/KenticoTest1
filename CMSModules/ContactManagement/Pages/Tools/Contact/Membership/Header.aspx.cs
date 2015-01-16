using System;

using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactid")]
// Title
[Title("", "", "onlinemarketing_contact_membershipusers")]
// Breadcrumbs
[Breadcrumbs(1)]
[Breadcrumb(0, "om.membership.list")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Header : CMSContactManagementContactsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get contact info object
        ContactInfo ci = (ContactInfo)EditedObject;
        if (ci == null)
        {
            return;
        }

        // Check permisssions
        CheckReadPermission(ci.ContactSiteID);

        // Check if running under site manager
        string siteManagerParam = "";
        if (IsSiteManager)
        {
            siteManagerParam = "&issitemanager=1";
        }

        int counter = 0;
        int contactId = ci.ContactID;

        // Initialize tabs
        InitTabs("membershipContent");
        SetTab(counter++, GetString("om.membership.userslist"), "Users.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_membershipusers');");
        if (ModuleEntry.IsModuleLoaded(ModuleEntry.ECOMMERCE))
        {
            SetTab(counter++, GetString("om.membership.customerslist"), "Customers.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_membershipcustomers');");
        }
        if (ModuleEntry.IsModuleLoaded(ModuleEntry.NEWSLETTER))
        {
            SetTab(counter++, GetString("om.membership.subscriberslist"), "Subscribers.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_membershipsubscribers');");
        }
    }
}