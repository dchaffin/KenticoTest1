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
[Breadcrumb(0, "macros.macrorule.list", "~/CMSModules/Workflows/Pages/MacroRule/List.aspx", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}")]

// Help
[Help("macros_macrorule_general", "helpTopic")]

// Tabs
[Tabs("content")]
[Tab(0, "general.general", "~/CMSModules/Workflows/Pages/MacroRule/Tab_General.aspx?ruleid={%EditedObject.ID%}", "SetHelpTopic('helpTopic', 'macros_macrorule_general');")]
[Tab(1, "general.parameters", "~/CMSModules/Workflows/Pages/MacroRule/Tab_Parameters.aspx?ruleid={%EditedObject.ID%}", "SetHelpTopic('helpTopic', 'macros_macrorule_parameters');")]

public partial class CMSModules_Workflows_Pages_MacroRule_Header : SiteManagerPage
{
}
