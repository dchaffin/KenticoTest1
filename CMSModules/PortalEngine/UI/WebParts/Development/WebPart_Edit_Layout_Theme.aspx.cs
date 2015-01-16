using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Layout_Theme : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = QueryHelper.GetBoolean("editonlycode", false);

        if (RequiresDialog)
        {
            QueryHelper.ValidateHash("hash");
        }
        else
        {
            CheckAccessToSiteManager();
        }

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Setup the file system browser
        int layoutId = QueryHelper.GetInteger("layoutid", 0);
        int webPartId = QueryHelper.GetInteger("webpartid", 0);

        if ((webPartId > 0) && (layoutId > 0))
        {
            WebPartLayoutInfo wpli = WebPartLayoutInfoProvider.GetWebPartLayoutInfo(layoutId);
            EditedObject = wpli;

            if (wpli != null)
            {
                // Ensure the theme folder
                themeElem.Path = wpli.GetThemePath();
            }
        }
        else
        {
            EditedObject = null;
        }
    }
}
