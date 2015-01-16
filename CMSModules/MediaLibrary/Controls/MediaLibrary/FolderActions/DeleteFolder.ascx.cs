using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.UIControls;
using CMS.SettingsProvider;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_DeleteFolder : CMSAdminControl
{
    #region "Delegates & Events"

    /// <summary>
    /// Delegate of event fired when 'Cancel' button of control is clicked.
    /// </summary>
    public delegate void OnCancelClickEventHandler();

    /// <summary>
    /// Delegate of event fired when folder has been deleted.
    /// </summary>
    public delegate void OnFolderDeletedEventHandler(string pathToSelect);

    /// <summary>
    /// Event raised when 'Cancel' button is clicked.
    /// </summary>
    public event OnCancelClickEventHandler CancelClick;

    /// <summary>
    /// Event raised when folder has been deleted.
    /// </summary>
    public event OnFolderDeletedEventHandler OnFolderDeleted;

    #endregion


    #region "Private variables"

    private int mLibraryId = 0;
    private string mFolderPath = null;
    private string mLibraryFolder = null;
    private string mRootFolderPath = null;
    private string mImageFolderPath = "";

    /// <summary>
    /// Path of the parent folder of currently deleted folder.
    /// </summary>
    private string mDeletedFolderParent = "";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Folder path where the images are stored.
    /// </summary>
    public string ImageFolderPath
    {
        get
        {
            if (mImageFolderPath == "")
            {
                mImageFolderPath = GetImageUrl("CMSModules/CMS_MediaLibrary/", IsLiveSite, true);
            }
            return mImageFolderPath.TrimEnd('/') + "/";
        }
        set
        {
            mImageFolderPath = value;
        }
    }


    /// <summary>
    /// Folder path of the currently processed library.
    /// </summary>
    public string FolderPath
    {
        get
        {
            if (mFolderPath == "")
            {
                mFolderPath = ValidationHelper.GetString(ViewState["LastKnownDeleteFolderPath"], "");
            }

            return mFolderPath;
        }
        set
        {
            mFolderPath = value;
            ViewState["LastKnownDeleteFolderPath"] = value;
        }
    }


    /// <summary>
    /// Library folder of the currently processed library.
    /// </summary>
    public string LibraryFolder
    {
        get
        {
            return mLibraryFolder;
        }
        set
        {
            mLibraryFolder = value;
        }
    }


    /// <summary>
    /// ID of the currently processed media library.
    /// </summary>
    public int LibraryID
    {
        get
        {
            return mLibraryId;
        }
        set
        {
            mLibraryId = value;
        }
    }


    /// <summary>
    /// Gets or sets library root folder path.
    /// </summary>
    public string RootFolderPath
    {
        get
        {
            return mRootFolderPath;
        }
        set
        {
            mRootFolderPath = value;
        }
    }

    #endregion


    protected override void CreateChildControls()
    {
        // Initialize controls
        SetupControl();

        base.CreateChildControls();
    }


    protected override void OnLoad(EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        base.OnLoad(e);
    }


    /// <summary>
    /// Reloads control's content.
    /// </summary>
    public void ReLoadUserControl()
    {
        SetupControl();
    }


    #region "Event handlers"

    protected void btnOk_Click(object sender, EventArgs e)
    {
        MediaLibraryInfo libInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryID);
        if (libInfo != null)
        {
            // Check 'Folder delete' permission
            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(libInfo, "folderdelete"))
            {
                lblError.Text = MediaLibraryHelper.GetAccessDeniedMessage("folderdelete");
                lblError.Visible = true;
                return;
            }

            try
            {
                // Delete folder and all files within
                MediaLibraryInfoProvider.DeleteMediaLibraryFolder(CMSContext.CurrentSiteName, LibraryID, FolderPath, false);

                // Get path of the parent folder of the currently deleted folder
                FolderPath = DirectoryHelper.CombinePath(LibraryFolder, FolderPath);
                mDeletedFolderParent = FolderPath.Remove(FolderPath.LastIndexOfCSafe("\\"));

                // Let the parent control know the folder was deleted
                if (OnFolderDeleted != null)
                {
                    OnFolderDeleted(mDeletedFolderParent);
                }

                lblError.Visible = false;
            }
            catch (Exception ex)
            {
                // Display an error to the user
                lblError.Text = GetString("general.erroroccurred") + " " + ex.Message;
                lblError.Visible = true;
            }
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // Let the parent control know about the event
        if (CancelClick != null)
        {
            CancelClick();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the control and all nested controls.
    /// </summary>
    private void SetupControl()
    {
        // Setup labels
        lblDeleteInfo.Text = string.Format(GetString("media.folder.delete.confirmation"), FolderPath);

        imgTitle.ImageUrl = GetImageUrl("CMSModules/CMS_MediaLibrary/librarydelete.png");

        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");
    }

    #endregion
}