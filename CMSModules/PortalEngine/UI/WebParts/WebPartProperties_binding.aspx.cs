using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_binding : CMSWebPartPropertiesPage
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public CMSModules_PortalEngine_UI_WebParts_WebPartProperties_binding()
    {
        PreInit += CMSDesk_PortalEngine_WebpartProperties_PreInit;
    }


    /// <summary>
    /// PreInit event handler.
    /// </summary>
    private void CMSDesk_PortalEngine_WebpartProperties_PreInit(object sender, EventArgs e)
    {
        // Initialize the control
        webPartBinding.AliasPath = QueryHelper.GetString("aliaspath", string.Empty);
        webPartBinding.CultureCode = QueryHelper.GetString("culture", CMSContext.PreferredCultureCode);
        webPartBinding.WebpartId = QueryHelper.GetString("webpartid", string.Empty);
        webPartBinding.ZoneId = QueryHelper.GetString("zoneid", string.Empty);
        webPartBinding.InstanceGUID = QueryHelper.GetGuid("instanceguid", Guid.Empty);
        webPartBinding.PageTemplateId = QueryHelper.GetInteger("templateid", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions for web part properties UI
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "WebPartProperties.Bindings"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "WebPartProperties.Bindings");
        }
    }
}