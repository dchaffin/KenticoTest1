using System;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_Properties_Default : CMSPropertiesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (CMSContext.ViewMode != ViewModeEnum.EditLive)
        {
            CMSContext.ViewMode = ViewModeEnum.Properties;
        }

        frameHeader.Attributes.Add("src", "header.aspx" + URLHelper.Url.Query);

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        EnsureSplitModeCrossoverScript();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ScriptHelper.RegisterTitleScript(this, GetString("content.ui.properties"));
    }

    #endregion
}