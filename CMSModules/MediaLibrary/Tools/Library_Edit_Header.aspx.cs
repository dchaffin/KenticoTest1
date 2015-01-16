using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MediaLibrary;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Tools_Library_Edit_Header : CMSMediaLibraryPage
{
    protected int libraryId;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the media library ID
        libraryId = QueryHelper.GetInteger("libraryId", 0);

        // Get library info
        MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(libraryId);
        if (mli != null)
        {
            InitalizeMenu();

            // Initialize the page title
            string[,] breadcrumbs = new string[2,3];
            breadcrumbs[0, 0] = GetString("media.header.itemlistlink");
            breadcrumbs[0, 1] = "~/CMSModules/MediaLibrary/Tools/Library_List.aspx";
            breadcrumbs[0, 2] = "_parent";
            breadcrumbs[1, 0] = mli.LibraryDisplayName;
            breadcrumbs[1, 1] = "";
            breadcrumbs[1, 2] = "";

            CurrentMaster.Title.TitleText = GetString("media.header.headercaption");
            CurrentMaster.Title.TitleImage = ResolveUrl(GetImageUrl("Objects/Media_Library/object.png"));
            CurrentMaster.Title.Breadcrumbs = breadcrumbs;

            CurrentMaster.Title.HelpTopicName = "library_files";
            CurrentMaster.Title.HelpName = "helpTopic";

            CurrentMaster.BodyClass += " MediaLibrary LightTabs";
        }
    }


    /// <summary>
    /// Initializes edit menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        string[,] tabs = new string[5,4];
        tabs[0, 0] = GetString("general.files");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'library_files');";
        tabs[0, 2] = "Library_Edit_Files.aspx?libraryid=" + libraryId;
        tabs[1, 0] = GetString("general.general");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'library_general');";
        tabs[1, 2] = "Library_Edit_General.aspx?libraryid=" + libraryId;
        tabs[2, 0] = GetString("general.security");
        tabs[2, 1] = "SetHelpTopic('helpTopic', 'library_security');";
        tabs[2, 2] = "Library_Edit_Security.aspx?libraryid=" + libraryId;

        CurrentMaster.Tabs.Tabs = tabs;
        CurrentMaster.Tabs.UrlTarget = "mediaContent";
    }
}