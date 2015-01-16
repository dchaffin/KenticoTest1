using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Help
[Help("onlinemarketing_account_new", "helptopic")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_New : CMSContactManagementAccountsPage
{
    #region "Variables"

    private int siteId = 0;

    #endregion


    #region "Page events"

    protected void Page_PreInit(object sender, EventArgs e)
    {
        siteId = QueryHelper.GetInteger("siteid", CMSContext.CurrentSiteID);

        string url = ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/Account/List.aspx");
        url = URLHelper.AddParameterToUrl(url, "siteid", siteId.ToString());
        if (IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "issitemanager", "1");

            // Set title only in Site manager
            CurrentMaster.Title.TitleText = GetString("om.account.new");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/OM_Account/new.png");
        }

        CurrentPage.InitBreadcrumbs(2);
        CurrentPage.SetBreadcrumb(0, GetString("om.account.list"), url, null, null);
        CurrentPage.SetBreadcrumb(1, GetString("om.account.new"), null, null, null);

        editElem.SiteID = siteId;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CheckReadPermission(siteId);
    }

    #endregion
}