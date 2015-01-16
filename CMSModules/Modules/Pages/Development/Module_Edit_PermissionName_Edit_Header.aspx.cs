using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Modules_Pages_Development_Module_Edit_PermissionName_Edit_Header : SiteManagerPage
{
    #region "Variables"

    private int mModuleId;
    private int mPermissionId;
    private int mSelectedTab;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mModuleId = QueryHelper.GetInteger("moduleid", 0);
        mPermissionId = QueryHelper.GetInteger("permissionID", 0);
        mSelectedTab = QueryHelper.GetInteger("tabindex", 0);

        CurrentMaster.Title.Breadcrumbs = GetTitleTabs();
        CurrentMaster.Title.HelpTopicName = (mSelectedTab == 0) ? "resource_permission_general" : "resource_permission_roles";
        CurrentMaster.Title.HelpName = "helpTopic";

        CurrentMaster.Tabs.Tabs = GetTabs();
        CurrentMaster.Tabs.UrlTarget = "editcontent";
        CurrentMaster.Tabs.SelectedTab = mSelectedTab;
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Gets page tabs.
    /// </summary>
    /// <returns>Two dimensional string array for page tabs.</returns>
    private string[,] GetTabs()
    {
        string[,] tabs = new string[2,4];
        tabs[0, 0] = GetString("general.general");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'resource_permission_general');";
        tabs[0, 2] = "Module_Edit_PermissionName_Edit.aspx?moduleId=" + mModuleId + "&permissionId=" + mPermissionId + "&hidebreadcrumbs=1";

        tabs[1, 0] = GetString("general.roles");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'resource_permission_roles');";
        tabs[1, 2] = "Module_Edit_PermissionName_Roles.aspx?moduleId=" + mModuleId + "&permissionId=" + mPermissionId;

        return tabs;
    }


    /// <summary>
    /// Gets breadcrumbs.
    /// </summary>
    /// <returns>Two dimensional string array for breadcrumbs.</returns>
    private string[,] GetTitleTabs()
    {
        string[,] pageTitleTabs = new string[2,3];

        string permName = "";

        // Get current permission display name
        PermissionNameInfo permInfo = PermissionNameInfoProvider.GetPermissionNameInfo(mPermissionId);
        if (permInfo != null)
        {
            permName = permInfo.PermissionDisplayName;
        }

        pageTitleTabs[0, 0] = GetString("Administration-Module_Edit.PermissionNames");
        pageTitleTabs[0, 1] = "~/CMSModules/Modules/Pages/Development/Module_Edit_PermissionNames.aspx?moduleId=" + mModuleId;
        pageTitleTabs[0, 2] = "content";

        pageTitleTabs[1, 0] = ResHelper.LocalizeString(permName);
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        return pageTitleTabs;
    }

    #endregion
}