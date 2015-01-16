using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_layout_frameset : CMSWebPartPropertiesPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Check permissions for web part properties UI
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "WebPartProperties.Layout"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "WebPartProperties.Layout");
        }

        frameMenu.Attributes.Add("src", "webpartproperties_layout_menu.aspx" + URLHelper.Url.Query);
        frameButtons.Attributes.Add("src", "webpartproperties_buttons.aspx" + URLHelper.Url.Query);
        // Content frame is loaded by menu page
    }
}
