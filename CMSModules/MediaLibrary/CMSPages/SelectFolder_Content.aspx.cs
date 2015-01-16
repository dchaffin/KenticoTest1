using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_CMSPages_SelectFolder_Content : CMSLiveModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetBrowserClass();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query values
        selectFolder.MediaLibraryID = QueryHelper.GetInteger("libraryid", 0);
        selectFolder.Action = QueryHelper.GetString("action", "");
        selectFolder.FolderPath = QueryHelper.GetString("folderpath", "").Replace("/", "\\");
        selectFolder.Files = QueryHelper.GetString("files", "").Trim('|');
        selectFolder.AllFiles = QueryHelper.GetBoolean("allFiles", false);
    }
}