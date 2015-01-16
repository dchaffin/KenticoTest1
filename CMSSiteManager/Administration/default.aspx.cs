using System;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

[RegisterTitle("Header.Administration")]
public partial class CMSSiteManager_Administration_default : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }
    }
}