using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MediaLibrary;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Tools_Library_Edit_Files : CMSMediaLibraryPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureScriptManager();

        CurrentMaster.PanelContent.CssClass = "";

        libraryElem.LibraryID = QueryHelper.GetInteger("libraryid", 0);
        libraryElem.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(libraryElem_OnCheckPermissions);
    }


    private void libraryElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        MediaLibraryInfo MediaLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(QueryHelper.GetInteger("libraryid", 0));
        if (permissionType.ToLowerCSafe() == "read")
        {
            // Check 'Read' permission
            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(MediaLibrary, permissionType))
            {
                RedirectToAccessDenied("cms.medialibrary", "Read");
            }
        }
    }
}