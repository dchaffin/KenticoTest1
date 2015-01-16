using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Roles_Role_New : CMSGroupRolesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set group id
        int groupId = QueryHelper.GetInteger("groupid", 0);

        // Pagetitle
        CurrentMaster.Title.HelpTopicName = "group_role_general";

        // Initializes page title breadcrumbs
        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("general.roles");
        pageTitleTabs[0, 1] = "~/CMSModules/Groups/Tools/Roles/Role_List.aspx?groupid=" + groupId;
        pageTitleTabs[0, 2] = "";
        pageTitleTabs[1, 0] = GetString("Administration-Role_New.NewRole");
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;

        // Edit/Create only roles from current site
        if (CMSContext.CurrentSite != null)
        {
            roleEditElem.SiteID = CMSContext.CurrentSite.SiteID;
        }
        roleEditElem.GroupID = groupId;

        roleEditElem.OnSaved += new EventHandler(roleEditElem_OnSaved);
        roleEditElem.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(roleEditElem_OnCheckPermissions);
    }


    private void roleEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckPermissions(roleEditElem.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }


    private void roleEditElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Role_Edit.aspx?roleId=" + roleEditElem.RoleID + "&groupid=" + roleEditElem.GroupID);
    }
}