using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_FormControls_Selectors_InsertImageOrMedia_NewMediaFolder : CMSModalPage
{
    #region "Private variables"

    private int mLibraryId = 0;
    private string mFolderPath = "";
    private MediaLibraryInfo mLibrary = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current library ID.
    /// </summary>
    private MediaLibraryInfo Library
    {
        get
        {
            if ((mLibrary == null) && (mLibraryId > 0))
            {
                mLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(mLibraryId);
            }
            return mLibrary;
        }
        set
        {
            mLibrary = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            // Check site availability
            if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.MediaLibrary", CMSContext.CurrentSiteName))
            {
                RedirectToResourceNotAvailableOnSite("CMS.MediaLibrary");
            }

            // Initialize controls
            SetupControls();
        }
        else
        {
            createFolder.Visible = false;
            string url = ResolveUrl(String.Format("~/CMSMessages/Error.aspx?title={0}&text={1}&cancel=1", GetString("dialogs.badhashtitle"), GetString("dialogs.badhashtext")));
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "redirect", ScriptHelper.GetScript(String.Format("if (window.parent != null) {{ window.parent.location = '{0}' }}", url)));
        }
    }


    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void SetupControls()
    {
        String identifier = QueryHelper.GetString("identifier", null);
        if (!String.IsNullOrEmpty(identifier))
        {
            Hashtable properties = WindowHelper.GetItem(identifier) as Hashtable;
            if (properties != null)
            {
                // Get data
                mLibraryId = ValidationHelper.GetInteger(properties["libraryid"], 0);
                mFolderPath = ValidationHelper.GetString(properties["path"], String.Empty);
                EditedObject = Library;

                if (Library != null)
                {
                    createFolder.OnFolderChange += createFolder_OnFolderChange;
                    createFolder.CancelClick += createFolder_CancelClick;
                    createFolder.IsLiveSite = false;

                    // Initialize information on library
                    createFolder.LibraryID = mLibraryId;
                    createFolder.LibraryFolder = Library.LibraryFolder;
                    createFolder.FolderPath = mFolderPath;
                    createFolder.DisplayCancel = true;
                }

                Page.Header.Title = GetString("dialogs.newfoldertitle");

                CurrentMaster.Title.TitleText = GetString("media.folder.new");
                CurrentMaster.Title.TitleImage = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/librarynew.png", true));
            }
        }
    }


    #region "Event handlers"

    protected void createFolder_CancelClick()
    {
        string cancelScript = null;

        bool postbackOnCancel = QueryHelper.GetBoolean("cancel", true);
        if (postbackOnCancel)
        {
            cancelScript = "wopener.SetAction('cancelfolder', ''); wopener.RaiseHiddenPostBack(); CloseDialog();";
        }
        else
        {
            cancelScript = " CloseDialog();";
        }

        ltlScript.Text += ScriptHelper.GetScript(cancelScript);
    }


    protected void createFolder_OnFolderChange(string pathToSelect)
    {
        ltlScript.Text = ScriptHelper.GetScript(String.Format("wopener.SetAction('newfolder', '{0}'); wopener.RaiseHiddenPostBack(); CloseDialog();", pathToSelect.Replace('\\', '|').Replace("'", "\\'")));
    }

    #endregion
}