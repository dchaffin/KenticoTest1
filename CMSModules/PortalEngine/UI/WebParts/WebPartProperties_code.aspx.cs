using System;
using System.Xml;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.ExtendedControls;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_code : CMSWebPartPropertiesPage
{
    #region "Variables"

    /// <summary>
    /// Current page info.
    /// </summary>
    private PageInfo pi = null;

    /// <summary>
    /// Page template info.
    /// </summary>
    private PageTemplateInfo pti = null;


    /// <summary>
    /// Current web part.
    /// </summary>
    private WebPartInstance webPart = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnInit
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check permissions for web part properties UI
        CurrentUserInfo currentUser = CMSContext.CurrentUser;

        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "WebPartProperties.Code"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "WebPartProperties.Code");
        }

        if (webpartId != "")
        {
            // Get page info
            pi = GetPageInfo(aliasPath, templateId, cultureCode);
            if (pi == null)
            {
                Visible = false;
                return;
            }

            pti = pi.UsedPageTemplateInfo;
            if ((pti != null) && ((pti.TemplateInstance != null)))
            {
                // Get web part instance
                webPart = pti.TemplateInstance.GetWebPart(instanceGuid, zoneVariantId, variantId) ?? pti.GetWebPart(webpartId);

                if (webPart != null)
                {
                    txtCode.Text = ValidationHelper.GetString(webPart.GetValue("WebPartCode"), "");
                }
                EditedObject = webPart;
            }
        }
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SettingsKeyProvider.UsingVirtualPathProvider)
        {
            lblInfo.Text = GetString("WebPartCode.ProviderNotRunning");
            lblInfo.CssClass = "ErrorLabel";
            txtCode.Enabled = false;
        }
        else
        {
            lblInfo.Text = GetString("WebPartCode.Info");

            // Register the OnSave event handler
            OnSave += OnSaveEventHandler;
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Design", "EditCode"))
        {
            txtCode.ReadOnly = true;
            lblInfo.Text = GetString("EditCode.NotAllowedNoHtml");
        }
    }


    /// <summary>
    /// Raised when the Save action is required.
    /// </summary>
    protected bool OnSaveEventHandler(object sender, EventArgs e)
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Design", "EditCode"))
        {
            return false;
        }

        webPart.SetValue("WebPartCode", txtCode.Text);

        bool isWebPartVariant = (variantId > 0) || (zoneVariantId > 0) || isNewVariant;
        if (!isWebPartVariant)
        {
            // Update page template
            PageTemplateInfoProvider.SetPageTemplateInfo(pti);
        }
        else
        {
            // Save the variant properties
            if ((webPart != null)
                && (webPart.ParentZone != null)
                && (webPart.ParentZone.ParentTemplateInstance != null)
                && (webPart.ParentZone.ParentTemplateInstance.ParentPageTemplate != null))
            {
                XmlDocument doc = new XmlDocument();
                XmlNode xmlWebParts = null;

                if (zoneVariantId > 0)
                {
                    // This webpart is in a zone variant therefore save the whole variant webparts
                    xmlWebParts = webPart.ParentZone.GetXmlNode(doc);
                    if (webPart.VariantMode == VariantModeEnum.MVT)
                    {
                        ModuleCommands.OnlineMarketingSaveMVTVariantWebParts(zoneVariantId, xmlWebParts);
                    }
                    else if (webPart.VariantMode == VariantModeEnum.ContentPersonalization)
                    {
                        ModuleCommands.OnlineMarketingSaveContentPersonalizationVariantWebParts(zoneVariantId, xmlWebParts);
                    }
                }
                else if (variantId > 0)
                {
                    // This webpart is a web part variant
                    xmlWebParts = webPart.GetXmlNode(doc);
                    if (webPart.VariantMode == VariantModeEnum.MVT)
                    {
                        ModuleCommands.OnlineMarketingSaveMVTVariantWebParts(variantId, xmlWebParts);
                    }
                    else if (webPart.VariantMode == VariantModeEnum.ContentPersonalization)
                    {
                        ModuleCommands.OnlineMarketingSaveContentPersonalizationVariantWebParts(variantId, xmlWebParts);
                    }
                }
            }
        }

        string parameters = aliasPath + "/" + zoneId + "/" + webpartId;
        string cacheName = "CMSVirtualWebParts|" + parameters.ToLowerCSafe().TrimStart('/');

        CacheHelper.Remove(cacheName);

        ShowChangesSaved();

        return true;
    }

    #endregion
}