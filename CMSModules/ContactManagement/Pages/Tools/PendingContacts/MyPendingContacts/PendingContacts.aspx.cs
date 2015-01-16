using System;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.ExtendedControls.ActionsConfig;
using CMS.SettingsProvider;

public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_MyPendingContacts_PendingContacts : CMSMyDeskPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int siteID = QueryHelper.GetInteger("siteID", CMSContext.CurrentSiteID);

        bool authorizedForSiteContacts = CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ReadContacts");
        bool authorizedForGlobalContacts = CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ReadGlobalContacts") && SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSCMGlobalContacts");

        // Check UIProfile
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.MyDesk", new string[] { "MyDeskDocuments", "Pending", "Contacts" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.MyDesk", "MyDeskDocuments;Pending;Contacts");
        }

        // Init site filter when user is authorized for global and site contacts
        if (authorizedForGlobalContacts && authorizedForSiteContacts)
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;

            // Set site selector
            if (!URLHelper.IsPostback())
            {
                siteOrGlobalSelector.SiteID = siteID;
            }

            listContacts.SiteID = siteOrGlobalSelector.SiteID;

        }
        else if (authorizedForSiteContacts)
        {
            // User is authorized only for site contacts so set current site id
            listContacts.SiteID = siteID;
        }
        else if (authorizedForGlobalContacts)
        {
            // User can read only global contacts
            listContacts.SiteID = UniSelector.US_GLOBAL_RECORD;
        }
        else
        {
            // User has no permissions
            RedirectToCMSDeskAccessDenied(ModuleEntry.CONTACTMANAGEMENT, "ReadContacts|ReadGlobalContacts");
        }
        
        // Add Refresh button
        CurrentMaster.Title.HeaderActions.AddAction(new HeaderAction()
        {
            ImageUrl = GetImageUrl("Objects/MA_AutomationState/refresh.png"),
            Text = GetString("general.Refresh"),
            RedirectUrl = "PendingContacts.aspx?siteid=" + listContacts.SiteID
        });
    }
}
