using System;
using System.Collections.Generic;
using System.Data;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_UI_MediaLibraryEdit : CMSAdminEditControl
{
    #region "Private variables"

    private int mMediaLibraryID = 0;
    private int mMediaLibraryGroupID = 0;
    private Guid mMediaLibraryGroupGUID = Guid.Empty;
    private bool mEnable = true;
    private MediaLibraryInfo mLibraryInfo = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current library info.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (MediaLibraryID > 0))
            {
                // Get data
                mLibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(MediaLibraryID);

                // Check whether library belongs to requested group when entered 
                if ((mLibraryInfo != null) && (MediaLibraryGroupID > 0) && (mLibraryInfo.LibraryGroupID != MediaLibraryGroupID))
                {
                    mLibraryInfo = null;
                }

                // Check whether library belongs to current site when not global admin
                if ((mLibraryInfo != null) && (!CMSContext.CurrentUser.IsGlobalAdministrator) && (mLibraryInfo.LibrarySiteID != CMSContext.CurrentSiteID))
                {
                    mLibraryInfo = null;
                }
            }

            return mLibraryInfo;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Gets or sets media library ID.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return mMediaLibraryID;
        }
        set
        {
            mMediaLibraryID = value;
            mLibraryInfo = null;
        }
    }


    /// <summary>
    /// Gets or sets media library group ID.
    /// </summary>
    public int MediaLibraryGroupID
    {
        get
        {
            return mMediaLibraryGroupID;
        }
        set
        {
            mMediaLibraryGroupID = value;
        }
    }


    /// <summary>
    /// Gets or sets media library group GUID.
    /// </summary>
    public Guid MediaLibraryGroupGUID
    {
        get
        {
            return mMediaLibraryGroupGUID;
        }
        set
        {
            mMediaLibraryGroupGUID = value;
        }
    }


    /// <summary>
    /// Indicates whether editing form is enabled.
    /// </summary>
    public bool Enable
    {
        get
        {
            return mEnable;
        }
        set
        {
            mEnable = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            plcMess.IsLiveSite = value;
            txtDisplayName.IsLiveSite = value;
            ucMetaFile.IsLiveSite = value;
        }
    }

    #endregion

    #region "Life-cycle events"

    protected void Page_Load(object sender, EventArgs e)
    {
        txtDisplayName.IsLiveSite = IsLiveSite;
        txtDescription.IsLiveSite = IsLiveSite;

        if (!StopProcessing)
        {
            // Initialize only when visible
            if (Visible)
            {
                InitializeControl();
            }

            if (!URLHelper.IsPostback() && !IsLiveSite)
            {
                LoadLibrary();
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Reload data for live site management
        if (Visible && IsLiveSite)
        {
            ReloadData();
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads control's content.
    /// </summary>
    public override void ReloadData()
    {
        InitializeControl();
        LoadLibrary();
    }


    /// <summary>
    /// Clears the content of editing form - used by Live control.
    /// </summary>
    public override void ClearForm()
    {
        txtCodeName.Text = string.Empty;
        txtDescription.Text = string.Empty;
        txtDisplayName.Text = string.Empty;
        txtFolder.Text = string.Empty;
        ucMetaFile.ClearControl();
    }

    #endregion


    #region "Private methods"

    private void InitializeControl()
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        // Hide code name edit for simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
        }

        // Initialize controls
        SetupControls();

        SetupTexts();

        if (!Enable)
        {
            DisableForm();
        }

        ControlsHelper.RegisterPostbackControl(btnOk);
    }


    /// <summary>
    /// Loads library data if available.
    /// </summary>
    private void LoadLibrary()
    {
        // Get info and load controls            
        if (LibraryInfo != null)
        {
            FillForm(LibraryInfo);
        }
        else if (MediaLibraryID > 0)
        {
            plcProperties.Visible = false;
            ShowError(GetString("general.invalidid"));
            EditedObject = LibraryInfo;
        }
        else
        {
            plcProperties.Visible = true;
        }
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Get media library info        
        if (LibraryInfo != null)
        {
            ucMetaFile.ObjectID = LibraryInfo.LibraryID;
            ucMetaFile.SiteID = LibraryInfo.LibrarySiteID;
            txtFolder.Enabled = false;

            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, PERMISSION_MANAGE))
            {
                // Disable MetaFile uploader
                ucMetaFile.Enabled = false;
            }
        }
        else
        {
            ucMetaFile.ObjectID = 0;
            txtFolder.Enabled = true;
        }
        ucMetaFile.ObjectType = MediaLibraryObjectType.MEDIALIBRARY;
        ucMetaFile.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;
        ucMetaFile.OnAfterDelete += ucMetaFile_OnAfterDelete;
        ucMetaFile.OnAfterUpload += ucMetaFile_OnAfterUpload;
        ucMetaFile.ReloadData();
    }


    /// <summary>
    /// Initializes inner control labels.
    /// </summary>
    private void SetupTexts()
    {
        lblDisplayName.ResourceString = "general.displayname";
        lblDisplayName.DisplayColon = true;
        lblCodeName.ResourceString = "general.codename";
        lblCodeName.DisplayColon = true;
        lblDescription.ResourceString = "general.description";
        lblDescription.DisplayColon = true;
        lblFolder.ResourceString = "general.foldername";
        lblFolder.DisplayColon = true;
        btnOk.ResourceString = "general.ok";
        lblTeaser.DisplayColon = true;
        lblTeaser.ResourceString = "media.general.teaser";
    }


    /// <summary>
    /// Fills editing form with selected library data.
    /// </summary>
    /// <param name="library">Library info holding data</param>
    private void FillForm(MediaLibraryInfo library)
    {
        txtDisplayName.Text = library.LibraryDisplayName;
        txtCodeName.Text = library.LibraryName;
        txtDescription.Text = library.LibraryDescription;
        txtFolder.Text = library.LibraryFolder;
        txtFolder.Enabled = false;
    }


    /// <summary>
    /// Returns a value indicating whether the specified media library code name is unique.
    /// </summary>
    /// <param name="codeName">A code name of the media library.</param>
    private bool IsCodeNameUnique(string codeName)
    {
        MediaLibraryInfo library = null;
        if (MediaLibraryGroupID > 0)
        {
            library = MediaLibraryInfoProvider.GetMediaLibraryInfo(codeName, CMSContext.CurrentSiteID, MediaLibraryGroupID);
        }
        else
        {
            library = MediaLibraryInfoProvider.GetMediaLibraryInfo(codeName, CMSContext.CurrentSiteName);
        }

        if ((library != null) && (library.LibraryID != MediaLibraryID))
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Returns a value indicating whether the specified media library root folder name is unique.
    /// </summary>
    /// <param name="folderName">A name of the media library root folder.</param>
    private bool IsFolderNameUnique(string folderName)
    {
        DataSet ds = MediaLibraryInfoProvider.GetMediaLibraries("LibraryFolder = '" + folderName.Replace("'", "''") + "' AND LibrarySiteID = " + CMSContext.CurrentSiteID, null);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            int libraryId = ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["LibraryID"], 0);
            if (MediaLibraryID != libraryId)
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Returns a value indicating whether it is possible to store a file to the specified directory path.
    /// </summary>
    /// <param name="path">A directory path.</param>
    private bool CanStoreTeaser(string path)
    {
        // Ensure meta files folder
        if (!Directory.Exists(path))
        {
            DirectoryHelper.EnsureDiskPath(path, SettingsKeyProvider.WebApplicationPhysicalPath);
        }

        // Check permission for image folder
        if (!DirectoryHelper.CheckPermissions(path))
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Validates user input data and returns the result.
    /// </summary>
    /// <param name="codeName">A code name of the media library.</param>
    private bool ValidateForm(string codeName)
    {
        bool result = true;

        txtDisplayName.Text = txtDisplayName.Text.Trim();
        txtDescription.Text = txtDescription.Text.Trim();
        txtFolder.Text = URLHelper.GetSafeFileName(txtFolder.Text.Trim(), CMSContext.CurrentSiteName);

        List<Validator> validators = new List<Validator>(7);
        validators.Add(new Validator().NotEmpty(txtDisplayName.Text, GetString("general.requiresdisplayname")));
        validators.Add(new Validator().NotEmpty(codeName, GetString("general.requirescodename")).IsCodeName(codeName, GetString("general.invalidcodename")).MatchesCondition(codeName, x => IsCodeNameUnique(x), GetString("general.codenameexists")));
        if (txtFolder.Enabled)
        {
            validators.Add(new Validator().NotEmpty(txtFolder.Text, GetString("media.error.FolderNameIsEmpty")).IsFolderName(txtFolder.Text, GetString("media.error.FolderNameIsNotValid")).MatchesCondition(txtFolder.Text, x => x != "." && x != "..", GetString("media.error.FolderNameIsRelative")).MatchesCondition(txtFolder.Text, x => IsFolderNameUnique(x), GetString("media.error.FolderExists")));
        }
        Validator teaserValidator = new Validator().MatchesCondition(ucMetaFile.IsValid(), x => x, ucMetaFile.ValidationError);
        if (!teaserValidator.IsValid && (ucMetaFile.PostedFile != null) && MetaFileInfoProvider.StoreFilesInFileSystem(CMSContext.CurrentSiteName))
        {
            string path = MetaFileInfoProvider.GetFilesFolderPath(CMSContext.CurrentSiteName);
            teaserValidator = teaserValidator.MatchesCondition(path, x => CanStoreTeaser(path), String.Format(GetString("media.AccessDeniedToPath"), path));
        }
        validators.Add(teaserValidator);

        foreach (Validator validator in validators)
        {
            if (!validator.IsValid)
            {
                AddError(HTMLHelper.HTMLEncode(validator.Result), null);
                result = false;
            }
        }

        return result;
    }


    /// <summary>
    /// Updates metafile image path.
    /// </summary>
    private void UpdateImagePath(MediaLibraryInfo mli)
    {
        // Update image path according to its meta file
        DataSet ds = MetaFileInfoProvider.GetMetaFiles(ucMetaFile.ObjectID, mli.TypeInfo.ObjectType, MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL, null, null);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            MetaFileInfo metaFile = new MetaFileInfo(ds.Tables[0].Rows[0]);
            mli.LibraryTeaserGuid = metaFile.MetaFileGUID;
            mli.LibraryTeaserPath = MetaFileInfoProvider.GetMetaFileUrl(metaFile.MetaFileGUID, metaFile.MetaFileName);
        }
        else
        {
            mli.LibraryTeaserGuid = Guid.Empty;
            mli.LibraryTeaserPath = String.Empty;
        }
    }


    /// <summary>
    /// Disable editing form.
    /// </summary>
    private void DisableForm()
    {
        lblCodeName.Enabled = false;
        lblDescription.Enabled = false;
        lblDisplayName.Enabled = false;
        lblFolder.Enabled = false;
        lblTeaser.Enabled = false;

        txtCodeName.Enabled = false;
        txtDescription.Enabled = false;
        txtDisplayName.Enabled = false;
        txtFolder.Enabled = false;

        ucMetaFile.Enabled = false;
        btnOk.Enabled = false;
    }


    /// <summary>
    /// Returns a value indicating whether the current user is authorized to manage the edited media library.
    /// </summary>
    private bool CheckPermissions()
    {
        if (LibraryInfo != null)
        {
            return MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "Manage");
        }
        if (CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.MediaLibrary", "Manage"))
        {
            return true;
        }
        if ((MediaLibraryGroupID > 0) && (CMSContext.CurrentUser.IsGroupAdministrator(MediaLibraryGroupID)))
        {
            return true;
        }

        return false;
    }

    #endregion


    #region "Event handlers"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check whether the current user is authorized to manage the edited media library
        if (!CheckPermissions())
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("Manage"));
            return;
        }

        // Get the code name of the edited media library
        string codeName = txtCodeName.Text;
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            if (MediaLibraryID == 0)
            {
                codeName = ValidationHelper.GetCodeName(txtDisplayName.Text, null, "_group_" + Guid.NewGuid());
            }
            else
            {
                codeName = LibraryInfo.LibraryName;
            }
        }
        codeName = codeName.Trim();

        // Validate user input
        if (!ValidateForm(codeName))
        {
            return;
        }

        // Create new object (record) if needed
        MediaLibraryInfo mli = (MediaLibraryID > 0) ? LibraryInfo : new MediaLibraryInfo();

        mli.LibraryDisplayName = txtDisplayName.Text;

        if (txtFolder.Enabled)
        {
            mli.LibraryFolder = txtFolder.Text;
        }

        mli.LibraryDescription = txtDescription.Text;
        mli.LibraryName = codeName;

        // If the library is group related
        if (MediaLibraryGroupID > 0)
        {
            mli.LibraryGroupID = MediaLibraryGroupID;
            // If creating new group library setup default security

            if (MediaLibraryID == 0)
            {
                // Set default group media library security
                mli.FileCreate = SecurityAccessEnum.GroupMembers;
                mli.FileDelete = SecurityAccessEnum.Nobody;
                mli.FileModify = SecurityAccessEnum.Nobody;
                mli.FolderCreate = SecurityAccessEnum.Nobody;
                mli.FolderDelete = SecurityAccessEnum.Nobody;
                mli.FolderModify = SecurityAccessEnum.Nobody;
                mli.Access = SecurityAccessEnum.GroupMembers;
            }
        }

        mli.LibrarySiteID = CMSContext.CurrentSiteID;

        try
        {
            MediaLibraryInfoProvider.SetMediaLibraryInfo(mli);
        }
        catch (Exception ex)
        {
            ShowError(GetString("general.errorsaving"), ex.Message, null);
            return;
        }

        if ((mli != null) && (mli.LibraryID != 0))
        {
            // Add teaser image to media library
            ucMetaFile.ObjectID = mli.LibraryID;
            ucMetaFile.SiteID = mli.LibrarySiteID;
            ucMetaFile.UploadFile();
            UpdateImagePath(mli);

            try
            {
                MediaLibraryInfoProvider.SetMediaLibraryInfo(mli);
            }
            catch (Exception ex)
            {
                ShowError(GetString("general.errorsaving"), ex.Message, null);
                return;
            }
            // Update current media library id
            MediaLibraryID = mli.LibraryID;

            ShowChangesSaved();

            FillForm(mli);

            RaiseOnSaved();
        }

        // Reload header if changes were saved
        ScriptHelper.RefreshTabHeader(Page, GetString("general.general"));
    }


    protected void ucMetaFile_OnAfterDelete(object sender, EventArgs e)
    {
        if (MediaLibraryID > 0)
        {
            MediaLibraryInfo info = LibraryInfo;
            if (info != null)
            {
                //info.LibraryTeaserPath = String.Empty;
                //info.LibraryTeaserGuid = Guid.Empty;
                UpdateImagePath(info);
                MediaLibraryInfoProvider.SetMediaLibraryInfo(info);
            }
        }
    }


    protected void ucMetaFile_OnAfterUpload(object sender, EventArgs e)
    {
        if (MediaLibraryID > 0) {
            MediaLibraryInfo info = LibraryInfo;
            if (info != null)
            {
                UpdateImagePath(info);
                MediaLibraryInfoProvider.SetMediaLibraryInfo(info);
            }
        }
    }

    #endregion
}