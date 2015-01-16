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

[Tabs("CMS.Ecommerce", "DiscountLevels", "DiscountLevelContent")]
[Title("Objects/Ecommerce_DiscountLevel/object.png", "com.discountlevel.edit", "new_levelgeneral_tab")]
public partial class CMSModules_Ecommerce_Pages_Tools_DiscountLevels_DiscountLevel_Edit_Header : CMSDiscountLevelsPage
{
    protected int mDiscountLevelId = 0;
    protected int editedSiteId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        string currentDiscount = "";

        // Get discountlLevel id from querystring		
        mDiscountLevelId = QueryHelper.GetInteger("discountLevelId", 0);
        if (mDiscountLevelId > 0)
        {
            DiscountLevelInfo di = DiscountLevelInfoProvider.GetDiscountLevelInfo(mDiscountLevelId);

            if (di != null)
            {
                editedSiteId = di.DiscountLevelSiteID;
                // Check if edited object belongs to configured site
                CheckEditedObjectSiteID(editedSiteId);

                currentDiscount = ResHelper.LocalizeString(di.DiscountLevelDisplayName);
            }
        }

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;

        int hideBreadcrumbs = QueryHelper.GetInteger("hidebreadcrumbs", 0);
        if (hideBreadcrumbs == 0)
        {
            // Initializes page title breadcrumbs control		
            string[,] breadcrumbs = new string[2, 3];
            breadcrumbs[0, 0] = GetString("DiscountLevel_Edit.ItemListLink");
            breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/DiscountLevels/DiscountLevel_List.aspx?siteId=" + SiteID;
            breadcrumbs[0, 2] = "ecommerceContent";
            breadcrumbs[1, 0] = FormatBreadcrumbObjectName(currentDiscount, editedSiteId);
            breadcrumbs[1, 1] = "";
            breadcrumbs[1, 2] = "";
            master.Title.Breadcrumbs = breadcrumbs;
        }

        // Tabs
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