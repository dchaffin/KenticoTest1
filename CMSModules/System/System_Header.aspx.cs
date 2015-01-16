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

public partial class CMSModules_System_System_Header : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Pagetitle
        CurrentMaster.Title.TitleText = GetString("Administration-System.Header");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_System/module.png");
        CurrentMaster.Title.HelpTopicName = "general_tab10";
        CurrentMaster.Title.HelpName = "helpTopic";

        if (!RequestHelper.IsPostBack())
        {
            InitalizeMenu();
        }
    }


    /// <summary>
    /// Initializes menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        bool dbseparationAvailable = LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.DBSeparation);
        string[,] tabs;
        if (dbseparationAvailable)
        {
            tabs = new string[7, 4];  
        }
        else
        {
            tabs = new string[6, 4];  
        }

        tabs[0, 0] = GetString("general.general");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'general_tab10');";
        tabs[0, 2] = "System.aspx";

        tabs[1, 0] = GetString("general.email");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'email_tab');";
        tabs[1, 2] = "System_Email.aspx";

        tabs[2, 0] = GetString("Administration-System.Files");
        tabs[2, 1] = "SetHelpTopic('helpTopic', 'files_tab');";
        tabs[2, 2] = "Files/System_FilesFrameset.aspx";

        tabs[3, 0] = GetString("Administration-System.Deployment");
        tabs[3, 1] = "SetHelpTopic('helpTopic', 'virtualobjects_tab');";
        tabs[3, 2] = "System_Deployment.aspx";

        // Debug tab
        tabs[4, 0] = GetString("Administration-System.Debug");
        tabs[4, 1] = "SetHelpTopic('helpTopic', 'debug_tab');";
        tabs[4, 2] = "Debug/System_DebugFrameset.aspx";

        // Macros tab
        tabs[5, 0] = GetString("Administration-System.Macros");
        tabs[5, 1] = "SetHelpTopic('helpTopic', 'macros_tab');";
        tabs[5, 2] = "System_Macros.aspx";

        // DB separation
        if (dbseparationAvailable)
        {
            tabs[6, 0] = GetString("separationDB.tabtitle");
            tabs[6, 1] = "SetHelpTopic('helpTopic', 'separationDB_tab');";
            tabs[6, 2] = "System_DBseparation.aspx";
        }

        CurrentMaster.Tabs.UrlTarget = "systemContent";
        CurrentMaster.Tabs.Tabs = tabs;
    }
}