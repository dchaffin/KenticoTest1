using System;
using System.Collections;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.UIControls;
using CMS.EventLog;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_Menu : CMSUserControl
{
    #region "Public properties"

    /// <summary>
    /// Target folder path for physical files.
    /// </summary>
    public string TargetFolderPath
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets which files with extensions are allowed to be uploaded.
    /// </summary>
    public string AllowedExtensions
    {
        get;
        set;
    }


    /// <summary>
    /// Extension of a new file allowed to be created
    /// </summary>
    public string NewTextFileExtension
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if help icon should be hidden.
    /// </summary>
    public bool RemoveHelpIcon
    {
        get;
        set;
    }


    /// <summary>
    /// Delete folder button
    /// </summary>
    public bool EnableDeleteFolder
    {
        get
        {
            return menuBtnDeleteFolder.Enabled;
        }
        set
        {
            menuBtnDeleteFolder.Enabled = value;
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


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            // Initialize actions menu
            InitializeActionsMenu();
        }
        else
        {
            Visible = false;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            // Initialize controls
            SetupControls();
        }
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


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        menuBtnDeleteFolder.OnClick += menuBtnDeleteFolder_OnClick;

        if (IsLiveSite || RemoveHelpIcon)
        {
            helpElem.StopProcessing = true;
            pnlHelp.Visible = false;
        }
        else
        {
            // Initialize help
            helpElem.TopicName = "dialogs_filesystem";
        }
    }


    /// <summary>
    /// Initializes menu with view mode selection.
    /// </summary>
    private void InitializeActionsMenu()
    {
        string url = null;

        // New folder button
        WindowHelper.Remove(Identifier);

        Hashtable properties = new Hashtable();
        properties.Add("targetpath", TargetFolderPath);

        string query = "?identifier=" + Identifier;
        url = URLHelper.ResolveUrl("~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/NewFolder.aspx") + query;
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url, false));

        menuBtnNewFolder.Tooltip = GetString("dialogs.actions.newfolder.desc");
        menuBtnNewFolder.OnClickJavascript = String.Format("modalDialog('{0}', 'NewFolder', 500, 350, null, true); return false;", url);
        menuBtnNewFolder.Text = String.Format("<div style=\"overflow:hidden; width:66px; white-space:nowrap\">{0}</div>", GetString("dialogs.actions.newfolder"));

        // Delete folder button
        menuBtnDeleteFolder.Tooltip = GetString("media.folder.delete");
        menuBtnDeleteFolder.OnClickJavascript = String.Format("if (!confirm('{0}')) {{ return false; }} ", GetString("General.ConfirmDelete"));
        menuBtnDeleteFolder.Text = String.Format("<div style=\"overflow:hidden; width:71px; white-space:nowrap\">{0}</div>", GetString("media.folder.delete"));

        // New file button
        if (!String.IsNullOrEmpty(NewTextFileExtension))
        {
            plcNewFile.Visible = true;

            properties.Add("newfileextension", NewTextFileExtension);

            url = URLHelper.ResolveUrl("~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/EditTextFile.aspx") + query;
            url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url, false));

            btnNew.Tooltip = GetString("dialogs.actions.newfile.desc");
            btnNew.OnClickJavascript = String.Format("modalDialog('{0}', 'NewFile', 905, 670, null, true); return false;", url);
            btnNew.Text = String.Format("<div style=\"overflow:hidden; width:66px; white-space:nowrap\">{0}</div>", GetString("dialogs.actions.newfile"));
        }
        else
        {
            plcNewFile.Visible = false;
        }

        WindowHelper.Add(Identifier, properties);

        // Initialize disabled button
        imgUploaderDisabled.EnableViewState = false;
        if (IsLiveSite)
        {
            imgUploaderDisabled.Src = ResolveUrl(GetImageUrl("CMSModules/CMS_Content/Dialogs/addfiledisabledlife.png"));
        }
        else
        {
            imgUploaderDisabled.Src = ResolveUrl(GetImageUrl("CMSModules/CMS_Content/Dialogs/addfiledisabled.png"));
        }
        imgUploaderDisabled.Alt = GetString("dialogs.actions.newfile");

        // Initialize file uploader
        fileUploader.SourceType = MediaSourceEnum.PhysicalFile;
        fileUploader.TargetFolderPath = TargetFolderPath;
        fileUploader.IsLiveSite = IsLiveSite;
        fileUploader.InnerDivClass = "DialogMenuInnerDiv";
        fileUploader.LoadingImageUrl = GetImageUrl("Design/Preloaders/preload16.gif");
        fileUploader.UploadMode = MultifileUploaderModeEnum.DirectMultiple;
        fileUploader.AllowedExtensions = AllowedExtensions;
        fileUploader.AfterSaveJavascript = "SetRefreshAction";
        fileUploader.InnerDivHtml = String.Format("<span>{0}</span>", GetString("dialogs.actions.uploadfile"));
    }


    private void menuBtnDeleteFolder_OnClick(object sender, EventArgs e)
    {
        try
        {
            // Delete the folder
            Directory.Delete(TargetFolderPath, true);

            // Refresh the tree
            string parentPath = Path.GetDirectoryName(TargetFolderPath);

            string script = String.Format("SetParentAction('{0}')", ScriptHelper.GetString(parentPath, false));

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "DeleteRefresh", ScriptHelper.GetScript(script));

            pnlUpdateActionsMenu.Update();
        }
        catch (Exception ex)
        {
            LogAndShowError("FileSystemSelector", "DELETEFOLDER", ex);
        }
    }

    #endregion
}