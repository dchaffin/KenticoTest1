using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.OnlineMarketing;
using CMS.UIControls;

[EditedObject("om.account", "accountId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_Subsidiaries : CMSContactManagementAccountsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";
        // Check read permission for account
        int siteID = AccountHelper.ObjectSiteID(EditedObject);
        CheckReadPermission(siteID);
    }
}