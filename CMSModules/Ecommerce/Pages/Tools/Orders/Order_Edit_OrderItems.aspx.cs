using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.Items")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_OrderItems : CMSOrdersPage
{
    protected int orderId = 0;
    private string mSessionKey = "CMSDeskOrderItemsShoppingCart";


    /// <summary>
    /// Shopping cart to use.
    /// </summary>
    private ShoppingCartInfo ShoppingCart
    {
        get
        {
            if (SessionHelper.GetValue(mSessionKey) == null)
            {
                SessionHelper.SetValue(mSessionKey, ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(orderId));
            }

            return (ShoppingCartInfo)SessionHelper.GetValue(mSessionKey);
        }
        set
        {
            SessionHelper.SetValue(mSessionKey, value);
        }
    }


    protected override void OnPreInit(EventArgs e)
    {
        CustomerID = QueryHelper.GetInteger("customerid", 0);
        base.OnPreInit(e);
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get order id
        orderId = QueryHelper.GetInteger("orderid", 0);

        if (!RequestHelper.IsPostBack())
        {
            if (!QueryHelper.GetBoolean("cartexist", false))
            {
                ShoppingCart = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(orderId);
            }
        }

        if (orderId > 0)
        {
            if ((ShoppingCart != null) && (ShoppingCart.Order != null))
            {
                // Check order site ID
                CheckOrderSiteID(ShoppingCart.Order.OrderSiteID);
            }

            Cart.LocalShoppingCart = ShoppingCart;
            Cart.EnableProductPriceDetail = true;
            Cart.ShoppingCartInfoObj.IsCreatedFromOrder = true;
            Cart.CheckoutProcessType = CheckoutProcessEnum.CMSDeskOrderItems;
            Cart.OnPaymentCompleted += new EventHandler(Cart_OnPaymentCompleted);
            Cart.OnPaymentSkipped += new EventHandler(Cart_OnPaymentSkipped);
            Cart.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(Cart_OnCheckPermissions);
            Cart.RequiredFieldsMark = CMS.EcommerceProvider.ShoppingCart.DEFAULT_REQUIRED_FIELDS_MARK;
        }


        if (!RequestHelper.IsPostBack())
        {
            // Display 'Changes saved' message        
            if (QueryHelper.GetBoolean("saved", false))
            {
                // Show message
                ShowChangesSaved();
            }
            // Display 'Payment completed' message
            else if (QueryHelper.GetBoolean("paymentcompleted", false))
            {
                ShowInformation(GetString("PaymentGateway.CMSDeskOrderItems.PaymentCompleted"));
            }
        }
    }


    private void Cart_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission(permissionType))
        {
            RedirectToAccessDenied("CMS.Ecommerce", permissionType);
        }
    }


    private void Cart_OnPaymentCompleted(object sender, EventArgs e)
    {
        Cart.CleanUpShoppingCart();
        URLHelper.Redirect("~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit_OrderItems.aspx?orderID=" + ShoppingCart.OrderId.ToString() + "&paymentcompleted=1");
    }


    private void Cart_OnPaymentSkipped(object sender, EventArgs e)
    {
        string saved = "";

        // Payment skipped because of no payment gateway was specified
        if (Cart.PaymentGatewayProvider != null)
        {
            // Do nothing
        }
        // Payment skipped from shopping cart step with payment - Button 'Skip payment' was pressed
        else
        {
            // Display 'Changes saved' message after page redirect
            saved = "&saved=1";
        }

        Cart.CleanUpShoppingCart();
        URLHelper.Redirect("~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit_OrderItems.aspx?orderID=" + ShoppingCart.OrderId.ToString() + saved);
    }


    /// <summary>
    /// On prerender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        ShoppingCart = Cart.LocalShoppingCart;
        base.OnPreRender(e);
    }
}