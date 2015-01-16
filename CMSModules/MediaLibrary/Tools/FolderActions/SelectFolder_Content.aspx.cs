using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Tools_FolderActions_SelectFolder_Content : CMSMediaLibraryModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetBrowserClass();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            String identifier = QueryHelper.GetString("identifier", null);
            if (!String.IsNullOrEmpty(identifier))
            {
                Hashtable properties = WindowHelper.GetItem(identifier) as Hashtable;
                if (properties != null)
                {
                    // Get query values
                    selectFolder.MediaLibraryID = ValidationHelper.GetInteger(properties["libraryid"], 0);
                    selectFolder.Action = QueryHelper.GetString("action", "");
                    selectFolder.FolderPath = ValidationHelper.GetString(properties["folderpath"], "").Replace("/", "\\");
                    selectFolder.Files = ValidationHelper.GetString(properties["files"], "").Trim('|');
                    selectFolder.AllFiles = ValidationHelper.GetBoolean(properties["allFiles"], false);
                }
            }
        }
    }
}