using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_properties : CMSWebPartPropertiesPage
{
    #region "Methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CurrentMaster.BodyClass += " WebpartProperties";
    }


    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check permissions for web part properties UI
        if (CMSContext.ViewMode == CMS.PortalEngine.ViewModeEnum.Wireframe)
        {
            // Check wireframes permissions
        }
        else
        {
            // Check design mode permissions
            CurrentUserInfo currentUser = CMSContext.CurrentUser;
            if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "WebPartProperties.General"))
            {
                RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "WebPartProperties.General");
            }
        }

        // Initialize the control
        webPartProperties.AliasPath = aliasPath;
        webPartProperties.CultureCode = cultureCode;
        webPartProperties.WebpartId = webpartId;
        webPartProperties.ZoneId = zoneId;
        webPartProperties.InstanceGUID = instanceGuid;
        webPartProperties.PageTemplateId = templateId;
        webPartProperties.IsNewWebPart = isNew;

        webPartProperties.Position = position;
        webPartProperties.PositionLeft = positionLeft;
        webPartProperties.PositionTop = positionTop;

        webPartProperties.IsNewVariant = isNewVariant;
        webPartProperties.VariantID = variantId;
        webPartProperties.ZoneVariantID = zoneVariantId;
        webPartProperties.VariantMode = variantMode;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register the OnSave event handler
        OnSave += OnSaveEventHandler;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register progress script
        ScriptHelper.RegisterProgress(this.Page);
    }

    /// <summary>
    /// Raised when the Save action is required.
    /// </summary>
    protected bool OnSaveEventHandler(object sender, EventArgs e)
    {
        return webPartProperties.OnSave();
    }

    #endregion
}