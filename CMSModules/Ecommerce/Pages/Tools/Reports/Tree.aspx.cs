using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_Tools_Reports_Tree : CMSEcommerceReportsPage
{
    public override void CheckPermissions()
    {
        CheckEcommercePermissions();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        AddMenuButtonSelectScript("ECReports", "", "ecommerceMenu");
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Call the script for tab which is selected
        if (treeElem.MenuEmpty)
        {
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "FirstTabSelection", ScriptHelper.GetScript(" parent.frames['ecommreports'].location.href = '" + URLHelper.ResolveUrl(UIHelper.GetInformationUrl("uiprofile.uinotavailable")) + "'; "));
        }
    }


    protected TreeNode treeElem_OnNodeCreated(UIElementInfo uiElement, TreeNode defaultNode)
    {
        // Hide UI element SampleDataGenerator if current user is not global administrator
        if (uiElement.ElementName.CompareToCSafe("SampleDataGenerator") == 0)
        {
            if (!CMSContext.CurrentUser.IsGlobalAdministrator)
            {
                return null;
            }
        }

        return defaultNode;
    }
}
