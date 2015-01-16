using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Ecommerce_Controls_UI_ProductFilter : CMSAbstractDataFilterControl
{
    #region "Variables"

    private TreeNode mParentNode = null;
    private CMSUserControl filteredControl = null;
    private string mFilterMode = null;
    private bool isAdvancedMode = false;
    private bool allowGlobalProducts = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current filter mode.
    /// </summary>
    public override string FilterMode
    {
        get
        {
            if (mFilterMode == null)
            {
                mFilterMode = ValidationHelper.GetString(filteredControl.GetValue("FilterMode"), "").ToLowerCSafe();
            }
            return mFilterMode;
        }
        set
        {
            mFilterMode = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    private TreeNode ParentNode
    {
        get
        {
            return mParentNode ?? (mParentNode = CMSPage.EditedObject as TreeNode);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        filteredControl = FilteredControl as CMSUserControl;

        // Hide filter button, this filter has its own
        UniGrid grid = filteredControl as UniGrid;
        if (grid != null)
        {
            grid.HideFilterButton = true;
        }

        allowGlobalProducts = ECommerceSettings.AllowGlobalProducts(CMSContext.CurrentSiteName);

        // Initialize controls
        if (!URLHelper.IsPostback())
        {
            FillThreeStateDDL(ddlNeedsShipping);
            FillThreeStateDDL(ddlAllowForSale);

            FillDocumentTypesDDL();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // General UI
        lnkShowAdvancedFilter.Text = GetString("general.displayadvancedfilter");
        imgShowAdvancedFilter.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/SortDown.png");
        lnkShowSimpleFilter.Text = GetString("general.displaysimplefilter");
        imgShowSimpleFilter.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/SortUp.png");

        btnFilter.Text = GetString("general.show");
        btnFilter.Click += btnFilter_Click;

        btnReset.Text = GetString("general.reset");
        btnReset.Click += btnReset_Click;

        // Setup filter mode
        EnsureFilterMode();

        // Fill department dropdownlist
        if ((CMSContext.CurrentUser != null) && !CMSContext.CurrentUser.IsGlobalAdministrator)
        {
            departmentElem.UserID = CMSContext.CurrentUser.UserID;
        }

        // When global SKUs can be included in listing
        if (allowGlobalProducts)
        {
            // Display global departments, manufacturers, suppliers and global statuses too
            departmentElem.DisplayGlobalItems = true;
            publicStatusElem.DisplayGlobalItems = true;
            internalStatusElem.DisplayGlobalItems = true;
            supplierElem.DisplayGlobalItems = true;
            manufacturerElem.DisplayGlobalItems = true;
        }

        plcSite.Visible = allowGlobalProducts;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition according to values selected in filter.
    /// </summary>
    private string GenerateWhereCondition()
    {
        string tempCondition = "";
        string where = "";

        string productNameColumnName = (ParentNode != null) ? "DocumentName" : "SKUName";

        // Append name/number condition
        if (!string.IsNullOrEmpty(txtNameOrNumber.Text))
        {
            string nameOrNumber = SqlHelperClass.GetSafeQueryString(txtNameOrNumber.Text.Trim(), false);
            tempCondition = string.Format("({0} LIKE N'%{1}%' OR SKUNumber LIKE N'%{1}%')", productNameColumnName, nameOrNumber);
            where = SqlHelperClass.AddWhereCondition(where, tempCondition);
        }

        // Append site condition
        if (allowGlobalProducts && (siteElem.SiteID != UniSelector.US_GLOBAL_OR_SITE_RECORD))
        {
            // Restrict SKUSiteID only for products not for product section (full listing mode)
            int selectedSiteID = (siteElem.SiteID > 0) ? siteElem.SiteID : 0;
            where = SqlHelperClass.AddWhereCondition(where, "(SKUID IS NULL) OR (ISNULL(SKUSiteID, 0) = " + selectedSiteID + ")");
        }

        // Append department condition
        if (departmentElem.DepartmentID > 0)
        {
            tempCondition = string.Format("SKUDepartmentID = {0}", departmentElem.DepartmentID);

            where = SqlHelperClass.AddWhereCondition(where, tempCondition);
        }
        else if (departmentElem.DepartmentID == -5)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SKUDepartmentID IS NULL");
        }

        // Append one level condition
        if (ParentNode != null)
        {
            if (!chkShowAllChildren.Checked)
            {
                tempCondition = string.Format("NodeParentID = {0} AND NodeLevel = {1}", ParentNode.NodeID, ParentNode.NodeLevel + 1);
            }

            where = SqlHelperClass.AddWhereCondition(where, tempCondition);
        }

        // Handle advanced mode fields
        if (isAdvancedMode)
        {
            // Append product type condition
            if ((selectProductTypeElem.Value != null) && (selectProductTypeElem.Value.ToString() != "ALL"))
            {
                tempCondition = string.Format("SKUProductType = N'{0}'", SqlHelperClass.GetSafeQueryString(selectProductTypeElem.Value.ToString()));
                where = SqlHelperClass.AddWhereCondition(where, tempCondition);
            }

            // Manufacturer value
            if (manufacturerElem.ManufacturerID >= 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "ISNULL(SKUManufacturerID, 0) = " + manufacturerElem.ManufacturerID);
            }

            // Supplier value
            if (supplierElem.SupplierID >= 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "ISNULL(SKUSupplierID, 0) = " + supplierElem.SupplierID);
            }

            // Internal status value
            if (internalStatusElem.InternalStatusID >= 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "ISNULL(SKUInternalStatusID, 0) = " + internalStatusElem.InternalStatusID);
            }

            // Store status value
            if (publicStatusElem.PublicStatusID >= 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "ISNULL(SKUPublicStatusID, 0) = " + publicStatusElem.PublicStatusID);
            }

            // Append needs shipping condition
            int needsShipping = ValidationHelper.GetInteger(ddlNeedsShipping.SelectedValue, -1);
            if (needsShipping >= 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "ISNULL(SKUNeedsShipping, 0) = " + needsShipping);
            }

            // Append allow for sale condition
            int allowForSale = ValidationHelper.GetInteger(ddlAllowForSale.SelectedValue, -1);
            if (allowForSale >= 0)
            {
                where = SqlHelperClass.AddWhereCondition(where, "SKUEnabled = " + allowForSale);
            }

            // When in document mode
            if (ParentNode != null)
            {
                int docTypeId = ValidationHelper.GetInteger(drpDocTypes.SelectedValue, 0);
                if (docTypeId > 0)
                {
                    // Append document type condition
                    where = SqlHelperClass.AddWhereCondition(where, "NodeClassID = " + docTypeId);
                }
            }
        }

        return where;
    }


    /// <summary>
    /// Applies filter to unigrid
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        UniGrid grid = filteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = filteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Sets the advanced mode.
    /// </summary>
    protected void lnkShowAdvancedFilter_Click(object sender, EventArgs e)
    {
        isAdvancedMode = true;
        ViewState["IsAdvancedMode"] = isAdvancedMode;

        SetFieldsVisibility();
    }


    /// <summary>
    /// Sets the simple mode.
    /// </summary>
    protected void lnkShowSimpleFilter_Click(object sender, EventArgs e)
    {
        isAdvancedMode = false;
        ViewState["IsAdvancedMode"] = isAdvancedMode;

        SetFieldsVisibility();
    }


    /// <summary>
    /// Ensures correct filter mode flag if filter mode was just changed.
    /// </summary>
    private void EnsureFilterMode()
    {
        if (URLHelper.IsPostback())
        {
            // Get current event target
            string uniqieId = ValidationHelper.GetString(Request.Params["__EVENTTARGET"], String.Empty);
            uniqieId = uniqieId.Replace("$", "_");

            // If postback was fired by mode switch, update isAdvancedMode variable
            if (uniqieId == lnkShowAdvancedFilter.ClientID)
            {
                isAdvancedMode = true;
            }
            else if (uniqieId == lnkShowSimpleFilter.ClientID)
            {
                isAdvancedMode = false;
            }
            else
            {
                isAdvancedMode = ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false);
            }
        }

        // Show/hide fields of filter
        SetFieldsVisibility();
    }


    /// <summary>
    /// Shows/hides fields of filter according to simple/advanced mode.
    /// </summary>
    private void SetFieldsVisibility()
    {
        plcSimpleFilter.Visible = !isAdvancedMode;
        plcAdvancedFilter.Visible = isAdvancedMode;

        plcAdvancedFilterType.Visible = isAdvancedMode;
        plcAdvancedFilterGeneral.Visible = isAdvancedMode;

        bool documentMode = ParentNode != null;
        plcSubsectionProducts.Visible = documentMode;
        plcAdvancedDocumentType.Visible = isAdvancedMode && documentMode;
    }


    /// <summary>
    /// Fills items 'Yes', 'No' and 'All' to given drop down list.
    /// </summary>
    /// <param name="dropDown">Drop down list to be filled.</param>
    private void FillThreeStateDDL(DropDownList dropDown)
    {
        dropDown.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        dropDown.Items.Add(new ListItem(GetString("general.yes"), "1"));
        dropDown.Items.Add(new ListItem(GetString("general.no"), "0"));
    }


    /// <summary>
    /// Fills dropdown list with document types.
    /// </summary>
    private void FillDocumentTypesDDL()
    {
        drpDocTypes.Items.Clear();

        // Add (All) record
        drpDocTypes.Items.Add(new ListItem(GetString("general.selectall"), "0"));

        // Select only document types from current site marked as product
        string where = "ClassIsDocumentType = 1 AND ClassIsProduct = 1 AND ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + CMSContext.CurrentSiteID + ")";
        DataSet ds = DataClassInfoProvider.GetClasses("ClassID, ClassDisplayName", where, "ClassDisplayName", 0);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string name = ValidationHelper.GetString(dr["ClassDisplayName"], "");
                int id = ValidationHelper.GetInteger(dr["ClassID"], 0);

                if (!string.IsNullOrEmpty(name) && (id > 0))
                {
                    // Handle document name
                    name = ResHelper.LocalizeString(CMSContext.ResolveMacros(name));

                    drpDocTypes.Items.Add(new ListItem(name, id.ToString()));
                }
            }
        }
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
        state.AddValue("AdvancedMode", isAdvancedMode);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        isAdvancedMode = state.GetBoolean("AdvancedMode");
        ViewState["IsAdvancedMode"] = isAdvancedMode;
        SetFieldsVisibility();
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtNameOrNumber.Text = String.Empty;
        siteElem.SiteID = UniSelector.US_GLOBAL_OR_SITE_RECORD;
        departmentElem.Value = -1;
        selectProductTypeElem.Value = "ALL";
        manufacturerElem.Value = -1;
        supplierElem.Value = -1;
        internalStatusElem.Value = -1;
        publicStatusElem.Value = -1;
        ddlNeedsShipping.SelectedIndex = 0;
        ddlAllowForSale.SelectedIndex = 0;
        drpDocTypes.SelectedIndex = 0;
        chkShowAllChildren.Checked = true;
        isAdvancedMode = false;
        ViewState["IsAdvancedMode"] = isAdvancedMode;
        SetFieldsVisibility();
    }

    #endregion
}