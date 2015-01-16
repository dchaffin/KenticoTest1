using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Groups_Tools_MediaLibrary_Library_List : CMSGroupMediaLibraryPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupId = QueryHelper.GetInteger("groupid", 0);
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_READ);

        // Prevent display non-group libraries
        if (groupId == 0)
        {
            groupId = -1;
        }

        elemList.GroupID = groupId;

        string[,] actions = new string[1,6];

        // New item link
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("media.list.newlibrary");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("Library_New.aspx") + "?groupid=" + elemList.GroupID;
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("CMSModules/CMS_MediaLibrary/add.png");

        CurrentMaster.HeaderActions.Actions = actions;

        elemList.OnEdit += new EventHandler(elemList_OnEdit);
    }


    private void elemList_OnEdit(object sender, EventArgs e)
    {
        URLHelper.Redirect(ResolveUrl("Library_Edit.aspx?libraryid=" + elemList.SelectedItemID + "&groupid=" + elemList.GroupID));
    }
}