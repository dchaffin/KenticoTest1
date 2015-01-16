using System;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSDesk_Administration_Default : CMSAdministrationPage
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ScriptHelper.RegisterTitleScript(this, GetString("cmsdesk.ui.administration"));
    }
}