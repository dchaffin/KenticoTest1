using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Pages_Tools_Contact_List : CMSContactManagementContactsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // In CMS Desk site ID is in control
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site contacts
            if (AuthorizedForGlobalContacts && AuthorizedForSiteContacts)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;
                if (!RequestHelper.IsPostBack())
                {
                    SiteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteId", CMSContext.CurrentSiteID);
                }
                listElem.SiteID = SiteOrGlobalSelector.SiteID;
                listElem.WhereCondition = SiteOrGlobalSelector.GetSiteWhereCondition("ContactSiteID");
            }
            // Authorized for site contacts only
            else if (AuthorizedForSiteContacts)
            {
                listElem.SiteID = CMSContext.CurrentSiteID;
                listElem.WhereCondition = "ContactSiteID = " + listElem.SiteID;
            }
            // Authorized for global contacts only
            else if (AuthorizedForGlobalContacts)
            {
                listElem.SiteID = UniSelector.US_GLOBAL_RECORD;
                listElem.WhereCondition = "ContactSiteID IS NULL";
            }
            // User is not authorized
            else
            {
                RedirectToCMSDeskAccessDenied(ModuleEntry.CONTACTMANAGEMENT, "ReadContacts");
            }
        }
        // In Site Manager "siteID" is in query string
        else
        {
            listElem.SiteID = SiteID;
            if (SiteID == UniSelector.US_GLOBAL_RECORD)
            {
                listElem.WhereCondition = "ContactSiteID IS NULL";
            }
            else if (SiteID > 0)
            {
                listElem.WhereCondition = "ContactSiteID = " + SiteID;
            }
        }

        // Set header actions (add button)
        string url = ResolveUrl("New.aspx?siteId=" + listElem.SiteID);
        if (IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "isSiteManager", "1");
        }
        string[,] actions = new string[1, 8];
        actions[0, 0] = "HyperLink";
        actions[0, 1] = GetString("om.contact.new");
        actions[0, 3] = url;
        actions[0, 5] = GetImageUrl("Objects/OM_Contact/add.png");
        hdrActions.Actions = actions;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        if (!ContactHelper.AuthorizedModifyContact(listElem.SiteID, false))
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