using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.IO;

public partial class CMSModules_Javascript_Pages_Scripts : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup page title text and image
        CurrentMaster.Title.TitleText = GetString("Development.Javascript");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Javascript/object.png");

        CurrentMaster.Title.HelpTopicName = "development_javascript_files";
        CurrentMaster.Title.HelpName = "helpTopic";

        CurrentMaster.PanelContent.CssClass = "";

        // Register scripts
        ScriptHelper.RegisterJQuery(Page);
        CMSDialogHelper.RegisterDialogHelper(Page);
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "InitResizers", "$j(InitResizers());", true);
        CSSHelper.RegisterCSSBlock(Page, ".TooltipImage{max-width:200px; max-height:200;}");

        // Ensure the directory
        string path = "~/CMSScripts/Custom";

        string filePath = String.Empty;
        try
        {
            filePath = Server.MapPath(path);
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
        config.StartingPath = path;
        config.AllowedExtensions = "js";
        config.NewTextFileExtension = "js";
        config.ShowFolders = false;
        config.AllowManage = true;

        selFile.Config = config;
    }
}