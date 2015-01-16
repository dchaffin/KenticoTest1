using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.CMSHelper;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_CSS : SiteManagerPage
{
    protected override void CreateChildControls()
    {
        // Get 'webpartid' from query string.
        int webPartId = QueryHelper.GetInteger("webpartid", 0);
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webPartId);
        EditedObject = wpi;
        if (wpi != null)
        {
            ucHierarchy.PreviewObjectName = wpi.WebPartName + "_css";
            ucHierarchy.PreviewURLSuffix = "&previewobjectidentifier=" + wpi.WebPartName;

            CMSContext.EditedObject = wpi;
        }

        base.CreateChildControls();
    }
}