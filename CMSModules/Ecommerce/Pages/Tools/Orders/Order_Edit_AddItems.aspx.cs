using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.Items")]
[Title("Objects/Ecommerce_OrderItem/new.png", "Order_Edit_AddItems.Title", "addorderitems")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_AddItems : CMSOrdersModalPage
{
    #region "Variables"

    private ShoppingCartInfo mShoppingCartObj = null;
    private bool allowGlobalProducts = false;
    private Dictionary<int, TextBox> quantityControls = new Dictionary<int, TextBox>();

    #endregion


    #region "Properties"

    /// <summary>
    /// Shopping cart object with order data.
    /// </summary>
    protected ShoppingCartInfo ShoppingCartObj
    {
        get
        {
            if (mShoppingCartObj == null)
            {
                string cartSessionName = QueryHelper.GetString("cart", "");
                if (cartSessionName != "")
                {
                    mShoppingCartObj = SessionHelper.GetValue(cartSessionName) as ShoppingCartInfo;
                }
            }
            return mShoppingCartObj;
        }
    }


    /// <summary>
    /// Shopping cart items selector SKU ID.
    /// </summary>
    private int SKUID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["SKUID"], 0);
        }
        set
        {
            ViewState["SKUID"] = value;
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnPreInit(EventArgs e)
    {
        // Get customer ID
        CustomerID = QueryHelper.GetInteger("customerid", 0);

        // Check whether global products are allowed
        allowGlobalProducts = ECommerceSettings.AllowGlobalProducts(CMSContext.CurrentSiteName);

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Init labels
        btnAdd.Text = GetString("Order_Edit_AddItems.Add");
        btnShow.Text = GetString("general.show");

        if (CultureHelper.IsUICultureRTL())
        {
            CSSHelper.RegisterCSSBlock(this, ".AddToCartContainer { text-align: left; }");
        }
        else
        {
            CSSHelper.RegisterCSSBlock(this, ".AddToCartContainer { text-align: right; }");
        }

        if (!RequestHelper.IsPostBack())
        {
            SwitchToProducts();
        }

        // Display global departments when using global products
        if (allowGlobalProducts)
        {
            departmentElem.DisplayGlobalItems = true;
        }

        bool isAdmin = (CMSContext.CurrentUser != null) && (CMSContext.CurrentUser.IsGlobalAdministrator);

        InitializeGrid();

        // Initialize shopping cart item selector
        cartItemSelector.SKUID = SKUID;
        cartItemSelector.ShoppingCart = ShoppingCartObj;
        cartItemSelector.OnAddToShoppingCart += new CancelEventHandler(cartItemSelector_OnAddToShoppingCart);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (plcSelector.Visible)
        {
            // Get product name and price
            SKUInfo skuObj = SKUInfoProvider.GetSKUInfo(SKUID);
            string skuName = "";

            if (skuObj != null)
            {
                lblPriceValue.Text = SKUInfoProvider.GetSKUFormattedPrice(skuObj, ShoppingCartObj, true, false);
                skuName = ResHelper.LocalizeString(skuObj.SKUName);
            }

            // Set SKU name label
            lblSKUName.Text = skuName;

            // Show info text
            lblPrice.Text = GetString("Order_Edit_AddItems.UnitPrice");

            // Initializes page title control		
            string[,] tabs = new string[2, 3];
            tabs[0, 0] = GetString("Order_Edit_AddItems.Products");
            tabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit_AddItems.aspx?cart=" + HTMLHelper.HTMLEncode(QueryHelper.GetString("cart", ""));
            tabs[0, 2] = "";
            tabs[1, 0] = skuName;
            tabs[1, 1] = "";
            tabs[1, 2] = "";
            CurrentMaster.Title.Breadcrumbs = tabs;
        }
    }

    #endregion


    #region "Event handlers"

    private DataSet gridProducts_OnAfterRetrieveData(DataSet ds)
    {
        // Clear dictionary containing textboxes for entering quantities
        quantityControls.Clear();

        // Disable add button when no data found
        btnAdd.Enabled = !DataHelper.DataSourceIsEmpty(ds);

        // No manipulation with data
        return ds;
    }


    private object gridProducts_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = null;

        switch (sourceName.ToLowerCSafe())
        {
            case "skuname":
                row = (DataRowView)parameter;
                string skuName = ValidationHelper.GetString(row["SKUName"], "");
                int skuId = ValidationHelper.GetInteger(row["SKUID"], 0);

                // Create link for adding one item using product name
                LinkButton link = new LinkButton();
                link.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(skuName));
                link.Click += new EventHandler(btnAddOneUnit_Click);
                link.CommandArgument = skuId.ToString();

                return link;

            case "price":
                // Format product price
                row = (DataRowView)parameter;
                return SKUInfoProvider.GetSKUFormattedPrice(new SKUInfo(row.Row), ShoppingCartObj, true, false);

            case "quantity":
                int id = ValidationHelper.GetInteger(parameter, 0);

                // Create textbox for entering quantity
                TextBox tb = new TextBox();
                tb.MaxLength = 9;
                tb.Width = 50;
                tb.ID = "txtQuantity" + id;

                // Add textbox to dictionary under SKUID key
                quantityControls.Add(id, tb);

                return tb;
        }

        return parameter;
    }


    private void btnAddOneUnit_Click(object sender, EventArgs e)
    {
        // Get SKU ID
        int skuId = ValidationHelper.GetInteger(((LinkButton)sender).CommandArgument, 0);

        // Get product
        SKUInfo product = SKUInfoProvider.GetSKUInfo(skuId);

        bool hasProductOptions = !DataHelper.DataSourceIsEmpty(OptionCategoryInfoProvider.GetSKUOptionCategories(skuId, true));
        bool isCustomizableDonation = ((product != null) && (product.SKUProductType == SKUProductTypeEnum.Donation))
                                      && (product.SKUPrivateDonation || !((product.SKUMinPrice == product.SKUPrice) && (product.SKUMaxPrice == product.SKUPrice)) || (product.SKUPrice == 0.0));

        // If product has product options or product is a customizable donation
        // -> abort inserting products to the shopping cart
        if (hasProductOptions || isCustomizableDonation)
        {
            // Set title message per specific case
            if (hasProductOptions && isCustomizableDonation)
            {
                lblTitle.ResourceString = "order_edit_additems.donationpropertiesproductoptions";
            }
            else if (hasProductOptions)
            {
                lblTitle.ResourceString = "order_edit_additems.productoptions";
            }
            else if (isCustomizableDonation)
            {
                lblTitle.ResourceString = "order_edit_additems.donationproperties";
            }

            // Save SKU ID to the viewstate
            SKUID = skuId;

            // Initialize shopping cart item selector
            cartItemSelector.SKUID = SKUID;
            cartItemSelector.ShowProductOptions = hasProductOptions;
            cartItemSelector.ShowDonationProperties = isCustomizableDonation;
            cartItemSelector.ReloadData();

            SwitchToOptions();
        }
        else
        {
            // Add product to shopping cart and close dialog window
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "addproduct", ScriptHelper.GetScript("AddProducts(" + skuId + ", 1);"));
        }
    }


    private void cartItemSelector_OnAddToShoppingCart(object sender, CancelEventArgs e)
    {
        // Get items parameters
        ShoppingCartItemParameters cartItemParams = cartItemSelector.GetShoppingCartItemParameters();

        if (ShoppingCartObj != null)
        {
            // Add item to shopping cart
            ShoppingCartItemInfo addedItem = ShoppingCartObj.SetShoppingCartItem(cartItemParams);
        }

        // Close dialog window and refresh content
        string script = "RefreshCart();";
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "addScript", ScriptHelper.GetScript(script));

        // Cancel further processing by shopping cart item selector control
        e.Cancel = true;
    }


    /// <summary>
    /// On BtnAdd click event.
    /// </summary>
    protected void BtnAdd_Click(object sender, EventArgs e)
    {
        // Check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }

        string allUnits = null;
        string allSkuId = null;

        foreach (KeyValuePair<int, TextBox> item in quantityControls)
        {
            // Get params
            int skuId = item.Key;
            TextBox txtUnits = item.Value;

            // Get unit count
            int units = ValidationHelper.GetInteger(txtUnits.Text, 0);

            if (units > 0)
            {
                // Get product and localized name
                SKUInfo product = SKUInfoProvider.GetSKUInfo(skuId);
                if (product != null)
                {
                    string skuName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(product.SKUName));

                    // Abort inserting products to the shopping cart ifIf product has some product options
                    if (!DataHelper.DataSourceIsEmpty(OptionCategoryInfoProvider.GetSKUOptionCategories(skuId, true)))
                    {
                        // Show error message
                        ShowError(string.Format(GetString("Order_Edit_AddItems.ProductOptionsRequired"), skuName));

                        return;
                    }

                    // If selected product is a donation
                    if ((product != null) && (product.SKUProductType == SKUProductTypeEnum.Donation))
                    {
                        // If donation is customizable
                        if (product.SKUPrivateDonation || !((product.SKUMinPrice == product.SKUPrice) && (product.SKUMaxPrice == product.SKUPrice)))
                        {
                            // Show error message
                            ShowError(string.Format(GetString("order_edit_additems.donationpropertiesrequired"), skuName));

                            return;
                        }
                    }

                    // Create strings with SKU IDs and units separated by ';'
                    allSkuId += skuId.ToString() + ";";
                    allUnits += units.ToString() + ";";
                }
            }
        }

        // Close this modal window and refresh parent values in window
        CloseWindow(allSkuId, allUnits);
    }

    #endregion


    #region "Methods"

    private void SwitchToProducts()
    {
        // Display products
        plcProducts.Visible = true;
        CurrentMaster.FooterContainer.Visible = true;

        // Hide shopping cart item selector
        plcSelector.Visible = false;
    }


    private void SwitchToOptions()
    {
        // Hide products
        plcProducts.Visible = false;
        CurrentMaster.FooterContainer.Visible = false;

        // Show shopping cart item selector
        plcSelector.Visible = true;
    }


    /// <summary>
    /// Grid initialization.
    /// </summary>
    protected void InitializeGrid()
    {
        string productNameFilter = txtProductName.Text.Trim().Replace("'", "''");
        string productCodeFilter = txtProductCode.Text.Trim().Replace("'", "''");

        // to display ONLY product - not product options
        string where = "(SKUEnabled = 1) AND (SKUOptionCategoryID IS NULL)";

        if (productNameFilter != "")
        {
            where += " AND (SKUName LIKE '%" + productNameFilter + "%')";
        }
        if (productCodeFilter != "")
        {
            where += " AND (SKUNumber LIKE '%" + productCodeFilter + "%')";
        }
        if (departmentElem.DepartmentID > 0)
        {
            where += " AND (SKUDepartmentID = " + departmentElem.DepartmentID + ")";
        }

        // Products for current site
        where += " AND (SKUSiteID = " + CMSContext.CurrentSiteID;

        // Global products when allowed
        if (allowGlobalProducts)
        {
            where += " OR SKUSiteID IS NULL";
        }

        where += ")";

        gridProducts.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridProducts_OnExternalDataBound);
        gridProducts.OnAfterRetrieveData += new OnAfterRetrieveData(gridProducts_OnAfterRetrieveData);
        gridProducts.ShowActionsMenu = false;
        gridProducts.WhereClause = where;
    }


    /// <summary>
    /// Generates script that closes the window and refreshes the parent page.
    /// </summary>
    /// <param name="skuIds">String with SKU IDs separated by ';'</param>
    /// <param name="units">String with SKU units separated by ';'</param>
    private void CloseWindow(string skuIds, string units)
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "addproductClose", ScriptHelper.GetScript("AddProducts('" + skuIds + "','" + units + "');"));
    }

    #endregion
}