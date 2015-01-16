using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_CssStylesheets_Pages_CssStylesheet_Edit : CMSModalPage
{
    protected int mHeight = 96;


    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.GetBoolean("editonlycode", false))
        {
            mHeight = 67;
        }
        else
        {
            // Page opened from Site Manager
            CheckAccessToSiteManager();
        }
    }
}