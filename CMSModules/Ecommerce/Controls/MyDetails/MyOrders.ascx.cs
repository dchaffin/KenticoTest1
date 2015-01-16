using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_MyDetails_MyOrders : CMSAdminControl
{
    #region "Variables"

    private int mCustomerId = 0;
    private bool mShowOrderTrackingNumber = false;
    private bool downloadLinksColumnVisible = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Customer ID.
    /// </summary>
    public int CustomerId
    {
        get
        {
            return mCustomerId;
        }
        set
        {
            mCustomerId = value;
        }
    }


    /// <summary>
    /// Indcicates if order tracking number should be displayed.
    /// </summary>
    public bool ShowOrderTrackingNumber
    {
        get
        {
            return mShowOrderTrackingNumber;
        }
        set
        {
            mShowOrderTrackingNumber = value;
        }
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["StopProcessing"], false);
        }
        set
        {
            ViewState["StopProcessing"] = value;
        }
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (CMSContext.CurrentUser.IsAuthenticated())
            {
                gridOrders.IsLiveSite = IsLiveSite;
                gridOrders.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridOrders_OnExternalDataBound);
                gridOrders.WhereCondition = "(CustomerID = " + CustomerId + ") AND (OrderSiteID = " + CMSContext.CurrentSiteID + ")";

                // Set pager links text on live site
                if (IsLiveSite)
                {
                    gridOrders.Pager.FirstPageText = "&lt;&lt;";
                    gridOrders.Pager.LastPageText = "&gt;&gt;";
                    gridOrders.Pager.PreviousPageText = "&lt;";
                    gridOrders.Pager.NextPageText = "&gt;";
                    gridOrders.Pager.PreviousGroupText = "...";
                    gridOrders.Pager.NextGroupText = "...";
                }
            }
            else
            {
                // Hide if user is not authenticated
                Visible = false;
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Set visibility of order traking number column
        gridOrders.GridView.Columns[4].Visible = ShowOrderTrackingNumber;

        // Set visibility of download links column
        gridOrders.GridView.Columns[6].Visible = downloadLinksColumnVisible;

        ScriptHelper.RegisterDialogScript(Page);
    }


    protected object gridOrders_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView dr = null;
        switch (sourceName.ToLowerCSafe())
        {
            case "totalprice":
                dr = (DataRowView)parameter;
                return HTMLHelper.HTMLEncode(String.Format(dr["CurrencyFormatString"].ToString(), dr["OrderTotalPrice"]));

            case "statusdisplayname":
                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(parameter)));

            case "invoice":
                return "<a target=\"_blank\" href=\"" + URLHelper.ResolveUrl("~/CMSModules/Ecommerce/CMSPages/GetInvoice.aspx?orderid=" + ValidationHelper.GetInteger(parameter, 0)) + "\">" + GetString("general.view") + "</a>";

            case "downloads":
                int orderId = ValidationHelper.GetInteger(parameter, 0);

                string where = String.Format("COM_OrderItemSKUFile.OrderItemID IN (SELECT OrderItemID FROM COM_OrderItem WHERE OrderItemOrderID = {0})", orderId);

                // Get order item SKU files for the order
                DataSet orderItemSkuFiles = SqlHelperClass.ExecuteQuery("ecommerce.orderitemskufile.selectallwithdetails", null, where, null);

                // If there are some downloads available for the order
                if (!DataHelper.DataSourceIsEmpty(orderItemSkuFiles))
                {
                    // Make download links column visible
                    downloadLinksColumnVisible = true;

                    // Show view action for this record
                    string url = URLHelper.ResolveUrl("~/CMSModules/Ecommerce/CMSPages/EProducts.aspx?orderid=" + orderId);
                    return String.Format("<a href=\"#\" onclick=\"modalDialog('{0}', 'DownloadLinks', 700, 420); return false;\">{1}</a>", url, GetString("general.view"));
                }

                return String.Empty;
        }
        return parameter;
    }


    /// <summary>
    /// Overridden SetValue - because of MyAccount webpart.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override void SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerCSafe())
        {
            case "customerid":
                CustomerId = ValidationHelper.GetInteger(value, 0);
                break;
            case "showordertrackingnumber":
                ShowOrderTrackingNumber = ValidationHelper.GetBoolean(value, false);
                break;
        }
    }
}