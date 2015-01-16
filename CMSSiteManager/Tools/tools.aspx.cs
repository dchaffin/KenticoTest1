using System;
using System.Collections.Generic;

using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Title("CMSModules/CMS_Tools/module.png", "Header.Tools", "")]
public partial class CMSSiteManager_Tools_tools : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        uiGuide.OnGuideItemCreated += uiGuide_OnGuideItemCreated;
    }


    protected List<string> uiGuide_OnGuideItemCreated(UIElementInfo uiElement, List<string> defaultItem)
    {
        switch (uiElement.ElementName.ToLowerCSafe())
        {
            case "tools.ecommerce":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Ecommerce, ModuleEntry.ECOMMERCE))
                {
                    return null;
                }
                break;

            case "tools.onlinemarketing":
            case "onlinemarketing.contactmanagement":
                if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.ContactManagement, ModuleEntry.ONLINEMARKETING))
                {
                    return null;
                }
                break;

            case "onlinemarketing.automationprocess":
                if (!WorkflowInfoProvider.IsMarketingAutomationAllowed())
                {
                    return null;
                }
                break;
        }

        return defaultItem;
    }
}