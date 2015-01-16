using System;
using System.Collections.Generic;

using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("CMSModules/CMS_Development/module.png", "Header.Development", "")]
public partial class CMSSiteManager_Development_development : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        uiGuide.OnGuideItemCreated += uiGuide_OnGuideItemCreated;
    }


    protected List<string> uiGuide_OnGuideItemCreated(UIElementInfo uiElement, List<string> defaultItem)
    {
        switch (uiElement.ElementName.ToLowerCSafe())
        {
            case "development.notifications":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Notifications, ModuleEntry.NOTIFICATIONS))
                {
                    return null;
                }
                break;

            case "development.customtables":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.CustomTables))
                {
                    return null;
                }
                break;

            case "development.translationservices":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.TranslationServices, ModuleEntry.TRANSLATIONSERVICES))
                {
                    return null;
                }
                break;

            case "development.workflows":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.WorkflowVersioning))
                {
                    return null;
                }
                break;
        }

        return defaultItem;
    }
}