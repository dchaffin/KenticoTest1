using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls.Configuration;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

// Set page title
[Title("CMSModules/CMS_MyDesk/RecycleBin/module.png", "MyDesk.RecycleBinTitle", "CMS_MyDesk_MyRecycleBin_Documents")]
// Set page tabs
[Tabs("CMS.MyDesk", "MyRecycleBin", "recbin_content")]
public partial class CMSModules_MyDesk_RecycleBin_RecycleBin_Header : CMSDeskPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        CurrentMaster.Tabs.OnTabCreated += Tabs_OnTabCreated;
    }


    protected override void OnPreRender(EventArgs e)
    {
        CurrentMaster.Tabs.SelectedTab = QueryHelper.GetInteger("selectedtab", 0);

        base.OnPreRender(e);
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Skip objects tab element if not have proper license
        if ((element.ElementName.ToLowerCSafe() == "myrecyclebin.objects") && !LicenseKeyInfoProvider.IsFeatureAvailable(FeatureEnum.ObjectVersioning))
        {
            return null;
        }
        return tab;
    }
}