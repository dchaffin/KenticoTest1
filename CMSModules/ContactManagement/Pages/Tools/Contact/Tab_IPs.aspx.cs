using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_IPs : CMSContactManagementContactsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ContactInfo ci = (ContactInfo)EditedObject;

        // Check permission read
        int siteID = ContactHelper.ObjectSiteID(EditedObject);
        CheckReadPermission(siteID);

        bool globalContact = (ci.ContactSiteID == 0);
        bool mergedContact = (ci.ContactMergedWithContactID > 0);

        listElem.ShowContactNameColumn = globalContact;
        listElem.ShowSiteNameColumn = IsSiteManager && globalContact;
        listElem.ShowRemoveButton = !mergedContact && !globalContact && ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, false);
        listElem.IsGlobalContact = globalContact;
        listElem.IsMergedContact = mergedContact;
        listElem.ContactID = ci.ContactID;

        // Restrict site IDs in CMSDesk
        if (!IsSiteManager)
        {
            listElem.WhereCondition = "ContactSiteID = " + CMSContext.CurrentSiteID;
        }
    }
}