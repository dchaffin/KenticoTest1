using System;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Theme : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the filesystem browser
        int webPartId = QueryHelper.GetInteger("webpartid", 0);
        if (webPartId > 0)
        {
            WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webPartId);
            EditedObject = wpi;

            if (wpi != null)
            {
                // Ensure the theme folder
                themeElem.Path = wpi.GetThemePath();
            }
        }
        else
        {
            EditedObject = null;
        }
    }
}