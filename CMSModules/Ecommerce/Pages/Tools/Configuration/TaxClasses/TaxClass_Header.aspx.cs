using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls.Configuration;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[Tabs("CMS.Ecommerce", "Configuration.TaxClasses", "taxClassContent")]
[Title("Objects/Ecommerce_TaxClass/object.png", "TaxClass_Edit.HeaderCaption-Properties", "new_classgeneral_tab")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_Header : CMSTaxClassesPage
{
    protected int taxClassId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        taxClassId = QueryHelper.GetInteger("taxclassid", 0);
        bool hideBreadcrumbs = QueryHelper.GetBoolean("hidebreadcrumbs", false);

        string taxClassName = "";
        TaxClassInfo tc = TaxClassInfoProvider.GetTaxClassInfo(taxClassId);
        if (tc != null)
        {
            CheckEditedObjectSiteID(tc.TaxClassSiteID);
            taxClassName = ResHelper.LocalizeString(tc.TaxClassDisplayName);
        }

        // initializes page title
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("TaxClass_Edit.ItemListLink");
        pageTitleTabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/TaxClasses/TaxClass_List.aspx?siteId=" + SiteID;
        pageTitleTabs[0, 2] = "_parent";
        pageTitleTabs[1, 0] = taxClassName;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;

        if (!hideBreadcrumbs)
        {
            master.Title.Breadcrumbs = pageTitleTabs;
        }

        master.Tabs.OnTabCreated += Tabs_OnTabCreated;
    }


    private TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "siteId", SiteID.ToString());
        }

        return tab;
    }
}