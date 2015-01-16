using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSWebParts_Ecommerce_Products_Products : CMSAbstractWebPart
{
    #region "Constants"

    private const string FILTER_ALL = "ALL";

    private const string PRODUCT_TYPE_PRODUCTS = "PRODUCTS";
    private const string PRODUCT_TYPE_PRODUCT_OPTIONS = "PRODUCTOPTIONS";

    private const string NEEDS_SHIPPING_YES = "YES";
    private const string NEEDS_SHIPPING_NO = "NO";

    private const string ALLOW_FOR_SALE_YES = "YES";
    private const string ALLOW_FOR_SALE_NO = "NO";

    private const string ORDER_BY_NAME_ASC = "Name ASC";
    private const string ORDER_BY_NAME_DESC = "Name DESC";
    private const string ORDER_BY_NUMBER_ASC = "Number ASC";
    private const string ORDER_BY_NUMBER_DESC = "Number DESC";
    private const string ORDER_BY_PRICE_ASC = "Price ASC";
    private const string ORDER_BY_PRICE_DESC = "Price DESC";
    private const string ORDER_BY_DATE_DESC = "Date DESC";
    private const string ORDER_BY_DATE_ASC = "Date ASC";
    private const string ORDER_BY_AVAILABLE_ITEMS_ASC = "AvailableItems ASC";
    private const string ORDER_BY_REORDER_DESC = "Reorder DESC";

    private const string COLUMN_NUMBER = "NUMBER";
    private const string COLUMN_PRICE = "PRICE";
    private const string COLUMN_DEPARTMENT = "DEPARTMENT";
    private const string COLUMN_MANUFACTURER = "MANUFACTURER";
    private const string COLUMN_SUPPLIER = "SUPPLIER";
    private const string COLUMN_PUBLIC_STATUS = "PUBLICSTATUS";
    private const string COLUMN_INTERNAL_STATUS = "INTERNALSTATUS";
    private const string COLUMN_REORDER_AT = "REORDERAT";
    private const string COLUMN_AVAILABLE_ITEMS = "AVAILABLEITEMS";
    private const string COLUMN_ITEMS_TO_BE_REORDERED = "ITEMSTOBEREORDERED";
    private const string COLUMN_ALLOW_FOR_SALE = "ALLOWFORSALE";

    #endregion


    #region "Properties"

    /// <summary>
    /// Include.
    /// </summary>
    public string ProductType
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ProductType"), "products");
        }
        set
        {
            this.SetValue("ProductType", value);
        }
    }


    /// <summary>
    /// Product type.
    /// </summary>
    public string Representing
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Representing"), "");
        }
        set
        {
            this.SetValue("Representing", value);
        }
    }


    /// <summary>
    /// Product number contains.
    /// </summary>
    public string ProductNumber
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ProductNumber"), "");
        }
        set
        {
            this.SetValue("ProductNumber", value);
        }
    }


    /// <summary>
    /// Department.
    /// </summary>
    public string Department
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Department"), "");
        }
        set
        {
            this.SetValue("Department", value);
        }
    }


    /// <summary>
    /// Manufacturer.
    /// </summary>
    public string Manufacturer
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Manufacturer"), "");
        }
        set
        {
            this.SetValue("Manufacturer", value);
        }
    }


    /// <summary>
    /// Supplier.
    /// </summary>
    public string Supplier
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Supplier"), "");
        }
        set
        {
            this.SetValue("Supplier", value);
        }
    }


    /// <summary>
    /// Needs shipping.
    /// </summary>
    public string NeedsShipping
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("NeedsShipping"), "all");
        }
        set
        {
            this.SetValue("NeedsShipping", value);
        }
    }


    /// <summary>
    /// Price from.
    /// </summary>
    public double PriceFrom
    {
        get
        {
            return ValidationHelper.GetDouble(this.GetValue("PriceFrom"), 0);
        }
        set
        {
            this.SetValue("PriceFrom", value);
        }
    }


    /// <summary>
    /// Price to.
    /// </summary>
    public double PriceTo
    {
        get
        {
            return ValidationHelper.GetDouble(this.GetValue("PriceTo"), 0);
        }
        set
        {
            this.SetValue("PriceTo", value);
        }
    }


    /// <summary>
    /// Public status.
    /// </summary>
    public string PublicStatus
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("PublicStatus"), "");
        }
        set
        {
            this.SetValue("PublicStatus", value);
        }
    }


    /// <summary>
    /// Internal status.
    /// </summary>
    public string InternalStatus
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("InternalStatus"), "");
        }
        set
        {
            this.SetValue("InternalStatus", value);
        }
    }


    /// <summary>
    /// Allow for sale.
    /// </summary>
    public string AllowForSale
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("AllowForSale"), "all");
        }
        set
        {
            this.SetValue("AllowForSale", value);
        }
    }


    /// <summary>
    /// Available items less than.
    /// </summary>
    public string AvailableItems
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("AvailableItems"), "");
        }
        set
        {
            this.SetValue("AvailableItems", value);
        }
    }


    /// <summary>
    /// Needs to be reordered.
    /// </summary>
    public bool NeedsToBeReordered
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("NeedsToBeReordered"), false);
        }
        set
        {
            this.SetValue("NeedsToBeReordered", value);
        }
    }


    /// <summary>
    /// Top N.
    /// </summary>
    public int TopN
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("TopN"), 0);
        }
        set
        {
            this.SetValue("TopN", value);
        }
    }


    /// <summary>
    /// Sort by.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("OrderBy"), "");
        }
        set
        {
            this.SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("PageSize"), 0);
        }
        set
        {
            this.SetValue("PageSize", value);
        }
    }


    /// <summary>
    /// Visible columns in listing.
    /// </summary>
    public string VisibleColumns
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("VisibleColumns"), "");
        }
        set
        {
            this.SetValue("VisibleColumns", value);
        }
    }


    /// <summary>
    /// Gets the messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        DisplayColumns();
    }

    #endregion


    #region "Event handlers"

    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = parameter as DataRowView;

        switch (sourceName.ToLowerCSafe())
        {
            case "optioncategory":
                int optionCategoryId = ValidationHelper.GetInteger(row["SKUOptionCategoryID"], 0);
                OptionCategoryInfo optionCategory = OptionCategoryInfoProvider.GetOptionCategoryInfo(optionCategoryId);

                // Return option category display name for product option or '-' for product
                if (optionCategory != null)
                {
                    return HTMLHelper.HTMLEncode(optionCategory.CategoryDisplayName ?? "");
                }
                return "-";

            case "skunumber":
                string skuNumber = ValidationHelper.GetString(row["SKUNumber"], null);
                return HTMLHelper.HTMLEncode(skuNumber ?? "");

            case "skuprice":
                double value = ValidationHelper.GetDouble(row["SKUPrice"], 0);
                int siteId = ValidationHelper.GetInteger(row["SKUSiteID"], 0);

                // Format price
                return CurrencyInfoProvider.GetFormattedPrice(value, siteId);

            case "skudepartmentid":
                // Tranform to display name and localize
                int departmentId = ValidationHelper.GetInteger(row["SKUDepartmentID"], 0);
                DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo(departmentId);

                return (department != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(department.DepartmentDisplayName)) : "";

            case "skumanufacturerid":
                // Tranform to display name and localize
                int manufacturerId = ValidationHelper.GetInteger(row["SKUManufacturerID"], 0);
                ManufacturerInfo manufacturer = ManufacturerInfoProvider.GetManufacturerInfo(manufacturerId);

                return (manufacturer != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(manufacturer.ManufacturerDisplayName)) : "";

            case "skusupplierid":
                // Tranform to display name and localize
                int supplierId = ValidationHelper.GetInteger(row["SKUSupplierID"], 0);
                SupplierInfo supplier = SupplierInfoProvider.GetSupplierInfo(supplierId);

                return (supplier != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(supplier.SupplierDisplayName)) : "";

            case "skuavailableitems":
                int? count = row["SKUAvailableItems"] as int?;
                int? reorderAt = row["SKUReorderAt"] as int?;

                // Emphasise the number when product needs to be reordered
                if (count.HasValue && ((reorderAt.HasValue && (count <= reorderAt)) || (!reorderAt.HasValue && (count <= 0))))
                {
                    // Format message informing about insufficient stock level
                    string reorderMsg = string.Format(GetString("com.sku.reorderatTooltip"), reorderAt);
                    return string.Format("<span class=\"OperationFailed\">{0}</span>", count);
                }
                return count;

            case "itemstobereordered":
                int skuReorderAt = ValidationHelper.GetInteger(row["SKUReorderAt"], 0);
                int skuAvailableItems = ValidationHelper.GetInteger(row["SKUAvailableItems"], 0);
                int difference = skuReorderAt - skuAvailableItems;

                // Return difference, or '-'
                return (difference > 0) ? difference.ToString() : "-";

            case "skusiteid":
                return UniGridFunctions.ColoredSpanYesNo(row["SKUSiteID"] == DBNull.Value);
        }

        return parameter;
    }

    #endregion


    #region "Methods - webpart"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();

        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedForPermission("ReadProducts"))
        {
            ShowError(String.Format(GetString("CMSMessages.AccessDeniedResource"), "EcommerceRead OR ReadProducts"));
            gridElem.Visible = false;
            return;
        }

        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        gridElem.WhereCondition = GetWhereCondition();
        gridElem.OrderBy = GetOrderBy();
        SetTopN();
        gridElem.Pager.DefaultPageSize = GetPageSize();
    }

    #endregion


    #region "Methods - private"

    /// <summary>
    /// Displays or hides columns based on VisibleColumns property.
    /// </summary>
    private void DisplayColumns()
    {
        string[] visibleColumns = VisibleColumns.Split('|');

        // Hide all first
        foreach (var item in gridElem.NamedColumns.Values)
        {
            item.Visible = false;
        }

        // Show columns that should be visible
        foreach (var item in visibleColumns)
        {
            string key = null;
            switch (item)
            {
                case COLUMN_NUMBER:
                    key = "Number";
                    break;

                case COLUMN_PRICE:
                    key = "Price";
                    break;

                case COLUMN_DEPARTMENT:
                    key = "Department";
                    break;

                case COLUMN_MANUFACTURER:
                    key = "Manufacturer";
                    break;

                case COLUMN_SUPPLIER:
                    key = "Supplier";
                    break;

                case COLUMN_PUBLIC_STATUS:
                    key = "PublicStatus";
                    break;

                case COLUMN_INTERNAL_STATUS:
                    key = "InternalStatus";
                    break;

                case COLUMN_REORDER_AT:
                    key = "ReorderAt";
                    break;

                case COLUMN_AVAILABLE_ITEMS:
                    key = "AvailableItems";
                    break;

                case COLUMN_ITEMS_TO_BE_REORDERED:
                    key = "ItemsToBeReordered";
                    break;

                case COLUMN_ALLOW_FOR_SALE:
                    key = "AllowForSale";
                    break;
            }

            // Show column
            if (key != null)
            {
                gridElem.NamedColumns[key].Visible = true;
            }
        }

        // Show option category column if not only product listed
        if (ProductType != PRODUCT_TYPE_PRODUCTS)
        {
            gridElem.NamedColumns["OptionCategory"].Visible = true;
        }

        // If global products are allowed, display column
        if (ECommerceSettings.AllowGlobalProducts(CMSContext.CurrentSiteName))
        {
            gridElem.NamedColumns["Global"].Visible = true;
        }
    }


    /// <summary>
    /// Returns where condition based on webpart fields.
    /// </summary>
    private string GetWhereCondition()
    {
        string where = null;

        // Show products only from current site or global too, based on setting
        if (ECommerceSettings.AllowGlobalProducts(CMSContext.CurrentSiteName))
        {
            where = "(SKUSiteID = " + CMSContext.CurrentSiteID + ") OR (SKUSiteID IS NULL)";
        }
        else
        {
            where = "SKUSiteID = " + CMSContext.CurrentSiteID;
        }

        // Product type filter
        if (!string.IsNullOrEmpty(ProductType) && (ProductType != FILTER_ALL))
        {
            if (ProductType == PRODUCT_TYPE_PRODUCTS)
            {
                where = SqlHelperClass.AddWhereCondition(where, "SKUOptionCategoryID IS NULL");
            }
            else if (ProductType == PRODUCT_TYPE_PRODUCT_OPTIONS)
            {
                where = SqlHelperClass.AddWhereCondition(where, "SKUOptionCategoryID IS NOT NULL");
            }
        }

        // Representing filter
        if (!string.IsNullOrEmpty(Representing) && (Representing != FILTER_ALL))
        {
            SKUProductTypeEnum productTypeEnum = SKUInfoProvider.GetSKUProductTypeEnum(Representing);
            string productTypeString = SKUInfoProvider.GetSKUProductTypeString(productTypeEnum);

            where = SqlHelperClass.AddWhereCondition(where, "SKUProductType = N'" + productTypeString + "'");
        }

        // Product number filter
        if (!string.IsNullOrEmpty(ProductNumber))
        {
            string safeProductNumber = SqlHelperClass.GetSafeQueryString(ProductNumber, true);
            where = SqlHelperClass.AddWhereCondition(where, "SKUNumber LIKE N'%" + safeProductNumber + "%'");
        }

        // Department filter
        DepartmentInfo di = DepartmentInfoProvider.GetDepartmentInfo(Department, CurrentSiteName);
        if (di != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUDepartmentID = " + di.DepartmentID);
        }

        // Manufacturer filter
        ManufacturerInfo mi = ManufacturerInfoProvider.GetManufacturerInfo(Manufacturer, CurrentSiteName);
        if (mi != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUManufacturerID = " + mi.ManufacturerID);
        }

        // Supplier filter
        SupplierInfo si = SupplierInfoProvider.GetSupplierInfo(Supplier, CurrentSiteName);
        if (si != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUSupplierID = " + si.SupplierID);
        }

        // Needs shipping filter
        if (!string.IsNullOrEmpty(NeedsShipping) && (NeedsShipping != FILTER_ALL))
        {
            if (NeedsShipping == NEEDS_SHIPPING_YES)
            {
                where = SqlHelperClass.AddWhereCondition(where, "SKUNeedsShipping = 1");
            }
            else if (NeedsShipping == NEEDS_SHIPPING_NO)
            {
                where = SqlHelperClass.AddWhereCondition(where, "(SKUNeedsShipping = 0) OR (SKUNeedsShipping IS NULL)");
            }
        }

        // Price from filter
        if (PriceFrom > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUPrice >= " + PriceFrom);
        }

        // Price to filter
        if (PriceTo > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUPrice <= " + PriceTo);
        }

        // Public status filter
        PublicStatusInfo psi = PublicStatusInfoProvider.GetPublicStatusInfo(PublicStatus, CurrentSiteName);
        if (psi != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUPublicStatusID = " + psi.PublicStatusID);
        }

        // Internal status filter
        InternalStatusInfo isi = InternalStatusInfoProvider.GetInternalStatusInfo(InternalStatus, CurrentSiteName);
        if (isi != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUInternalStatusID = " + isi.InternalStatusID);
        }

        // Allow for sale filter
        if (!string.IsNullOrEmpty(AllowForSale) && (AllowForSale != FILTER_ALL))
        {
            if (AllowForSale == ALLOW_FOR_SALE_YES)
            {
                where = SqlHelperClass.AddWhereCondition(where, "SKUEnabled = 1");
            }
            else if (AllowForSale == ALLOW_FOR_SALE_NO)
            {
                where = SqlHelperClass.AddWhereCondition(where, "(SKUEnabled = 0) OR (SKUEnabled IS NULL)");
            }
        }

        // Available items filter
        if (!string.IsNullOrEmpty(AvailableItems))
        {
            int value = ValidationHelper.GetInteger(AvailableItems, int.MaxValue);
            where = SqlHelperClass.AddWhereCondition(where, "SKUAvailableItems <= " + value);
        }

        // Needs to be reordered filter
        if (NeedsToBeReordered)
        {
            where = SqlHelperClass.AddWhereCondition(where, "((SKUReorderAt IS NULL) AND (SKUAvailableItems <= 0)) OR ((SKUReorderAt IS NOT NULL) AND (SKUAvailableItems <= SKUReorderAt))");
        }

        return where;
    }


    /// <summary>
    /// Set TopN property to unigrid and disables Pager, if TopN field is specified.
    /// </summary>
    private void SetTopN()
    {
        if (TopN > 0)
        {
            gridElem.TopN = TopN;
            gridElem.Pager.DisplayPager = false;
        }
    }


    /// <summary>
    /// Returns page size for unigrid.
    /// </summary>
    private int GetPageSize()
    {
        switch (PageSize)
        {
            case -1:
            case 10:
            case 25:
            case 50:
            case 100:
                return PageSize;
            default:
                return 10;
        }
    }

    /// <summary>
    /// Returns string for ORDER BY clause.
    /// </summary>
    private string GetOrderBy()
    {
        // OrderBy specified by drop-down list
        string column = null;
        switch (OrderBy)
        {
            case ORDER_BY_NAME_DESC:
                column = "SKUName DESC";
                break;

            case ORDER_BY_NUMBER_ASC:
                column = "SKUNumber ASC";
                break;

            case ORDER_BY_NUMBER_DESC:
                column = "SKUNumber DESC";
                break;

            case ORDER_BY_PRICE_ASC:
                column = "SKUPrice ASC";
                break;

            case ORDER_BY_PRICE_DESC:
                column = "SKUPrice DESC";
                break;

            case ORDER_BY_DATE_DESC:
                column = "SKUCreated DESC";
                break;

            case ORDER_BY_DATE_ASC:
                column = "SKUCreated ASC";
                break;

            case ORDER_BY_AVAILABLE_ITEMS_ASC:
                column = "SKUAvailableItems ASC";
                break;

            case ORDER_BY_REORDER_DESC:
                column = "(ISNULL(SKUReorderAt,0) - SKUAvailableItems) DESC";
                break;

            case ORDER_BY_NAME_ASC:
            default:
                column = "SKUName ASC";
                break;
        }

        // Disables remembering unigrid state
        gridElem.RememberState = false;

        return SqlHelperClass.AddOrderBy("", column);
    }

    #endregion
}