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

public partial class CMSModules_Groups_Tools_Polls_Polls_Edit_General : CMSGroupPollsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PollProperties.ItemID = QueryHelper.GetInteger("pollid", 0);
        PollProperties.SiteID = CMSContext.CurrentSiteID;
        PollProperties.GroupID = QueryHelper.GetInteger("groupid", 0);
        PollProperties.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(PollProperties_OnCheckPermissions);
        PollProperties.Enabled = CheckGroupPermissions(PollProperties.GroupID, CMSAdminControl.PERMISSION_MANAGE, false);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (!RequestHelper.IsPostBack())
        {
            PollProperties.ReloadData();
        }
    }


    private void PollProperties_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckPermissions(PollProperties.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }
}