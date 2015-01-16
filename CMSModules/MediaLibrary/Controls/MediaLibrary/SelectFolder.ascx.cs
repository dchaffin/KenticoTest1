using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_SelectFolder : CMSAdminControl
{
    #region "Variables"

    private int mMediaLibraryID = 0;
    private string mAction = null;
    private string mFolderPath = null;
    private string mFiles = null;
    private bool mAllFiles = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the media library to display.
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
        }
    }


    /// <summary>
    /// Action control is displayed for.
    /// </summary>
    public string Action
    {
        get
        {
            return mAction;
        }
        set
        {
            mAction = value;
        }
    }


    /// <summary>
    /// Folder path of the files action is related to.
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
    /// Sets of file names action is related to.
    /// </summary>
    public string Files
    {
        get
        {
            return mFiles;
        }
        set
        {
            mFiles = value;
        }
    }


    /// <summary>
    /// Indicates whether all available files should be processed.
    /// </summary>
    public bool AllFiles
    {
        get
        {
            return mAllFiles;
        }
        set
        {
            mAllFiles = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            SetupControls();
        }
        else
        {
            mediaLibrary.StopProcessing = true;
            mediaLibrary.ShouldProcess = false;
            string url = ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1");
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "redirect", ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }"));
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (IsLiveSite)
        {
            // Register custom CSS
            CSSHelper.RegisterCSSLink(Page, CMSContext.CurrentDocumentStylesheetName, "Skin.css");
        }
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Setup title
        InitializeTitle();

        mediaLibrary.IsLiveSite = IsLiveSite;
        mediaLibrary.ShouldProcess = true;
        mediaLibrary.LibraryID = MediaLibraryID;
        mediaLibrary.Action = Action;
        mediaLibrary.CopyMovePath = FolderPath;
        mediaLibrary.Files = Files;
        mediaLibrary.AllFiles = AllFiles;
    }


    #region "Private methods"

    /// <summary>
    /// Setup title according to action.
    /// </summary>
    private void InitializeTitle()
    {
        if ((Files != "") || AllFiles)
        {
            if (Action == "copy")
            {
                titleElem.TitleText = GetString("media.tree.copyfiles");
                titleElem.TitleImage = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/filecopy.png", IsLiveSite));
            }
            else
            {
                titleElem.TitleText = GetString("media.tree.movefiles");
                titleElem.TitleImage = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/filemove.png", IsLiveSite));
            }
        }
        else
        {
            if (Action == "copy")
            {
                titleElem.TitleText = GetString("media.tree.copyfolder");
                titleElem.TitleImage = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/foldercopy.png", IsLiveSite));
            }
            else
            {
                titleElem.TitleText = GetString("media.tree.movefolder");
                titleElem.TitleImage = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/foldermove.png", IsLiveSite));
            }
        }
    }

    #endregion
}