using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Title("Objects/Ecommerce_OptionCategory/new.png", "OptionCategory_List.NewItemCaption", "new_option_category")]
public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_New : CMSProductOptionCategoriesPage
{
    protected int editedSiteId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Field validator error messages initialization
        rfvCategoryName.ErrorMessage = GetString("general.requirescodename");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        // Control initializations				
        radText.Text = GetString("com.categorytype.text");
        radSelection.Text = GetString("com.categorytype.selection");

        editedSiteId = ConfiguredSiteID;

        // Initializes page title control
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("optioncategory_edit.itemlistlink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/ProductOptions/OptionCategory_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = FormatBreadcrumbObjectName(GetString("optioncategory_list.newitemcaption"), editedSiteId);
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check module permissions
        bool global = (editedSiteId <= 0);
        if (!ECommerceContext.IsUserAuthorizedToModifyOptionCategory(global))
        {
            // Check module permissions
            if (global)
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
            }
            else
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyProducts");
            }
        }

        // Check input value from textboxes
        string errorMessage = new Validator().NotEmpty(txtDisplayName.Text, GetString("general.requiresdisplayname"))
            .NotEmpty(txtCategoryName.Text, GetString("general.requirescodename"))
            .IsCodeName(txtCategoryName.Text, GetString("general.invalidcodename")).Result;

        if (errorMessage == "")
        {
            // Category code name must be unique
            OptionCategoryInfo optionCategoryObj = null;
            string siteWhere = (ConfiguredSiteID > 0) ? " AND (CategorySiteID = " + ConfiguredSiteID + " OR CategorySiteID IS NULL)" : "";
            DataSet ds = OptionCategoryInfoProvider.GetOptionCategories("CategoryName = '" + txtCategoryName.Text.Trim().Replace("'", "''") + "'" + siteWhere, null, 1, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                optionCategoryObj = new OptionCategoryInfo(ds.Tables[0].Rows[0]);
            }

            // If category code name value is unique														
            if (optionCategoryObj == null)
            {
                // Create, fill and set OptionCategoryInfo object
                optionCategoryObj = new OptionCategoryInfo();
                optionCategoryObj.CategoryDisplayName = txtDisplayName.Text.Trim();
                optionCategoryObj.CategoryName = txtCategoryName.Text.Trim();
                optionCategoryObj.CategorySelectionType = radSelection.Checked ? OptionCategorySelectionTypeEnum.Dropdownlist : OptionCategorySelectionTypeEnum.TextBox;
                optionCategoryObj.CategoryEnabled = true;
                optionCategoryObj.CategoryDefaultRecord = "";
                optionCategoryObj.CategoryDefaultOptions = "";
                optionCategoryObj.CategorySiteID = ConfiguredSiteID;

                // Create category and option under transaction
                using (CMSTransactionScope tr = new CMSTransactionScope())
                {
                    OptionCategoryInfoProvider.SetOptionCategoryInfo(optionCategoryObj);

                    if (radText.Checked)
                    {
                        // Create text product option
                        SKUInfo option = new SKUInfo()
                                             {
                                                 SKUOptionCategoryID = optionCategoryObj.CategoryID,
                                                 SKUProductType = SKUProductTypeEnum.Text,
                                                 SKUSiteID = ConfiguredSiteID,
                                                 SKUName = optionCategoryObj.CategoryDisplayName,
                                                 SKUDepartmentID = 0,
                                                 SKUPrice = 0,
                                                 SKUNeedsShipping = false,
                                                 SKUWeight = 0,
                                                 SKUEnabled = true
                                             };

                        SKUInfoProvider.SetSKUInfo(option);
                    }

                    // Commit a transaction
                    tr.Commit();
                }

                URLHelper.Redirect("OptionCategory_Edit.aspx?categoryId=" + Convert.ToString(optionCategoryObj.CategoryID) + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                // Show error message
                ShowError(GetString("optioncategory_new.errorExistingCodeName"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }
}