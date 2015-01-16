using System;
using System.Collections.Generic;

using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("CMSModules/CMS_Administration/module.png", "Header.Administration", "")]
public partial class CMSSiteManager_Administration_administration : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        uiGuide.OnGuideItemCreated += uiGuide_OnGuideItemCreated;
    }


    protected List<string> uiGuide_OnGuideItemCreated(UIElementInfo uiElement, List<string> defaultItem)
    {
        switch (uiElement.ElementName.ToLowerCSafe())
        {
            case "administration.integrationbus":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.IntegrationBus))
                {
                    return null;
                }
                break;

            case "administration.webfarm":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Webfarm))
                {
                    return null;
                }
                break;
        }

        return defaultItem;
    }
}