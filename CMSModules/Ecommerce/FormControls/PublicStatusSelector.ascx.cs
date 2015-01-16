using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_FormControls_PublicStatusSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseStatusNameForSelection = true;
    private bool mAddAllItemsRecord = true;
    private bool mAddNoneRecord = false;
    private bool mDisplayOnlyEnabled = true;
    private bool? mUsingGlobalObjects = null;
    private bool? mDisplayGlobalItems = null;
    private bool mReflectGlobalProductsUse = false;
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
            EnsureChildControls();
            if (UseStatusNameForSelection)
            {
                return PublicStatusName;
            }
            else
            {
                if (PublicStatusID == 0)
                {
                    return null;
                }
                return PublicStatusID;
            }
        }
        set
        {
            EnsureChildControls();
            if (UseStatusNameForSelection)
            {
                PublicStatusName = ValidationHelper.GetString(value, "");
            }
            else
            {
                PublicStatusID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the PublicStatus ID.
    /// </summary>
    public int PublicStatusID
    {
        get
        {
            EnsureChildControls();
            if (UseStatusNameForSelection)
            {
                string name = ValidationHelper.GetString(uniSelector.Value, "");
                PublicStatusInfo psi = PublicStatusInfoProvider.GetPublicStatusInfo(name, SiteInfoProvider.GetSiteName(SiteID));
                if (psi != null)
                {
                    return psi.PublicStatusID;
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
            EnsureChildControls();
            if (UseStatusNameForSelection)
            {
                PublicStatusInfo psi = PublicStatusInfoProvider.GetPublicStatusInfo(value);
                if (psi != null)
                {
                    uniSelector.Value = psi.PublicStatusID;
                }
            }
            else
            {
                uniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the PublicStatus code name.
    /// </summary>
    public string PublicStatusName
    {
        get
        {
            EnsureChildControls();
            if (UseStatusNameForSelection)
            {
                return ValidationHelper.GetString(uniSelector.Value, "");
            }
            else
            {
                int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
                PublicStatusInfo psi = PublicStatusInfoProvider.GetPublicStatusInfo(id);
                if (psi != null)
                {
                    return psi.PublicStatusName;
                }
                return "";
            }
        }
        set
        {
            EnsureChildControls();
            if (UseStatusNameForSelection)
            {
                uniSelector.Value = value;
            }
            else
            {
                PublicStatusInfo psi = PublicStatusInfoProvider.GetPublicStatusInfo(value, SiteInfoProvider.GetSiteName(SiteID));
                if (psi != null)
                {
                    uniSelector.Value = psi.PublicStatusName;
                }
            }
        }
    }


    /// <summary>
    ///  If true, selected value is PublicStatusName, if false, selected value is PublicStatusID.
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
            EnsureChildControls();
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
    /// Inner control.
    /// </summary>
    public DropDownList InnerControl
    {
        get
        {
            EnsureChildControls();
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdownlist.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            EnsureChildControls();
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
    /// Id of items which have to be displayed. Use ',' or ';' as separator if more ids required.
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
                if ((SiteID == 0) || UsingGlobalObjects || (ReflectGlobalProductsUse && ECommerceSettings.AllowGlobalProducts(CMSContext.CurrentSiteName)))
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
    /// Gets or sets a value that indicates if the global items should be displayed when the global products are used on the site.
    /// </summary>
    public bool ReflectGlobalProductsUse
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ReflectGlobalProductsUse"), mReflectGlobalProductsUse);
        }
        set
        {
            SetValue("ReflectGlobalProductsUse", value);
            mReflectGlobalProductsUse = value;
        }
    }


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
                    mUsingGlobalObjects = ECommerceSettings.UseGlobalPublicStatus(si.SiteName);
                }
            }

            return mUsingGlobalObjects.Value;
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
        TryInitByForm();

        uniSelector.EnabledColumnName = "PublicStatusEnabled";
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowAll = AddAllItemsRecord;
        uniSelector.AllowEmpty = AddNoneRecord;
        uniSelector.ReturnColumnName = (UseStatusNameForSelection ? "PublicStatusName" : "PublicStatusID");

        SetupWhereCondition();

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        SetupWhereCondition();

        if (URLHelper.IsPostback() && DependsOnAnotherField)
        {
            pnlUpdate.Update();
        }
    }

    #endregion


    #region "Methods"

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
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }


    /// <summary>
    /// Inits the selector.
    /// </summary>
    protected void SetupWhereCondition()
    {
        SetFormSiteID();

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

            where += "(ISNULL(PublicStatusSiteID, 0) = " + filteredSiteId + ") ";
        }

        // Append global items if requested
        if (DisplayGlobalItems && (!string.IsNullOrEmpty(where)))
        {
            where = SqlHelperClass.AddWhereCondition(where, "PublicStatusSiteID IS NULL", "OR");
        }

        // Filter out only enabled items
        if (DisplayOnlyEnabled)
        {
            where = SqlHelperClass.AddWhereCondition(where, "PublicStatusEnabled = 1");
        }

        // Add items which have to be on the list
        string additionalList = SqlHelperClass.GetSafeQueryString(AdditionalItems, false);
        if ((!string.IsNullOrEmpty(where)) && (!string.IsNullOrEmpty(additionalList)))
        {
            where = SqlHelperClass.AddWhereCondition(where, "PublicStatusID IN (" + additionalList + ")", "OR");
        }

        // Selected item must be on the list
        if (EnsureSelectedItem && (!string.IsNullOrEmpty(where)) && (PublicStatusID > 0))
        {
            where = SqlHelperClass.AddWhereCondition(where, "PublicStatusID = " + PublicStatusID, "OR");
        }

        // Set where condition
        uniSelector.WhereCondition = where;

        if (UseStatusNameForSelection)
        {
            uniSelector.AllRecordValue = "";
            uniSelector.NoneRecordValue = "";
        }
    }


    /// <summary>
    /// Sets the SiteID if the SiteName field is available in the form.
    /// </summary>
    private void SetFormSiteID()
    {
        if (DependsOnAnotherField && (Form != null) && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), null);
            if (!String.IsNullOrEmpty(siteName))
            {
                SiteInfo siteObj = SiteInfoProvider.GetSiteInfo(siteName);
                if (siteObj != null)
                {
                    SiteID = siteObj.SiteID;
                }
            }
            else
            {
                SiteID = -1;
            }
        }
    }

    #endregion
}