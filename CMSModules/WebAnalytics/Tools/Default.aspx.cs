using System;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_WebAnalytics_Tools_Default : CMSWebAnalyticsPage
{
    protected override void OnPreRender(EventArgs e)
    {
        analyticsTree.Attributes["src"] = "Analytics_Statistics.aspx" + URLHelper.Url.Query;

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        ScriptHelper.RegisterTitleScript(this, GetString("tools.ui.webanalytics"));

        base.OnPreRender(e);
    }
}