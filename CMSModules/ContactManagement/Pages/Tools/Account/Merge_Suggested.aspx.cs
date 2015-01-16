using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.ACCOUNT, "accountID")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Merge_Suggested : CMSContactManagementAccountsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission for accounts
        int siteID = AccountHelper.ObjectSiteID(EditedObject);
        CheckReadPermission(siteID);
    }
}