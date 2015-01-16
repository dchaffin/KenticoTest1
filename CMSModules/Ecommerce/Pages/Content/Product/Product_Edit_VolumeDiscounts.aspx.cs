using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

[Security(Resource = "CMS.Content", UIElements = "ContentProduct.VolumeDiscounts")]
public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_VolumeDiscounts : CMSContentProductPage
{
    #region "Variables"

    protected SKUInfo sku = null;
    protected CurrencyInfo productCurrency = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Get currency for discount value formatting
            sku = SKUInfoProvider.GetSKUInfo(Node.NodeSKUID);

            EditedObject = sku;

            productCurrency = CurrencyInfoProvider.GetMainCurrency(sku != null ? sku.SKUSiteID : Node.NodeSiteID);

            if (sku != null)
            {
                // Check site ID
                CheckProductSiteID(sku.SKUSiteID);

                // Display product price
                lblProductPrice.Text = CurrencyInfoProvider.GetFormattedPrice(sku.SKUPrice, productCurrency);

                // Set action handler
                gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
                gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
                gridElem.WhereCondition = "VolumeDiscountSKUID = " + sku.SKUID;
            }

            // Set the master page actions element        
            CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
            {
                ControlType = HeaderActionTypeEnum.LinkButton,
                Text = GetString("Product_Edit_VolumeDiscount_List.NewItemCaption"),
                OnClientClick = "modalDialog('" + URLHelper.GetAbsoluteUrl("~/CMSModules/Ecommerce/Pages/Content/Product/Product_Edit_VolumeDiscount_Edit.aspx") + "?ProductID=" + sku.SKUID + "&modaldialog=1', 'VolumeDiscounts', 500, 350);",
                ImageUrl = GetImageUrl("Objects/Ecommerce_VolumeDiscount/add.png"),
            });

            // Set help topic
            CurrentMaster.HeaderActions.HelpTopicName = "cms_ecommerce_products_volumediscounts";

            // Register modal dialog script
            ScriptHelper.RegisterDialogScript(this);
        }

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.VolumeDiscounts;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display product price only if grid is not empty
        plcProductPrice.Visible = gridElem.GridView.Rows.Count > 0;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName.ToLowerCSafe() == "delete")
        {
            if ((sku != null) && CheckProductPermissions(sku))
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
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
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
                // Open modal dialog with volume discount edit form
                int volumeDiscountId = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem)[0], 0);
                ((WebControl)sender).Attributes["onclick"] = "modalDialog('" + URLHelper.GetAbsoluteUrl("~/CMSModules/Ecommerce/Pages/Content/Product/Product_Edit_VolumeDiscount_Edit.aspx") + "?ProductID=" + sku.SKUID + "&VolumeDiscountID=" + volumeDiscountId + "&modaldialog=1', 'VolumeDiscounts', 500, 350); return false;";

                break;
        }

        return parameter;
    }

    #endregion
}