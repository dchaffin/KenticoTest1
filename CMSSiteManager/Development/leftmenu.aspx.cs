using System;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSSiteManager_Development_leftmenu : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Handle 'System development' section displaying
        if (SettingsKeyProvider.DevelopmentMode)
        {
            if (CultureHelper.IsUICultureRTL())
            {
                treeElemDev.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree", false, false);
            }
            else
            {
                treeElemDev.LineImagesFolder = GetImageUrl("Design/Controls/Tree", false, false);
            }

            String imagesUrl = GetImageUrl("CMSModules/CMS_SystemDevelopment/", false, true);

            TreeNode rootNode = new TreeNode();
            rootNode.Text = "<span class=\"ContentTreeItem\"\"><img class=\"TreeItemImage\" src=\"" + imagesUrl + "sysdev.png" + "\" alt=\"\" /><span class=\"Name\">" + GetString("Development.SysDev") + "</span></span>";
            rootNode.Expanded = true;
            rootNode.NavigateUrl = "#";
            treeElemDev.Nodes.Add(rootNode);

            TreeNode newSubNode = new TreeNode();
            newSubNode.Text = "<span class=\"ContentTreeItem\" name=\"treeNode\"><img class=\"TreeItemImage\" src=\"" + imagesUrl + "settings.png" + "\" alt=\"\" /><span class=\"Name\">" + GetString("Development.SysDev.Settings") + "</span></span>";
            newSubNode.NavigateUrl = "~/CMSModules/Settings/Development/CustomSettings/Default.aspx?treeroot=settings";
            newSubNode.Target = "frameMain";
            rootNode.ChildNodes.Add(newSubNode);

            newSubNode = new TreeNode();
            newSubNode.Text = "<span class=\"ContentTreeItem\" name=\"treeNode\"><img class=\"TreeItemImage\" src=\"" + imagesUrl + "classes.png" + "\" alt=\"\" /><span class=\"Name\">" + GetString("Development.SysDev.Classes") + "</span></span>";
            newSubNode.NavigateUrl = "~/CMSModules/SystemDevelopment/Development/Classes/Class_List.aspx";
            newSubNode.Target = "frameMain";
            rootNode.ChildNodes.Add(newSubNode);

            newSubNode = new TreeNode();
            newSubNode.Text = "<span class=\"ContentTreeItem\" name=\"treeNode\"><img class=\"TreeItemImage\" src=\"" + imagesUrl + "resources.png" + "\" alt=\"\" /><span class=\"Name\">" + GetString("Development.SysDev.Resources") + "</span></span>";
            newSubNode.NavigateUrl = "~/CMSModules/SystemDevelopment/Development/Resources/UICulture_StringsDefault_List.aspx";
            newSubNode.Target = "frameMain";
            rootNode.ChildNodes.Add(newSubNode);
        }

        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    protected TreeNode treeElem_OnNodeCreated(UIElementInfo uiElement, TreeNode defaultNode)
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

        return defaultNode;
    }
}