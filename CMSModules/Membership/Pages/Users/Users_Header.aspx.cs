using System;

using CMS.CMSHelper;
using CMS.Controls.Configuration;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Tabs("CMS.Users", "", "usersContent")]
public partial class CMSModules_Membership_Pages_Users_Users_Header : CMSUsersPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Users administration";
        OnTabCreated += CMSModules_Membership_Pages_Users_Users_Header_OnTabCreated;

        // Initialize the master page elements
        InitializeMasterPage();
    }


    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        // Set the master page title
        CurrentMaster.Title.TitleText = GetString("general.users");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_User/object.png");

        CurrentMaster.Title.HelpTopicName = "users_list";
        CurrentMaster.Title.HelpName = "helpTopic";
        
        // Load action page directly, if set by URL
        switch (QueryHelper.GetString("action", null))
        {
            case "newuser":
                CurrentMaster.Tabs.StartPageURL = URLHelper.ResolveUrl("user_new.aspx" + GetQueryParameters());
                break;
        }
    }


    /// <summary>
    /// Tab created event.
    /// </summary>
    TabItem CMSModules_Membership_Pages_Users_Users_Header_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl) && (SiteID != CMSContext.CurrentSiteID))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "SiteId", SiteID.ToString());
        }

        if ((element.ElementName.ToLowerCSafe() == "waiting_for_approval") && !SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSRegistrationAdministratorApproval"))
        {
            return null;
        }

        if ((element.ElementName.ToLowerCSafe() == "online_users") && !LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.OnlineUsers))
        {
            return null;
        }

        if ((element.ElementName.ToLowerCSafe() == "kicked_users") && (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.OnlineUsers) || !SessionManager.StoreOnlineUsersInDatabase))
        {
            return null;
        }
        return tab;
    }


    private string GetQueryParameters()
    {
        if (SiteID != 0)
        {
            return "?siteid=" + SiteID;
        }

        return "";
    }
}