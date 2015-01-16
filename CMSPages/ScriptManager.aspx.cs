using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.PortalControls;
using CMS.UIControls;

public partial class CMSPages_ScriptManager : CMSPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Ensure script manager
        PortalHelper.EnsureScriptManager(Page);
    }
}