using System;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

/// <summary>
/// Enables the user to specify filter criteria to filter the order list.
/// </summary>
public partial class CMSModules_Ecommerce_Controls_Filters_OrderFilter : CMSAbstractDataFilterControl
{
    #region "Private properties"

    /// <summary>
    /// Gets the customer identifier from the context.
    /// </summary>
    /// <remarks>
    /// The presence of the customer identifier affects the control's appearance.
    /// </remarks>
    private int FilterCustomerID
    {
        get
        {
            return QueryHelper.GetInteger("customerId", 0);
        }
    }


    /// <summary>
    /// Gets the site identifier for filtering.
    /// </summary>
    /// <remarks>
    /// The site identifier depends on whether the site filter is enabled.
    /// </remarks>
    private int FilterSiteID
    {
        get
        {
            if (SiteFilterEnabled)
            {
                return siteSelector.SiteID;
            }

            return CMSContext.CurrentSiteID;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether filtering by site is enabled.
    /// </summary>
    private bool SiteFilterEnabled
    {
        get
        {
            return plcSiteFilter.Visible;
        }
        set
        {
            plcSiteFilter.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether filtering by customer's properties is enabled.
    /// </summary>
    private bool CustomerFilterEnabled
    {
        get
        {
            return plcCustomerFilter.Visible;
        }
        set
        {
            plcCustomerFilter.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the associated UniGrid control displays a column with the order site's name.
    /// </summary>
    private bool SiteColumnEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["SiteColumnEnabled"],false);
        }
        set
        {
            ViewState["SiteColumnEnabled"] = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the advanced filter is displayed or not. 
    /// </summary>
    private bool AdvancedFilterEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["AdvancedFilterEnabled"],false);
        }
        set
        {
            ViewState["AdvancedFilterEnabled"] = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the SQL condition for filtering the order list.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetFilterWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Retrieve customer from the database
        CustomerInfo customer = CustomerInfoProvider.GetCustomerInfo(FilterCustomerID);

        // If a valid customer is specified, disable filtering by customer's properties
        if (customer != null)
        {
            CustomerFilterEnabled = false;
        }

        // If the current user is a global administrator, he or she is allowed to filter by site, otherwise, disable filtering by site
        if ((CMSContext.CurrentUser != null) && CMSContext.CurrentUser.IsGlobalAdministrator)
        {
            // If a valid customer is specified and he or she is assciated with a CMS user, display only sites that the user can access, otherwise, disable filtering by site
            if (customer != null)
            {
                if (customer.CustomerUserID > 0)
                {
                    siteSelector.UserId = customer.CustomerUserID;
                }
                else
                {
                    SiteFilterEnabled = false;
                }
            }
        }
        else
        {
            SiteFilterEnabled = false;
        }

        // Initialize the control with values All, Yes and No
        InitializeThreeStateDropDownList(drpOrderIsPaid);

        // If this control is associated with an UniGrid control, disable UniGrid's buttons and initialize the Reset button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.HideFilterButton = true;
            // Reset button is available only when UniGrid remembers its state
            if (grid.RememberState)
            {
                btnReset.Text = GetString("general.reset");
                btnReset.Click += btnReset_Click;
            }
            else
            {
                btnReset.Visible = false;
            }
        }

        // Initialize the Show button
        btnFilter.Text = GetString("general.show");
        btnFilter.Click += btnFilter_Click;

        // Assign images 
        imgArrowUp.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/SortDown.png");
        imgArrowDown.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/SortUp.png");

        // Hide unwanted elements
        plcAdvancedGroup.Visible = false;
        plcSimpleFilter.Visible = false;

        // Initialize site selector
        siteSelector.DropDownSingleSelect.SelectedIndexChanged += new EventHandler(DropDownSingleSelect_SelectedIndexChanged);
        CurrencySelector.SiteID = statusSelector.SiteID;
        
        // Preselect current siteID by default when siteSelector is enabled
        if (!URLHelper.IsPostback())
        {
            siteSelector.SiteID = CMSContext.CurrentSiteID;
        } 
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // As the site selector might have changed its value, it is neccessary to refresh dependent controls
        RefreshStatusSelector();
        ReloadUniselectors();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // If the site column is not enabled, update the associated UniGrid
        if (!SiteColumnEnabled)
        {
            UniGrid grid = FilteredControl as UniGrid;
            if (grid != null && grid.NamedColumns.ContainsKey("OrderSiteID"))
            {
                grid.NamedColumns["OrderSiteID"].Visible = false;
            }
        }

        AdvandcedFilterShowHide();
    }

    #endregion


    #region "Event handlers"

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            SiteColumnEnabled = (FilterSiteID < 1);
            grid.ApplyFilter(sender, e);
        }
    }


    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            SiteColumnEnabled = (FilterSiteID < 1);
            grid.Reset();
        }
    }


    void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetSelectors();
        ReloadUniselectors();
    }


    protected void btnAdvancedFilter_Click(object sender, EventArgs e)
    {
        AdvancedFilterEnabled = !AdvancedFilterEnabled;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the specified controls with values All, Yes and No.
    /// </summary>
    /// <param name="control">A control to initialize.</param>
    private void InitializeThreeStateDropDownList(DropDownList control)
    {
        control.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        control.Items.Add(new ListItem(GetString("general.yes"), "1"));
        control.Items.Add(new ListItem(GetString("general.no"), "0"));
    }


    /// <summary>
    /// Reloads the order status selector if it does not match the current site for filtering
    /// </summary>
    private void RefreshStatusSelector()
    {
        if (statusSelector.SiteID != FilterSiteID)
        {
            statusSelector.SiteID = FilterSiteID;
            statusSelector.Reload();
        }
    }


    /// <summary>
    /// Builds a SQL condition for filtering the order list, and returns it.
    /// </summary>
    /// <returns>A SQL condition for filtering the order list.</returns>
    private string GetFilterWhereCondition()
    {
        string condition = String.Empty;

        // Order status
        if (statusSelector.OrderStatusID > 0)
        {
            condition = SqlHelperClass.AddWhereCondition(condition, String.Format("OrderStatusID = {0}", statusSelector.OrderStatusID));
        }

        // Order site
        if (FilterSiteID > 0)
        {
            condition = SqlHelperClass.AddWhereCondition(condition, String.Format("OrderSiteID = {0}", FilterSiteID));
        }

        // Order identifier
        string filterOrderId = txtOrderId.Text.Trim();
        if (filterOrderId != String.Empty)
        {
            int orderId = ValidationHelper.GetInteger(filterOrderId, 0);
            // Include also an invoice number
            condition = SqlHelperClass.AddWhereCondition(condition, String.Format("OrderID = {0} OR OrderInvoiceNumber LIKE N'%{1}%'", orderId, SqlHelperClass.GetSafeQueryString(filterOrderId, false)));
        }

        // Customer's properties, applicable only if a valid customer is not specified
        if (CustomerFilterEnabled)
        {
            // First, Last name and Company search
            string customerNameOrCompany = txtCustomerNameOrCompany.Text.Trim();
            if (customerNameOrCompany != String.Empty)
            {
                condition = SqlHelperClass.AddWhereCondition(condition, String.Format("(CustomerFirstName LIKE N'%{0}%') OR (CustomerLastName LIKE N'%{0}%') OR (CustomerCompany LIKE N'%{0}%')", SqlHelperClass.GetSafeQueryString(customerNameOrCompany, false)));
            }
        }

        // Order is paid
        switch (drpOrderIsPaid.SelectedValue)
        {
            case "0":
                condition = SqlHelperClass.AddWhereCondition(condition, "ISNULL(OrderIsPaid, 0) = 0");
                break;
            case "1":
                condition = SqlHelperClass.AddWhereCondition(condition, "ISNULL(OrderIsPaid, 0) = 1");
                break;
        }

        // Advanced Filter
        if (AdvancedFilterEnabled)
        {
            // Specific currecny was selected
            if (CurrencySelector.CurrencyID > 0)
            {
                condition = SqlHelperClass.AddWhereCondition(condition, "OrderCurrencyID = " + CurrencySelector.CurrencyID);
            }

            // Specific payment method was selected
            if (PaymentSelector.PaymentID > 0)
            {
                condition = SqlHelperClass.AddWhereCondition(condition, "OrderPaymentOptionID = " + PaymentSelector.PaymentID);
            }

            // Specific shipping option was selected
            if (ShippingSelector.ShippingID > 0)
            {
                condition = SqlHelperClass.AddWhereCondition(condition, "OrderShippingOptionID = " + ShippingSelector.ShippingID);
            }

            // Specific tracking number was provided
            if (txtTrackingNumber.Text != string.Empty)
            {
                condition = SqlHelperClass.AddWhereCondition(condition, "OrderTrackingNumber LIKE '%" + SqlHelperClass.GetSafeQueryString(txtTrackingNumber.Text) + "%'");
            }

            // Specific created date was selected
            if ((dtpCreatedTo.SelectedDateTime < dtpCreatedTo.MaxDate) && (dtpCreatedTo.SelectedDateTime > dtpCreatedTo.MinDate))
            {
                condition = SqlHelperClass.AddWhereCondition(condition, "OrderDate <= '" + CMS.DataEngine.TableManager.GetDateTimeString(dtpCreatedTo.SelectedDateTime) + "'");
            }

            // Specific from date was selected
            if ((dtpFrom.SelectedDateTime < dtpFrom.MaxDate) && (dtpFrom.SelectedDateTime > dtpFrom.MinDate))
            {
                condition = SqlHelperClass.AddWhereCondition(condition, "OrderDate >= '" + CMS.DataEngine.TableManager.GetDateTimeString(dtpFrom.SelectedDateTime) + "'");
            }
        }

        return condition;
    }


    /// <summary>
    /// Assigns FilterSiteID to all selectors and reloads them
    /// </summary>
    private void ReloadUniselectors()
    {
        CurrencySelector.SiteID = FilterSiteID;
        PaymentSelector.SiteID = FilterSiteID;
        ShippingSelector.SiteID = FilterSiteID;

        RefreshStatusSelector();
        CurrencySelector.ReloadData();
        PaymentSelector.ReloadData();
        ShippingSelector.Reload(true);
    }


    /// <summary>
    /// Resets selectors.
    /// </summary>
    private void ResetSelectors()
    {
        CurrencySelector.CurrencyID = -1;
        PaymentSelector.PaymentID = -1;
        ShippingSelector.ShippingID = -1;
        statusSelector.OrderStatusID = -1;
        drpOrderIsPaid.SelectedIndex = 0;
    }


    /// <summary>
    /// Change visibility of the filter
    /// </summary>
    private void AdvandcedFilterShowHide()
    {
        plcAdvancedFilter.Visible = !AdvancedFilterEnabled;
        plcSimpleFilter.Visible = AdvancedFilterEnabled;

        plcAdvancedGroup.Visible = AdvancedFilterEnabled;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);

        // Store additional state properties
        state.AddValue("SiteColumnEnabled", SiteColumnEnabled);
        state.AddValue("AdvancedFilterEnabled", AdvancedFilterEnabled);
        state.AddValue("dtpFrom", dtpFrom.SelectedDateTime);
        state.AddValue("dtpCreatedTo", dtpCreatedTo.SelectedDateTime);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);

        // Restore additional state properties
        SiteColumnEnabled = state.GetBoolean("SiteColumnEnabled");
        AdvancedFilterEnabled = state.GetBoolean("AdvancedFilterEnabled");
        dtpFrom.SelectedDateTime = state.GetDateTime("dtpFrom");
        dtpCreatedTo.SelectedDateTime = state.GetDateTime("dtpCreatedTo");
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtOrderId.Text = String.Empty;
        txtCustomerNameOrCompany.Text = String.Empty;
        statusSelector.Value = null;
        siteSelector.SiteID = CMSContext.CurrentSiteID;
        siteSelector.Reload(true);
        dtpFrom.DateTimeTextBox.Text = string.Empty;
        dtpCreatedTo.DateTimeTextBox.Text = string.Empty;
        txtTrackingNumber.Text = string.Empty;
        ResetSelectors();
        ReloadUniselectors();
    }

    #endregion
}