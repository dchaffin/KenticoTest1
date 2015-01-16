using System;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

[RegisterTitle("Header.Development")]
public partial class CMSSiteManager_Development_Default : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }
    }
}