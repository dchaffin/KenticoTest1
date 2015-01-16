using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_MediaLibrary_Library_New : CMSGroupMediaLibraryPage
{
    private int mGroupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        mGroupId = QueryHelper.GetInteger("groupid", 0);
        CheckGroupPermissions(mGroupId, CMSAdminControl.PERMISSION_READ);

        // Init breadcrumbs
        string[,] breadcrumbs = new string[2,3];
        breadcrumbs[0, 0] = GetString("media.new.librarylistlink");
        breadcrumbs[0, 1] = "~/CMSModules/Groups/Tools/MediaLibrary/Library_List.aspx?groupid=" + mGroupId;
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = GetString("media.new.newlibrary");
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = breadcrumbs;

        if (mGroupId > 0)
        {
            elemEdit.MediaLibraryID = QueryHelper.GetInteger("libraryid", 0);
            elemEdit.MediaLibraryGroupID = mGroupId;
            elemEdit.OnSaved += new EventHandler(elemEdit_OnSaved);
        }
        else
        {
            elemEdit.Enable = false;
        }

        CurrentMaster.Title.HelpTopicName = "library_new";
        CurrentMaster.Title.HelpName = "helpTopic";
    }


    private void elemEdit_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect(ResolveUrl("Library_Edit.aspx?libraryid=" + elemEdit.MediaLibraryID + "&groupid=" + mGroupId));
    }
}