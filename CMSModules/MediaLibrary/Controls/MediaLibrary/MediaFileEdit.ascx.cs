using System;
using System.Linq;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

using MediaHelper = CMS.GlobalHelper.MediaHelper;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileEdit : CMSAdminControl
{
    #region "Event & delegates"

    /// <summary>
    /// Event fired after saved succeeded.
    /// </summary>
    public event OnActionEventHandler Action;

    #endregion


    #region "Private variables"

    private bool mHasCustomFields = false;

    private MediaFileInfo mFileInfo = null;
    private MediaLibraryInfo mLibraryInfo = null;
    private SiteInfo mLibrarySiteInfo = null;
    private bool mForceReload = false;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether the custom fields tab is displayed.
    /// </summary>
    private bool HasCustomFields
    {
        get
        {
            return mHasCustomFields;
        }
        set
        {
            mHasCustomFields = value;
        }
    }


    /// <summary>
    /// Indicates whether the current file has a preview.
    /// </summary>
    private bool HasPreview
    {
        get
        {
            return MediaLibraryHelper.HasPreview(LibrarySiteInfo.SiteName, MediaLibraryID, FileInfo.FilePath);
        }
    }


    /// <summary>
    /// Previous file id used in custom fields loading.
    /// </summary>
    private int OldFileID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["OldFileID"], 0);
        }
        set
        {
            ViewState["OldFileID"] = value;
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
            if (pnlTabs.SelectedTab == tabEdit)
            {
                return plcMessEdit;
            }
            if (pnlTabs.SelectedTab == tabCustomFields)
            {
                return plcMessCustom;
            }
            return base.MessagesPlaceHolder;
        }
    }


    /// <summary>
    /// Currently edited file info.
    /// </summary>
    public MediaFileInfo FileInfo
    {
        get
        {
            if ((mFileInfo == null) && (FileID > 0))
            {
                mFileInfo = MediaFileInfoProvider.GetMediaFileInfo(FileID);
            }
            return mFileInfo;
        }
        set
        {
            mFileInfo = value;
        }
    }


    /// <summary>
    /// File ID.
    /// </summary>
    public int FileID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["FileID"], 0);
        }
        set
        {
            ViewState["FileID"] = value;
            FileInfo = null;
        }
    }


    /// <summary>
    /// Current file path.
    /// </summary>
    public string FilePath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FilePath"], "");
        }
        set
        {
            ViewState["FilePath"] = value;
        }
    }


    /// <summary>
    /// Current folder path.
    /// </summary>
    public string FolderPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FolderPath"], "");
        }
        set
        {
            ViewState["FolderPath"] = value;
        }
    }


    /// <summary>
    /// Media library ID.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["MediaLibraryID"], 0);
        }
        set
        {
            ViewState["MediaLibraryID"] = value;
        }
    }


    /// <summary>
    /// Gets library info object.
    /// </summary>
    public MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (MediaLibraryID > 0))
            {
                LibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(MediaLibraryID);
            }
            return mLibraryInfo;
        }
        set
        {
            mLibraryInfo = value;
        }
    }


    /// <summary>
    /// Info on the site related to the current library.
    /// </summary>
    public SiteInfo LibrarySiteInfo
    {
        get
        {
            if (mLibrarySiteInfo == null)
            {
                mLibrarySiteInfo = SiteInfoProvider.GetSiteInfo(LibraryInfo.LibrarySiteID);
            }
            return mLibrarySiteInfo;
        }
        set
        {
            mLibrarySiteInfo = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site
    /// Required for versions tab to ensure that control IsLiveSite property presits postback
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsLiveSite"], true);
        }
        set
        {
            ViewState["IsLiveSite"] = value;
        }
    }

    #endregion


    /// <summary>
    /// Page init event
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set IsLiveSite
        imagePreview.IsLiveSite = IsLiveSite;
        mediaPreview.IsLiveSite = IsLiveSite;
        fileUplPreview.IsLiveSite = IsLiveSite;

        formMediaFileCustomFields.IsLiveSite = IsLiveSite;
        formMediaFileCustomFields.StopProcessing = true;
    }


    /// <summary>
    /// Page pre render event
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        
        if ((FileInfo != null) && (LibraryInfo != null) && HasPreview)
        {
            plcPreview.Visible = true;

            string fileName = FileInfo.FileName + "." + FileInfo.FileExtension.TrimStart('.');
            string url = MediaFileInfoProvider.GetMediaFileUrl(FileInfo.FileGUID, fileName);
            url = URLHelper.UpdateParameterInUrl(url, "preview", "1");
            lblPreviewPermaLink.Text = GetFileLinkHtml(URLHelper.ResolveUrl(url));

            if (MediaLibraryHelper.IsExternalLibrary(CMSContext.CurrentSiteName, LibraryInfo.LibraryFolder))
            {
                plcPrevDirPath.Visible = false;
            }
            else
            {
                plcPrevDirPath.Visible = true;
                lblPrevDirectLinkVal.Text = GetFileLinkHtml(GetPrevDirectPath());
            }
        }
        else
        {
            lblNoPreview.Text = GetString("media.file.nothumb");

            plcNoPreview.Visible = true;
            plcPreview.Visible = false;
        }
        pnlUpdatePreviewDetails.Update();

        // Refresh versions tab if selected and reload was forced
        if (mForceReload && (pnlTabs.SelectedTabIndex == tabVersions.Index))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "ReloadVersionsTab", "$j(\"#" + objectVersionList.RefreshButton.ClientID + "\").click();", true);
        }
    }


    /// <summary>
    /// Page load
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            ReLoadUserControl(false);
        }

        plcMessEdit.IsLiveSite = plcMessCustom.IsLiveSite = IsLiveSite;

        // Initialize header actions on edit tab
        btnEdit.HeaderActions = headerActionsEdit;
        btnEdit.ComponentName = headerActionsEdit.ComponentName = "MediaFile_TabEdit";

        // Initialize header actions on custom fields tab
        BasicForm customFields = formMediaFileCustomFields.BasicForm;
        if (customFields != null)
        {
            FormSubmitButton btnCustom = customFields.SubmitButton;
            btnCustom.HeaderActions = headerActionsCustom;
            btnCustom.ComponentName = headerActionsCustom.ComponentName = "MediaFile_TabCustomFields";
        }

        plcMessEdit.WrapperControlClientID = pnlTabEdit.ClientID;
        plcMessCustom.WrapperControlClientID = pnlTabCustomFields.ClientID;
        objectVersionList.MessagesPlaceHolder.WrapperControlClientID = pnlTabVersions.ClientID;

        fileUplPreview.StopProcessing = StopProcessing;
    }


    #region "Public methods"

    /// <summary>
    /// Reloads controls content.
    /// </summary>
    public void ReLoadUserControl()
    {
        ReLoadUserControl(true);
    }


    /// <summary>
    /// Reloads controls content.
    /// </summary>
    /// <param name="forceReload">Indicates whether the content should be reloaded as well</param>
    public void ReLoadUserControl(bool forceReload)
    {
        if (!StopProcessing)
        {
            Visible = true;
            SetupControls(forceReload);
            mForceReload = forceReload;
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Ensures required actions when the file was saved recently.
    /// </summary>
    /// <param name="file">Recently saved file info</param>
    public void AfterSave()
    {
        SetupFile();
        SetupVersions(false);
        pnlUpdateVersions.Update();
    }


    /// <summary>
    /// Sets default values and clear textboxes.
    /// </summary>
    public void SetDefault()
    {
        txtEditDescription.Text = "";
        txtEditName.Text = "";
        txtEditTitle.Text = "";
    }


    /// <summary>
    /// Setup all labels and buttons text.
    /// </summary>
    public void SetupTexts()
    {
        // File form
        lblDirPath.Text = GetString("media.file.dirpath");
        lblPermaLink.Text = GetString("media.file.permalink");

        // Edit form
        lblEditTitle.Text = GetString("media.file.filetitle");
        lblEditDescription.Text = GetString("general.description") + ResHelper.Colon;

        lblCreatedBy.Text = GetString("media.file.createdby");
        lblCreatedWhen.Text = GetString("media.file.createdwhen");
        lblExtension.Text = GetString("media.file.extension");
        lblDimensions.Text = GetString("media.file.dimensions");
        lblModified.Text = GetString("media.file.modified");
        lblFileModified.Text = GetString("media.file.filemodified");
        lblSize.Text = GetString("media.file.size");
        lblFileSize.Text = GetString("media.file.filesize");
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    /// <param name="forceReload">Indicates whether the content should be reloaded as well</param>
    private void SetupControls(bool forceReload)
    {
        ShowProperTabs(forceReload);

        SetupTexts();
    }


    /// <summary>
    /// Returns link HTML to media file.
    /// </summary>
    /// <param name="url">Url to media file</param>
    private static string GetFileLinkHtml(string url)
    {
        return String.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", url);
    }


    /// <summary>
    /// Setup general values.
    /// </summary>
    private void SetupFile()
    {
        // Get file and library info
        if ((FileInfo != null) && (LibraryInfo != null))
        {
            formMediaFileCustomFields.IsLiveSite = IsLiveSite;

            if (MediaLibraryHelper.IsExternalLibrary(CMSContext.CurrentSiteName, LibraryInfo.LibraryFolder))
            {
                plcDirPath.Visible = false;
            }
            else
            {
                string url = MediaFileInfoProvider.GetMediaFileUrl(LibrarySiteInfo.SiteName, LibraryInfo.LibraryFolder, FileInfo.FilePath);
                ltrDirPathValue.Text = GetFileLinkHtml(ResolveUrl(url));
            }
            ltrPermaLinkValue.Text = GetFileLinkHtml(ResolveUrl(MediaFileInfoProvider.GetMediaFileUrl(FileInfo.FileGUID, FileInfo.FileName)));
            if (ImageHelper.IsImage(FileInfo.FileExtension))
            {
                // Ensure max side size 200
                int[] maxsize = ImageHelper.EnsureImageDimensions(0, 0, 200, FileInfo.FileImageWidth, FileInfo.FileImageHeight);
                imagePreview.Width = maxsize[0];
                imagePreview.Height = maxsize[1];

                // If is Image show image properties
                imagePreview.URL = URLHelper.AddParameterToUrl(MediaFileInfoProvider.GetMediaFileUrl(FileInfo.FileGUID, CMSContext.CurrentSiteName), "maxsidesize", "200");
                imagePreview.URL = URLHelper.AddParameterToUrl(imagePreview.URL, "chset", Guid.NewGuid().ToString());
                plcImagePreview.Visible = true;
                plcMediaPreview.Visible = false;

                pnlPrew.Visible = true;
            }
            else if (MediaHelper.IsFlash(FileInfo.FileExtension) || MediaHelper.IsAudio(FileInfo.FileExtension) ||
                     MediaHelper.IsVideo(FileInfo.FileExtension))
            {
                if (MediaHelper.IsAudio(FileInfo.FileExtension))
                {
                    mediaPreview.Height = 45;
                }
                else
                {
                    mediaPreview.Height = 180;
                }
                mediaPreview.Width = 270;

                mediaPreview.AutoPlay = false;
                mediaPreview.AVControls = true;
                mediaPreview.Loop = false;
                mediaPreview.Menu = true;
                mediaPreview.Type = FileInfo.FileExtension;

                // If is Image show image properties
                mediaPreview.Url = MediaFileInfoProvider.GetMediaFileUrl(FileInfo.FileGUID, FileInfo.FileName);
                plcMediaPreview.Visible = true;
                plcImagePreview.Visible = false;

                pnlPrew.Visible = true;
            }
            else
            {
                pnlPrew.Visible = false;
            }
        }
        else
        {
            pnlPrew.Visible = false;
        }
    }


    /// <summary>
    /// Setup preview values.
    /// </summary>
    private void SetupPreview()
    {
        if ((FileInfo != null) && (LibraryInfo != null))
        {
            fileUplPreview.EnableUpdate = MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify");
            fileUplPreview.StopProcessing = false;
            fileUplPreview.IsLiveSite = IsLiveSite;
            fileUplPreview.LibraryFolderPath = FolderPath;
            fileUplPreview.LibraryID = LibraryInfo.LibraryID;
            fileUplPreview.MediaFileID = FileID;
            fileUplPreview.FileInfo = FileInfo;
            fileUplPreview.ReloadData();
        }
        else
        {
            plcPreview.Visible = false;
        }
    }


    /// <summary>
    /// Setup versions tab values.
    /// </summary>
    private void SetupVersions(bool dataReload)
    {
        if (!IsLiveSite && (FileInfo != null) && ObjectVersionManager.DisplayVersionsTab(FileInfo))
        {
            tabVersions.Visible = true;
            tabVersions.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
            objectVersionList.Visible = true;
            objectVersionList.Object = FileInfo;

            // Bind refresh tab script to tab click event
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TabVersionsOnClick", ScriptHelper.GetScript("$j(document).ready(function () {$j(\"#" + tabVersions.ClientID + "_head\").children().click( function() { $j(\"#" + objectVersionList.RefreshButton.ClientID + "\").click();});})"));

            // Register script to refresh content
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ReloadMediaFileEdit", ScriptHelper.GetScript("function RefreshContent() { var button = document.getElementById('" + btnHidden.ClientID + "'); if (button){button.click();}}"));
        }
        else
        {
            tabVersions.Visible = false;
        }
    }


    /// <summary>
    /// Setup edit values.
    /// </summary>
    private void SetupEdit()
    {
        if (FileInfo != null)
        {
            // Fill edit form
            txtEditName.Text = FileInfo.FileName;
            txtEditDescription.Text = FileInfo.FileDescription;
            txtEditTitle.Text = FileInfo.FileTitle;
            UserInfo currentUserInfo = CMSContext.CurrentUser;
            SiteInfo currentSiteInfo = CMSContext.CurrentSite;

            // Created by
            string userName = null;

            UserInfo ui = UserInfoProvider.GetFullUserInfo(FileInfo.FileCreatedByUserID);
            if (ui != null)
            {
                if (ui.IsPublic())
                {
                    userName = GetString("general.na");
                }
                else
                {
                    userName = ui.FullName;
                }
            }
            else
            {
                userName = GetString("general.na");
            }
            lblCreatedByVal.Text = userName;

            // Created when
            DateTime dtCreated = ValidationHelper.GetDateTime(FileInfo.FileCreatedWhen, DateTimeHelper.ZERO_TIME);
            lblCreatedWhenVal.Text = TimeZoneHelper.ConvertToUserTimeZone(dtCreated, false, currentUserInfo, currentSiteInfo);

            // Modified when
            DateTime dtModified = ValidationHelper.GetDateTime(FileInfo.FileModifiedWhen, DateTimeHelper.ZERO_TIME);
            lblModifiedVal.Text = TimeZoneHelper.ConvertToUserTimeZone(dtModified, false, currentUserInfo, currentSiteInfo);

            // Get system file info
            string filePath = MediaFileInfoProvider.GetMediaFilePath(FileInfo.FileLibraryID, FileInfo.FilePath);
            if (File.Exists(filePath))
            {
                FileInfo sysFileInfo = CMS.IO.FileInfo.New(filePath);

                // File modified when
                DateTime dtFileModified = ValidationHelper.GetDateTime(sysFileInfo.LastWriteTime, DateTimeHelper.ZERO_TIME);
                // Display only if system time is 
                if ((dtFileModified - dtModified).TotalSeconds > 5)
                {
                    lblFileModifiedVal.Text = TimeZoneHelper.ConvertToUserTimeZone(dtFileModified, false, currentUserInfo, currentSiteInfo);

                    plcFileModified.Visible = true;
                    plcRefresh.Visible = true;
                }
                else
                {
                    plcFileModified.Visible = false;
                    plcRefresh.Visible = false;
                }

                // File size
                if (sysFileInfo.Length != FileInfo.FileSize)
                {
                    lblFileSizeVal.Text = DataHelper.GetSizeString(sysFileInfo.Length);
                    plcFileSize.Visible = true;
                    plcRefresh.Visible = true;
                }
                else
                {
                    plcFileSize.Visible = false;
                    plcRefresh.Visible = false;
                }
            }

            // Size
            lblSizeVal.Text = DataHelper.GetSizeString(FileInfo.FileSize);

            // Extension
            lblExtensionVal.Text = FileInfo.FileExtension.TrimStart('.').ToLowerCSafe();

            // Dimensions
            if (ImageHelper.IsImage(FileInfo.FileExtension))
            {
                lblDimensionsVal.Text = FileInfo.FileImageWidth + " x " + FileInfo.FileImageHeight;
                plcDimensions.Visible = true;
            }
            else
            {
                plcDimensions.Visible = false;
            }
        }
        else
        {
            txtEditName.Text = "";
            txtEditDescription.Text = "";
            txtEditTitle.Text = "";
        }
    }


    /// <summary>
    /// Ensures that specified script is passed to the parent control.
    /// </summary>
    /// <param name="script">Script to pass</param>
    private void EnsureParentScript(string script)
    {
        RaiseOnAction("setscript", script);
    }


    /// <summary>
    /// Raises action event.
    /// </summary>
    /// <param name="actionName">Name of the action occurring</param>
    /// <param name="actionArgument">Argument related to the action</param>
    private void RaiseOnAction(string actionName, object actionArgument)
    {
        if (Action != null)
        {
            Action(actionName, actionArgument);
        }
    }


    /// <summary>
    /// Display or hides the tabs according to the ViewMode setting.
    /// </summary>
    /// <param name="forceReload">Indicates whether the content should be reloaded as well</param> 
    private void ShowProperTabs(bool forceReload)
    {
        ScriptHelper.RegisterJQuery(Page);

        // We need to remove the header text for unused tabs, because of bug
        // in AjaxToolkit Tab control (when hiding the tab text is still visible)
        tabGeneral.HeaderText = GetString("general.file");
        tabPreview.HeaderText = GetString("general.thumbnail");
        tabEdit.HeaderText = GetString("general.edit");
        tabCustomFields.HeaderText = GetString("general.customfields");
        tabVersions.HeaderText = GetString("objectversioning.tabtitle");

        DisplayCustomFields(forceReload);

        if (forceReload)
        {
            SetupEdit();
        }
        SetupFile();
        SetupPreview();
        SetupVersions(forceReload);
    }


    /// <summary>
    /// Handles custom fields tab displaying.
    /// </summary>
    private void DisplayCustomFields()
    {
        DisplayCustomFields(false);
    }


    /// <summary>
    /// Handles custom fields tab displaying.
    /// </summary>
    /// <param name="forceReload">Indicates whether the content should be reloaded as well</param> 
    private void DisplayCustomFields(bool forceReload)
    {
        // Initialize DataForm
        if ((FileID > 0) && Visible)
        {
            formMediaFileCustomFields.OnBeforeSave += formMediaFileCustomFields_OnBeforeSave;
            formMediaFileCustomFields.OnAfterSave += formMediaFileCustomFields_OnAfterSave;
            formMediaFileCustomFields.OnValidationFailed += formMediaFileCustomFields_OnValidationFailed;

            formMediaFileCustomFields.IsLiveSite = IsLiveSite;
            formMediaFileCustomFields.StopProcessing = false;
            formMediaFileCustomFields.Info = FileInfo;
            formMediaFileCustomFields.ID = "formMediaFileCustomFields" + FileID;

            if ((forceReload) && (OldFileID != FileID))
            {
                formMediaFileCustomFields.ReloadData();
                formMediaFileCustomFields.BasicForm.HideSystemFields = true;
                formMediaFileCustomFields.BasicForm.SubmitButton.CssClass = "SubmitButton";

                // Set proper custom fields data in case new file was created
                formMediaFileCustomFields.BasicForm.Data = FileInfo;
                formMediaFileCustomFields.BasicForm.ReloadData();

                OldFileID = FileID;
            }

            // Initialize custom fields tab if visible
            HasCustomFields = formMediaFileCustomFields.BasicForm.FormInformation.GetFormElements(true, false, true).Any();
            if (HasCustomFields)
            {
                if ((formMediaFileCustomFields.BasicForm != null) && formMediaFileCustomFields.BasicForm.SubmitButton.Visible)
                {
                    // Register the postback control
                    ScriptManager manager = ScriptManager.GetCurrent(Page);
                    if (manager != null)
                    {
                        manager.RegisterPostBackControl(formMediaFileCustomFields.BasicForm.SubmitButton);
                    }
                }

                tabCustomFields.Visible = true;
                plcMediaFileCustomFields.Visible = true;
            }
            else
            {
                formMediaFileCustomFields.StopProcessing = true;
                formMediaFileCustomFields.Enabled = false;
                formMediaFileCustomFields.Visible = false;
                tabCustomFields.Visible = false;
                tabCustomFields.HeaderText = "";
                plcMediaFileCustomFields.Visible = false;
            }
        }

        pnlUpdateCustomFields.Update();
    }


    /// <summary>
    /// Gets direct path for preview image of currently edited media file.
    /// </summary>
    private string GetPrevDirectPath()
    {
        string prevUrl = "";

        // Direct path
        string previewPath = null;
        string previewFolder = null;

        if (Path.GetDirectoryName(FileInfo.FilePath).EndsWithCSafe(MediaLibraryHelper.GetMediaFileHiddenFolder(CMSContext.CurrentSiteName)))
        {
            previewFolder = Path.GetDirectoryName(FileInfo.FilePath) + "\\" + MediaLibraryHelper.GetPreviewFileName(FileInfo.FileName, FileInfo.FileExtension, ".*", CMSContext.CurrentSiteName);
            previewPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(FileInfo.FileLibraryID) + "\\" + previewFolder;
        }
        else
        {
            previewFolder = Path.GetDirectoryName(FileInfo.FilePath) + "\\" + MediaLibraryHelper.GetMediaFileHiddenFolder(CMSContext.CurrentSiteName) + "\\" + MediaLibraryHelper.GetPreviewFileName(FileInfo.FileName, FileInfo.FileExtension, ".*", CMSContext.CurrentSiteName);
            previewPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(FileInfo.FileLibraryID) + "\\" + previewFolder;
        }
        if (Directory.Exists(Path.GetDirectoryName(previewPath)))
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(previewPath), Path.GetFileName(previewPath));
            if (files.Length > 0)
            {
                previewFolder = Path.GetDirectoryName(previewFolder).Replace('\\', '/').TrimStart('/');
                string prevFileName = Path.GetFileName(files[0]);

                prevUrl = MediaFileInfoProvider.GetMediaFileUrl(CMSContext.CurrentSiteName, LibraryInfo.LibraryFolder, previewFolder + '/' + prevFileName);
                prevUrl = URLHelper.ResolveUrl(prevUrl);
            }
        }

        return prevUrl;
    }

    #endregion


    #region "Edit tab"

    /// <summary>
    /// Edit file event handler.
    /// </summary>
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        // Check 'File modify' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filemodify"));

            SetupTexts();
            SetupEdit();

            // Update form
            pnlUpdateFileInfo.Update();
            return;
        }

        FileInfo fi = CMS.IO.FileInfo.New(MediaFileInfoProvider.GetMediaFilePath(CMSContext.CurrentSiteName, LibraryInfo.LibraryFolder, FilePath));
        if ((fi != null) && (LibraryInfo != null))
        {
            // Check if the file exists
            if (!fi.Exists)
            {
                ShowError(GetString("media.error.FileDoesNotExist"));
                return;
            }

            string path = MediaLibraryHelper.EnsurePath(FilePath);
            string fileName = URLHelper.GetSafeFileName(txtEditName.Text.Trim(), CMSContext.CurrentSiteName, false);
            string origFileName = Path.GetFileNameWithoutExtension(fi.FullName);

            Validator fileNameValidator = new Validator()
                .NotEmpty(fileName, GetString("media.error.FileNameIsEmpty"))
                .IsFileName(fileName, GetString("media.error.FileNameIsNotValid"))
                .MatchesCondition(fileName, x => x != "." && x != "..", GetString("media.error.FileNameIsRelative"));
            
            if (!fileNameValidator.IsValid)
            {
                ShowError(HTMLHelper.HTMLEncode(fileNameValidator.Result));
                return;
            }

            if (FileInfo != null)
            {
                if ((CMSContext.CurrentUser != null) && (!CMSContext.CurrentUser.IsPublic()))
                {
                    FileInfo.FileModifiedWhen = CMSContext.CurrentUser.DateTimeNow;
                    FileInfo.FileModifiedByUserID = CMSContext.CurrentUser.UserID;
                }
                else
                {
                    FileInfo.FileModifiedWhen = DateTime.Now;
                }
                // Check if filename is changed ad move file if necessary
                if (fileName != origFileName)
                {
                    try
                    {
                        // Check if file with new file name exists
                        string newFilePath = Path.GetDirectoryName(fi.FullName) + "\\" + fileName + fi.Extension;
                        if (!File.Exists(newFilePath))
                        {
                            string newPath = (string.IsNullOrEmpty(Path.GetDirectoryName(path)) ? "" : Path.GetDirectoryName(path) + "/") + fileName + FileInfo.FileExtension;
                            MediaFileInfoProvider.MoveMediaFile(CMSContext.CurrentSiteName, FileInfo.FileLibraryID, path, newPath, false);
                            FileInfo.FilePath = MediaLibraryHelper.EnsurePath(newPath);
                        }
                        else
                        {
                            ShowError(GetString("media.error.FileExists"));
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(GetString("media.error.RenameFileException"), ex.Message, null);
                        return;
                    }
                }
                // Set media file info
                FileInfo.FileName = fileName;
                FileInfo.FileTitle = txtEditTitle.Text;
                FileInfo.FileDescription = txtEditDescription.Text;

                // Save
                MediaFileInfoProvider.SetMediaFileInfo(FileInfo);
                FilePath = FileInfo.FilePath;

                // Update file modified if not moving physical file
                if (fileName == origFileName)
                {
                    fi.LastWriteTime = FileInfo.FileModifiedWhen;
                }

                // Inform user on success
                ShowChangesSaved();

                SetupEdit();
                pnlUpdateFileInfo.Update();

                SetupTexts();
                SetupFile();
                pnlUpdateGeneral.Update();

                SetupPreview();
                pnlUpdatePreviewDetails.Update();

                SetupVersions(false);
                pnlUpdateVersions.Update();

                RaiseOnAction("rehighlightitem", Path.GetFileName(FileInfo.FilePath));
            }
        }
    }


    /// <summary>
    /// Edit file event handler.
    /// </summary>
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        // Check 'File modify' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filemodify"));

            SetupFile();
            return;
        }

        FileInfo fi = CMS.IO.FileInfo.New(MediaFileInfoProvider.GetMediaFilePath(CMSContext.CurrentSiteName, LibraryInfo.LibraryFolder, FilePath));
        if ((fi != null) && (LibraryInfo != null))
        {
            if (FileInfo != null)
            {
                FileInfo.FileModifiedWhen = DateTime.Now;
                // Set media file info
                FileInfo.FileSize = fi.Length;
                if (ImageHelper.IsImage(FileInfo.FileExtension))
                {
                    ImageHelper ih = new ImageHelper();
                    ih.LoadImage(File.ReadAllBytes(fi.FullName));
                    FileInfo.FileImageWidth = ih.ImageWidth;
                    FileInfo.FileImageHeight = ih.ImageHeight;
                }
                FileInfo.FileTitle = txtEditTitle.Text.Trim();
                FileInfo.FileDescription = txtEditDescription.Text.Trim();

                // Save
                MediaFileInfoProvider.SetMediaFileInfo(FileInfo);

                // Remove old thumbnails
                MediaFileInfoProvider.DeleteMediaFileThumbnails(FileInfo);

                // Inform user on success
                ShowConfirmation(GetString("media.refresh.success"));

                SetupTexts();

                SetupFile();
                pnlUpdateGeneral.Update();

                SetupPreview();
                pnlUpdatePreviewDetails.Update();

                SetupEdit();
                pnlUpdateFileInfo.Update();

                SetupVersions(false);
                pnlUpdateVersions.Update();

                RaiseOnAction("rehighlightitem", Path.GetFileName(FileInfo.FilePath));
            }
        }
    }


    /// <summary>
    /// BreadCrumbs in edit file form.
    /// </summary>
    protected void lnkEditList_Click(object sender, EventArgs e)
    {
        // Hide preview/edit form and show unigrid
        RaiseOnAction("showlist", null);
    }


    /// <summary>
    /// Stores new media file info into the DB.
    /// </summary>
    /// <param name="fi">Info on file to be stored</param>
    /// <param name="description">Description of new media file</param>
    /// <param name="name">Name of new media file</param>
    public MediaFileInfo SaveNewFile(FileInfo fi, string title, string description, string name, string filePath)
    {
        string path = MediaLibraryHelper.EnsurePath(filePath);
        string fileName = name;

        string fullPath = fi.FullName;
        string extension = URLHelper.GetSafeFileName(fi.Extension, CMSContext.CurrentSiteName);

        // Check if filename is changed ad move file if necessary
        if (fileName + extension != fi.Name)
        {
            string oldPath = path;
            fullPath = MediaLibraryHelper.EnsureUniqueFileName(Path.GetDirectoryName(fullPath) + "\\" + fileName + extension);
            path = MediaLibraryHelper.EnsurePath(Path.GetDirectoryName(path) + "/" + Path.GetFileName(fullPath)).TrimStart('/');
            MediaFileInfoProvider.MoveMediaFile(CMSContext.CurrentSiteName, MediaLibraryID, oldPath, path, true);
            fileName = Path.GetFileNameWithoutExtension(fullPath);
        }

        // Create media file info
        MediaFileInfo fileInfo = new MediaFileInfo(fullPath, LibraryInfo.LibraryID, MediaLibraryHelper.EnsurePath(Path.GetDirectoryName(path)), 0, 0, 0);

        fileInfo.FileTitle = title;
        fileInfo.FileDescription = description;

        // Save media file info
        MediaFileInfoProvider.ImportMediaFileInfo(fileInfo);

        // Save FileID in ViewState
        FileID = fileInfo.FileID;
        FilePath = fileInfo.FilePath;

        return fileInfo;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Button hidden click
    /// </summary>
    protected void btnHidden_Click(object sender, EventArgs e)
    {
        // Refresh tree structure after rollback
        RaiseOnAction("reloadmedialibrary", FileID + "|" + Path.GetDirectoryName(FileInfo.FilePath));

        // Reload data and refresh update panels
        ReLoadUserControl();
        pnlUpdateGeneral.Update();
        pnlUpdatePreviewDetails.Update();
        pnlUpdateFileInfo.Update();
    }


    private void formMediaFileCustomFields_OnValidationFailed(object sender, EventArgs e)
    {
        pnlUpdateCustomFields.Update();
    }


    private void formMediaFileCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        ShowChangesSaved();

        SetupEdit();

        SetupVersions(false);
        pnlUpdateVersions.Update();
    }


    private void formMediaFileCustomFields_OnBeforeSave(object sender, EventArgs e)
    {
        // Check 'File modify' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filemodify"));

            DisplayCustomFields(true);
            formMediaFileCustomFields.StopProcessing = true;

            // Update form
            SetupEdit();
        }
    }

    #endregion
}