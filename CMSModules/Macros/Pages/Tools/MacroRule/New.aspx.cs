using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "macros.macrorule.list", "~/CMSModules/Macros/Pages/Tools/MacroRule/List.aspx?module={?module?}&displaytitle={?displaytitle?}", null)]
[Breadcrumb(1, "macros.macrorule.new")]

public partial class CMSModules_Macros_Pages_Tools_MacroRule_New : CMSToolsPage
{
    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        this.editElem.ResourceName = QueryHelper.GetString("module", "");

        if (QueryHelper.GetBoolean("displaytitle", true))
        {
            CurrentMaster.Title.TitleText = GetString("macros.macrorule.new");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_MacroRule/object.png");
        }

        CurrentMaster.Title.HelpTopicName = "macros_macrorule_general";
    }

    #endregion
}
