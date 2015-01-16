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
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_New : CMSOrdersPage
{
    private int customerId = 0;


    /// <summary>
    /// Shopping cart to use.
    /// </summary>
    private ShoppingCartInfo ShoppingCart
    {
        get
        {
            ShoppingCartInfo sci = SessionHelper.GetValue(SessionKey) as ShoppingCartInfo;
            if (sci == null)
            {
                sci = GetNewCart();
                SessionHelper.SetValue(SessionKey, sci);
            }

            return sci;
        }
        set
        {
            SessionHelper.SetValue(SessionKey, value);
        }
    }


    /// <summary>
    /// Shopping cart session key.
    /// </summary>
    private string SessionKey
    {
        get
        {
            if (customerId > 0)
            {
                return "CMSDeskNewCustomerOrderShoppingCart";
            }
            else
            {
                return "CMSDeskNewOrderShoppingCart";
            }
        }
    }


    protected ShoppingCartInfo GetNewCart()
    {
        ShoppingCartInfo newCart = ShoppingCartInfoProvider.CreateShoppingCartInfo(CMSContext.CurrentSite.SiteID);
        if (customerId > 0)
        {
            CustomerInfo ci = CustomerInfoProvider.GetCustomerInfo(customerId);
            if (ci != null)
            {
                UserInfo ui = null;
                if (ci.CustomerUserID > 0)
                {
                    ui = UserInfoProvider.GetUserInfo(ci.CustomerUserID);
                    newCart.User = ui;
                }
                //if (ui == null)
                //{
                //    ui = CMSContext.GlobalPublicUser;
                //}
                  //newCart.UserInfoObj = ui;
                newCart.ShoppingCartCustomerID = customerId;
            }
        }

        return newCart;
    }


    protected override void OnPreInit(EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerid", 0);
        CustomerID = customerId;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            ShoppingCart = GetNewCart();
        }

        Cart.LocalShoppingCart = ShoppingCart;
        Cart.EnableProductPriceDetail = true;
        Cart.OnPaymentCompleted += new EventHandler(Cart_OnPaymentCompleted);
        Cart.OnPaymentSkipped += new EventHandler(Cart_OnPaymentSkipped);
        Cart.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(Cart_OnCheckPermissions);
        Cart.RequiredFieldsMark = CMS.EcommerceProvider.ShoppingCart.DEFAULT_REQUIRED_FIELDS_MARK;

        if (customerId > 0)
        {
            Cart.CheckoutProcessType = CheckoutProcessEnum.CMSDeskCustomer;
            AddMenuButtonSelectScript("Customers", "");
        }
        else
        {
            Cart.CheckoutProcessType = CheckoutProcessEnum.CMSDeskOrder;
        }
    }


    private void Cart_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check ecommerce permission
        if (!ECommerceContext.IsUserAuthorizedForPermission(permissionType))
        {
            string message = permissionType;
            if (permissionType.ToLowerCSafe().StartsWithCSafe("modify"))
            {
                message = "EcommerceModify OR " + message;
            }

            RedirectToAccessDenied("CMS.Ecommerce", message);
        }
    }


    private void Cart_OnPaymentSkipped(object sender, EventArgs e)
    {
        URLHelper.Redirect("~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit.aspx?orderID=" + ShoppingCart.OrderId.ToString() + "&customerid=" + customerId);
    }


    private void Cart_OnPaymentCompleted(object sender, EventArgs e)
    {
        URLHelper.Redirect("~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit.aspx?orderID=" + ShoppingCart.OrderId.ToString() + "&customerid=" + customerId);
    }


    protected void Page_Prerender()
    {
        int customerId = QueryHelper.GetInteger("customerId", 0);

        // For all steps
        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("Order_New.Orders");
        pageTitleTabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_List.aspx";
        if (customerId != 0)
        {
            pageTitleTabs[0, 1] += "?customerId=" + customerId.ToString();
        }
        pageTitleTabs[0, 2] = "";
        pageTitleTabs[1, 0] = GetString("Order_New.HeaderCaption");
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "order_step_1";
        if (this.customerId <= 0)
        {
            CurrentMaster.Title.TitleText = GetString("Order_New.HeaderCaption");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Ecommerce_Order/new.png");
        }
    }
}