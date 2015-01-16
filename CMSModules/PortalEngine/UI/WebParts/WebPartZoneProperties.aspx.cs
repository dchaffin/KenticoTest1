using System;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

using CMS.Controls.Configuration;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartZoneProperties : CMSModalDesignPage
{
    #region "Variables"

    private Control currentControl = null;
    private bool isVariantTab = false;
    protected Guid instanceGuid = QueryHelper.GetGuid("instanceguid", Guid.Empty);
    protected int templateId = QueryHelper.GetInteger("templateid", 0);
    protected int variantId = QueryHelper.GetInteger("variantid", 0);
    protected int zoneVariantId = QueryHelper.GetInteger("zonevariantid", 0);
    protected VariantModeEnum variantMode = VariantModeFunctions.GetVariantModeEnum(QueryHelper.GetString("variantmode", string.Empty));

    #endregion


    #region "Page methods"

    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // When displaying an existing variant of a web part, get the variant mode for its original web part
        if ((variantId > 0) || (zoneVariantId > 0))
        {
            PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
            if ((pti != null) && ((pti.TemplateInstance != null)))
            {
                // Get the original webpart and retrieve its variant mode
                WebPartInstance webpartInstance = pti.TemplateInstance.GetWebPart(instanceGuid, zoneVariantId, 0);
                if (webpartInstance != null)
                {
                    variantMode = webpartInstance.VariantMode;
                }
            }
        }
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI elements for web part zone
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Design", "Design.WebPartZoneProperties" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Design;Design.WebPartZoneProperties");
        }

        // Show the tabs when displaying a zone variant
        if (variantId > 0)
        {
            tabsElem.JavaScriptHandler = "TabSelect";
            tabsElem.OnTabCreated += tabElem_OnTabCreated;
            pnlTabsContainer.Visible = true;
        }

        string postBackReference = ControlsHelper.GetPostBackEventReference(pnlUpdate, "");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TabSelect", ScriptHelper.GetScript("function TabSelect(name){currSelElem = document.getElementById('" + hdnSelectedTab.ClientID + "'); if ((currSelElem != null)) { origVal = currSelElem.value; currSelElem.value = name; if (origVal != '') {" + postBackReference + " }}}"));

        // Title
        CurrentMaster.Title.TitleText = GetString("webpartzone.propertiesheader");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_PortalEngine/WebpartZoneProperties/title.png");
        CurrentMaster.Title.HelpTopicName = "webpartzoneproperties";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Tabs header css class
        CurrentMaster.PanelHeader.CssClass = "WebpartTabsPageHeader";

        // UI Strings
        btnOk.Text = GetString("general.ok");
        btnApply.Text = GetString("general.apply");
        btnCancel.Text = GetString("general.cancel");
        chkRefresh.Text = GetString("webpartzone.refresh");

        // Default control path 
        string controlPath = "~/CMSModules/PortalEngine/Controls/WebParts/WebPartZoneProperties.ascx";
        string ctrlId = "wpzp";

        // Set personalized control path if selected
        switch (hdnSelectedTab.Value.ToLowerCSafe())
        {
            case "webpartzoneproperties.variant":
                controlPath = "~/CMSModules/OnlineMarketing/Controls/WebParts/WebPartZonePersonalized.ascx";
                ctrlId = "pers";
                isVariantTab = true;
                break;
        }

        // Load selected control
        currentControl = LoadUserControl(controlPath);
        currentControl.ID = ctrlId;
        // Add to control collection
        plcDynamicContent.Controls.Add(currentControl);
    }


    /// <summary>
    /// Saves the webpart zone properties and closes the window.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        string script = string.Empty;
        bool saved = false;

        // Save webpart properties
        if (currentControl != null)
        {
            if (!isVariantTab)
            {
                CMSModules_PortalEngine_Controls_WebParts_WebPartZoneProperties webPartZonePropertiesElem = currentControl as CMSModules_PortalEngine_Controls_WebParts_WebPartZoneProperties;
                if ((webPartZonePropertiesElem != null) && webPartZonePropertiesElem.Save())
                {
                    saved = true;
                    if (chkRefresh.Checked)
                    {
                        if ((variantId == 0) && (webPartZonePropertiesElem.WebPartZoneInstance != null))
                        {
                            // Display the new variant by default
                            script = "UpdateVariantPosition('Variant_Zone_" + webPartZonePropertiesElem.WebPartZoneInstance.ZoneID + "', -1); ";
                        }

                        script += "RefreshPage();";
                    }
                }
            }
            else
            {
                IWebPartZoneProperties webPartZoneVariantElem = currentControl as IWebPartZoneProperties;
                if (webPartZoneVariantElem != null)
                {
                    saved = webPartZoneVariantElem.Save();
                    tabsElem.SelectedTab = 1;
                }
            }
        }

        // Close the window
        if (saved)
        {
            script += "CloseDialog();";
            ScriptHelper.RegisterStartupScript(this, typeof(string), "closeZoneProperties", script, true);
        }
    }


    /// <summary>
    /// Saves the webpart zone properties.
    /// </summary>
    protected void btnApply_Click(object sender, EventArgs e)
    {
        // Save webpart properties
        if (currentControl != null)
        {
            if (!isVariantTab)
            {
                CMSModules_PortalEngine_Controls_WebParts_WebPartZoneProperties webPartZonePropertiesElem = currentControl as CMSModules_PortalEngine_Controls_WebParts_WebPartZoneProperties;
                if (webPartZonePropertiesElem != null)
                {
                    webPartZonePropertiesElem.Save();
                    tabsElem.SelectedTab = 0;
                }
            }
            else
            {
                IWebPartZoneProperties webPartZoneVariantElem = currentControl as IWebPartZoneProperties;
                if (webPartZoneVariantElem != null)
                {
                    webPartZoneVariantElem.Save();
                    tabsElem.SelectedTab = 1;
                }
            }
        }
    }


    /// <summary>
    /// Tabs the elem_ on tab created.
    /// </summary>
    protected TabItem tabElem_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        if (!String.IsNullOrEmpty(tabsElem.JavaScriptHandler))
        {
            tab.OnClientClick = "if (" + tabsElem.JavaScriptHandler + ") { " + tabsElem.JavaScriptHandler + "(" + ScriptHelper.GetString(element.ElementName) + "); } ";
        }

        switch (element.ElementName.ToLowerCSafe())
        {
            case "webpartzoneproperties.variant":
                String helpText = (variantMode == VariantModeEnum.MVT) ? "mvtvariant_edit" : "cpvariant_edit";
                tab.OnClientClick += "if (window.SetHelpTopic) { window.SetHelpTopic('helpTopic', '" + helpText + "')}; ";
                bool isNewVariant = QueryHelper.GetBoolean("isnewvariant", false);
                int variantId = QueryHelper.GetInteger("variantid", 0);

                if ((variantId <= 0) || isNewVariant)
                {
                    return null;
                }
                break;

            case "webpartzoneproperties.general":
                tab.OnClientClick += "if (window.SetHelpTopic) { window.SetHelpTopic('helpTopic', 'webpartzoneproperties')}; ";
                break;
        }

        return tab;
    }

    #endregion
}