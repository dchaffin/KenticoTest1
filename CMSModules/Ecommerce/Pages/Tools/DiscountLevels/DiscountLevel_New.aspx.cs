using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Title("Objects/Ecommerce_DiscountLevel/new.png", "DiscountLevel_List.NewItemCaption", "cms_ecommerce_discountlevels_general")]
public partial class CMSModules_Ecommerce_Pages_Tools_DiscountLevels_DiscountLevel_New : CMSDiscountLevelsPage
{
    protected int mDiscountLevelId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Control initializations
        lblDiscountLevelValidFrom.Text = GetString("DiscountLevel_Edit.DiscountLevelValidFromLabel");
        lblDiscountLevelValue.Text = GetString("DiscountLevel_Edit.DiscountLevelValueLabel");
        lblDiscountLevelValidTo.Text = GetString("DiscountLevel_Edit.DiscountLevelValidToLabel");
        dtPickerDiscountLevelValidFrom.SupportFolder = "~/CMSAdminControls/Calendar";
        dtPickerDiscountLevelValidTo.SupportFolder = "~/CMSAdminControls/Calendar";

        // Init Validators
        rfvDiscountLevelDisplayName.ErrorMessage = GetString("DiscountLevel_Edit.rfvDiscountLevelDisplayName.ErrorMessage");
        rfvDiscountLevelName.ErrorMessage = GetString("DiscountLevel_Edit.rfvDiscountLevelName.ErrorMessage");
        rfvDiscountLevelValue.ErrorMessage = GetString("DiscountLevel_Edit.rfvDiscountLevelValue.ErrorMessage");

        // Initializes page title breadcrumbs control
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("DiscountLevel_Edit.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/DiscountLevels/DiscountLevel_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = FormatBreadcrumbObjectName(GetString("DiscountLevel_Edit.NewItemCaption"), ConfiguredSiteID);
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
        bool global = (ConfiguredSiteID <= 0);
        if (!ECommerceContext.IsUserAuthorizedToModifyDiscountLevel(global))
        {
            if (global)
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
            }
            else
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyDiscounts");
            }
        }

        string errorMessage = new Validator().NotEmpty(txtDiscountLevelDisplayName.Text.Trim(), GetString("DiscountLevel_Edit.rfvDiscountLevelDisplayName.ErrorMessage"))
            .NotEmpty(txtDiscountLevelName.Text.Trim(), GetString("DiscountLevel_Edit.rfvDiscountLevelName.ErrorMessage"))
            .IsCodeName(txtDiscountLevelName.Text.Trim(), GetString("General.ErrorCodeNameInIdentifierFormat"))
            .NotEmpty(txtDiscountLevelValue.Text.Trim(), GetString("DiscountLevel_Edit.rfvDiscountLevelValue.ErrorMessage")).Result;

        // Discount value validation
        if (errorMessage == "")
        {
            if (!ValidationHelper.IsInRange(0, 100, ValidationHelper.GetDouble(txtDiscountLevelValue.Text.Trim(), -1)))
            {
                errorMessage = GetString("Com.Error.RelativeDiscountValue");
            }
        }

        // From/to date validation
        if (errorMessage == "")
        {
            if ((!dtPickerDiscountLevelValidFrom.IsValidRange()) || (!dtPickerDiscountLevelValidTo.IsValidRange()))
            {
                errorMessage = GetString("general.errorinvaliddatetimerange");
            }

            if ((dtPickerDiscountLevelValidFrom.SelectedDateTime != DateTime.MinValue) &&
                (dtPickerDiscountLevelValidTo.SelectedDateTime != DateTime.MinValue) &&
                (dtPickerDiscountLevelValidFrom.SelectedDateTime >= dtPickerDiscountLevelValidTo.SelectedDateTime))
            {
                errorMessage = GetString("General.DateOverlaps");
            }
        }

        if (errorMessage == "")
        {
            // Discount level code name must be unique
            DiscountLevelInfo discountLevelObj = null;
            string siteWhere = (ConfiguredSiteID > 0) ? " AND (DiscountLevelSiteID = " + ConfiguredSiteID + " OR DiscountLevelSiteID IS NULL)" : "";
            DataSet ds = DiscountLevelInfoProvider.GetDiscountLevels("DiscountLevelName = '" + txtDiscountLevelName.Text.Trim().Replace("'", "''") + "'" + siteWhere, null, 1, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                discountLevelObj = new DiscountLevelInfo(ds.Tables[0].Rows[0]);
            }

            // If name is unique OR ids are same
            if ((discountLevelObj == null) || (discountLevelObj.DiscountLevelID == mDiscountLevelId))
            {
                // If name is unique
                if ((discountLevelObj == null))
                {
                    // And id does not exist -> insert new
                    discountLevelObj = DiscountLevelInfoProvider.GetDiscountLevelInfo(mDiscountLevelId);
                    if (discountLevelObj == null)
                    {
                        // Create new DiscountLevelInfo
                        discountLevelObj = new DiscountLevelInfo();
                        discountLevelObj.DiscountLevelSiteID = ConfiguredSiteID;
                    }
                }

                // Set values
                discountLevelObj.DiscountLevelValidFrom = dtPickerDiscountLevelValidFrom.SelectedDateTime;
                discountLevelObj.DiscountLevelEnabled = chkDiscountLevelEnabled.Checked;
                discountLevelObj.DiscountLevelName = txtDiscountLevelName.Text.Trim();
                discountLevelObj.DiscountLevelValue = Convert.ToDouble(txtDiscountLevelValue.Text.Trim());
                discountLevelObj.DiscountLevelValidTo = dtPickerDiscountLevelValidTo.SelectedDateTime;
                discountLevelObj.DiscountLevelDisplayName = txtDiscountLevelDisplayName.Text.Trim();

                DiscountLevelInfoProvider.SetDiscountLevelInfo(discountLevelObj);

                URLHelper.Redirect("DiscountLevel_Edit_Frameset.aspx?discountLevelId=" + Convert.ToString(discountLevelObj.DiscountLevelID) + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                //Show error message
                ShowError(GetString("DiscountLevel_Edit.DiscountLevelNameExists"));
            }
        }
        else
        {
            //Show error message
            ShowError(errorMessage);
        }
    }
}