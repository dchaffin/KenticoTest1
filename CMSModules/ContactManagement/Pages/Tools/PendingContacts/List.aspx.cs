using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.OnlineMarketing;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_List : CMSAutomationPage
{

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check UIProfile
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.OnlineMarketing", new string[] { "OMCMGroup", "ContactsFrameset", "PendingContacts" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.OnlineMarketing", "OMCMGroup;ContactsFrameset;PendingContacts");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site contacts
            if (AuthorizedForGlobalContacts && AuthorizedForSiteContacts)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;

                // Set site selector
                if (!URLHelper.IsPostback())
                {
                    siteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteid", CMSContext.CurrentSiteID);
                }

                listContacts.SiteID = siteOrGlobalSelector.SiteID;
            }
            else if (AuthorizedForSiteContacts)
            {
                // User is authorized only for site contacts so set current site id
                listContacts.SiteID = CMSContext.CurrentSiteID;
            }
            else if (AuthorizedForGlobalContacts)
            {
                // User can read only global contacts
                listContacts.SiteID = UniSelector.US_GLOBAL_RECORD;
            }
            else
            {
                // User has no permissions
                RedirectToCMSDeskAccessDenied(ModuleEntry.CONTACTMANAGEMENT, "ReadContacts");
            }
        }
        else
        {
            siteOrGlobalSelector.Visible = false;
            listContacts.SiteID = SiteID;
        }

        // Add Refresh action button
        AddHeaderAction(new HeaderAction()
        {
            ImageUrl = GetImageUrl("Objects/MA_AutomationState/refresh.png"),
            Text = GetString("general.Refresh"),
            RedirectUrl = "List.aspx?siteid=" + listContacts.SiteID + (IsSiteManager ? "&issitemanager=1" : String.Empty)
        });
    }
}
