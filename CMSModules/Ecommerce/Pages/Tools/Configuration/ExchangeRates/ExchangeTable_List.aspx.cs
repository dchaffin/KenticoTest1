using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[Title("Objects/Ecommerce_ExchangeTable/object.png", "ExchangeTable_List.HeaderCaption", "exchange_rates_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ExchangeRates_ExchangeTable_List : CMSExchangeRatesPage
{
    #region "Variables"

    private int mCurrentTableId = 0;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // New item link
        CurrentMaster.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("ExchangeTable_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("ExchangeTable_Edit.aspx?siteId=" + SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_ExchangeTable/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });

        FindCurrentTableID();

        gridElem.GridView.RowDataBound += new GridViewRowEventHandler(GridView_RowDataBound);
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.ZeroRowsText = GetString("general.nodatafound");

        // Configuring global records from specific site
        if ((ConfiguredSiteID == 0) && (SiteID != 0))
        {
            ShowInformation(GetString("com.UsingGlobalSettings"));
        }

        // Filter records by site
        gridElem.WhereCondition = "ISNULL(ExchangeTableSiteID, 0) = " + ConfiguredSiteID;
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("ExchangeTable_Edit.aspx?exchangeid=" + Convert.ToString(actionArgument) + "&siteId=" + SiteID);
        }
        else if (actionName == "delete")
        {
            CheckConfigurationModification();

            int tableId = ValidationHelper.GetInteger(actionArgument, 0);
            // Delete ExchangeTableInfo object from database
            ExchangeTableInfoProvider.DeleteExchangeTableInfo(tableId);

            // If current table deleted
            if (mCurrentTableId == tableId)
            {
                // Find new current table
                FindCurrentTableID();
            }
        }
    }


    /// <summary>
    /// Handles the databoud event (for coloring valid/invalid exchange rates)
    /// </summary>
    private void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataRowView drv = e.Row.DataItem as DataRowView;
        if (drv != null)
        {
            int tableId = ValidationHelper.GetInteger(drv.Row["ExchangeTableID"], 0);
            if (tableId == mCurrentTableId)
            {
                // Exchange rate which will be used for all operations is highlighted
                e.Row.Style.Add("background-color", "rgb(221, 250, 222)");
            }
        }
    }

    #endregion


    #region "Private methods"

    private void FindCurrentTableID()
    {
        mCurrentTableId = 0;

        // Get current table (it will be highlighted in the listing)
        ExchangeTableInfo eti = ExchangeTableInfoProvider.GetLastExchangeTableInfo(SiteID);
        if (eti != null)
        {
            mCurrentTableId = eti.ExchangeTableID;
        }
    }

    #endregion
}