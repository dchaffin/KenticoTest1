using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.SCORERULE, "ruleid")]
[Actions(1)]
[Action(0, "Objects/OM_Score/rule_add.png", "om.score.newrule", "Tab_Rules_Edit.aspx")]
public partial class CMSModules_Scoring_Pages_Tab_Rules : CMSRulePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Scoring", null, "menu");
        if (CurrentMaster.HeaderActions.ActionsList.Count > 0)
        {
            CurrentMaster.HeaderActions.ActionsList[0].RedirectUrl += URLHelper.Url.Query;
        }
    }
}