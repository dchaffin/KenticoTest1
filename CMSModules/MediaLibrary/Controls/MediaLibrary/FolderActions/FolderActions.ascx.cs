using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_FolderActions : CMSAdminItemsControl
{
    /// <summary>
    /// Delegate for the events fired whenever some action occurs.
    /// </summary>
    public event OnActionEventHandler OnAction;


    #region "Variables"

    private string mDeleteScript = "";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Path to the folder currently processed.
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
    /// Currently processed library ID.
    /// </summary>
    public int LibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["LibraryID"], 0);
        }
        set
        {
            ViewState["LibraryID"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the DELETE action should be displayed.
    /// </summary>
    public bool DisplayDelete
    {
        get
        {
            return plcDelete.Visible;
        }
        set
        {
            plcDelete.Visible = value;
        }
    }


    /// <summary>
    /// Indicates whether the COPY action should be displayed.
    /// </summary>
    public bool DisplayCopy
    {
        get
        {
            return plcCopy.Visible;
        }
        set
        {
            plcCopy.Visible = value;
        }
    }


    /// <summary>
    /// Indicates whether the copy action is enabled.
    /// </summary>
    public bool CopyEnabled
    {
        get
        {
            return lnkCopy.Enabled;
        }
        set
        {
            lnkCopy.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates whether the MOVE action should be displayed.
    /// </summary>
    public bool DisplayMove
    {
        get
        {
            return plcMove.Visible;
        }
        set
        {
            plcMove.Visible = value;
        }
    }


    /// <summary>
    /// JavaScript called when Delete button is clicked. If specified no postback is raised.
    /// </summary>
    public string DeleteScript
    {
        get
        {
            return mDeleteScript;
        }
        set
        {
            mDeleteScript = value;
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


    protected override void OnLoad(EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        base.OnLoad(e);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize nested controls
            SetupControl();
        }
        else
        {
            Visible = false;
        }
    }


    #region "Event handlers"

    protected void lnkRename_Click(object sender, EventArgs e)
    {
        RaiseOnActionEvent("rename");
    }


    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        RaiseOnActionEvent("delete");
    }


    protected void lnkNew_Click(object sender, EventArgs e)
    {
        RaiseOnActionEvent("new");
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls and control itself.
    /// </summary>
    private void SetupControl()
    {
        // Register modal dialog handling script
        ScriptHelper.RegisterDialogScript(Page);

        // Setup copy action
        lnkCopy.Text = GetString("general.copy");
        lnkCopy.ToolTip = GetString("media.tree.copyfolder");
        imgCopy.ToolTip = GetString("media.tree.copyfolder");

        if (lnkCopy.Enabled)
        {
            imgCopy.ImageUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/librarycopy.png", IsLiveSite));
        }
        else
        {
            imgCopy.ImageUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/librarycopydisabled.png", IsLiveSite));
            lnkCopy.Attributes["style"] = "cursor: default;";
            lnkCopy.OnClientClick = "";
        }

        // Setup move action
        lnkMove.Text = GetString("general.move");
        lnkMove.ToolTip = GetString("media.tree.movefolder");
        imgMove.ToolTip = GetString("media.tree.movefolder");

        // Setup delete action
        lnkDelete.Text = GetString("general.delete");
        lnkDelete.ToolTip = GetString("media.folder.delete");
        imgDelete.ToolTip = GetString("media.folder.delete");

        // If delete script is set
        if (!string.IsNullOrEmpty(DeleteScript))
        {
            // Register delete script
            lnkDelete.OnClientClick = DeleteScript.Replace("##FOLDERPATH##", FolderPath.Replace("\\", "/").Replace("'", "\\'"));
            lnkDelete.Attributes["href"] = "#";
        }

        // If folder path is set
        if (String.IsNullOrEmpty(FolderPath))
        {
            // Disable delete action
            imgDelete.ImageUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/librarydeletedisabled.png", IsLiveSite));
            lnkDelete.Enabled = false;
            lnkDelete.Attributes["style"] = "cursor: default;";
            lnkDelete.OnClientClick = "";

            // Disable move action
            imgMove.ImageUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/librarymovedisabled.png", IsLiveSite));
            lnkMove.Enabled = false;
            lnkMove.Attributes["style"] = "cursor: default;";
            lnkMove.OnClientClick = "";
        }
        else
        {
            // Set enabled images
            imgDelete.ImageUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/librarydelete.png", IsLiveSite));
            imgMove.ImageUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_MediaLibrary/librarymove.png", IsLiveSite));
        }
    }


    /// <summary>
    /// Fires the OnAction event.
    /// </summary>
    /// <param name="actionName">Name of the action that takes place</param>
    private void RaiseOnActionEvent(string actionName)
    {
        // Let other controls now the action takes place
        if (OnAction != null)
        {
            OnAction(actionName, FolderPath);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        string url = (IsLiveSite) ? "~/CMSModules/MediaLibrary/CMSPages/SelectFolder.aspx" : 
            "~/CMSModules/MediaLibrary/Tools/FolderActions/SelectFolder.aspx";
        
        WindowHelper.Remove(Identifier);

        Hashtable properties = new Hashtable();
        properties.Add("folderpath", FolderPath);
        properties.Add("libraryid", LibraryID);

        WindowHelper.Add(Identifier, properties);

        // Add query into url
        url += "?action={0}&identifier=" + Identifier;

        // Create copy and move url
        string copyUrl = String.Format(url, "copy");
        string moveUrl = String.Format(url, "move");

        // Add security hash to urls
        copyUrl = URLHelper.AddParameterToUrl(copyUrl, "hash", QueryHelper.GetHash(copyUrl, false));
        moveUrl = URLHelper.AddParameterToUrl(moveUrl, "hash", QueryHelper.GetHash(moveUrl, false));

        // Register modal dialogs
        lnkCopy.OnClientClick = String.Format("modalDialog('{0}', 'CopyFolder', '90%', '70%'); return false;", ResolveUrl(copyUrl));
        if (!String.IsNullOrEmpty(FolderPath))
        {
            lnkMove.OnClientClick = String.Format("modalDialog('{0}', 'MoveFolder', '90%', '70%'); return false;", ResolveUrl(moveUrl));
        }

        base.OnPreRender(e);
    }

    #endregion
}