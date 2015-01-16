using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.PortalEngine;

using CMS.Controls.Configuration;

public partial class CMSModules_Content_CMSDesk_Properties_Header : CMSPropertiesPage
{
    #region "Variables"

    private string selected = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        selected = DataHelper.GetNotEmpty(QueryHelper.GetString("tab", String.Empty).ToLowerCSafe(), PropertyTabCode.ToString(UIContext.PropertyTab).ToLowerCSafe());

        CurrentMaster.Tabs.OnTabCreated += tabElem_OnTabCreated;
        CurrentMaster.Tabs.ModuleName = "CMS.Content";
        CurrentMaster.Tabs.ElementName = "Properties";
        CurrentMaster.Tabs.UrlTarget = "propedit";
        CurrentMaster.SetRTL();
    }


    protected TabItem tabElem_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        bool splitViewSupported = true;
        string elementName = element.ElementName.ToLowerCSafe();
        switch (elementName)
        {
            case "properties.languages":
                splitViewSupported = false;
                if (!CultureInfoProvider.IsSiteMultilignual(CMSContext.CurrentSiteName) || !CultureInfoProvider.LicenseVersionCheck())
                {
                    return null;
                }
                break;

            case "properties.security":
            case "properties.relateddocs":
            case "properties.linkeddocs":
                splitViewSupported = false;
                break;

            case "properties.wireframe":
                {
                    // Hide wireframe tab if wireframe is not present
                    if ((DocumentManager.Node == null) || (DocumentManager.Node.NodeWireframeTemplateID <= 0))
                    {
                        return null;
                    }

                    splitViewSupported = false;
                }
                break;

            case "properties.variants":

                if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
                {
                    // Check license and whether content personalization is enabled and whether exists at least one variant for current template
                    if ((DocumentManager.Node == null) 
                        || !LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.ContentPersonalization, ModuleEntry.ONLINEMARKETING)
                        || !ResourceSiteInfoProvider.IsResourceOnSite("CMS.ContentPersonalization", CMSContext.CurrentSiteName)
                        || !PortalContext.ContentPersonalizationEnabled
                        || (ModuleCommands.OnlineMarketingGetContentPersonalizationVariantId(DocumentManager.Node.GetUsedPageTemplateId(), String.Empty) <= 0))
                    {
                        return null;
                    }
                }


                break;

            case "properties.workflow":
            case "properties.versions":
                if (DocumentManager.Workflow == null)
                {
                    return null;
                }
                break;
        }

        // Ensure tab pre-selection
        if (elementName.StartsWithCSafe("properties.") && (elementName.Substring("properties.".Length) == selected))
        {
            CurrentMaster.Tabs.SelectedTab = tabIndex;
        }

        // Ensure split view mode
        if (splitViewSupported && CMSContext.DisplaySplitMode)
        {
            tab.RedirectUrl = GetSplitViewUrl(tab.RedirectUrl);
        }

        return tab;
    }
}