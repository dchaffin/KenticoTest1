using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls.Configuration;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Modules_Pages_Development_Module_UI_Header : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query parameters
        int moduleId = QueryHelper.GetInteger("moduleid", 0);
        int elementId = QueryHelper.GetInteger("elementid", 0);
        int parentId = QueryHelper.GetInteger("parentid", 0);

        string currentElement = "";
        bool hideRolesTab = false;

        // Get edited element
        UIElementInfo elemInfo = UIElementInfoProvider.GetUIElementInfo(elementId);
        if (elemInfo != null)
        {
            // Get current element display name
            currentElement = elemInfo.ElementDisplayName;

            // Check if editing group element
            if (elemInfo.ElementName.ToLowerCSafe().StartsWithCSafe("group."))
            {
                // Get parent module
                ResourceInfo module = elemInfo.Parent as ResourceInfo;

                // Check if editing element under wysiwyg module
                hideRolesTab = (module != null) && (module.ResourceName.EqualsCSafe("cms.wysiwygeditor", true));
            }
        }

        // Setup breadcrumbs
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("resource.ui.element");
        pageTitleTabs[0, 1] = "~/CMSModules/Modules/Pages/Development/Module_UI_Frameset.aspx?moduleid=" + moduleId;
        pageTitleTabs[0, 2] = "content";
        pageTitleTabs[1, 0] = ResHelper.LocalizeString(currentElement);
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;

        // Setup Tabs if no creating and no parent element
        if (parentId > 0)
        {
            UITabs tabs = CurrentMaster.Tabs;
            tabs.UrlTarget = "editcontent";

            // Create General tab
            tabs.TabItems.Add(new TabItem()
            {
                Text = GetString("general.general"),
                OnClientClick = "SaveTab(0);SetHelpTopic('helpTopic', 'resource_ui_general');",
                RedirectUrl = "Module_UI_General.aspx?moduleID=" + moduleId + "&elementId=" + elementId + "&parentId=" + parentId
            });

            CurrentMaster.Title.HelpTopicName = "resource_ui_general";
            CurrentMaster.Title.HelpName = "helpTopic";

            if (!hideRolesTab)
            {
                // Create roles tab
                tabs.TabItems.Add(new TabItem()
                {
                    Text = GetString("general.roles"),
                    OnClientClick = "SaveTab(1);SetHelpTopic('helpTopic', 'resource_ui_roles');",
                    RedirectUrl = "Module_UI_Roles.aspx?moduleID=" + moduleId + "&elementId=" + elementId + "&parentId=" + parentId
                });

                int selectedTab = QueryHelper.GetInteger("tabindex", 0);
                tabs.SelectedTab = selectedTab;
                CurrentMaster.Title.HelpTopicName = (selectedTab == 0) ? "resource_ui_general" : "resource_ui_roles";
            }

            CurrentMaster.HeadElements.Visible = true;
            CurrentMaster.HeadElements.Text = ScriptHelper.GetScript("function SaveTab(tabIndex){\n if ((window.parent != null) && (window.parent.parent != null) && (window.parent.parent.frames['tree'])){\n window.parent.parent.frames['tree'].setTab(tabIndex);\n}}");
        }
    }
}