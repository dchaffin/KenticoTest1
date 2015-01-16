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

[Tabs("CMS.Ecommerce", "Configuration.ShippingOptions", "shippingOptionContent")]
[Title("Objects/Ecommerce_ShippingOption/object.png", "ShippingOption_EditHeader.TitleText", "newgeneral_tab2")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_Header : CMSShippingOptionsPage
{
    #region "Variables"

    protected int mShippingOptionId = 0;
    protected int editedSiteId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mShippingOptionId = QueryHelper.GetInteger("shippingOptionID", 0);

        string shippingOptionName = "";
        ShippingOptionInfo soi = ShippingOptionInfoProvider.GetShippingOptionInfo(mShippingOptionId);
        if (soi != null)
        {
            editedSiteId = soi.ShippingOptionSiteID;

            // Check site ID
            CheckEditedObjectSiteID(editedSiteId);
            shippingOptionName = ResHelper.LocalizeString(soi.ShippingOptionDisplayName);
        }

        // Initializes page title and breadcrumbs
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("ShippingOption_EditHeader.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingOptions/ShippingOption_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = FormatBreadcrumbObjectName(shippingOptionName, editedSiteId);
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Title.Breadcrumbs = breadcrumbs;
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

    #endregion
}