using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Pages_Tools_Account_List : CMSContactManagementAccountsPage
{
    #region "Variables"

    private int siteId = CMSContext.CurrentSiteID;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // In CMS Desk site ID is available in control
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site accounts
            if (AuthorizedForGlobalAccounts && AuthorizedForSiteAccounts)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;
                if (!RequestHelper.IsPostBack())
                {
                    SiteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteId", siteId);
                }
                siteId = SiteOrGlobalSelector.SiteID;
                listElem.WhereCondition = SiteOrGlobalSelector.GetSiteWhereCondition("AccountSiteID");
            }
            // User is authorized for site accounts
            else if (AuthorizedForSiteAccounts)
            {
                // Use default value = current site ID
                siteId = CMSContext.CurrentSiteID;
                listElem.WhereCondition = "AccountSiteID = " + siteId;
            }
            // User is authorized only for global accounts
            else if (AuthorizedForGlobalAccounts)
            {
                siteId = UniSelector.US_GLOBAL_RECORD;
                listElem.WhereCondition = "AccountSiteID IS NULL";
            }
            // User is not authorized
            else
            {
                RedirectToCMSDeskAccessDenied(ModuleEntry.CONTACTMANAGEMENT, "ReadAccounts");
            }
        }
        // In Site Manager site ID is in query string
        else
        {
            siteId = SiteID;
            if (SiteID == UniSelector.US_GLOBAL_RECORD)
            {
                listElem.WhereCondition = "AccountSiteID IS NULL";
            }
            else if (SiteID > 0)
            {
                listElem.WhereCondition = "AccountSiteID = " + SiteID;
            }
        }

        // Set header actions (add button)
        listElem.SiteID = siteId;
        string url = ResolveUrl("New.aspx?siteId=" + siteId);
        if (IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "isSiteManager", "1");
        }
        string[,] actions = new string[1, 8];
        actions[0, 0] = "HyperLink";
        actions[0, 1] = GetString("om.account.new");
        actions[0, 3] = url;
        actions[0, 5] = GetImageUrl("Objects/OM_Account/add.png");
        hdrActions.Actions = actions;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        if (!AccountHelper.AuthorizedModifyAccount(listElem.SiteID, false))
        {
            hdrActions.Enabled = false;
        }
        // Allow new button only for particular sites or (global) site
        else if ((listElem.SiteID < 0) && (listElem.SiteID != UniSelector.US_GLOBAL_RECORD))
        {
            hdrActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
    }

    #endregion
}