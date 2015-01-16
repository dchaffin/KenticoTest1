using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_Macros_Pages_Tools_MacroRule_Frameset : CMSToolsPage
{
    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.GetBoolean("displaytitle", true))
        {
            frm.FrameHeight = 58;
        }
    }

    #endregion
}