using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_FormControls_ManufacturerSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseManufacturerNameForSelection = false;
    private bool mAddAllItemsRecord = true;
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
            if (UseManufacturerNameForSelection)
            {
                return ManufacturerName;
            }
            else
            {
                if (ManufacturerID == 0)
                {
                    return null;
                }
                return ManufacturerID;
            }
        }
        set
        {
            if (UseManufacturerNameForSelection)
            {
                ManufacturerName = ValidationHelper.GetString(value, "");
            }
            else
            {
                ManufacturerID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    ///  If true, selected value is ManufacturerName, if false, selected value is ManufacturerID.
    /// </summary>
    public bool UseManufacturerNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseManufacturerNameForSelection"), mUseManufacturerNameForSelection);
        }
        set
        {
            SetValue("UseManufacturerNameForSelection", value);
            mUseManufacturerNameForSelection = value;
        }
    }


    /// <summary>
    /// Gets or sets the Manufacturer ID.
    /// </summary>
    public int ManufacturerID
    {
        get
        {
            return ValidationHelper.GetInteger(uniSelector.Value, 0);
        }
        set
        {
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the Manufacturer code name.
    /// </summary>
    public string ManufacturerName
    {
        get
        {
            int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
            ManufacturerInfo mi = ManufacturerInfoProvider.GetManufacturerInfo(id);
            if (mi != null)
            {
                return mi.ManufacturerName;
            }
            return "";
        }
        set
        {
            ManufacturerInfo mi = ManufacturerInfoProvider.GetManufacturerInfo(value, SiteInfoProvider.GetSiteName(SiteID));
            if (mi != null)
            {
                uniSelector.Value = mi.ManufacturerID;
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
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            uniSelector.StopProcessing = value;
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
    /// Inner control.
    /// </summary>
    public DropDownList InnerControl
    {
        get
        {
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Allows to display manufacturers only for specified site ID. Use 0 for global manufacturers. Default value is current site ID.
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
    /// Returns true if site given by SiteID uses global manufacturers beside site-specific ones.
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
                    mAllowGlobalObjects = ECommerceSettings.AllowGlobalManufacturers(si.SiteName);
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
    /// IDs of items which has to be displayed regardless of other settings. Use ',' or ';' as separator if more IDs are required.
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
            TryInitByForm();

            uniSelector.EnabledColumnName = "ManufacturerEnabled";
            uniSelector.IsLiveSite = IsLiveSite;
            uniSelector.AllowAll = AddAllItemsRecord;
            uniSelector.AllowEmpty = AddNoneRecord;

            SetupWhereCondition();
        }

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        SetupWhereCondition();

        uniSelector.Reload(true);

        base.OnPreRender(e);
    }

    #endregion


    #region "Initialization"

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


    /// <summary>
    /// Inits the selector.
    /// </summary>
    protected void SetupWhereCondition()
    {
        string where = "";
        // Add global items
        if (DisplayGlobalItems)
        {
            where = SqlHelperClass.AddWhereCondition(where, "ManufacturerSiteID IS NULL", "OR");
        }
        // Add site specific items
        if (DisplaySiteItems)
        {
            where = SqlHelperClass.AddWhereCondition(where, "ManufacturerSiteID = " + SiteID, "OR");
        }

        // Filter out only enabled items
        if (DisplayOnlyEnabled)
        {
            where = SqlHelperClass.AddWhereCondition(where, "ManufacturerEnabled = 1");
        }

        // Add items which have to be on the list
        string additionalList = SqlHelperClass.GetSafeQueryString(AdditionalItems, false);
        if (!string.IsNullOrEmpty(additionalList))
        {
            where = SqlHelperClass.AddWhereCondition(where, "ManufacturerID IN (" + additionalList + ")", "OR");
        }

        // Selected item must be on the list
        if (EnsureSelectedItem && (ManufacturerID > 0))
        {
            where = SqlHelperClass.AddWhereCondition(where, "(ManufacturerID = " + ManufacturerID + ")", "OR");
        }

        uniSelector.WhereCondition = where;
    }

    #endregion
}