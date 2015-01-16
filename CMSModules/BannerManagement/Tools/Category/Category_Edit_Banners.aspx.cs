using System;
using System.Data;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SettingsProvider;
using System.Web.UI.WebControls;

[Actions(1)]
[Action(0, "Objects/CMS_Banner/add.png", "banner.bannercategory_edit_banner.newbanner", "~/CMSModules/BannerManagement/Tools/Banner/Banner_New.aspx?siteid={?siteid?}&categoryid={?categoryid?}")]

[EditedObject(SiteObjectType.BANNERCATEGORY, "categoryid")]

public partial class CMSModules_BannerManagement_Tools_Category_Category_Edit_Banners : CMSBannerManagementEditPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the grid view  
        this.gridBanners.OnAction += new OnActionEventHandler(gridBanners_OnAction);
        this.gridBanners.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridBanners_OnExternalDataBound);
        this.gridBanners.ZeroRowsText = ResHelper.GetString("banner.bannercategory_edit_banners.nobanners");

        // Look for category ID in the query string
        BannerCategoryInfo bci = EditedObject as BannerCategoryInfo;
        if (bci != null)
        {
            gridBanners.WhereCondition = "(BannerCategoryID = " + bci.BannerCategoryID + ")";
        }
        else
        {
            gridBanners.Visible = false;
            gridBanners.StopProcessing = true;
        }
    }

    #endregion


    #region "UniGrid events"

    private void gridBanners_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            // Deleteing of the category was fired
            case "delete":
                int bannerId = ValidationHelper.GetInteger(actionArgument, -1);
                BannerInfoProvider.DeleteBannerInfo(bannerId);                      
                break;
        }
    }


    object gridBanners_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "bannertype":
                BannerTypeEnum bannerType = (BannerTypeEnum)(int)parameter;
                
                return HTMLHelper.HTMLEncode(ResHelper.GetString("banner.bannertype." + bannerType.ToString().ToLowerCSafe()));
            case "hitsclicksleft":
                if (parameter == DBNull.Value)
                {
                    return "<i>" + GetString("general.unlimited") + "</i>";
                }

                int value = (int)parameter;

                if (value == 0)
                {
                    return "<span class=\"StatusDisabled\">" + value + "</span>";
                }

                return value;
            case "delete":
                DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                int? siteID = row.IsNull("BannerSiteID") ? (int?)null : ValidationHelper.GetInteger(row["BannerSiteID"], 0);

                string iconName;

                ImageButton button = ((ImageButton)sender);

                if (HasUserModifyPermission(siteID))
                {
                    iconName = "Delete";
                }
                else
                {
                    iconName = "Deletedisabled";
                    button.Enabled = false;
                }

                button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/" + iconName + ".png");

                break;
        }

        return parameter;
    }

    #endregion
}
