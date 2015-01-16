using System;
using System.Web.UI.WebControls;

using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSSiteManager_Tools_leftmenu : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    protected TreeNode treeElem_OnNodeCreated(UIElementInfo uiElement, TreeNode defaultNode)
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

        return defaultNode;
    }
}