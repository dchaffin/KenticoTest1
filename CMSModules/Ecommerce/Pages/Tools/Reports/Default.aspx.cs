using System;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_Tools_Reports_Default : CMSEcommerceReportsPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        frmMenu.Attributes.Add("src", "Tree.aspx" + URLHelper.Url.Query);

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }
    }
}
