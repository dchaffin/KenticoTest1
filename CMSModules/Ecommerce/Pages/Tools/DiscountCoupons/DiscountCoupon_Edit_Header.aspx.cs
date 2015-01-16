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

[Tabs("CMS.Ecommerce", "DiscountCoupons", "DiscountContent")]
[Title("Objects/Ecommerce_DiscountCoupon/object.png", "com.discountcoupon.edit", "new_coupongeneral_tab")]
public partial class CMSModules_Ecommerce_Pages_Tools_DiscountCoupons_DiscountCoupon_Edit_Header : CMSDiscountCouponsPage
{
    protected int mDiscountId = 0;
    protected int editedSiteId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        string currentDiscountCoupon = "";
        mDiscountId = QueryHelper.GetInteger("discountid", 0);
        if (mDiscountId > 0)
        {
            DiscountCouponInfo discountCouponObj = DiscountCouponInfoProvider.GetDiscountCouponInfo(mDiscountId);
            if (discountCouponObj != null)
            {
                editedSiteId = discountCouponObj.DiscountCouponSiteID;

                // Check if edited object belongs to configured site
                CheckEditedObjectSiteID(editedSiteId);

                currentDiscountCoupon = ResHelper.LocalizeString(discountCouponObj.DiscountCouponDisplayName);
            }
        }

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;

        // Initializes page title breadcrumbs control		
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("DiscounCoupon_Edit.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/DiscountCoupons/DiscountCoupon_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "ecommerceContent";
        breadcrumbs[1, 0] = FormatBreadcrumbObjectName(currentDiscountCoupon, editedSiteId);
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;

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