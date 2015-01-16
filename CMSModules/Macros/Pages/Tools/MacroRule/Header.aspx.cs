using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.SettingsProvider;

// Edited object
[EditedObject(SiteObjectType.MACRORULE, "ruleid")]


// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "macros.macrorule.list", "~/CMSModules/Macros/Pages/Tools/MacroRule/List.aspx?module={?module?}&displaytitle={?displaytitle?}", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}")]

// Tabs
[Tabs("content")]
[Tab(0, "general.general", "Tab_General.aspx?ruleid={%EditedObject.ID%}", "SetHelpTopic('helpTopic', 'macros_macrorule_general');")]
[Tab(1, "general.parameters", "Tab_Parameters.aspx?ruleid={%EditedObject.ID%}", "SetHelpTopic('helpTopic', 'macros_macrorule_parameters');")]

public partial class CMSModules_Macros_Pages_Tools_MacroRule_Header : CMSToolsPage
{
    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.GetBoolean("displaytitle", true))
        {
            CurrentMaster.Title.TitleText = GetString("macros.macrorule.edit");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_MacroRule/object.png");
        }

        CurrentMaster.Title.HelpTopicName = "macros_macrorule_general";
        CurrentMaster.Title.HelpName = "helpTopic";
    }

    #endregion
}
