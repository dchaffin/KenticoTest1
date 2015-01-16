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
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.Invoice")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_InvoicePrint : CMSOrdersPage
{
    protected override void OnPreInit(EventArgs e)
    {
        CustomerID = QueryHelper.GetInteger("customerid", 0);
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int orderId = QueryHelper.GetInteger("orderid", 0);
        OrderInfo order = OrderInfoProvider.GetOrderInfo(orderId);
        if (order != null)
        {
            lblInvoice.Text = URLHelper.MakeLinksAbsolute(order.OrderInvoice);
        }
    }
}