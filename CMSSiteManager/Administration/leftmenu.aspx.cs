using System;
using System.Web.UI.WebControls;

using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSSiteManager_Administration_leftmenu : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    protected TreeNode treeElem_OnNodeCreated(UIElementInfo uiElement, TreeNode defaultNode)
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

        return defaultNode;
    }
}