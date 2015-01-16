using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.Shipping")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Shipping : CMSOrdersPage
{
    protected int orderId = 0;
    protected int customerId = 0;
    protected ShoppingCartInfo mShoppingCartInfoObj = null;


    private ShoppingCartInfo ShoppingCartInfoObj
    {
        get
        {
            if (mShoppingCartInfoObj == null)
            {
                mShoppingCartInfoObj = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(orderId);
            }

            return mShoppingCartInfoObj;
        }
    }


    protected override void OnPreInit(EventArgs e)
    {
        CustomerID = QueryHelper.GetInteger("customerid", 0);
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AddressChange", ScriptHelper.GetScript("   function AddressChange(AddressId) { if (AddressId > 0) { document.getElementById('" + hdnAddress.ClientID + "').value = AddressId; " + ClientScript.GetPostBackEventReference(btnChange, "") + " } } "));

        string urlsScript = string.Format("var addressEditUrl = {0};", ScriptHelper.GetString(URLHelper.ResolveUrl("Order_Edit_Address.aspx")));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DialogUrls", ScriptHelper.GetScript(urlsScript));

        lblAddress.Text = GetString("Order_Edit_Shipping.lblAddress");
        lblOption.Text = GetString("Order_Edit_Shipping.lblOption");
        lblTrackingNumber.Text = GetString("order_edit.lblTrackingNumber");

        btnEdit.Text = GetString("general.edit");
        btnNew.Text = GetString("general.new");

        btnOk.Click += new EventHandler(btnOk_Click);
        addressElem.DropDownSingleSelect.SelectedIndexChanged += new EventHandler(DropDownSingleSelect_SelectedIndexChanged);
        addressElem.DropDownSingleSelect.AutoPostBack = true;

        orderId = QueryHelper.GetInteger("orderId", 0);
        LoadData();

        btnNew.OnClientClick = "AddAddress('" + customerId + "'); return false;";
        btnEdit.OnClientClick = "EditAddress('" + customerId + "','" + addressElem.AddressID + "'); return false;";
    }


    protected void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        hdnAddress.Value = addressElem.AddressID.ToString();
        // Enable edit address only if address selected
        btnEdit.Enabled = addressElem.AddressID != 0;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }

        // Get order
        OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);
        if (oi != null)
        {
            // Are taxes based on shipping address?
            bool taxesBasedOnShipping = false;
            SiteInfo si = SiteInfoProvider.GetSiteInfo(oi.OrderSiteID);
            if (si != null)
            {
                taxesBasedOnShipping = (ECommerceSettings.ApplyTaxesBasedOn(si.SiteName) == ApplyTaxBasedOnEnum.ShippingAddress);
            }

            // Is shipping needed?
            bool isShippingNeeded = ((ShoppingCartInfoObj != null) && (ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCartInfoObj)));

            // If shipping address is required
            if (isShippingNeeded || taxesBasedOnShipping)
            {
                // If shipping address is not set
                if (addressElem.AddressID <= 0)
                {
                    // Show error message
                    ShowError(GetString("Order_Edit_Shipping.NoAddress"));

                    return;
                }
            }

            try
            {
                // Load the shopping cart to process the data
                ShoppingCartInfo sci = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(orderId);
                if (sci != null)
                {
                    // Set order new properties
                    sci.ShoppingCartShippingOptionID = drpShippingOption.ShippingID;
                    sci.ShoppingCartShippingAddressID = addressElem.AddressID;

                    // Evaluate order data
                    ShoppingCartInfoProvider.EvaluateShoppingCart(sci);

                    // Update order data
                    ShoppingCartInfoProvider.SetOrder(sci, false);
                }

                // Update tracking number
                oi.OrderTrackingNumber = txtTrackingNumber.Text.Trim();
                OrderInfoProvider.SetOrderInfo(oi);

                // Show message
                ShowChangesSaved();

                // Update shipping charge in selector
                if (taxesBasedOnShipping)
                {
                    drpShippingOption.ShoppingCart = sci;
                    drpShippingOption.Reload(false);
                }
            }
            catch (Exception ex)
            {
                // Show error message
                ShowError(ex.Message);
            }
        }
    }


    private void LoadData()
    {
        OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);
        EditedObject = oi;

        if (oi != null)
        {
            // Check order site ID
            CheckOrderSiteID(oi.OrderSiteID);

            customerId = oi.OrderCustomerID;
            addressElem.CustomerID = customerId;
            drpShippingOption.ShoppingCart = ShoppingCartInfoObj;

            if (!RequestHelper.IsPostBack())
            {
                txtTrackingNumber.Text = oi.OrderTrackingNumber;
                addressElem.CustomerID = customerId;
                addressElem.AddressID = oi.OrderShippingAddressID;
                drpShippingOption.ShippingID = oi.OrderShippingOptionID;
            }
        }

        // Enable edit address only if address selected
        btnEdit.Enabled = addressElem.AddressID != 0;

        // If order is paid
        if ((oi != null) && (oi.OrderIsPaid))
        {
            // Disable shipping option edit
            drpShippingOption.Enabled = false;

            // Get site information
            SiteInfo si = SiteInfoProvider.GetSiteInfo(oi.OrderSiteID);

            // If tax is based on shipping address
            if ((si != null) && (ECommerceSettings.ApplyTaxesBasedOn(si.SiteName) == ApplyTaxBasedOnEnum.ShippingAddress))
            {
                // Disable shipping address edit
                addressElem.Enabled = false;
                btnEdit.Enabled = false;
                btnNew.Enabled = false;
            }
        }
    }
}