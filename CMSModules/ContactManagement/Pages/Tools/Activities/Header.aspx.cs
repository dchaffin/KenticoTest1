using System;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;

[Title("Objects/OM_Activity/object.png", "om.activity.list", "activities")]
[Tabs("activitiesContent")]
[Tab(0, "om.activity.list", "Activity/List.aspx", "SetHelpTopic('helpTopic', 'activities'); return false;")]
[Tab(1, "om.activitytype.list", "ActivityType/List.aspx", "SetHelpTopic('helpTopic', 'activity_types'); return false;")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_Header : CMSContactManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Activities", null, "menu");

        // If in Site Manager...
        if (ContactHelper.IsSiteManager)
        {
            // Set Breadcrumbs
            InitBreadcrumbs(1);
            SetBreadcrumb(0, GetString("om.activity.list"), null, null, null);

            // Hide title
            CurrentMaster.Title.TitleText = CurrentMaster.Title.TitleImage = string.Empty;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        UITabs tabs = ((CMSMasterPage)CurrentMaster).Tabs;

        for (int i = 0; i < tabs.TabItems.Count; i++)
        {
            if (!String.IsNullOrEmpty(tabs.TabItems[i].RedirectUrl))
            {
                tabs.TabItems[i].RedirectUrl += URLHelper.Url.Query;
            }
        }
    }
}