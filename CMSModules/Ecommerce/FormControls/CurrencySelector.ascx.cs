using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_FormControls_CurrencySelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseCurrencyNameForSelection = false;
    private bool mShowAllItems = false;
    private bool mAddAllItemsRecord = false;
    private bool mAddNoneRecord = false;
    private bool mAddSelectRecord = false;
    private bool mDisplayOnlyWithExchangeRate = false;
    private bool mRenderInline = false;
    private bool mAddSiteDefaultCurrency = false;
    private bool mExcludeSiteDefaultCurrency = false;
    private bool mDisplayOnlyEnabled = true;
    private bool? mUsingGlobalObjects = null;
    private int mSiteId = -1;
    private string mAdditionalItems = "";
    private bool mIncludeSelected = true;
    private bool mDoFullPostback = false;

    #endregion


    #region "Public properties"

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
    /// Indicates whether to show all items ("more items" is not displayed).
    /// </summary>
    public bool ShowAllItems
    {
        get
        {
            return mShowAllItems;
        }
        set
        {
            mShowAllItems = value;
        }
    }


    /// <summary>
    /// Indicates whether to render update panel in inline mode.
    /// </summary>
    public bool RenderInline
    {
        get
        {
            return mRenderInline;
        }
        set
        {
            mRenderInline = value;
        }
    }


    /// <summary>
    /// Indicates if only currencies with exchange rate will be displayed. Main currency will be included. Default value is false. 
    /// </summary>
    public bool DisplayOnlyWithExchangeRate
    {
        get
        {
            return mDisplayOnlyWithExchangeRate;
        }
        set
        {
            mDisplayOnlyWithExchangeRate = value;
        }
    }


    /// <summary>
    /// Add (all) record to the dropdownlist.
    /// </summary>
    public bool AddAllItemsRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddAllItemsRecord"), mAddAllItemsRecord);
        }
        set
        {
            mAddAllItemsRecord = value;
            SetValue("AddAllItemsRecord", value);
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
        }
    }


    /// <summary>
    /// Add none record to the dropdownlist.
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

            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            uniSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Add (select) record to the dropdownlist.
    /// </summary>
    public bool AddSelectRecord
    {
        get
        {
            return mAddSelectRecord;
        }
        set
        {
            mAddSelectRecord = value;
        }
    }


    /// <summary>
    /// Indicates whether to add current site default currency.
    /// </summary>
    public bool AddSiteDefaultCurrency
    {
        get
        {
            return mAddSiteDefaultCurrency;
        }
        set
        {
            mAddSiteDefaultCurrency = value;
        }
    }


    /// <summary>
    /// Indicates whether to exclude current site default currency.
    /// </summary>
    public bool ExcludeSiteDefaultCurrency
    {
        get
        {
            return mExcludeSiteDefaultCurrency;
        }
        set
        {
            mExcludeSiteDefaultCurrency = value;
        }
    }


    /// <summary>
    /// Indicates if selected value will be included in the list despite of other conditions.
    /// </summary>
    public bool IncludeSelected
    {
        get
        {
            return mIncludeSelected;
        }
        set
        {
            mIncludeSelected = value;
        }
    }


    /// <summary>
    /// Currency ID.
    /// </summary>
    public int CurrencyID
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
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (UseCurrencyNameForSelection)
            {
                CurrencyInfo currency = CurrencyInfoProvider.GetCurrencyInfo(CurrencyID);
                return currency != null ? currency.CurrencyName : "";
            }

            return CurrencyID;
        }
        set
        {
            if (UseCurrencyNameForSelection)
            {
                CurrencyInfo currency = CurrencyInfoProvider.GetCurrencyInfo(ValidationHelper.GetString(value, ""), SiteInfoProvider.GetSiteName(SiteID));
                CurrencyID = currency != null ? currency.CurrencyID : 0;
            }
            else
            {
                CurrencyID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    ///  If true, selected value is CurrencyName, if false, selected value is CurrencyID.
    /// </summary>
    public bool UseCurrencyNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseCurrencyNameForSelection"), mUseCurrencyNameForSelection);
        }
        set
        {
            SetValue("UseCurrencyNameForSelection", value);
            mUseCurrencyNameForSelection = value;
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
    /// Returns inner DropDownList control.
    /// </summary>
    public DropDownList DropDownSingleSelect
    {
        get
        {
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Returns inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }

    /// <summary>
    /// Allows to display currencies only for specified site id. Use 0 for global currencies. Default value is current site id.
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


    /// <summary>
    /// Indicates whether update panel is enabled.
    /// </summary>
    public bool DoFullPostback
    {
        get
        {
            return mDoFullPostback;
        }
        set
        {
            mDoFullPostback = value;
        }
    }

    #endregion


    #region "Protected properties"

    /// <summary>
    /// Returns true if site given by SiteID uses global currencies.
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
                    mUsingGlobalObjects = ECommerceSettings.UseGlobalCurrencies(si.SiteName);
                }
            }

            return mUsingGlobalObjects.Value;
        }
    }

    #endregion


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


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        InitSelector();
        uniSelector.Reload(true);
    }


    /// <summary>
    /// Inits the selector.
    /// </summary>
    protected void InitSelector()
    {
        if (RenderInline)
        {
            pnlUpdate.RenderMode = UpdatePanelRenderMode.Inline;
        }
        if (ShowAllItems)
        {
            uniSelector.MaxDisplayedItems = 1000;
        }

        // Setup selector
        uniSelector.EnabledColumnName = "CurrencyEnabled";
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowEmpty = AddNoneRecord;
        uniSelector.AllowAll = AddAllItemsRecord;

        if (DoFullPostback)
        {
            ControlsHelper.RegisterPostbackControl(uniSelector.DropDownSingleSelect);
        }

        if (AddSelectRecord)
        {
            string[,] fields = new string[1, 2];
            fields[0, 0] = GetString("currencyselector.select");
            fields[0, 1] = "-1";
            uniSelector.SpecialFields = fields;
        }

        CurrencyInfo main = CurrencyInfoProvider.GetMainCurrency(SiteID);
        int mainCurrencyId = (main != null) ? main.CurrencyID : 0;

        // Prepare where condition
        string where = "";
        if (DisplayOnlyWithExchangeRate)
        {
            ExchangeTableInfo tableInfo = ExchangeTableInfoProvider.GetLastExchangeTableInfo(SiteID);
            if (tableInfo != null)
            {
                where = "(CurrencyID = " + mainCurrencyId + " OR CurrencyID IN (SELECT ExchangeRateToCurrencyID FROM COM_CurrencyExchangeRate WHERE COM_CurrencyExchangeRate.ExchangeTableID = " + tableInfo.ExchangeTableID + ") AND CurrencyEnabled = 1)";
            }
            else
            {
                where = "(0=1)";
            }
        }

        // Add site main currency when required
        if (AddSiteDefaultCurrency && (main != null))
        {
            where = SqlHelperClass.AddWhereCondition(where, "CurrencyID = " + mainCurrencyId, "OR");
        }

        // Exclude site main currency when required
        if (ExcludeSiteDefaultCurrency && (main != null))
        {
            where = SqlHelperClass.AddWhereCondition(where, "(NOT CurrencyID = " + mainCurrencyId + ")");
        }

        // Select only records by speciffied site
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

            where = SqlHelperClass.AddWhereCondition(where, "(ISNULL(CurrencySiteID, 0) = " + filteredSiteId + ")");
        }

        // Filter out only enabled items
        if (DisplayOnlyEnabled)
        {
            where = SqlHelperClass.AddWhereCondition(where, "CurrencyEnabled = 1");
        }

        // Add items which have to be on the list (if any)
        string additionalList = SqlHelperClass.GetSafeQueryString(AdditionalItems, false);
        if ((!string.IsNullOrEmpty(where)) && (!string.IsNullOrEmpty(additionalList)))
        {
            where = SqlHelperClass.AddWhereCondition(where, "(CurrencyID IN (" + additionalList + "))", "OR");
        }

        // Selected value (if any) must be on the list
        if ((!string.IsNullOrEmpty(where)) && (CurrencyID > 0) && (IncludeSelected))
        {
            where = SqlHelperClass.AddWhereCondition(where, "(CurrencyID = " + CurrencyID + ")", "OR");
        }

        // Set where condition
        uniSelector.WhereCondition = where;
    }
}