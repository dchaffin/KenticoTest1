using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

[Actions(1)]
[Action(0, "Objects/OM_ActivityType/add.png", "om.activitytype.new", "New.aspx")]
[Security(Resource = ModuleEntry.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_ActivityType_List : CMSContactManagementActivitiesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string currSiteName = null;
        int currSiteId = 0;

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

        if (CurrentMaster.HeaderActions.ActionsList.Count > 0)
        {
            CurrentMaster.HeaderActions.ActionsList[0].RedirectUrl += URLHelper.Url.Query;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        CurrentMaster.HeaderActions.Enabled = CMSContext.CurrentUser.IsAuthorizedPerResource(ModuleEntry.CONTACTMANAGEMENT, "ManageActivities");
    }

    #endregion
}