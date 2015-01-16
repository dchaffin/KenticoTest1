using System;
using System.Collections;
using System.Text;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_Menu : CMSUserControl
{
    #region "Private variables"

    private MediaSourceEnum mSourceType = MediaSourceEnum.MediaLibraries;
    private DialogConfiguration mConfig = null;

    private bool mFileSystemActionsEnabled = true;

    // Attachments
    private int mNodeId = 0;
    private int mParentNodeId = 0;
    private int mDocumentID = 0;
    private Guid mFormGUID = Guid.Empty;
    private int mResizeToHeight = 0;
    private int mResizeToWidth = 0;
    private int mResizeToMaxSideSize = 0;

    // Library
    private int mLibraryId = 0;
    private string mLibraryFolderPath = String.Empty;
    private string mNewFolderDialogUrl = null;

    // Fullscreen mode
    private bool mAllowFullscreen = false;
    private bool mIsCopyMoveLinkDialog = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether the control is displayed as part of the copy/move dialog.
    /// </summary>
    public bool IsCopyMoveLinkDialog
    {
        get
        {
            return mIsCopyMoveLinkDialog;
        }
        set
        {
            mIsCopyMoveLinkDialog = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether the Fullscreen button is visible or not.
    /// </summary>
    public bool AllowFullscreen
    {
        get
        {
            return mAllowFullscreen;
        }
        set
        {
            mAllowFullscreen = value;
        }
    }


    /// <summary>
    /// URL of the new media folder dialog.
    /// </summary>
    public string NewFolderDialogUrl
    {
        get
        {
            return mNewFolderDialogUrl;
        }
        set
        {
            mNewFolderDialogUrl = value;
        }
    }

    /// <summary>
    /// Selected source type.
    /// </summary>
    public MediaSourceEnum SourceType
    {
        get
        {
            return mSourceType;
        }
        set
        {
            mSourceType = value;
        }
    }


    /// <summary>
    /// Gets or sets dialog configuration.
    /// </summary>
    public DialogConfiguration Config
    {
        get
        {
            return mConfig;
        }
        set
        {
            mConfig = value;
        }
    }


    /// <summary>
    /// Returns currently selected tab view mode.
    /// </summary>
    public DialogViewModeEnum SelectedViewMode
    {
        get
        {
            return menuView.SelectedViewMode;
        }
        set
        {
            menuView.SelectedViewMode = value;
        }
    }


    public int NodeID
    {
        get
        {
            return mNodeId;
        }
        set
        {
            mNodeId = value;
        }
    }


    /// <summary>
    /// ID of the parent node.
    /// </summary>
    public int ParentNodeID
    {
        get
        {
            return mParentNodeId;
        }
        set
        {
            mParentNodeId = value;
        }
    }


    /// <summary>
    /// ID of the current library.
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
    /// Folder path of the current library.
    /// </summary>
    public string LibraryFolderPath
    {
        get
        {
            return mLibraryFolderPath;
        }
        set
        {
            mLibraryFolderPath = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the document attachments are related to.
    /// </summary>
    public int DocumentID
    {
        get
        {
            return mDocumentID;
        }
        set
        {
            mDocumentID = value;
        }
    }


    /// <summary>
    /// Gets or sets GUID of the form temporary attachments are related to.
    /// </summary>
    public Guid FormGUID
    {
        get
        {
            return mFormGUID;
        }
        set
        {
            mFormGUID = value;
        }
    }


    /// <summary>
    /// Height of attachment.
    /// </summary>
    public int ResizeToHeight
    {
        get
        {
            return mResizeToHeight;
        }
        set
        {
            mResizeToHeight = value;
        }
    }


    /// <summary>
    /// Width of attachment.
    /// </summary>
    public int ResizeToWidth
    {
        get
        {
            return mResizeToWidth;
        }
        set
        {
            mResizeToWidth = value;
        }
    }


    /// <summary>
    /// Max side size of attachment.
    /// </summary>
    public int ResizeToMaxSideSize
    {
        get
        {
            return mResizeToMaxSideSize;
        }
        set
        {
            mResizeToMaxSideSize = value;
        }
    }


    /// <summary>
    /// Indicates if file system actions are enabled.
    /// </summary>
    public bool FileSystemActionsEnabled
    {
        get
        {
            return mFileSystemActionsEnabled;
        }
        set
        {
            mFileSystemActionsEnabled = value;
        }
    }


    /// <summary>
    /// ID that, together with MetaFileObjectType and AttachmentCategory, specifies object the metafile is related to. 
    /// </summary>
    public int MetaFileObjectID
    {
        get;
        set;
    }


    /// <summary>
    /// Object type that, together with MetaFileObjectID and AttachmentCategory, specifies object the metafile is related to.
    /// </summary>
    public string MetaFileObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Object category that, together with MetaFileObjectID and MetaFileObjectType, specifies object the metafile is related to.
    /// </summary>
    public string MetaFileCategory
    {
        get;
        set;
    }


    /// <summary>
    /// Control identifier.
    /// </summary>
    protected string Identifier
    {
        get
        {
            String identifier = ViewState["Identifier"] as String;
            if (identifier == null)
            {
                ViewState["Identifier"] = identifier = Guid.NewGuid().ToString("N");
            }

            return identifier;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize controls
            SetupControls();

            // Initialize actions menu
            InitializeActionsMenu();

            // Initialize view mode menu
            InitializeViewModeMenu();
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reloads the View mode menu.
    /// </summary>
    public void UpdateViewMenu()
    {
        // Initialize actions menu
        InitializeActionsMenu();

        // Initialize view mode menu
        InitializeViewModeMenu();

        // Apply updated information
        fileUploader.ReloadData();

        pnlUpdateActionsMenu.Update();
    }


    /// <summary>
    /// Reloads part of the menu providing file related actions.
    /// </summary>
    public void UpdateActionsMenu()
    {
        // Initialize actions menu
        InitializeActionsMenu();

        // Apply updated information
        fileUploader.ReloadData();

        pnlUpdateActionsMenu.Update();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Register modal dialog script
        ScriptHelper.RegisterDialogScript(Page);

        string disableMenuItem = 
@"function DisableNewFileBtn() {                                      
    $j('#dialogsUploaderDiv').attr('style', 'display:none;');                                 
    $j('#dialogsUploaderDisabledDiv').removeAttr('style');
}

function DisableNewFolderBtn() { " + menuBtnNewFolder.DisableButtonFunction + " }";

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "disableMenuItem", ScriptHelper.GetScript(disableMenuItem));

        if (!FileSystemActionsEnabled)
        {
            // Disable file and folder actions
            menuBtnNewFolder.Enabled = false;
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "disableNewFile", ScriptHelper.GetScript("DisableNewFileBtn();"));
        }

        if (!IsLiveSite)
        {
            // Initialize help
            switch (SourceType)
            {
                case MediaSourceEnum.DocumentAttachments:
                    helpElem.TopicName = "dialogs_{0}attachments";
                    break;
                case MediaSourceEnum.MediaLibraries:
                    helpElem.TopicName = "dialogs_{0}media";
                    break;
                case MediaSourceEnum.Web:
                    helpElem.TopicName = "dialogs_{0}web";
                    break;
                case MediaSourceEnum.MetaFile:
                    helpElem.TopicName = "dialogs_{0}metafiles";
                    break;
                default:
                    helpElem.TopicName = "dialogs_{0}content";
                    break;
            }

            Config = DialogConfiguration.GetDialogConfiguration();
            if ((Config != null) && (Config.OutputFormat == OutputFormatEnum.BBLink) || (Config.OutputFormat == OutputFormatEnum.HTMLLink))
            {
                helpElem.TopicName = string.Format(helpElem.TopicName, "link_");
            }
            else
            {
                helpElem.TopicName = helpElem.TopicName.Replace("{0}", "");
            }

            switch (Config.CustomFormatCode.ToLowerCSafe())
            {
                case "copy":
                    helpElem.TopicName = "document_copy";
                    break;

                case "move":
                    helpElem.TopicName = "document_move";
                    break;

                case "link":
                    helpElem.TopicName = "document_link";
                    menuBtnNewFolder.Visible = false;
                    break;

                case "linkdoc":
                    helpElem.TopicName = "document_linkdoc";
                    menuBtnNewFolder.Visible = false;
                    break;

                case "relationship":
                    helpElem.TopicName = "select_node_for_relationship";
                    menuBtnNewFolder.Visible = false;
                    break;

                case "selectpath":
                    helpElem.TopicName = "select_path";
                    menuBtnNewFolder.Visible = false;
                    break;
            }

            // Media library mode
            if (DisplayMode == ControlDisplayModeEnum.Simple)
            {
                helpElem.StopProcessing = true;
                pnlHelp.Visible = false;
            }
        }
        else
        {
            helpElem.StopProcessing = true;
            pnlHelp.Visible = false;
        }

        // Register resizer script for fullscreen mode
        if (AllowFullscreen)
        {
            ScriptHelper.RegisterResizer(Page);
        }
    }


    /// <summary>
    /// Initializes menu with view mode selection.
    /// </summary>
    private void InitializeActionsMenu()
    {
        if ((SourceType != MediaSourceEnum.DocumentAttachments) && (SourceType != MediaSourceEnum.MetaFile))
        {
            string selectors = (IsLiveSite ? "LiveSelectors" : "Selectors");

            // Get new folder dialog URL
            if (SourceType == MediaSourceEnum.MediaLibraries)
            {
                WindowHelper.Remove(Identifier);

                Hashtable properties = new Hashtable();
                properties.Add("libraryid", LibraryID);
                properties.Add("path", LibraryFolderPath);
                properties.Add("cancel", false);

                WindowHelper.Add(Identifier, properties);

                if (IsLiveSite)
                {
                    if (CMSContext.CurrentUser.IsAuthenticated())
                    {
                        NewFolderDialogUrl = "~/CMS/Dialogs/CMSModules/MediaLibrary/FormControls/LiveSelectors/InsertImageOrMedia/NewMediaFolder.aspx?identifier=" + Identifier;
                    }
                    else
                    {
                        NewFolderDialogUrl = "~/CMSModules/MediaLibrary/FormControls/LiveSelectors/InsertImageOrMedia/NewMediaFolder.aspx?identifier=" + Identifier;
                    }
                }
                else
                {
                    NewFolderDialogUrl = "~/CMSModules/MediaLibrary/FormControls/Selectors/InsertImageOrMedia/NewMediaFolder.aspx?identifier=" + Identifier;
                }
            }
            else
            {
                NewFolderDialogUrl = "~/CMSFormControls/" + selectors + "/InsertImageOrMedia/NewCMSFolder.aspx?nodeid=" + NodeID + "&culture=" + Config.Culture;
            }
            // Add security hash
            NewFolderDialogUrl = URLHelper.AddParameterToUrl(NewFolderDialogUrl, "hash", QueryHelper.GetHash(NewFolderDialogUrl, false));

            menuBtnNewFolder.Tooltip = GetString("dialogs.actions.newfolder.desc");
            menuBtnNewFolder.OnClickJavascript = "modalDialog('" + URLHelper.ResolveUrl(NewFolderDialogUrl) + "', 'NewFolder', 500, 350, null, true); return false;";
            menuBtnNewFolder.Text = "<div style=\"overflow:hidden; width:66px; white-space:nowrap\">" + GetString("dialogs.actions.newfolder") + "</div>";
        }
        else
        {
            // Hide New folder button for attachments
            menuBtnNewFolder.Visible = false;
            plcActionsMenu.Visible = false;
            pnlLeft.CssClass += " Smaller ";
        }

        // Initialize disabled button
        imgUploaderDisabled.EnableViewState = false;
        imgUploaderDisabled.Alt = GetString("dialogs.actions.newfile");

        // If attachments are being displayed and no document or form is specified - hide uploader
        if (!IsCopyMoveLinkDialog && (((SourceType != MediaSourceEnum.DocumentAttachments) && (SourceType != MediaSourceEnum.MetaFile))
            || ((SourceType == MediaSourceEnum.DocumentAttachments) && (Config.AttachmentDocumentID > 0 || Config.AttachmentFormGUID != Guid.Empty))
            || ((SourceType == MediaSourceEnum.MetaFile) && ((MetaFileObjectID > 0) && !string.IsNullOrEmpty(MetaFileObjectType) && !string.IsNullOrEmpty(MetaFileCategory)))))
        {
            // Initialize file uploader
            if (SourceType == MediaSourceEnum.MetaFile)
            {
                fileUploader.ObjectID = MetaFileObjectID;
                fileUploader.ObjectType = MetaFileObjectType;
                fileUploader.Category = MetaFileCategory;

                BaseInfo info = BaseAbstractInfoProvider.GetInfoById(MetaFileObjectType, MetaFileObjectID);

                fileUploader.SiteID = info != null ? info.Generalized.ObjectSiteID : CMSContext.CurrentSiteID;
            }
            else
            {
                fileUploader.DocumentID = DocumentID;
                fileUploader.FormGUID = FormGUID;
                fileUploader.NodeParentNodeID = ((NodeID > 0) ? NodeID : ParentNodeID);
                fileUploader.NodeClassName = URLHelper.EncodeQueryString("CMS.File");
                fileUploader.LibraryID = LibraryID;
                fileUploader.LibraryFolderPath = LibraryFolderPath;
                fileUploader.ResizeToHeight = ResizeToHeight;
                fileUploader.ResizeToMaxSideSize = ResizeToMaxSideSize;
                fileUploader.ResizeToWidth = ResizeToWidth;
                fileUploader.CheckPermissions = true;
            }
            fileUploader.ParentElemID = CMSDialogHelper.GetMediaSource(SourceType);
            fileUploader.SourceType = SourceType;
            fileUploader.IsLiveSite = IsLiveSite;
            fileUploader.InnerDivClass = "DialogMenuInnerDiv";
            fileUploader.InnerDivHtml = String.Format("<span>{0}</span>", GetString("dialogs.actions.newfile"));
            fileUploader.LoadingImageUrl = GetImageUrl("Design/Preloaders/preload16.gif");
            fileUploader.InnerLoadingDivHtml = "&nbsp;";
            fileUploader.UploadMode = MultifileUploaderModeEnum.DirectMultiple;
        }
        else
        {
            plcDirectFileUploader.Visible = false;
            fileUploader.StopProcessing = true;
        }
    }


    /// <summary>
    /// Initializes menu with basic operations.
    /// </summary>
    private void InitializeViewModeMenu()
    {
        if (!IsCopyMoveLinkDialog)
        {
            // Fullscreen
            if (AllowFullscreen)
            {
                // Get fullscreen images
                string imageOff = ResolveUrl(GetImageUrl("CMSModules/CMS_Content/Dialogs/modefullscreenoff.png", IsLiveSite));
                string imageOn = ResolveUrl(GetImageUrl("CMSModules/CMS_Content/Dialogs/modefullscreenon.png", IsLiveSite));

                menuBtnFullScreen.IconUrl = imageOff;
                menuBtnFullScreen.Tooltip = GetString("dialogs.viewmode.fullscreen.desc");
                menuBtnFullScreen.OnClickJavascript = "ToogleFullScreen(elem);return false;";
                menuBtnFullScreen.Text = GetString("general.fullscreen");

                // Create full screen toggle function
                StringBuilder sb = new StringBuilder();
                sb.Append(
@"
function ToogleFullScreen(elem) {
    if (window.maximized) {
        window.maximized = false;
        $j(elem).find('img').attr('src','" + imageOff + @"');
        MaximizeAll(top.window);
    } else {
        window.maximized = true;
        $j(elem).find('img').attr('src','" + imageOn + @"');
        MinimizeAll(top.window);
    }
}
"
                );

                // Register full screen toggle function
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "toogleFullScreen", ScriptHelper.GetScript(sb.ToString()));
            }
            else
            {
                pnlFullScreen.Visible = false;
            }
        }
        else
        {
            menuView.Visible = false;
            pnlFullScreen.Visible = false;
        }
    }

    #endregion
}