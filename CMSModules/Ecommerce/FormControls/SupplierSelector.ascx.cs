using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_FormControls_SupplierSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseSupplierNameForSelection = false;
    private bool mAddAllItemsRecord = false;
    private bool mAddNoneRecord = false;
    private bool mDisplayOnlyEnabled = true;
    private bool? mAllowGlobalObjects = null;
    private bool? mDisplaySiteItems = null;
    private bool? mDisplayGlobalItems = null;
    private int mSiteId = -1;
    private string mAdditionalItems = "";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (UseSupplierNameForSelection)
            {
                return SupplierName;
            }
            else
            {
                if (SupplierID == 0)
                {
                    return null;
                }
                return SupplierID;
            }
        }
        set
        {
            if (UseSupplierNameForSelection)
            {
                SupplierName = ValidationHelper.GetString(value, "");
            }
            else
            {
                SupplierID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    ///  If true, selected value is ManufacturerName, if false, selected value is ManufacturerID.
    /// </summary>
    public bool UseSupplierNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseSupplierNameForSelection"), mUseSupplierNameForSelection);
        }
        set
        {
            SetValue("UseSupplierNameForSelection", value);
            mUseSupplierNameForSelection = value;
        }
    }


    /// <summary>
    /// Gets or sets the Supplier ID.
    /// </summary>
    public int SupplierID
    {
        get
        {
            return ValidationHelper.GetInteger(uniSelector.Value, 0);
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the Supplier code name.
    /// </summary>
    public string SupplierName
    {
        get
        {
            int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
            SupplierInfo si = SupplierInfoProvider.GetSupplierInfo(id);
            if (si != null)
            {
                return si.SupplierName;
            }
            return "";
        }
        set
        {
            SupplierInfo si = SupplierInfoProvider.GetSupplierInfo(value, SiteInfoProvider.GetSiteName(SiteID));
            if (si != null)
            {
                uniSelector.Value = si.SupplierID;
            }
        }
    }

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdownlist.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Allows to display suppliers only for specified site id. Use 0 for global suppliers. Default value is current site id. 
    /// </summary>
    public int SiteID
    {
        get
        {
            // No site id given
            if (mSiteId == -1)
            {
                mSiteId = CMSContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;

            mDisplayGlobalItems = null;
            mDisplaySiteItems = null;
            mAllowGlobalObjects = null;
        }
    }


    /// <summary>
    /// Returns true if site given by SiteID uses global suppliers beside site-specific ones.
    /// </summary>
    protected bool AllowGlobalObjects
    {
        get
        {
            // Unknown yet
            if (!mAllowGlobalObjects.HasValue)
            {
                mAllowGlobalObjects = false;
                // Try to figure out from settings
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                if (si != null)
                {
                    mAllowGlobalObjects = ECommerceSettings.AllowGlobalSuppliers(si.SiteName);
                }
            }

            return mAllowGlobalObjects.Value;
        }
    }


    /// <summary>
    /// Indicates if site items will be displayed. By default, value is based on SiteID property and global objects setting.
    /// </summary>
    public bool DisplaySiteItems
    {
        get
        {
            // Unknown yet
            if (!mDisplaySiteItems.HasValue)
            {
                // Display site item when working with site records
                mDisplaySiteItems = (SiteID != 0);
            }

            return mDisplaySiteItems.Value;
        }
        set
        {
            mDisplaySiteItems = value;
        }
    }


    /// <summary>
    /// Indicates if global items will be displayed. By default, value is based on SiteID property and global objects setting.
    /// </summary>
    public bool DisplayGlobalItems
    {
        get
        {
            // Unknown yet
            if (!mDisplayGlobalItems.HasValue)
            {
                mDisplayGlobalItems = false;
                if ((SiteID == 0) || AllowGlobalObjects)
                {
                    mDisplayGlobalItems = true;
                }
            }

            return mDisplayGlobalItems.Value;
        }
        set
        {
            mDisplayGlobalItems = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add all item record to the dropdownlist.
    /// </summary>
    public bool AddAllItemsRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddAllItemsRecord"), mAddAllItemsRecord);
        }
        set
        {
            SetValue("AddAllItemsRecord", value);
            mAddAllItemsRecord = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddNoneRecord"), mAddNoneRecord);
        }
        set
        {
            SetValue("AddNoneRecord", value);
            mAddNoneRecord = value;
        }
    }


    /// <summary>
    /// Allows to display only enabled items. Default value is true.
    /// </summary>
    public bool DisplayOnlyEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyEnabled"), mDisplayOnlyEnabled);
        }
        set
        {
            SetValue("DisplayOnlyEnabled", value);
            mDisplayOnlyEnabled = value;
        }
    }


    /// <summary>
    /// Id of items which has to be displayed regardless other settings. Use ',' or ';' as separator if more ids required.
    /// </summary>
    public string AdditionalItems
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AdditionalItems"), mAdditionalItems);
        }
        set
        {
            // Prevent from setting null value
            if (value != null)
            {
                mAdditionalItems = value.Replace(';', ',');
            }
            else
            {
                mAdditionalItems = "";
            }

            SetValue("AdditionalItems", mAdditionalItems);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the currently selected item will be available for selection, even if it does not meet the where condition criteria.
    /// </summary>
    public bool EnsureSelectedItem
    {
        get;
        set;
    }

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            InitSelector();
        }

        base.OnLoad(e);
    }

    #endregion


    #region "Initialization"

    /// <summary>
    /// Inits the selector.
    /// </summary>
    protected void InitSelector()
    {
        TryInitByForm();

        uniSelector.EnabledColumnName = "SupplierEnabled";
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowAll = AddAllItemsRecord;
        uniSelector.AllowEmpty = AddNoneRecord;

        string where = "";

        // Add global items
        if (DisplayGlobalItems)
        {
            where = SqlHelperClass.AddWhereCondition(where, "(SupplierSiteID IS NULL)", "OR");
        }

        // Add site specific items
        if (DisplaySiteItems)
        {
            where = SqlHelperClass.AddWhereCondition(where, "(SupplierSiteID = " + SiteID + ")", "OR");
        }

        // Filter out only enabled items
        if (DisplayOnlyEnabled)
        {
            where = SqlHelperClass.AddWhereCondition(where, "SupplierEnabled = 1");
        }

        // Add items which have to be on the list
        string additionalList = SqlHelperClass.GetSafeQueryString(AdditionalItems, false);
        if (!string.IsNullOrEmpty(additionalList))
        {
            where = SqlHelperClass.AddWhereCondition(where, "SupplierID IN (" + additionalList + ")", "OR");
        }

        // Selected value must be on the list
        if (EnsureSelectedItem && (SupplierID > 0))
        {
            where = SqlHelperClass.AddWhereCondition(where, "(SupplierID = " + SupplierID + ")", "OR");
        }

        uniSelector.WhereCondition = where;
    }


    private void TryInitByForm()
    {
        if (Form == null)
        {
            return;
        }

        if (Form.Data.ContainsColumn("SKUSiteID"))
        {
            SiteID = ValidationHelper.GetInteger(Form.Data.GetValue("SKUSiteID"), 0);
        }

        if (Form.Data.ContainsColumn("SKUID"))
        {
            int skuid = ValidationHelper.GetInteger(Form.Data.GetValue("SKUID"), -1);
            EnsureSelectedItem = skuid != -1;
        }
    }

    #endregion
}