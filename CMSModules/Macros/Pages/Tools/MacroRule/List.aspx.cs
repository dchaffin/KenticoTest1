using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;

// Actions
[Actions(1)]
[Action(0, "Objects/CMS_MacroRule/add.png", "macros.macrorule.new", "New.aspx?module={?module?}&displaytitle={?displaytitle?}")]

public partial class CMSModules_Macros_Pages_Tools_MacroRule_List : CMSToolsPage
{
    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        this.listElem.ResourceName = QueryHelper.GetString("module", "");

        if (QueryHelper.GetBoolean("displaytitle", true))
        {
            CurrentMaster.Title.TitleText = GetString("macros.macrorule.list");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_MacroRule/object.png");
            CurrentMaster.Title.HelpTopicName = "macros_macrorule_list";
        }
    }

    #endregion
}
