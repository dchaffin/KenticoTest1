using System;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

[Security(Resource = ModuleEntry.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_Activity_List : CMSContactManagementActivitiesPage
{
    #region "Variables"

    private int currSiteId;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string currSiteName = null;

        // Get current site ID/name
        if (ContactHelper.IsSiteManager)
        {
            currSiteId = SiteID;
            currSiteName = SiteInfoProvider.GetSiteName(currSiteId);
        }
        else
        {
            currSiteName = CMSContext.CurrentSiteName;
            currSiteId = CMSContext.CurrentSiteID;
        }

        bool globalObjectsSelected = (currSiteId == UniSelector.US_GLOBAL_RECORD);
        bool allSitesSelected = (currSiteId == UniSelector.US_ALL_RECORDS);

        // Show warning if activity logging is disabled (do not show anything if global objects or all sites is selected)
        ucDisabledModule.SettingsKeys = "CMSEnableOnlineMarketing";
        ucDisabledModule.InfoText = GetString("om.onlinemarketing.disabled");
        ucDisabledModule.ParentPanel = pnlDis;

        pnlDis.Visible = !globalObjectsSelected && !allSitesSelected && !ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(currSiteName);

        // Initialize list and filter controls
        listElem.SiteID = currSiteId;

        // Show site name column if activities of all sites are displayed
        listElem.ShowSiteNameColumn = allSitesSelected || globalObjectsSelected;
        listElem.ShowIPAddressColumn = ActivitySettingsHelper.IPLoggingEnabled(currSiteName);
        listElem.OrderBy = "ActivityCreated DESC";

        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            ShowChangesSaved();
        }

        // Set header actions (add button)
        string url = ResolveUrl("New.aspx?siteId=" + currSiteId);
        if (IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "isSiteManager", "1");
        }
        string[,] actions = new string[1, 8];
        actions[0, 0] = "HyperLink";
        actions[0, 1] = GetString("om.activity.newcustom");
        actions[0, 3] = url;
        actions[0, 5] = GetImageUrl("Objects/OM_Activity/add.png");
        hdrActions.Actions = actions;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable manual creation of activity if no custom activity type is available
        DataSet ds = ActivityTypeInfoProvider.GetActivityTypes("ActivityTypeIsCustom=1 AND ActivityTypeEnabled=1 AND ActivityTypeManualCreationAllowed=1", null, 1, "ActivityTypeID");
        bool aCustomActivityExists = !DataHelper.DataSourceIsEmpty(ds);

        // Disable actions for unauthorized users
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ManageActivities"))
        {
            hdrActions.Enabled = false;
        }
        // Allow new button only if custom activity exists
        else if (!aCustomActivityExists)
        {
            lblWarnNew.ResourceString = "om.activities.nocustomactivity";
            hdrActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
        // Allow new button only for particular sites
        else if (currSiteId <= 0)
        {
            lblWarnNew.ResourceString = "om.choosesite";
            hdrActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
    }

    #endregion
}