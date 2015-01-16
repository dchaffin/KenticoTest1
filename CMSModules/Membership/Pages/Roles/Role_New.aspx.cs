using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Roles_Role_New : CMSRolesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check "Modify" permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Roles", "Modify"))
        {
            RedirectToAccessDenied("CMS.Roles", "Modify");
        }

        string roleListUrl = "~/CMSModules/Membership/Pages/Roles/Role_List.aspx";
        if (SiteID != 0)
        {
            roleEditElem.SiteID = SiteID;
            roleListUrl += "?siteid=" + SiteID;
        }
        else if (SelectedSiteID != 0)
        {
            roleEditElem.SiteID = SelectedSiteID;
            roleListUrl += "?selectedsiteid=" + roleEditElem.SiteID;
        }
        if ((SiteID == 0) && (SelectedSiteID == 0) && CMSContext.CurrentUser.UserSiteManagerAdmin)
        {
            roleEditElem.GlobalRole = true;
            roleEditElem.SiteID = 0;
        }

        roleEditElem.OnSaved += new EventHandler(roleEditElem_OnSaved);

        // Pagetitle
        CurrentMaster.Title.HelpTopicName = "new_role";
        CurrentMaster.Title.TitleText = GetString("Administration-Role_New.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Role/new.png");

        // Initializes page title breadcrumbs
        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("general.roles");
        pageTitleTabs[0, 1] = roleListUrl;
        pageTitleTabs[1, 0] = GetString("Administration-Role_New.NewRole");
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }


    /// <summary>
    /// OnOK click event handler.
    /// </summary>
    private void roleEditElem_OnSaved(object sender, EventArgs e)
    {
        StringBuilder parameters = new StringBuilder();
        //CMS desk site context        
        if (SiteID != 0)
        {
            parameters.Append("&siteid=", SiteID);
        }

        // CMS site manager site context        
        if (SelectedSiteID != 0)
        {
            parameters.Append("&selectedsiteid=", SelectedSiteID);
        }

        URLHelper.Redirect("Role_Edit_Frameset.aspx?roleId=" + roleEditElem.ItemID + parameters.ToString());
    }
}