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
using CMS.SiteProvider;

[Title("Objects/Ecommerce_ShippingOption/new.png", "com_shippingoption_edit.newheadercaption", "new_shipping_option")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_New : CMSShippingOptionsPage
{
    protected int mShippingOptionID = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initializes page title and breadcrumbs
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("ShippingOption_EditHeader.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingOptions/ShippingOption_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = FormatBreadcrumbObjectName(GetString("com_shippingoption_edit.newitemcaption"), ConfiguredSiteID);
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = breadcrumbs;

        // Required field validator error messages initialization
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvName.ErrorMessage = GetString("COM_ShippingOption_Edit.NameError");
        txtShippingOptionCharge.EmptyErrorMessage = GetString("COM_ShippingOption_Edit.ChargeError");
        txtShippingOptionCharge.ValidationErrorMessage = GetString("COM_ShippingOption_Edit.ChargePositive");

        // Control initializations						
        lblShippingOptionCharge.Text = GetString("COM_ShippingOption_Edit.ShippingOptionChargeLabel");

        // Ensure correct currency code after price value
        txtShippingOptionCharge.CurrencySiteID = ConfiguredSiteID;

        // Check presence of main currency
        string currencyWarning = CheckMainCurrency(ConfiguredSiteID);
        if (!string.IsNullOrEmpty(currencyWarning))
        {
            ShowWarning(currencyWarning, null, null);
        }

        // Init file uploader
        file.ObjectType = ECommerceObjectType.SHIPPINGOPTION;
        file.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;
        file.SiteID = ConfiguredSiteID;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check permissions
        CheckConfigurationModification(ConfiguredSiteID);

        string errorMessage = new Validator()
            .NotEmpty(txtShippingOptionDisplayName.Text.Trim(), rfvDisplayName.ErrorMessage)
            .NotEmpty(txtShippingOptionName.Text.Trim(), rfvName.ErrorMessage).Result;

        if (!ValidationHelper.IsCodeName(txtShippingOptionName.Text.Trim()))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        if (errorMessage == "")
        {
            errorMessage = txtShippingOptionCharge.Validate(false);
        }

        if (errorMessage == "")
        {
            // ShippingOptionName must be unique
            ShippingOptionInfo shippingOptionObj = null;
            string siteWhere = (ConfiguredSiteID > 0) ? " AND (ShippingOptionSiteID = " + ConfiguredSiteID + " OR ShippingOptionSiteID IS NULL)" : "";
            DataSet ds = ShippingOptionInfoProvider.GetShippingOptions("ShippingOptionName = '" + txtShippingOptionName.Text.Trim().Replace("'", "''") + "'" + siteWhere, null, 1, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                shippingOptionObj = new ShippingOptionInfo(ds.Tables[0].Rows[0]);
            }

            // If shippingOptionName value is unique														
            if ((shippingOptionObj == null) || (shippingOptionObj.ShippingOptionID == mShippingOptionID))
            {
                // If shippingOptionName value is unique -> determine whether it is update or insert 
                if ((shippingOptionObj == null))
                {
                    // Get ShippingOptionInfo object by primary key
                    shippingOptionObj = ShippingOptionInfoProvider.GetShippingOptionInfo(mShippingOptionID);
                    if (shippingOptionObj == null)
                    {
                        // Create new item -> insert
                        shippingOptionObj = new ShippingOptionInfo();
                        shippingOptionObj.ShippingOptionSiteID = ConfiguredSiteID;
                    }
                }

                shippingOptionObj.ShippingOptionDisplayName = txtShippingOptionDisplayName.Text.Trim();
                shippingOptionObj.ShippingOptionDescription = txtDescription.Text.Trim();
                shippingOptionObj.ShippingOptionCharge = txtShippingOptionCharge.Price;
                shippingOptionObj.ShippingOptionName = txtShippingOptionName.Text.Trim();
                shippingOptionObj.ShippingOptionEnabled = chkShippingOptionEnabled.Checked;

                // Save record
                ShippingOptionInfoProvider.SetShippingOptionInfo(shippingOptionObj);

                // Upload file
                file.ObjectID = shippingOptionObj.ShippingOptionID;
                file.UploadFile();

                URLHelper.Redirect("ShippingOption_Edit_Frameset.aspx?ShippingOptionID=" + Convert.ToString(shippingOptionObj.ShippingOptionID) + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                // Show error message
                ShowError(GetString("ShippingOption_Edit.ShippingOptionNameExists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }
}