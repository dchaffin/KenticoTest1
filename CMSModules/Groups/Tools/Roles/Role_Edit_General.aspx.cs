using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Roles_Role_Edit_General : CMSGroupRolesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the editing control
        roleEditElem.ItemID = QueryHelper.GetInteger("roleid", 0);

        // Edit/Create only roles from current site
        if (CMSContext.CurrentSite != null)
        {
            roleEditElem.SiteID = CMSContext.CurrentSite.SiteID;
        }

        roleEditElem.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(roleEditElem_OnCheckPermissions);

        roleEditElem.GroupID = QueryHelper.GetInteger("groupid", 0);
    }


    private void roleEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckPermissions(roleEditElem.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }
}