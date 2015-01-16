using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Merge_Choose : CMSContactManagementContactsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission
        CheckReadPermission(ContactHelper.ObjectSiteID(EditedObject));
    }
}