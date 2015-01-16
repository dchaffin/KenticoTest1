using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSDesk_Tools_Default : CMSToolsPage
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ScriptHelper.RegisterTitleScript(this, GetString("cmsdesk.ui.tools"));
    }
}