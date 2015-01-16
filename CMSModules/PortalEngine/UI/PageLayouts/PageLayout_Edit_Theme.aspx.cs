using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Edit_Theme : CMSModalPage
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

        // Setup the filesystem browser
        int layoutId = QueryHelper.GetInteger("layoutid", 0);
        if (layoutId > 0)
        {
            LayoutInfo li = LayoutInfoProvider.GetLayoutInfo(layoutId);
            EditedObject = li;

            if (li != null)
            {
                // Ensure the theme folder
                themeElem.Path = li.GetThemePath();
            }
        }
        else
        {
            EditedObject = null;
        }
    }
}