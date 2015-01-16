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
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FileUpload : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Private variables"

    private string mFolderPath = "";
    private int mLibraryId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Returns the current folder path where the files should be uploaded.
    /// </summary>
    public string FolderPath
    {
        get
        {
            return mFolderPath;
        }
        set
        {
            mFolderPath = value;
        }
    }


    /// <summary>
    /// ID of the library file belongs to.
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

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        MediaLibraryInfo libInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryID);
        EditedObject = libInfo;

        if (libInfo != null)
        {
            // Set the request timeout
            Server.ScriptTimeout = AttachmentHelper.ScriptTimeout;

            // Register script handling repeated media file creation
            ltlPostBackScript.Text = ScriptHelper.GetScript("function LocalSaveAnother() {" + Page.ClientScript.GetPostBackEventReference(this, null) + "}"
                                                            + " function SaveDocument(nodeId, createAnother){ if ( !createAnother ) { LocalSave(createAnother) } else { LocalSaveAnother() } }");
        }
        else
        {
            pnlMenu.Visible = false;
            pnlForm.Visible = false;

            lblError.Visible = true;
            lblError.Text = GetString("media.newfile.librarynotexists");
        }

        // Register wopener script
        ScriptHelper.RegisterWOpenerScript(Page);
    }


    #region "Event handlers"

    protected void btnOk_Click(object sender, EventArgs e)
    {
        MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryID);

        // Check 'File create' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "filecreate"))
        {
            RaiseOnNotAllowed("filecreate");
            return;
        }

        // No file for upload specified
        if (!FileUpload.HasFile)
        {
            lblError.Text = GetString("media.newfile.errorempty");
            return;
        }

        // Check if preview file is image
        if ((PreviewUpload.HasFile) &&
            (!ImageHelper.IsImage(Path.GetExtension(PreviewUpload.FileName))) &&
            (Path.GetExtension(PreviewUpload.FileName).ToLowerCSafe() != ".ico") &&
            (Path.GetExtension(PreviewUpload.FileName).ToLowerCSafe() != ".tif") &&
            (Path.GetExtension(PreviewUpload.FileName).ToLowerCSafe() != ".tiff") &&
            (Path.GetExtension(PreviewUpload.FileName).ToLowerCSafe() != ".wmf"))
        {
            lblError.Text = GetString("Media.File.PreviewIsNotImage");
            return;
        }

        if (mli != null)
        {
            // Get file extension
            string fileExtension = Path.GetExtension(FileUpload.FileName).TrimStart('.');

            // Check file extension
            if (MediaLibraryHelper.IsExtensionAllowed(fileExtension))
            {
                try
                {
                    // Create new media file record
                    MediaFileInfo mediaFile = new MediaFileInfo(FileUpload.PostedFile, LibraryID, FolderPath);
                    mediaFile.FileDescription = txtFileDescription.Text;

                    // Save the new file info
                    MediaFileInfoProvider.SetMediaFileInfo(mediaFile);

                    // Save preview if presented
                    if (PreviewUpload.HasFile)
                    {
                        string previewSuffix = MediaLibraryHelper.GetMediaFilePreviewSuffix(CMSContext.CurrentSiteName);

                        if (!String.IsNullOrEmpty(previewSuffix))
                        {
                            string previewExtension = Path.GetExtension(PreviewUpload.PostedFile.FileName);
                            string previewName = Path.GetFileNameWithoutExtension(MediaLibraryHelper.GetPreviewFileName(mediaFile.FileName, mediaFile.FileExtension, previewExtension, CMSContext.CurrentSiteName, previewSuffix));
                            string previewFolder = MediaLibraryHelper.EnsurePath(FolderPath.TrimEnd('/')) + "/" + MediaLibraryHelper.GetMediaFileHiddenFolder(CMSContext.CurrentSiteName);

                            byte[] previewFileBinary = new byte[PreviewUpload.PostedFile.ContentLength];
                            PreviewUpload.PostedFile.InputStream.Read(previewFileBinary, 0, PreviewUpload.PostedFile.ContentLength);

                            // Save preview file
                            MediaFileInfoProvider.SaveFileToDisk(CMSContext.CurrentSiteName, mli.LibraryFolder, previewFolder, previewName, previewExtension, mediaFile.FileGUID, previewFileBinary, false);

                            // Log synchronization task
                            SynchronizationHelper.LogObjectChange(mediaFile, TaskTypeEnum.UpdateObject);
                        }
                    }

                    string normalizedFolderPath = MediaLibraryHelper.EnsurePath(FolderPath).Trim('/');

                    // If the event was fired by control itself- no save another media file should be proceeded
                    if (e != null)
                    {
                        ltlScript.Text += ScriptHelper.GetScript("RefreshAndClose('" + normalizedFolderPath.Replace("\'", "\\'") + "');");
                    }
                    else
                    {
                        ltlScript.Text += ScriptHelper.GetScript("RefreshParent('" + normalizedFolderPath.Replace("\'", "\\'") + "');");
                    }
                }
                catch (Exception ex)
                {
                    // Creation of new media file failed
                    lblError.Text = GetString("media.newfile.failed") + ": " + ex.Message;
                }
            }
            else
            {
                // The file with extension selected isn't allowed 
                lblError.Text = String.Format(GetString("media.newfile.extensionnotallowed"), fileExtension);
            }
        }
    }

    #endregion


    #region "Post back event handler"

    public void RaisePostBackEvent(string eventArgument)
    {
        // Save the media file in a proper way
        btnOk_Click(btnOk, null);

        // The another media file should be created- clean up the fields
        txtFileDescription.Text = "";
    }

    #endregion
}