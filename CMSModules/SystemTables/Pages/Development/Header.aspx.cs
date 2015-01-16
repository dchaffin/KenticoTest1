using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_SystemTables_Pages_Development_Header : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("SystemTable_List.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_SystemTable/object.png");
        CurrentMaster.Title.HelpTopicName = "system_tables_tables";
        CurrentMaster.Title.HelpName = "helpTopic";

        if (!RequestHelper.IsPostBack())
        {
            // Initialize menu
            InitializeMenu();
        }
    }


    /// <summary>
    /// Initializes edit header.
    /// </summary>
    protected void InitializeMenu()
    {
        string[,] tabs = new string[3,4];
        tabs[0, 0] = GetString("general.tables");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'system_tables_tables');";
        tabs[0, 2] = "SystemTable/List.aspx";
        tabs[1, 0] = GetString("systbl.header.views");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'systemtables_views_list');";
        tabs[1, 2] = "Views/Views_List.aspx";
        tabs[2, 0] = GetString("systbl.header.procedures");
        tabs[2, 1] = "SetHelpTopic('helpTopic', 'systemtables_procs_list');";
        tabs[2, 2] = "Views/Proc_List.aspx";

        CurrentMaster.Tabs.Tabs = tabs;
        CurrentMaster.Tabs.UrlTarget = "mcontent";
    }
}