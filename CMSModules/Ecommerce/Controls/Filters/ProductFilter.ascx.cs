using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_Controls_Filters_ProductFilter : CMSAbstractDataFilterControl
{
    #region "Variables"

    private bool mShowPublicStatusFilter = true;
    private bool mShowManufacturerFilter = true;
    private bool mShowPagingFilter = true;
    private bool mShowStockFilter = true;
    private bool mShowSortingFilter = true;
    private bool? mUsingGlobalObjects = null;
    protected CMSButton button;

    #endregion


    #region "Properties"

    /// <summary>
    /// Show public status filter.
    /// </summary>
    public bool ShowPublicStatusFilter
    {
        get
        {
            return mShowPublicStatusFilter;
        }
        set
        {
            mShowPublicStatusFilter = value;
        }
    }


    /// <summary>
    /// Show manufacturer filter.
    /// </summary>
    public bool ShowManufacturerFilter
    {
        get
        {
            return mShowManufacturerFilter;
        }
        set
        {
            mShowManufacturerFilter = value;
        }
    }


    /// <summary>
    /// Show paging filter.
    /// </summary>
    public bool ShowPagingFilter
    {
        get
        {
            return mShowPagingFilter;
        }
        set
        {
            mShowPagingFilter = value;
        }
    }


    /// <summary>
    /// Show stock filter.
    /// </summary>
    public bool ShowStockFilter
    {
        get
        {
            return mShowStockFilter;
        }
        set
        {
            mShowStockFilter = value;
        }
    }


    /// <summary>
    /// Show sorting filter.
    /// </summary>
    public bool ShowSortingFilter
    {
        get
        {
            return mShowSortingFilter;
        }
        set
        {
            mShowSortingFilter = value;
        }
    }


    /// <summary>
    /// Show search filter.
    /// </summary>
    public bool ShowSearchFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Paging filter options (values separated by comma).
    /// </summary>
    public string PagingOptions
    {
        get;
        set;
    }


    /// <summary>
    /// Filter by query parameters.
    /// </summary>
    public bool FilterByQuery
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the site for which selectors are to be loaded. Uses current site name when empty or null.
    /// </summary>
    public override string SiteName
    {
        get
        {
            return base.SiteName;
        }
        set
        {
            base.SiteName = DataHelper.GetNotEmpty(value, CMSContext.CurrentSiteName);
            int siteId = SiteInfoProvider.GetSiteID(SiteName);

            // Clear flag
            mUsingGlobalObjects = null;

            // Propagate siteId to selectors
            manufacturerSelector.SiteID = siteId;
            statusSelector.SiteID = siteId;
            manufacturerSelector.DisplayGlobalItems = UsingGlobalObjects;
            statusSelector.DisplayGlobalItems = UsingGlobalObjects;
        }
    }


    /// <summary>
    /// Returns true if site given by SiteID uses global products.
    /// </summary>
    protected bool UsingGlobalObjects
    {
        get
        {
            // Unknown yet
            if (!mUsingGlobalObjects.HasValue)
            {
                mUsingGlobalObjects = false;
                // Try to figure out from settings
                string siteName = SiteName;
                if (string.IsNullOrEmpty(siteName))
                {
                    siteName = CMSContext.CurrentSiteName;
                }

                mUsingGlobalObjects = ECommerceSettings.AllowGlobalProducts(siteName);
            }

            return mUsingGlobalObjects.Value;
        }
    }

    #endregion


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        if (FilterByQuery)
        {
            // Handle all query parameters
            string url = URLHelper.RawUrl;

            url = URLHelper.RemoveParameterFromUrl(url, "statusid");
            if (Convert.ToInt32(statusSelector.Value) > 0)
            {
                url = URLHelper.AddParameterToUrl(url, "statusid", statusSelector.Value.ToString());
            }

            url = URLHelper.RemoveParameterFromUrl(url, "manufacturerid");
            if (Convert.ToInt32(manufacturerSelector.Value) > 0)
            {
                url = URLHelper.AddParameterToUrl(url, "manufacturerid", manufacturerSelector.Value.ToString());
            }

            url = URLHelper.RemoveParameterFromUrl(url, "available");
            if (chkStock.Checked)
            {
                url = URLHelper.AddParameterToUrl(url, "available", "1");
            }

            url = URLHelper.RemoveParameterFromUrl(url, "pagesize");
            if (drpPaging.SelectedValue != string.Empty)
            {
                url = URLHelper.AddParameterToUrl(url, "pagesize", drpPaging.SelectedValue);
            }

            url = URLHelper.RemoveParameterFromUrl(url, "order");
            if (drpSort.SelectedValue != string.Empty)
            {
                url = URLHelper.AddParameterToUrl(url, "order", drpSort.SelectedValue);
            }

            url = URLHelper.RemoveParameterFromUrl(url, "search");
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                url = URLHelper.AddParameterToUrl(url, "search", txtSearch.Text);
            }

            // Redirect with new query parameters
            URLHelper.Redirect(url);
        }
    }


    private void InitControls()
    {
        lblManufacturer.Text = ResHelper.GetString("ecommerce.filter.product.manufacturer");
        manufacturerSelector.InnerControl.CssClass = "DropDownList";

        lblStatus.Text = ResHelper.GetString("ecommerce.filter.product.status");
        statusSelector.InnerControl.CssClass = "DropDownList";

        lblStatus.Text = ResHelper.GetString("ecommerce.filter.product.status");
        statusSelector.InnerControl.CssClass = "DropDownList";

        chkStock.Text = ResHelper.GetString("ecommerce.filter.product.stock");

        lblSort.Text = ResHelper.GetString("ecommerce.filter.product.sort");

        lblSearch.Text = ResHelper.GetString("ecommerce.filter.product.search");
    }


    private void SetControlsVisibility()
    {
        // Show/Hide manufacturer filter
        lblManufacturer.Visible = manufacturerSelector.Visible = ShowManufacturerFilter;

        // Show/Hide public status filter
        lblStatus.Visible = statusSelector.Visible = ShowPublicStatusFilter;

        // Show/Hide stock filter
        chkStock.Visible = ShowStockFilter;

        // Show/Hide paging filter
        lblPaging.Visible = drpPaging.Visible = ShowPagingFilter;

        // Show/Hide sorting filter
        lblSort.Visible = drpSort.Visible = ShowSortingFilter;

        // Show/Hide search filter
        lblSearch.Visible = txtSearch.Visible = ShowSearchFilter;
    }


    /// <summary>
    /// Setups the control.
    /// </summary>
    private void SetupControl()
    {
        if (!StopProcessing)
        {
            // Initialize labels
            InitControls();

            // Initialize visibility of filter fields
            SetControlsVisibility();

            // Dispsay filter rows
            bool firstRowVisible = (ShowSearchFilter || ShowPublicStatusFilter || ShowManufacturerFilter || ShowStockFilter);
            bool secondRowVisible = (ShowPagingFilter || ShowSortingFilter);

            plcFirstRow.Visible = firstRowVisible;
            plcFirstButton.Visible = (firstRowVisible && !secondRowVisible);
            plcSecondRow.Visible = secondRowVisible;
            plcSecButton.Visible = secondRowVisible;

            // Filter button
            button = secondRowVisible ? btnSecFilter : btnFirstFilter;
            button.Text = ResHelper.GetString("ecommerce.filter.product.filter");
            pnlContainer.DefaultButton = button.ID;

            if (!RequestHelper.IsPostBack())
            {
                // Initialize dropdowns
                drpSort.Items.Add(new ListItem(ResHelper.GetString("ecommerce.filter.product.nameasc"), "nameasc"));
                drpSort.Items.Add(new ListItem(ResHelper.GetString("ecommerce.filter.product.namedesc"), "namedesc"));
                drpSort.Items.Add(new ListItem(ResHelper.GetString("ecommerce.filter.product.priceasc"), "priceasc"));
                drpSort.Items.Add(new ListItem(ResHelper.GetString("ecommerce.filter.product.pricedesc"), "pricedesc"));

                drpPaging.Items.Add(new ListItem(ResHelper.GetString("General.SelectAll"), "0"));

                if ((PagingOptions != null) && (PagingOptions != string.Empty))
                {
                    // Parse PagingOptions string and fill dropdown
                    string[] pageOptions = PagingOptions.Split(',');
                    foreach (string pageOption in pageOptions)
                    {
                        if (pageOption.Trim() != string.Empty)
                        {
                            drpPaging.Items.Add(new ListItem(pageOption.Trim()));
                        }
                    }
                }
            }

            // Hide paging
            if (drpPaging.Items.Count > 0)
            {
                drpPaging.Visible = false;
                lblPaging.Visible = false;
            }

            // Section 508 validation
            lblManufacturer.AssociatedControlClientID = manufacturerSelector.ValueElementID;
            lblStatus.AssociatedControlClientID = statusSelector.ValueElementID;
            lblSearch.AssociatedControlClientID = txtSearch.ClientID;

            // Get filter setings from query parameter values
            if (FilterByQuery)
            {
                GetFilter();
            }
        }
        else
        {
            statusSelector.StopProcessing = true;
            manufacturerSelector.StopProcessing = true;
        }
    }


    private void SetFilter()
    {
        string where = string.Empty;
        int paging = 0;
        string order = null;

        if (FilterByQuery)
        {
            // Collect values from query string
            int status = QueryHelper.GetInteger("statusid", 0);
            int manufacturer = QueryHelper.GetInteger("manufacturerid", 0);
            bool stock = QueryHelper.GetBoolean("available", false);
            paging = QueryHelper.GetInteger("pagesize", 12);
            order = QueryHelper.GetString("order", string.Empty);
            string search = QueryHelper.GetString("search", string.Empty);

            if (status > 0)
            {
                // Add public status condition
                where = SqlHelperClass.AddWhereCondition(where, "SKUPublicStatusID = " + status);
            }
            if (manufacturer > 0)
            {
                // Add manufacturer condition
                where = SqlHelperClass.AddWhereCondition(where, "SKUManufacturerID = " + manufacturer);
            }
            if (stock)
            {
                // Add available items condition
                where = SqlHelperClass.AddWhereCondition(where, "SKUAvailableItems > 0");
            }
            if (!string.IsNullOrEmpty(search))
            {
                where = SqlHelperClass.AddWhereCondition(where, String.Format("SKUName LIKE N'%{0}%'", SqlHelperClass.GetSafeQueryString(search, false)));
            }

            // Prepare order by clause
            switch (order.ToLowerCSafe())
            {
                case "nameasc":
                    order = "SKUName";
                    break;

                case "namedesc":
                    order = "SKUName DESC";
                    break;

                case "priceasc":
                    order = "SKUPrice";
                    break;

                case "pricedesc":
                    order = "SKUPrice DESC";
                    break;

                default:
                    order = string.Empty;
                    break;
            }
        }
        else
        {
            // Build where condition according to dropdowns setings
            if (Convert.ToInt32(statusSelector.Value) > 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "SKUPublicStatusID = " + statusSelector.Value);

            }
            if (Convert.ToInt32(manufacturerSelector.Value) > 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "SKUManufacturerID = " + manufacturerSelector.Value);

            }
            if (chkStock.Checked)
            {
                where = SqlHelperClass.AddWhereCondition(where, "SKUAvailableItems > 0");
            }
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                where = SqlHelperClass.AddWhereCondition(where, String.Format("SKUName LIKE N'%{0}%'", SqlHelperClass.GetSafeQueryString(txtSearch.Text, false)));
            }

            // Process drpSort dropdown
            if (drpPaging.SelectedValue != string.Empty)
            {
                // If paging not visible, set -1 and dont change page settings at all
                paging = drpPaging.Visible ? ValidationHelper.GetInteger(drpPaging.SelectedValue, 12) : -1;
            }

            // Process drpSort dropdown
            if (drpSort.Visible && (drpSort.SelectedValue != string.Empty))
            {
                switch (drpSort.SelectedValue.ToLowerCSafe())
                {
                    case "nameasc":
                        order = "SKUName";
                        break;

                    case "namedesc":
                        order = "SKUName DESC";
                        break;

                    case "priceasc":
                        order = "SKUPrice";
                        break;

                    case "pricedesc":
                        order = "SKUPrice DESC";
                        break;
                }
            }
        }

        if (where != string.Empty)
        {
            // Set where condition
            WhereCondition = where;
        }
        if (order != string.Empty)
        {
            // Set orderBy condition
            OrderBy = order;
        }
        if (paging >= 0)
        {
            // Set paging
            PageSize = paging;
        }

        // Filter changed event
        RaiseOnFilterChanged();
    }


    private void GetFilter()
    {
        int status = QueryHelper.GetInteger("statusid", 0);
        int manufacturer = QueryHelper.GetInteger("manufacturerid", 0);
        bool stock = QueryHelper.GetBoolean("available", false);
        int paging = QueryHelper.GetInteger("pagesize", 12);
        string order = QueryHelper.GetString("order", string.Empty);
        string search = QueryHelper.GetString("search", string.Empty);

        // Set internal status if in query
        if (status > 0)
        {
            statusSelector.Value = status;
        }

        // Set manufacturer if in query
        if (manufacturer > 0)
        {
            manufacturerSelector.Value = manufacturer;
        }

        // Set search if in query
        if (search != string.Empty)
        {
            txtSearch.Text = search;
        }

        // Set only in stock if in query
        if (stock)
        {
            chkStock.Checked = true;
        }

        // Set paging if in query
        if (paging >= 0)
        {
            try
            {
                drpPaging.SelectedValue = paging.ToString();
            }
            catch
            {
            }
        }

        // Set order if in query
        if (order != string.Empty)
        {
            try
            {
                drpSort.SelectedValue = order;
            }
            catch
            {
            }
        }
    }


    /// <summary>
    /// Child control creation.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();
    }


    protected override void OnInit(EventArgs e)
    {
        // Init controls
        SetupControl();

        base.OnInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        if (!StopProcessing)
        {
            // Set filter only after postback or in case the filter is in a query mode
            if (RequestHelper.IsPostBack() || FilterByQuery)
            {
                // Apply the filter to the displayed data
                SetFilter();
            }
        }
        else
        {
            statusSelector.StopProcessing = true;
            manufacturerSelector.StopProcessing = true;
        }

        base.OnLoad(e);
    }
}