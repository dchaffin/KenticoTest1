using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.ACCOUNT, "accountID")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Merge_Header : CMSContactManagementAccountsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission for account
        CheckReadPermission(AccountHelper.ObjectSiteID(EditedObject));

        ((CMSMasterPage)CurrentMaster).PanelBody.CssClass += " Separator";

        // Check if running under site manager (and distribute "site manager" flag to other tabs)
        string siteManagerParam = "";
        if (IsSiteManager)
        {
            siteManagerParam = "&issitemanager=1";
        }

        // Initialize tabs
        InitTabs("mergeContent");
        SetTab(0, GetString("om.contact.mergesuggested"), "Merge_Suggested.aspx?accountid=" + QueryHelper.GetInteger("accountID", 0) + siteManagerParam, "parent.parent.frames['menu'].SetHelpTopic('helpTopic', 'onlinemarketing_account_merge');");
        SetTab(1, GetString("om.account.choose"), "Merge_Choose.aspx?accountid=" + QueryHelper.GetInteger("accountID", 0) + siteManagerParam, "parent.parent.frames['menu'].SetHelpTopic('helpTopic', 'onlinemarketing_account_mergechoose');");
        SetTab(2, GetString("om.contact.split"), "Merge_Split.aspx?accountid=" + QueryHelper.GetInteger("accountID", 0) + siteManagerParam, "parent.parent.frames['menu'].SetHelpTopic('helpTopic', 'onlinemarketing_account_split');");
    }
}