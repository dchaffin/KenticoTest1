using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Community;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Group_Delete : CMSGroupPage
{
    #region "Private variables"

    private string mGroupListUrl = "~/CMSModules/Groups/Tools/Group_List.aspx";
    private GroupInfo gi = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Only community manager can delete group
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToCMSDeskAccessDenied("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE);
        }

        int groupId = QueryHelper.GetInteger("groupid", 0);
        gi = GroupInfoProvider.GetGroupInfo(groupId);

        if (gi != null)
        {
            lblMsg.Style.Add("font-weight", "bold");
            mGroupListUrl = ResolveUrl(mGroupListUrl);

            // Pagetitle
            CurrentMaster.Title.TitleText = GetString("group.deletegroup") + " \"" + HTMLHelper.HTMLEncode(gi.GroupDisplayName) + "\"";
            CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Groups/groupdelete.png");

            // Initializes breadcrumbs
            string[,] breadcrumbs = new string[2,3];
            breadcrumbs[0, 0] = GetString("group.deletegroup.listlink");
            breadcrumbs[0, 1] = mGroupListUrl;
            breadcrumbs[0, 2] = "";
            breadcrumbs[1, 0] = HTMLHelper.HTMLEncode(gi.GroupDisplayName);
            breadcrumbs[1, 1] = "";
            breadcrumbs[1, 2] = "";
            CurrentMaster.Title.Breadcrumbs = breadcrumbs;

            btnDelete.Click += new EventHandler(btnDelete_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }
    }


    private void btnDelete_Click(object sender, EventArgs e)
    {
        // Only community manager can delete group
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToCMSDeskAccessDenied("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE);
        }

        if (gi != null)
        {
            try
            {
                GroupInfoProvider.DeleteGroupInfo(gi, chkDeleteAll.Checked);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, EventLogProvider.GetExceptionLogMessage(ex), null);
                return;
            }
        }
        URLHelper.Redirect(mGroupListUrl);
    }


    private void btnCancel_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(mGroupListUrl);
    }

    #endregion
}