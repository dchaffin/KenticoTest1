using System;
using System.Linq;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.WorkflowEngine;

// Edited object
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Header : CMSContactManagementContactsPage
{
    #region "Variables"

    private bool isDialogMode = false;

    #endregion


    #region "Page events"

    protected void Page_PreInit(object sender, EventArgs e)
    {
        isDialogMode = QueryHelper.GetBoolean("dialogmode", false);

        // Modify header master page in modal dialog
        if (isDialogMode)
        {
            Page.MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }
        else
        {
            Page.MasterPageFile = "~/CMSMasterPages/UI/TabsHeader.master";
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Contacts", null, "menu");

        // Get current user info
        CurrentUserInfo user = CMSContext.CurrentUser;

        // Get contact info object
        ContactInfo ci = (ContactInfo)EditedObject;
        if (ci == null)
        {
            return;
        }

        // Check permission read
        ContactHelper.AuthorizedReadContact(ci.ContactSiteID, true);

        // Check if running under site manager (and distribute "site manager" flag to other tabs)
        string siteManagerParam = string.Empty;
        if (IsSiteManager)
        {
            siteManagerParam = "&issitemanager=1";
        }

        // Set default help topic
        SetHelp("onlinemarketing_contact_general", "helpTopic");

        // register scripts in modal dialog
        if (isDialogMode)
        {
            RegisterModalPageScripts();
        }

        string append = null;
        if (ci.ContactMergedWithContactID != 0)
        {
            // Append '(merged)' behind contact name in breadcrumbs
            append = " " + GetString("om.contact.mergedsuffix");
        }
        else if (ci.ContactSiteID == 0)
        {
            // Append '(global)' behind contact name in breadcrumbs
            append = " " + GetString("om.contact.globalsuffix");
        }

        // Modify header appearance in modal dialog (display title instead of breadcrumbs)
        if (QueryHelper.GetBoolean("dialogmode", false))
        {
            CurrentMaster.Title.TitleText = GetString("om.contact.edit") + " - " + HTMLHelper.HTMLEncode(ContactInfoProvider.GetContactFullName(ci)) + append;
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/OM_Contact/object.png");
        }
        else
        {
            // Get url for breadcrumbs
            string url = ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/Contact/List.aspx");
            url = URLHelper.AddParameterToUrl(url, "siteid", SiteID.ToString());
            if (IsSiteManager)
            {
                url = URLHelper.AddParameterToUrl(url, "issitemanager", "1");
            }

            CurrentPage.InitBreadcrumbs(2);
            CurrentPage.SetBreadcrumb(0, GetString("om.contact.list"), url, "_parent", null);
            CurrentPage.SetBreadcrumb(1, ContactInfoProvider.GetContactFullName(ci) + append, null, null, null);
        }

        // Check if contact has any custom fields
        int i = 0;

        FormInfo formInfo = FormHelper.GetFormInfo(ci.Generalized.DataClass.ClassName, false);
        if (formInfo.GetFormElements(true, false, true).Any())
        {
            i = 1;
        }

        int contactId = ci.ContactID;
        bool ipTabAvailable = ActivitySettingsHelper.IPLoggingEnabled(CMSContext.CurrentSiteName) || ContactHelper.IsSiteManager;
        int counter = 0;

        // Initialize tabs
        InitTabs("content");

        SetTab(counter++, GetString("general.general"), "Tab_General.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_general');");

        if (i > 0)
        {
            // Add tab for custom fields
            SetTab(counter++, GetString("general.customfields"), "Tab_CustomFields.aspx?contactid=" + ci.ContactID + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_customfields');");
        }

        if (AccountHelper.AuthorizedReadAccount(ci.ContactSiteID, false) || ContactHelper.AuthorizedReadContact(ci.ContactSiteID, false))
        {
            SetTab(counter++, GetString("om.account.list"), "Tab_Accounts.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_accounts');");
        }

        SetTab(counter++, GetString("om.membership.list"), "Membership/Tab_Membership.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_membership');");

        SetTab(counter++, GetString("om.activity.list"), "Tab_Activities.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_activities');");

        if (ipTabAvailable)
        {
            SetTab(counter++, GetString("om.activity.iplist"), "Tab_IPs.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_ips');");
        }

        // Show contact groups
        SetTab(counter++, GetString("om.contactgroup.list"), "Tab_ContactGroups.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_contactgroups');");

        // Show scoring tab for site contacts
        if (ci.ContactSiteID > 0)
        {
            SetTab(counter++, GetString("om.score.list"), "Tab_Scoring.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_scoring');");
        }

        // Hide last 3 tabs if the contact is merged
        if (ci.ContactMergedWithContactID == 0)
        {
            SetTab(counter++, GetString("om.contact.merge"), "Tab_Merge.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_merge');");
        }

        // Data.com
        SetTab(counter++, "Data.com", "Tab_DataCom.aspx?contactid=" + contactId + siteManagerParam, "SetHelpTopic('helpTopic', 'onlinemarketing_contact_datacom');");

        // Marketing automation
        if (WorkflowInfoProvider.IsMarketingAutomationAllowed())
        {
            string stateUrl = String.Format("{0}?objectid={1}{2}", "Tab_Processes.aspx", contactId, siteManagerParam);
            SetTab(counter++, GetString("ma.contact.processes"), stateUrl, "SetHelpTopic('helpTopic', 'automation_state_list');");
        }
    }

    #endregion
}