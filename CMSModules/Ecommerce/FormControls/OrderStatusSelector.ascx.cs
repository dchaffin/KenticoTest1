using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_FormControls_OrderStatusSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseStatusNameForSelection = true;
    private bool mAddAllItemsRecord = true;
    private bool mDisplayOnlyEnabled = true;
    private bool mAddNoneRecord = false;
    private bool? mUsingGlobalObjects = null;
    private int mSiteId = -1;
    private string mAdditionalItems = "";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (UseStatusNameForSelection)
            {
                return OrderStatusName;
            }
            else
            {
                return OrderStatusID;
            }
        }
        set
        {
            if (UseStatusNameForSelection)
            {
                OrderStatusName = ValidationHelper.GetString(value, "");
            }
            else
            {
                OrderStatusID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the OrderStatus ID.
    /// </summary>
    public int OrderStatusID
    {
        get
        {
            if (mUseStatusNameForSelection)
            {
                string name = ValidationHelper.GetString(uniSelector.Value, "");
                OrderStatusInfo oi = OrderStatusInfoProvider.GetOrderStatusInfo(name, SiteInfoProvider.GetSiteName(SiteID));
                if (oi != null)
                {
                    return oi.StatusID;
                }
                return 0;
            }
            else
            {
                return ValidationHelper.GetInteger(uniSelector.Value, 0);
            }
        }
        set
        {
            if (mUseStatusNameForSelection)
            {
                OrderStatusInfo tgi = OrderStatusInfoProvider.GetOrderStatusInfo(value);
                if (tgi != null)
                {
                    uniSelector.Value = tgi.StatusID;
                }
            }
            else
            {
                uniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the OrderStatus code name.
    /// </summary>
    public string OrderStatusName
    {
        get
        {
            if (mUseStatusNameForSelection)
            {
                return ValidationHelper.GetString(uniSelector.Value, "");
            }
            else
            {
                int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
                OrderStatusInfo oi = OrderStatusInfoProvider.GetOrderStatusInfo(id);
                if (oi != null)
                {
                    return oi.StatusName;
                }
                return "";
            }
        }
        set
        {
            if (mUseStatusNameForSelection)
            {
                uniSelector.Value = value;
            }
            else
            {
                OrderStatusInfo tgi = OrderStatusInfoProvider.GetOrderStatusInfo(value, SiteInfoProvider.GetSiteName(SiteID));
                if (tgi != null)
                {
                    uniSelector.Value = tgi.StatusName;
                }
            }
        }
    }


    /// <summary>
    ///  If true, selected value is OrderStatusName, if false, selected value is OrderStatusID.
    /// </summary>
    public bool UseStatusNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseStatusNameForSelection"), mUseStatusNameForSelection);
        }
        set
        {
            SetValue("UseStatusNameForSelection", value);
            mUseStatusNameForSelection = value;
        }
    }
    

    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddAllItemsRecord
    {
        get
        {
            return mAddAllItemsRecord;
        }
        set
        {
            mAddAllItemsRecord = value;
        }
    }


    /// <summary>
    /// Determines whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return mAddNoneRecord;
        }
        set
        {
            mAddNoneRecord = value;
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
    /// Allows to display statuses only for specified site id. Use 0 for global statuses. Default value is current site id.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId == -1)
            {
                mSiteId = CMSContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;
            mUsingGlobalObjects = null;
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
            mDisplayOnlyEnabled = value;
            SetValue("DisplayOnlyEnabled", value);
        }
    }


    /// <summary>
    /// Id of items which have to be displayed. Use ',' or ';' as separator if more ids required.
    /// </summary>
    public string AdditionalItems
    {
        get
        {
            return mAdditionalItems;
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
        }
    }

    #endregion


    #region "Protected properties"

    /// <summary>
    /// Returns true if site given by SiteID uses global order statuses.
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
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                if (si != null)
                {
                    mUsingGlobalObjects = ECommerceSettings.UseGlobalOrderStatus(si.SiteName);
                }
            }

            return mUsingGlobalObjects.Value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            InitSelector();
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Reloads selector.
    /// </summary>
    public void Reload()
    {
        InitSelector();

        uniSelector.Reload(true);
    }


    /// <summary>
    /// Inits the selector.
    /// </summary>
    protected void InitSelector()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowEmpty = AddNoneRecord;
        uniSelector.AllowAll = AddAllItemsRecord;
        uniSelector.ReturnColumnName = (UseStatusNameForSelection ? "StatusName" : "StatusID");
        uniSelector.EnabledColumnName = "StatusEnabled";
        uniSelector.OrderBy = "StatusOrder";

        string where = "";
        // Select records by speciffied site
        if (SiteID >= 0)
        {
            // Show global records by default
            int filteredSiteId = 0;

            // Check configuration when site specified
            if (SiteID > 0)
            {
                // Show site specific records when not using global statuses
                filteredSiteId = UsingGlobalObjects ? 0 : SiteID;
            }

            where += "(ISNULL(StatusSiteID, 0) = " + filteredSiteId + ") ";
        }

        // Filter out only enabled items
        if (DisplayOnlyEnabled)
        {
            where = SqlHelperClass.AddWhereCondition(where, "StatusEnabled = 1");
        }

        // Add items which have to be on the list
        string additionalList = SqlHelperClass.GetSafeQueryString(AdditionalItems, false);
        if ((!string.IsNullOrEmpty(where)) && (!string.IsNullOrEmpty(additionalList)))
        {
            where = SqlHelperClass.AddWhereCondition(where, "StatusID IN (" + additionalList + ")", "OR");
        }

        // Selected value must be on the list
        if ((!string.IsNullOrEmpty(where)) && (OrderStatusID > 0))
        {
            where = SqlHelperClass.AddWhereCondition(where, "StatusID = " + OrderStatusID, "OR");
        }

        // Set where condition
        uniSelector.WhereCondition = where;

        if (UseStatusNameForSelection)
        {
            uniSelector.AllRecordValue = "";
            uniSelector.NoneRecordValue = "";
        }
    }

    #endregion
}