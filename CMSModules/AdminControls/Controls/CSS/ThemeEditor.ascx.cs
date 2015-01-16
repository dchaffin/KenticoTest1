using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_CSS_ThemeEditor : CMSUserControl
{
    /// <summary>
    /// Path to the folder that is edited
    /// </summary>
    public string Path
    {
        get;
        set;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the filesystem browser
        if (!String.IsNullOrEmpty(Path))
        {
            // Register scripts
            ScriptHelper.RegisterJQuery(Page);
            CMSDialogHelper.RegisterDialogHelper(Page);
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "InitResizers", "$j(InitResizers());", true);
            CSSHelper.RegisterCSSBlock(Page, ".TooltipImage{max-width:200px; max-height:200;}");

            string filePath = String.Empty;
            try
            {
                filePath = Server.MapPath(Path);
            }
            catch (Exception ex)
            {
                selFile.Visible = false;
                ShowError(ex.Message);
                return;
            }

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // Setup the browser
            FileSystemDialogConfiguration config = new FileSystemDialogConfiguration();
            config.StartingPath = Path;
            config.AllowedExtensions = "gif;png;bmp;jpg;jpeg;css;skin";
            config.NewTextFileExtension = "css";
            config.ShowFolders = false;
            config.AllowManage = true;

            selFile.Config = config;
        }
    }
}