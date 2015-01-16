using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_MediaLibrary_Library_Edit_General : CMSGroupMediaLibraryPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupId = QueryHelper.GetInteger("groupid", 0);

        if (groupId <= 0)
        {
            elemEdit.Enable = false;
        }
        else
        {
            elemEdit.MediaLibraryID = QueryHelper.GetInteger("libraryid", 0);
            elemEdit.MediaLibraryGroupID = groupId;
        }
    }
}