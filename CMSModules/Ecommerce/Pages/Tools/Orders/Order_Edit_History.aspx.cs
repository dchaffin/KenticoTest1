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

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.History")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_History : CMSOrdersPage
{
    protected override void OnPreInit(EventArgs e)
    {
        CustomerID = QueryHelper.GetInteger("customerid", 0);
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int orderId = QueryHelper.GetInteger("orderid", 0);
        
        OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);
        if (oi != null)
        {
            // Check order site ID
            CheckOrderSiteID(oi.OrderSiteID);

            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            gridElem.GridView.RowDataBound += gridElem_RowDataBound;
            gridElem.WhereCondition = "OrderID = " + orderId;
            gridElem.ShowActionsMenu = true;
        }
    }


    protected void gridElem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int orderStatId = ValidationHelper.GetInteger(((DataRowView)(e.Row.DataItem)).Row["ToStatusID"], 0);

            OrderStatusInfo osi = OrderStatusInfoProvider.GetOrderStatusInfo(orderStatId);
            if (osi != null)
            {
                e.Row.Style.Add("background-color", osi.StatusColor);
            }
        }
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Localize status display name
        if (sourceName.ToLowerCSafe() == "tostatusdisplayname")
        {
            return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(parameter)));
        }
        else if (sourceName.ToLowerCSafe() == "formattedusername")
        {
            return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(Convert.ToString(parameter)));
        }

        return HTMLHelper.HTMLEncode(parameter.ToString());
    }
}