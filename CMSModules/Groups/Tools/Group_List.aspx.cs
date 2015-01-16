using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Groups_Tools_Group_List : CMSGroupPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Pagetitle
        CurrentMaster.Title.TitleText = GetString("group.HeaderCaption");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Community_Group/object.png");
        CurrentMaster.Title.HelpTopicName = "group_list";
        CurrentMaster.Title.HelpName = "helpTopic";

        // New item link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("Group.NewItemCaption");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("~/CMSModules/Groups/Tools/Group_New.aspx");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/Community_Group/add.png");
        CurrentMaster.HeaderActions.Actions = actions;

        // Only current site groups can be listed
        if (CMSContext.CurrentSite != null)
        {
            groupListElem.SiteID = CMSContext.CurrentSite.SiteID;
        }

        groupListElem.OnAction += new CommandEventHandler(groupListElem_OnAction);
    }


    private void groupListElem_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.Trim().ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("~/CMSModules/Groups/Tools/Group_Edit.aspx?groupid=" + e.CommandArgument.ToString());
                break;

            case "delete":
                CheckPermissions(ValidationHelper.GetInteger(e.CommandArgument, 0), CMSAdminControl.PERMISSION_MANAGE);
                URLHelper.Redirect("~/CMSModules/Groups/Tools/Group_Delete.aspx?groupid=" + e.CommandArgument.ToString());
                break;
        }
    }
}