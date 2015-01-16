using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "ProductOptions.Options")]
public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Edit_Options : CMSProductOptionsPage
{
    #region "Variables"

    protected int categoryId = 0;
    protected OptionCategoryInfo categoryObj = null;
    protected int editedSiteId = 0;
    protected bool allowActions = true;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get category ID and department ID from URL
        categoryId = QueryHelper.GetInteger("categoryid", 0);
        categoryObj = OptionCategoryInfoProvider.GetOptionCategoryInfo(categoryId);

        EditedObject = categoryObj;

        if (categoryObj != null)
        {
            editedSiteId = categoryObj.CategorySiteID;

            // Check edited objects site id
            CheckEditedObjectSiteID(editedSiteId);

            // Allow actions only for non-text categories
            allowActions = (categoryObj.CategorySelectionType != OptionCategorySelectionTypeEnum.TextBox) && (categoryObj.CategorySelectionType != OptionCategorySelectionTypeEnum.TextArea);

            grid.ShowObjectMenu = allowActions;

            if (allowActions)
            {
                HeaderActions hdrActions = CurrentMaster.HeaderActions;

                // New option action
                hdrActions.ActionsList.Add(new HeaderAction()
                {
                    ControlType = HeaderActionTypeEnum.Hyperlink,
                    Text = GetString("ProductOptions.NewItemCaption"),
                    RedirectUrl = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx?categoryid=" + categoryId + "&siteId=" + SiteID),
                    ImageUrl = GetImageUrl("Objects/Ecommerce_SKU/add.png")
                });

                // Sort action
                hdrActions.ActionsList.Add(new HeaderAction()
                {
                    ControlType = HeaderActionTypeEnum.LinkButton,
                    Text = GetString("ProductOptions.SortAlphabetically"),
                    ImageUrl = GetImageUrl("CMSModules/CMS_Ecommerce/optionssort.png"),
                    CommandName = "lnkSort_Click"
                });

                hdrActions.ActionPerformed += HeaderActions_ActionPerformed;
            }

            // Unigrid
            grid.OnAction += grid_OnAction;
            grid.OnExternalDataBound += grid_OnExternalDataBound;
            grid.WhereCondition = "SKUOptionCategoryID = " + categoryId;
        }
    }

    #endregion


    #region "Event handlers"

    private object grid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "skuprice":
                DataRowView row = (DataRowView)parameter;
                double value = ValidationHelper.GetDouble(row["SKUPrice"], 0);
                int siteId = ValidationHelper.GetInteger(row["SKUSiteID"], 0);

                // Format price
                return CurrencyInfoProvider.GetFormattedPrice(value, siteId);

            case "delete":
            case "moveup":
            case "movedown":
                {
                    ImageButton button = sender as ImageButton;
                    if (button != null)
                    {
                        // Hide actions when not allowed
                        button.Visible = allowActions;
                    }
                }
                break;
        }

        return parameter;
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "lnksort_click":
                if (allowActions)
                {
                    // Check permissions
                    CheckModifyPermission();

                    SKUInfoProvider.SortProductOptionsAlphabetically(categoryId);
                    grid.ReloadData();
                }
                break;
        }
    }


    private void grid_OnAction(string actionName, object actionArgument)
    {
        int skuId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_Frameset.aspx?categoryid=" + categoryId + "&productid=" + skuId + "&siteId=" + categoryObj.CategorySiteID);
                break;

            case "delete":
                // Check permissions
                CheckModifyPermission();

                // Check dependencies
                if (SKUInfoProvider.CheckDependencies(skuId))
                {
                    // Show error message
                    ShowError(GetString("Ecommerce.DeleteDisabled"));

                    return;
                }

                SKUInfoProvider.DeleteSKUInfo(skuId);
                grid.ReloadData();

                break;

            case "moveup":
                // Check permissions
                CheckModifyPermission();

                SKUInfoProvider.MoveSKUOptionUp(skuId);
                break;

            case "movedown":
                // Check permissions
                CheckModifyPermission();

                SKUInfoProvider.MoveSKUOptionDown(skuId);
                break;
        }
    }


    private void CheckModifyPermission()
    {
        // Check permissions
        bool global = (editedSiteId <= 0);
        if (!ECommerceContext.IsUserAuthorizedToModifyOptionCategory(global))
        {
            if (global)
            {
                RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
            }
            else
            {
                RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyProducts");
            }
        }
    }

    #endregion
}