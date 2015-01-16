using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;


public partial class CMSModules_Settings_Development_CustomSettings_CustomSettings_Header : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string treeRoot = QueryHelper.GetString("treeroot", "customsettings");

        // Decide which category to use as root: custom settings or settings
        switch (treeRoot)
        {
            case ("settings"):
                CurrentMaster.Title.TitleText = GetString("Development.Settings");
                CurrentMaster.Title.HelpTopicName = "settings_main";
                break;
            case ("customsettings"):
            default:
                CurrentMaster.Title.TitleText = GetString("Development.CustomSettings");
                CurrentMaster.Title.HelpTopicName = "custom_settings_main";
                break;
        }

        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_CustomSettings/object.png");
        CurrentMaster.Title.HelpName = "helpTopic";
    }
}