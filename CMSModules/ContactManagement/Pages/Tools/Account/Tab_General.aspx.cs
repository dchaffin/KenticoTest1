using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.ACCOUNT, "accountId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_General : CMSContactManagementAccountsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission for accounts
        int siteID = AccountHelper.ObjectSiteID(EditedObject);
        CheckReadPermission(siteID);

        editElem.OnSaved += new EventHandler(editElem_OnSaved);
    }


    private void editElem_OnSaved(object sender, EventArgs e)
    {
        // Refresh breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, null);
    }
}