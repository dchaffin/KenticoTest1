using System;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_Validation_Default : CMSValidationPage
{
    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        DocumentManager.RedirectForNonExistingDocument = false;
        UpdateViewMode(CMSContext.ViewMode);
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        frameHeader.Attributes.Add("src", "header.aspx" + URLHelper.Url.Query);

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }
    }

    #endregion
}