using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "om.activity.list", "~/CMSModules/ContactManagement/Pages/Tools/Activities/Activity/List.aspx", null)]
[Breadcrumb(1, "om.activity.newcustom")]
// Help
[Help("activity_new", "helptopic")]
[Security(Resource = ModuleEntry.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_Activity_New : CMSContactManagementActivitiesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        int siteId = QueryHelper.GetInteger("siteid", 0);
        int contactId = QueryHelper.GetInteger("contactid", 0);

        editElem.SiteID = siteId;
        editElem.ContactID = contactId;
        editElem.ShowSiteSelector = false;

        if (contactId > 0)
        {
            // New custom activity page was opened from pages of edited contact
            CurrentMaster.Title.Breadcrumbs[0, 1] = "~/CMSModules/ContactManagement/Pages/Tools/Contact/Tab_Activities.aspx?contactId=" + contactId.ToString();
        }

        CurrentMaster.Title.Breadcrumbs[0, 1] = AddSiteQuery(CurrentMaster.Title.Breadcrumbs[0, 1], siteId);
    }

    #endregion
}