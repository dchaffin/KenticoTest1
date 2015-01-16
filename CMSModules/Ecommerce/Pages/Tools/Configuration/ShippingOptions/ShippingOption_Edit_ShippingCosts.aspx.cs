using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.ShippingOptions.ShippingCosts")]
[Help("shippingcosts_list", "helpTopic")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_ShippingCosts : CMSShippingOptionsPage
{
    #region "Variables"

    protected int mShippingOptionId = 0;
    protected ShippingOptionInfo mShippingOptionInfoObj = null;
    protected CurrencyInfo currency = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mShippingOptionId = QueryHelper.GetInteger("shippingoptionid", 0);

        mShippingOptionInfoObj = ShippingOptionInfoProvider.GetShippingOptionInfo(mShippingOptionId);
        EditedObject = mShippingOptionInfoObj;

        if (mShippingOptionInfoObj != null)
        {
            CheckEditedObjectSiteID(mShippingOptionInfoObj.ShippingOptionSiteID);
            currency = CurrencyInfoProvider.GetMainCurrency(mShippingOptionInfoObj.ShippingOptionSiteID);
        }

        // Init unigrid
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.OnAfterRetrieveData += new OnAfterRetrieveData(gridElem_OnAfterRetrieveData);
        gridElem.WhereCondition = "ShippingCostShippingOptionID = " + mShippingOptionId;
        gridElem.ZeroRowsText = GetString("com.ui.shippingcost.edit_nodata");
        gridElem.GridView.AllowSorting = false;

        // Prepare the new shipping cost header action
        CurrentMaster.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("com.ui.shippingcost.edit_new"),
            RedirectUrl = ResolveUrl("ShippingOption_Edit_ShippingCosts_Edit.aspx?shippingCostShippingOptionId=" + mShippingOptionId + "&siteId=" + SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_ShippingOption/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });
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
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("ShippingOption_Edit_ShippingCosts_Edit.aspx?shippingCostID=" + Convert.ToString(actionArgument) + "&shippingCostShippingOptionId=" + mShippingOptionId + "&siteId=" + SiteID);
                break;
            case "delete":
                // Check permissions
                CheckConfigurationModification(mShippingOptionInfoObj.ShippingOptionSiteID);

                // Delete ShippingCostInfo object from database
                ShippingCostInfoProvider.DeleteShippingCostInfo(Convert.ToInt32(actionArgument));
                break;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAfterRetrieveData event. Appends cost for zero weight.
    /// </summary>
    /// <param name="ds">Input data</param>
    protected DataSet gridElem_OnAfterRetrieveData(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds) && (mShippingOptionInfoObj != null))
        {
            // Create row for zero weight shipping cost
            DataTable table = ds.Tables[0];
            DataRow zeroWeightRow = table.NewRow();

            zeroWeightRow["ShippingCostID"] = 0;
            zeroWeightRow["ShippingCostMinWeight"] = 0;
            zeroWeightRow["ShippingCostValue"] = mShippingOptionInfoObj.ShippingOptionCharge;

            // Insert recort at first position
            table.Rows.InsertAt(zeroWeightRow, 0);
        }

        return ds;
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        int shippingCostId = -1;
        switch (sourceName.ToLowerCSafe())
        {
            case "shippingcostvalue":
                double value = ValidationHelper.GetDouble(parameter, 0);

                return CurrencyInfoProvider.GetFormattedPrice(value, currency);

            case "edit":
            case "delete":
                if (sender is ImageButton)
                {
                    // Hide editing/deleting of zero-row (cost from general tab)
                    shippingCostId = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem)[0], 0);
                    if (shippingCostId == 0)
                    {
                        ImageButton button = sender as ImageButton;
                        button.Visible = false;
                    }
                }

                break;
        }
        return parameter;
    }

    #endregion
}