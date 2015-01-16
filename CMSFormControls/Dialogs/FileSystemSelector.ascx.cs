using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;

public partial class CMSFormControls_Dialogs_FileSystemSelector : FormEngineUserControl, ICallbackEventHandler
{
    #region "Variables"

    private FileSystemDialogConfiguration mDialogConfig = null;
    private bool mAllowEmptyValue = true;
    private string mSelectedPathPrefix = String.Empty;
    private bool mEnabled = true;

    /// <summary>
    /// Hidden value field.
    /// </summary>
    protected HiddenField mHiddenUrl = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Selector value: path of the file or folder.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtPath.Text;
        }
        set
        {
            if (value != null)
            {
                txtPath.Text = value.ToString();
            }
            else
            {
                txtPath.Text = String.Empty;
            }
        }
    }


    /// <summary>
    /// Configuration of the dialog for inserting Images.
    /// </summary>
    public FileSystemDialogConfiguration DialogConfig
    {
        get
        {
            if (mDialogConfig == null)
            {
                mDialogConfig = new FileSystemDialogConfiguration();
            }
            return mDialogConfig;
        }
        set
        {
            mDialogConfig = value;
        }
    }


    /// <summary>
    /// Gets or sets if value of form control could be empty.
    /// </summary>
    public bool AllowEmptyValue
    {
        get
        {
            return mAllowEmptyValue;
        }
        set
        {
            mAllowEmptyValue = value;
        }
    }


    /// <summary>
    /// Gets or sets prefix for paths with preselected source folder(webparts,form controls,...).
    /// </summary>
    public string SelectedPathPrefix
    {
        get
        {
            return mSelectedPathPrefix;
        }
        set
        {
            mSelectedPathPrefix = value;
        }
    }


    /// <summary>
    /// Validates the return value of form control.
    /// </summary>
    public override bool IsValid()
    {
        String value = txtPath.Text.Trim();
        if (AllowEmptyValue && String.IsNullOrEmpty(value))
        {
            return true;
        }

        if (!AllowEmptyValue)
        {
            if (String.IsNullOrEmpty(value))
            {
                if (!DialogConfig.ShowFolders)
                {
                    ValidationError = ResHelper.GetString("UserControlSelector.RequireFileName");
                }
                else
                {
                    ValidationError = ResHelper.GetString("UserControlSelector.RequireFolderName");
                }
                return false;
            }
        }

        if (DialogConfig.ShowFolders)
        {
            if (!IsAllowedAndNotExcludedFolder(value))
            {
                ValidationError = ResHelper.GetString("dialogs.filesystem.NotAllowedFolder");
                return false;
            }
        }
        else
        {
            string ext = (value.Contains(".") ? value.Substring(value.LastIndexOfCSafe(".")) : String.Empty);
            if (!IsAllowedExtension(ext) || IsExcludedExtension(ext))
            {
                if (!String.IsNullOrEmpty(DialogConfig.AllowedExtensions))
                {
                    string allowedExt = ";" + DialogConfig.AllowedExtensions + ";";

                    if (!String.IsNullOrEmpty(DialogConfig.ExcludedExtensions))
                    {
                        foreach (string excludedExt in DialogConfig.ExcludedExtensions.Split(';'))
                        {
                            allowedExt = allowedExt.Replace(";" + excludedExt + ";", ";");
                        }
                    }
                    ValidationError = ResHelper.GetString("dialogs.filesystem.NotAllowedExtension").Replace("%%extensions%%", FormatExtensions(allowedExt));
                }
                else
                {
                    ValidationError = ResHelper.GetString("dialogs.filesystem.ExcludedExtension").Replace("%%extensions%%", FormatExtensions(DialogConfig.ExcludedExtensions));
                }
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Gets or sets if value can be changed.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            txtPath.Enabled = value;
            btnSelect.Enabled = value;
            btnClear.Enabled = value;
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Init event.
    /// </summary>
    /// <param name="sender">Sender parameter</param>
    /// <param name="e">Arguments</param>
    protected void Page_Init(object sender, EventArgs e)
    {
        CreateChildControls();
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
        }
    }


    /// <summary>
    /// Ensure creation of controls.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (mHiddenUrl == null)
        {
            mHiddenUrl = new HiddenField();
            mHiddenUrl.ID = "hidUrl";

            Controls.Add(mHiddenUrl);
        }
    }


    /// <summary>
    /// Setup all contained controls.
    /// </summary>
    private void SetupControls()
    {
        ScriptHelper.RegisterJQuery(Page);
        btnSelect.Text = ResHelper.GetString("General.select");
        btnClear.Text = ResHelper.GetString("General.clear");

        if (Enabled)
        {
            // Configure FileSystem dialog
            string width = DialogConfig.DialogWidth.ToString();
            string height = DialogConfig.DialogHeight.ToString();

            if (DialogConfig.UseRelativeDimensions)
            {
                width += "%";
                height += "%";
            }

            DialogConfig.EditorClientID = txtPath.ClientID;
            DialogConfig.SelectedPath = txtPath.Text;

            ApplyProperties();

            string url = GetDialogURL(DialogConfig, SelectedPathPrefix);

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(Page);

            StringBuilder sb = new StringBuilder();

            sb.Append(
@"
function UpdateModalDialogURL_", ClientID, @"(newValue,context) { 
    var item = document.getElementById(context + '_hidUrl'); 
    if ((item != null) && (item.value != null)) {
        item.value = newValue;
    }
}

function SetMediaValue_", ClientID, @"(selectorId) {
    if (window.Changed) {
        Changed();
    }
    var newValue = document.getElementById(selectorId + '_txtPath').value;
    ", Page.ClientScript.GetCallbackEventReference(this, "newValue", "UpdateModalDialogURL_" + ClientID, "selectorId"), @";
}

function ClearSelection_", ClientID, @"(selectorId) { 
    document.getElementById(selectorId + '_txtPath').value='';
    SetMediaValue_", ClientID, @"(selectorId);
}
"
            );

            // Register the Path related javascript functions
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "FileSystemSelector_" + ClientID, ScriptHelper.GetScript(sb.ToString()));

            // Setup buttons
            txtPath.Attributes.Add("onchange", "SetMediaValue_" + ClientID + "('" + ClientID + "');");

            ScriptHelper.RegisterStartupScript(this, typeof(string), DialogConfig.EditorClientID + "script", ScriptHelper.GetScript(String.Format(
@"
var hidField_{0} = document.getElementById('{0}' + '_hidUrl'); 
if (hidField_{0}) {{
    hidField_{0}.value='{1}';
}}
",
                ClientID,
                url
            )));

            // Setup the buttons
            btnSelect.Attributes.Add("onclick", String.Format(
                "var url_{0} = document.getElementById('{0}' + '_hidUrl').value; modalDialog(url_{0}, 'SelectFile', '{1}', '{2}', null); return false;",
                ClientID,
                width,
                height
            ));

            btnClear.Attributes.Add("onclick", String.Format(
                "ClearSelection_{0}('{0}'); return false;",
                ClientID
            ));
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns query string which will be passed to the CMS dialogs (Insert image or media/Insert link).
    /// </summary>
    /// <param name="config">Dialog configuration</param>  
    /// <param name="selectedPathPrefix">Path prefix of selected value</param>
    public string GetDialogURL(FileSystemDialogConfiguration config, string selectedPathPrefix)
    {
        StringBuilder builder = new StringBuilder();

        // Set constraints
        // Allowed files extensions            
        if (!String.IsNullOrEmpty(config.AllowedExtensions))
        {
            builder.Append("&allowed_extensions=" + Server.UrlEncode(config.AllowedExtensions));
        }

        // New text file extension
        if (!String.IsNullOrEmpty(config.NewTextFileExtension))
        {
            builder.Append("&newfile_extension=" + Server.UrlEncode(config.NewTextFileExtension));
        }

        // Excluded extensions
        if (!String.IsNullOrEmpty(config.ExcludedExtensions))
        {
            builder.Append("&excluded_extensions=" + Server.UrlEncode(config.ExcludedExtensions));
        }

        // Allowed folders 
        if (!String.IsNullOrEmpty(config.AllowedFolders))
        {
            builder.Append("&allowed_folders=" + Server.UrlEncode(config.AllowedFolders));
        }

        // Excluded folders
        if (!String.IsNullOrEmpty(config.ExcludedFolders))
        {
            builder.Append("&excluded_folders=" + Server.UrlEncode(config.ExcludedFolders));
        }

        // Default path-preselected path in filesystem tree
        if (!String.IsNullOrEmpty(config.DefaultPath))
        {
            builder.Append("&default_path=" + Server.UrlEncode(config.DefaultPath));
        }

        // Deny non-application starting path
        if (!config.AllowNonApplicationPath)
        {
            builder.Append("&allow_nonapp_path=0");
        }

        // Allow manage
        if (config.AllowManage)
        {
            builder.Append("&allow_manage=1");
        }

        // SelectedPath - actual value of textbox
        if (!String.IsNullOrEmpty(config.SelectedPath))
        {
            string selectedPath = config.SelectedPath;
            if (!(selectedPath.StartsWithCSafe("~") || selectedPath.StartsWithCSafe("/") || selectedPath.StartsWithCSafe(".") || selectedPath.StartsWithCSafe("\\"))
                && ((String.IsNullOrEmpty(config.StartingPath)) || (config.StartingPath.StartsWithCSafe("~"))) && (!String.IsNullOrEmpty(selectedPathPrefix)))
            {
                selectedPath = selectedPathPrefix.TrimEnd('/') + "/" + selectedPath.TrimStart('/');
            }
            builder.Append("&selected_path=" + Server.UrlEncode(selectedPath));
        }

        // Starting path in filesystem
        if (!String.IsNullOrEmpty(config.StartingPath))
        {
            builder.Append("&starting_path=" + Server.UrlEncode(config.StartingPath));
        }

        // Show only folders|files
        builder.Append("&show_folders=" + Server.UrlEncode(config.ShowFolders.ToString()));

        // Editor client id
        if (!String.IsNullOrEmpty(config.EditorClientID))
        {
            builder.Append("&editor_clientid=" + Server.UrlEncode(config.EditorClientID));
        }

        // Get hash for complete query string
        string query = HttpUtility.UrlPathEncode("?" + builder.ToString().TrimStart('&'));
        
        // Get complete query string with attached hash
        query = URLHelper.AddParameterToUrl(query, "hash", QueryHelper.GetHash(query));

        string baseUrl = "~/CMSFormControls/Selectors/";

        // Get complet URL
        return ResolveUrl(baseUrl + "SelectFileOrFolder/Default.aspx" + query);
    }


    /// <summary>
    /// Check if manually typed extension is allowed.
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>True if allowed, false otherwise</returns>
    private bool IsAllowedExtension(string extension)
    {
        if (String.IsNullOrEmpty(DialogConfig.AllowedExtensions))
        {
            return true;
        }
        else
        {
            string extensions = ";" + DialogConfig.AllowedExtensions.ToLowerCSafe() + ";";
            if (extensions.Contains(";" + extension.ToLowerCSafe().TrimStart('.') + ";"))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Check if manually typed extension is excluded.
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>True if excluded, false otherwise</returns>
    private bool IsExcludedExtension(string extension)
    {
        if (String.IsNullOrEmpty(DialogConfig.ExcludedExtensions))
        {
            return false;
        }
        else
        {
            string extensions = ";" + DialogConfig.ExcludedExtensions.ToLowerCSafe() + ";";
            if (extensions.Contains(";" + extension.ToLowerCSafe().TrimStart('.') + ";"))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Check if folder is allowed and not excluded.
    /// </summary>
    /// <param name="info">DiretoryInfo to check</param>
    /// <returns>True if folder isallowed and not excluded otherwise false</returns>
    private bool IsAllowedAndNotExcludedFolder(string folder)
    {
        bool isAllowed = false;
        bool isExcluded = false;

        string startPath = DialogConfig.StartingPath;

        // Resolve relative URL with ~
        if (startPath.StartsWithCSafe("~"))
        {
            startPath = ResolveUrl(startPath);
        }

        // Map to server if not network path
        if (!startPath.Contains("\\\\"))
        {
            startPath = Server.MapPath(startPath);
        }

        startPath = DirectoryHelper.EnsurePathBackSlash(startPath.ToLowerCSafe());
        string folderName = DirectoryHelper.EnsurePathBackSlash(folder.ToLowerCSafe());
        try
        {
            folderName = Server.MapPath(folderName);
        }
        catch
        {
        }

        folderName = folderName.ToLowerCSafe();

        // Check if folder is allowed
        if (String.IsNullOrEmpty(DialogConfig.AllowedFolders))
        {
            isAllowed = true;
        }
        else
        {
            foreach (string path in DialogConfig.AllowedFolders.ToLowerCSafe().Split(';'))
            {
                if (folderName.StartsWithCSafe(startPath + path))
                {
                    isAllowed = true;
                }
            }
        }

        // Check if folder isn't excluded
        if (!String.IsNullOrEmpty(DialogConfig.ExcludedFolders))
        {
            foreach (string path in DialogConfig.ExcludedFolders.ToLowerCSafe().Split(';'))
            {
                if (folderName.StartsWithCSafe(startPath + path))
                {
                    isExcluded = true;
                }
            }
        }
        return (isAllowed) && (!isExcluded);
    }


    /// <summary>
    /// Format extensions.
    /// </summary>
    /// <param name="extensions">Extensions string to be displayed</param>
    private string FormatExtensions(string extensions)
    {
        string ext = ";" + extensions.Trim(';');
        return ext.Replace(";", ";.").TrimStart(';').Replace(";", ", ");
    }

    #endregion


    #region "Callback handling"

    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        //LoadDisplayValues(eventArgument);

        // Configure dialog
        FileSystemDialogConfiguration config = DialogConfig;

        string width = config.DialogWidth.ToString();
        string height = config.DialogHeight.ToString();
        if (config.UseRelativeDimensions)
        {
            width += "%";
            height += "%";
        }
        config.SelectedPath = eventArgument;

        ApplyProperties();

        string url = GetDialogURL(config, SelectedPathPrefix);

        mHiddenUrl.Value = url;
    }


    /// <summary>
    /// Applies properties to the dialog configuration
    /// </summary>
    private void ApplyProperties()
    {
        FileSystemDialogConfiguration config = DialogConfig;

        // Apply starting path
        string startingPath = ValidationHelper.GetString(GetValue("StartingPath"), null);
        if (!String.IsNullOrEmpty(startingPath))
        {
            config.StartingPath = startingPath;
        }

        // Apply allowed extensions
        string allowedExtensions = ValidationHelper.GetString(GetValue("AllowedExtensions"), null);
        if (!String.IsNullOrEmpty(allowedExtensions))
        {
            config.AllowedExtensions = allowedExtensions;
        }

        // Apply allowed extensions
        string newExtension = ValidationHelper.GetString(GetValue("NewTextFileExtension"), null);
        if (!String.IsNullOrEmpty(newExtension))
        {
            config.NewTextFileExtension = newExtension;
        }

        // Allow management
        object allowManage = GetValue("AllowManage");
        if (allowManage != null)
        {
            config.AllowManage = ValidationHelper.GetBoolean(allowManage, false);
        }
    }


    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return mHiddenUrl.Value;
    }

    #endregion
}