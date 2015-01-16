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

[Tabs("CMS.Ecommerce", "ProductOptions", "OptionCategoryEdit")]
[Title("Objects/Ecommerce_OptionCategory/object.png", "com.optioncategory.edit", "edit_option_category___general")]
public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Edit_Header : CMSProductOptionCategoriesPage
{
    protected int categoryId = 0;
    protected int editedSiteId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        bool hideBreadcrumbs = QueryHelper.GetBoolean("hidebreadcrumbs", false);
        // Get option category ID from querystring
        categoryId = QueryHelper.GetInteger("categoryID", 0);

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;

        // Get localized option category name
        string categName = "";
        OptionCategoryInfo categObj = OptionCategoryInfoProvider.GetOptionCategoryInfo(categoryId);
        if (categObj != null)
        {
            categName = ResHelper.LocalizeString(categObj.CategoryDisplayName);
            editedSiteId = categObj.CategorySiteID;

            // Check if edited object belongs to configured site
            CheckEditedObjectSiteID(editedSiteId);
        }

        if (!hideBreadcrumbs)
        {
            // Initializes page title control
            string[,] breadcrumbs = new string[2, 3];
            breadcrumbs[0, 0] = GetString("optioncategory_edit.itemlistlink");
            breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/ProductOptions/OptionCategory_List.aspx?siteId=" + SiteID;
            breadcrumbs[0, 2] = "ecommerceContent";
            breadcrumbs[1, 0] = FormatBreadcrumbObjectName(categName, editedSiteId);
            breadcrumbs[1, 1] = "";
            breadcrumbs[1, 2] = "";
            CurrentMaster.Title.Breadcrumbs = breadcrumbs;
        }

        master.Tabs.OnTabCreated += Tabs_OnTabCreated;
        master.Tabs.OpenTabContentAfterLoad = false;
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "siteId", SiteID.ToString());
        }

        return tab;
    }
}