using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.DocumentEngine;

public partial class CMSModules_PortalEngine_UI_WebParts_webpartproperties_layout : CMSWebPartPropertiesPage
{
    String layoutCodeName = String.Empty;
    WebPartLayoutInfo wpli = null;

    protected override void OnPreInit(EventArgs e)
    {
        WebPartInstance webPart = null;

        // Check permissions for web part properties UI
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "WebPartProperties.Layout"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "WebPartProperties.Layout");
        }

        if (webpartId != "")
        {
            // Get pageinfo
            PageInfo pi = GetPageInfo(aliasPath, templateId, cultureCode);
            if (pi == null)
            {
                return;
            }

            // Get page template
            PageTemplateInfo pti = pi.UsedPageTemplateInfo;
            if ((pti != null) && ((pti.TemplateInstance != null)))
            {
                webPart = pti.TemplateInstance.GetWebPart(instanceGuid, zoneVariantId, variantId) ?? pti.GetWebPart(webpartId);
            }

        }

        if (webPart != null)
        {
            WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webPart.WebPartType);
            layoutCodeName = QueryHelper.GetString("layoutcodename", String.Empty);

            if (String.IsNullOrEmpty(layoutCodeName))
            {
                // Get the current layout name
                layoutCodeName = ValidationHelper.GetString(webPart.GetValue("WebPartLayout"), "");
            }

            if (wpi != null)
            {
                wpli = WebPartLayoutInfoProvider.GetWebPartLayoutInfo(wpi.WebPartName, layoutCodeName);
                if (wpli != null)
                {
                    if ((layoutCodeName != "|default|") && (layoutCodeName != "|new|"))
                    {
                        SetEditedObject(wpli, "WebPartProperties_layout_frameset_frameset.aspx");
                    }
                }
            }
        }

        base.OnPreInit(e);
    }


    protected override void CreateChildControls()
    {
        if (wpli != null)
        {
            CMSContext.EditedObject = wpli;
        }

        ucHierarchy.PreviewObjectName = layoutCodeName;
        ucHierarchy.PreviewURLSuffix = "&previewguid=" + instanceGuid;
        ucHierarchy.IgnoreSessionValues = true;

        base.CreateChildControls();
    }
}