using System;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DocumentEngine;

using CMS.Controls.Configuration;
using CMS.CMSHelper;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_header : CMSWebPartPropertiesPage
{
    #region "Variables"

    private bool showCodeTab = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSShowWebPartCodeTab"], false);
    private bool showBindingTab = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSShowWebPartBindingTab"], false);

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize page title
        PageTitle.TitleText = GetString("WebpartProperties.Title");
        PageTitle.TitleImage = GetImageUrl("CMSModules/CMS_PortalEngine/Webpartproperties.png");

        if (!RequestHelper.IsPostBack())
        {
            InitalizeMenu();
        }

        tabsElem.OnTabCreated += tabElem_OnTabCreated;

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.NEWWINDOW_SCRIPT_KEY, ScriptHelper.NewWindowScript);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        tabsElem.DoTabSelection();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        if (webpartId != string.Empty)
        {
            // get pageinfo
            PageInfo pi = GetPageInfo(aliasPath, templateId, cultureCode);
            if (pi == null)
            {
                Visible = false;
                return;
            }

            PageTemplateInfo pti = pi.UsedPageTemplateInfo;
            if (pti != null)
            {
                WebPartInfo wi = null;

                // Get web part
                WebPartInstance webPart = pti.GetWebPart(instanceGuid, webpartId);
                if (webPart != null)
                {
                    wi = WebPartInfoProvider.GetWebPartInfo(webPart.WebPartType);
                    if (ValidationHelper.GetString(webPart.GetValue("WebPartCode"), string.Empty).Trim() != string.Empty)
                    {
                        showCodeTab = true;
                    }
                    if (webPart.Bindings.Count > 0)
                    {
                        showBindingTab = true;
                    }
                }
                else
                {
                    wi = WebPartInfoProvider.GetWebPartInfo(ValidationHelper.GetInteger(webpartId, 0));
                }

                if (wi != null)
                {
                    // Generate documentation link
                    Literal ltr = new Literal();
                    PageTitle.RightPlaceHolder.Controls.Add(ltr);
                    string docScript = "NewWindow('" + ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/WebPartDocumentationPage.aspx") + "?webpartid=" + ScriptHelper.GetString(wi.WebPartName, false) + "', 'WebPartPropertiesDocumentation', 800, 800); return false;";
                    ltr.Text += "<a onclick=\"" + docScript + "\" href=\"#\"><img src=\"" + ResolveUrl(GetImageUrl("General/HelpLargeDark.png")) + "\" style=\"border-width: 0px;\"></a>";
                    PageTitle.TitleText = GetString("WebpartProperties.Title") + " (" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(wi.WebPartDisplayName)) + ")";
                }
            }
        }

        tabsElem.UrlTarget = "webpartpropertiescontent";
    }

    #endregion


    #region "Control events"

    protected TabItem tabElem_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        String script = "SetTabsContext('');";
        String defaultParam = tab.OnClientClick;
        switch (element.ElementName.ToLowerCSafe())
        {

            case "webpartproperties.general":
                if (CMSContext.ViewMode == ViewModeEnum.Wireframe)
                {
                    tab.SkipCheckPermissions = true;
                }
                break;

            case "webpartproperties.code":
                if (!showCodeTab || isNew || isNewVariant)
                {
                    return null;
                }
                break;

            case "webpartproperties.variant":
                script = "SetTabsContext('variants');";
                String varName = (variantMode == VariantModeEnum.MVT) ? "mvtvariant_edit" : "cpvariant_edit";
                defaultParam = "if (window.SetHelpTopic) { window.SetHelpTopic('helpTopic', '" + varName + "')}";
                if ((variantId <= 0) || isNew || isNewVariant)
                {
                    return null;
                }
                break;

            case "webpartzoneproperties.variant":
                if ((zoneVariantId <= 0) || isNew)
                {
                    return null;
                }
                break;

            case "webpartproperties.bindings":
                if (!showBindingTab || isNew || isNewVariant)
                {
                    return null;
                }
                break;

            case "webpartproperties.layout":
                if (isNew || isNewVariant)
                {
                    return null;
                }
                break;
        }

        tab.OnClientClick = script + defaultParam;

        return tab;
    }

    #endregion
}