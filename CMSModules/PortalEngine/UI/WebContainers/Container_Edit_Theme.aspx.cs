using System;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebContainers_Container_Edit_Theme : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the filesystem browser
        int containerId = QueryHelper.GetInteger("containerid", 0);
        if (containerId > 0)
        {
            WebPartContainerInfo ci = WebPartContainerInfoProvider.GetWebPartContainerInfo(containerId);
            EditedObject = ci;

            if (ci != null)
            {
                // Ensure the theme folder
                themeElem.Path = ci.GetThemePath();
            }
        }
        else
        {
            EditedObject = null;
        }
    }
}