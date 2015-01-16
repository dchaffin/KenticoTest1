using System;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Tab_Membership : CMSContactManagementContactsPage
{
    protected int contactId = 0;
    protected string siteManagerParam = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        contactId = QueryHelper.GetInteger("contactId", 0);
        if (IsSiteManager)
        {
            siteManagerParam = "&issitemanager=1";
        }
    }
}