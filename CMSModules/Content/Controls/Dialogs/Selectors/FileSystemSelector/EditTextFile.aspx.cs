using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_EditTextFile : CMSModalSiteManagerPage
{
    #region "Variables"

    private string filePath = null;
    private string fileName = null;

    private string extension = null;

    private bool newFile = false;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            String identifier = QueryHelper.GetString("identifier", null);
            if (!String.IsNullOrEmpty(identifier))
            {
                Hashtable properties = WindowHelper.GetItem(identifier) as Hashtable;
                if (properties != null)
                {
                    lblName.Text = GetString("general.filename");

                    filePath = Server.MapPath(ValidationHelper.GetString(properties["filepath"], ""));
                    extension = ValidationHelper.GetString(properties["newfileextension"], "");

                    if (!String.IsNullOrEmpty(extension))
                    {
                        // New file
                        newFile = true;

                        if (!extension.StartsWith("."))
                        {
                            extension = "." + extension;
                        }

                        filePath = ValidationHelper.GetString(properties["targetpath"], "");

                        CurrentMaster.Title.TitleText = GetString("filemanagernew.header.file");
                    }
                    else
                    {
                        // Edit file
                        if (!File.Exists(filePath))
                        {
                            // Redirect to error page
                            URLHelper.Redirect("~/CMSMessages/Error.aspx?text=" + GetString("general.filedoesntexist") + "&cancel=1");
                        }

                        fileName = Path.GetFileNameWithoutExtension(filePath);

                        // Setup the controls
                        if (!RequestHelper.IsPostBack())
                        {
                            txtName.Text = fileName;
                            txtContent.Text = File.ReadAllText(filePath);
                        }

                        extension = Path.GetExtension(filePath);

                        CurrentMaster.Title.TitleText = GetString("filemanageredit.header.file");
                    }

                    // Setup the syntax highlighting
                    switch (extension.TrimStart('.').ToLowerCSafe())
                    {
                        case "css":
                            txtContent.Language = LanguageEnum.CSS;
                            break;

                        case "skin":
                            txtContent.Language = LanguageEnum.ASPNET;
                            break;

                        case "xml":
                            txtContent.Language = LanguageEnum.XML;
                            break;

                        case "html":
                            txtContent.Language = LanguageEnum.HTMLMixed;
                            break;

                        case "cs":
                            txtContent.Language = LanguageEnum.CSharp;
                            break;

                        case "js":
                            txtContent.Language = LanguageEnum.JavaScript;
                            break;
                    }

                    // Setup the labels
                    lblExt.Text = extension;

                    CurrentMaster.Title.TitleImage = ResolveUrl(GetFileIconUrl(extension, "icons"));
                }
            }

            txtContent.FullScreenParentElementID = "divContent";
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check valid input
        string errMsg = new Validator().
            NotEmpty(txtName.Text, GetString("img.errors.filename")).
            IsFolderName(txtName.Text, GetString("img.errors.filename")).
            Result;

        // Prepare the path
        string path = filePath;
        if (!newFile)
        {
            path = Path.GetDirectoryName(path);
        }
        path += "\\" + txtName.Text + extension;

        // Check the file name for existence
        if (!txtName.Text.EqualsCSafe(fileName, true))
        {
            if (File.Exists(path))
            {
                errMsg = GetString("general.fileexists");
            }
        }

        if (!String.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);
        }
        else
        {
            try
            {
                if (!newFile && !path.EqualsCSafe(filePath, true))
                {
                    // Move the file to the new location
                    File.WriteAllText(filePath, txtContent.Text);
                    File.Move(filePath, path);
                }
                else
                {
                    // Create the file or write into it
                    File.WriteAllText(path, txtContent.Text);
                }

                string script = "wopener.SetRefreshAction(); CloseDialog()";

                ltlScript.Text = ScriptHelper.GetScript(script);
            }
            catch (Exception ex)
            {
                ShowError(GetString("general.saveerror"), ex.Message, null);

                // Log the exception
                EventLogProvider.LogException("FileSystemSelector", "SAVEFILE", ex);
            }
        }
    }
}