using System;

using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.CMSHelper;
using CMS.PortalEngine;
using CMS.GlobalHelper;

public partial class CMSPages_PortalTemplate : PortalPage
{
    #region "Properties"

    /// <summary>
    /// Document manager
    /// </summary>
    public override ICMSDocumentManager DocumentManager
    {
        get
        {
            // Enable document manager
            docMan.Visible = true;
            docMan.StopProcessing = false;
            docMan.RegisterSaveChangesScript = (CMSContext.ViewMode == ViewModeEnum.Edit);
            return docMan;
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Init the header tags
        tags.Text = HeaderTags;

        if (CMSContext.ViewMode == ViewModeEnum.Wireframe)
        {
            CSSHelper.RegisterWireframesMode(this);
        }
    }
}