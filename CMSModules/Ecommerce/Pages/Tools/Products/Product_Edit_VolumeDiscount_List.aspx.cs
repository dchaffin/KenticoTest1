using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[RegisterTitle("com.ui.productsvolumediscounts")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Products.VolumeDiscounts")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_VolumeDiscount_List : CMSProductsAdvancedPage
{
    #region "Variables"

    private SKUInfo product = null;
    private CurrencyInfo productCurrency = null;
    private bool disableEditing = false;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        product = SKUInfoProvider.GetSKUInfo(ProductID);

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.VolumeDiscounts;

        EditedObject = product;

        if (product != null)
        {
            // Check site id
            CheckEditedObjectSiteID(product.SKUSiteID);

            // Load product currency
            productCurrency = CurrencyInfoProvider.GetMainCurrency(product.SKUSiteID);

            // Display product price
            lblProductPrice.Text = CurrencyInfoProvider.GetFormattedPrice(product.SKUPrice, productCurrency);
        }

        // Set unigrid properties
        SetUnigridProperties();

        // Initialize the master page elements
        InitializeMasterPage();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display product price only if grid is not empty
        plcProductPrice.Visible = UniGrid.GridView.Rows.Count > 0;
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        string newUrl = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_VolumeDiscount_Edit.aspx?ProductID=" + ProductID + "&siteId=" + SiteID;

        // Set new volume discount action
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("Product_Edit_VolumeDiscount_List.NewItemCaption"),
            RedirectUrl = AddNodeIDParameterToUrl(newUrl),
            ImageUrl = GetImageUrl("Objects/Ecommerce_VolumeDiscount/add.png")
        });

        // Set master title
        if (DisplayTitle)
        {
            CurrentMaster.Title.TitleText = GetString("com.ui.productsvolumediscounts");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Ecommerce_VolumeDiscount/object.png");
            CurrentMaster.Title.HelpTopicName = "cms_ecommerce_products_volumediscounts";
        }
    }


    /// <summary>
    /// Initializes unigrid properties.
    /// </summary>
    private void SetUnigridProperties()
    {
        UniGrid.OnAction += new OnActionEventHandler(UniGrid_OnAction);
        UniGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGrid_OnExternalDataBound);
        UniGrid.WhereCondition = "VolumeDiscountSKUID = " + ProductID;
        UniGrid.OrderBy = "VolumeDiscountMinCount ASC";
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName.ToLowerCSafe() == "edit")
        {
            string editUrl = "Product_Edit_VolumeDiscount_Edit.aspx?productID=" + ProductID + "&volumeDiscountID=" + Convert.ToString(actionArgument) + "&siteId=" + SiteID;
            URLHelper.Redirect(AddNodeIDParameterToUrl(editUrl));
        }
        else if (actionName.ToLowerCSafe() == "delete")
        {
            SKUInfo sku = SKUInfoProvider.GetSKUInfo(ProductID);
            if (sku == null)
            {
                return;
            }

            if (CheckProductPermissions(sku))
            {
                // Delete VolumeDiscountInfo object from database
                VolumeDiscountInfoProvider.DeleteVolumeDiscountInfo(Convert.ToInt32(actionArgument));
            }
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "discountvalue":
                DataRowView row = (DataRowView)parameter;
                double value = ValidationHelper.GetDouble(row["VolumeDiscountValue"], 0);
                bool isFlat = ValidationHelper.GetBoolean(row["VolumeDiscountIsFlatValue"], false);
                // If value is relative, add "%" next to the value.
                if (isFlat)
                {
                    return CurrencyInfoProvider.GetFormattedPrice(value, productCurrency);
                }
                else
                {
                    return value.ToString() + "%";
                }
            case "edit":
                if (disableEditing)
                {
                    ImageButton editButton = ((ImageButton)sender);
                    editButton.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Editdisabled.png");
                    editButton.OnClientClick = "return false;";
                }
                break;

            case "delete":
                if (disableEditing)
                {
                    ImageButton deleteButton = ((ImageButton)sender);
                    deleteButton.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Deletedisabled.png");
                    deleteButton.OnClientClick = "return false;";
                }
                break;
        }

        return parameter;
    }

    #endregion
}