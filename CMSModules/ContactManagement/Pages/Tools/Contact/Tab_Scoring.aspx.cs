using System;

using CMS.CMSHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

[EditedObject(OnlineMarketingObjectType.CONTACT, "contactID")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Scoring : CMSScorePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set up control
        ContactInfo ci = (ContactInfo)EditedObject;
        cScoring.ContactID = ci.ContactID;
        cScoring.SiteID = ci.ContactSiteID;
        cScoring.UniGrid.ZeroRowsText = GetString("om.score.notfound");
    }


    /// <summary>
    /// Check read permissions.
    /// </summary>
    protected override void CheckReadPermission()
    {
        // Check read permission for score or contact
        int siteID = ContactHelper.ObjectSiteID(EditedObject);
        CurrentUserInfo user = CMSContext.CurrentUser;
        if (!ContactHelper.AuthorizedReadContact(siteID, false) && !user.IsAuthorizedPerResource("CMS.Scoring", "Read"))
        {
            RedirectToCMSDeskAccessDenied(ModuleEntry.CONTACTMANAGEMENT, "ReadContacts");
        }
    }
}