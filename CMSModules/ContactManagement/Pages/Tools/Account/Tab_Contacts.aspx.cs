using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

[EditedObject("om.account", "accountId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_Contacts : CMSContactManagementAccountsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";

        int siteID = AccountHelper.ObjectSiteID(EditedObject);

        // Check read permission 
        if (!AccountHelper.AuthorizedReadAccount(siteID, false) && !ContactHelper.AuthorizedReadContact(siteID, false))
        {
            RedirectToCMSDeskAccessDenied(ModuleEntry.CONTACTMANAGEMENT, "ReadAccounts");
        }
    }
}