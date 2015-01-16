using System;
using System.Data;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_FormControls_DepartmentSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseDepartmentNameForSelection = true;
    private bool mAddNoneRecord = false;
    private bool mAddAllItemsRecord = true;
    private bool mAddAllMyRecord = false;
    private int mUserId = 0;
    private bool? mAllowGlobalObjects = null;
    private bool? mDisplaySiteItems = null;
    private bool? mDisplayGlobalItems = null;
    private bool mReflectGlobalProductsUse = false;
    private int mSiteId = -1;
    private string mAdditionalItems = "";
    private bool mDropDownListMode = true;
    private bool mShowAllSites = false;
    private bool mAddWithoutDepartmentRecord = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the ID of the user the departments of which should be displayed. 0 means all departments are displayed.
    /// </summary>
    public int UserID
    {
        get
        {
            if (mUserId == 0)
            {
                return CMSContext.CurrentUser.UserID;
            }
            return mUserId;
        }
        set
        {
            mUserId = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            if (UseDepartmentNameForSelection)
            {
                return DepartmentName;
            }
            else
            {
                if (DepartmentID == 0)
                {
                    return null;
                }
                return DepartmentID;
            }
        }
        set
        {
            EnsureChildControls();
            if (UseDepartmentNameForSelection)
            {
                DepartmentName = ValidationHelper.GetString(value, "");
            }
            else
            {
                DepartmentID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the Department ID.
    /// </summary>
    public int DepartmentID
    {
        get
        {
            EnsureChildControls();
            if (UseDepartmentNameForSelection)
            {
                string name = ValidationHelper.GetString(uniSelector.Value, "");
                DepartmentInfo tgi = DepartmentInfoProvider.GetDepartmentInfo(name, SiteInfoProvider.GetSiteName(SiteID));
                if (tgi != null)
                {
                    return tgi.DepartmentID;
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
            if (UseDepartmentNameForSelection)
            {
                DepartmentInfo tgi = DepartmentInfoProvider.GetDepartmentInfo(value);
                if (tgi != null)
                {
                    uniSelector.Value = tgi.DepartmentName;
                }
            }
            else
            {
                uniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the Department code name.
    /// </summary>
    public string DepartmentName
    {
        get
        {
            EnsureChildControls();
            if (UseDepartmentNameForSelection)
            {
                return ValidationHelper.GetString(uniSelector.Value, "");
            }
            else
            {
                int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
                DepartmentInfo di = DepartmentInfoProvider.GetDepartmentInfo(id);
                if (di != null)
                {
                    return di.DepartmentName;
                }
                return "";
            }
        }
        set
        {
            EnsureChildControls();
            if (UseDepartmentNameForSelection)
            {
                uniSelector.Value = value;
            }
            else
            {
                DepartmentInfo di = null;
                if (ShowAllSites)
                {
                    DataSet ds = DepartmentInfoProvider.GetDepartments("DepartmentName = '" + value.Replace("'", "''") + "'", "DepartmentSiteID", 1, null);
                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        di = new DepartmentInfo(ds.Tables[0].Rows[0]);
                    }
                }
                else
                {
                    di = DepartmentInfoProvider.GetDepartmentInfo(value, SiteInfoProvider.GetSiteName(SiteID));
                }

                if (di != null)
                {
                    uniSelector.Value = di.DepartmentID;
                }
            }
        }
    }


    /// <summary>
    ///  If true, selected value is DepartmentName, if false, selected value is DepartmentID.
    /// </summary>
    public bool UseDepartmentNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseDepartmentNameForSelection"), mUseDepartmentNameForSelection);
        }
        set
        {
            SetValue("UseDepartmentNameForSelection", value);
            mUseDepartmentNameForSelection = value;
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
    /// Returns ClientID of the dropdownlist.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            EnsureChildControls();
            if (DropDownListMode)
            {
                return uniSelector.DropDownSingleSelect.ClientID;
            }
            else
            {
                return uniSelector.TextBoxSelect.ClientID;
            }
        }
    }


    /// <summary>
    /// Allows to display departments for specified site ID. Use 0 for global departments. Default value is current site id.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId > -1)
            {
                return mSiteId;
            }

            // No site ID given
            return CMSContext.CurrentSiteID;
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
    /// Returns true if site given by SiteID uses global departments.
    /// </summary>
    protected bool AllowGlobalObjects
    {
        get
        {
            if (ShowAllSites)
            {
                return true;
            }

            // Unknown yet
            if (!mAllowGlobalObjects.HasValue)
            {
                mAllowGlobalObjects = false;
                // Try to figure out from settings
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                if (si != null)
                {
                    mAllowGlobalObjects = ECommerceSettings.AllowGlobalDepartments(si.SiteName);
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
            if (!mDisplayGlobalItems.HasValue)
            {
                mDisplayGlobalItems = false;
                if ((SiteID == 0) || AllowGlobalObjects || (ReflectGlobalProductsUse && ECommerceSettings.AllowGlobalProducts(CMSContext.CurrentSiteName)))
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
    /// Indicates if drop down list mode is used. Default value is true.
    /// </summary>
    public bool DropDownListMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DropDownListMode"), mDropDownListMode);
        }
        set
        {
            SetValue("DropDownListMode", value);
            mDropDownListMode = value;
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
    /// Gets or sets the value which determines, whether to add all my departments item record to the dropdownlist.
    /// </summary>
    public bool AddAllMyRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddAllMyRecord"), mAddAllMyRecord);
        }
        set
        {
            SetValue("AddAllMyRecord", value);
            mAddAllMyRecord = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add 'without deparmtnet' item record to the dropdownlist.
    /// </summary>
    public bool AddWithoutDepartmentRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddWithoutDepartmentRecord"), mAddWithoutDepartmentRecord);
        }
        set
        {
            SetValue("AddWithoutDepartmentRecord", value);
            mAddWithoutDepartmentRecord = value;
        }
    }


    /// <summary>
    /// Indicates if departments for all sites will be listed.
    /// </summary>
    public bool ShowAllSites
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAllSites"), mShowAllSites);
        }
        set
        {
            SetValue("ShowAllSites", value);
            mShowAllSites = value;
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
    /// Gets or sets the value that indicates if the currently selected item will be available for selection, even if it does not meet the where condition criteria.
    /// </summary>
    public bool EnsureSelectedItem
    {
        get;
        set;
    }

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        uniSelector.SelectionMode = (DropDownListMode ? SelectionModeEnum.SingleDropDownList : SelectionModeEnum.SingleTextBox);

        base.OnInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        TryInitByForm();

        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowEmpty = AddNoneRecord;
        uniSelector.AllowAll = AddAllItemsRecord;
        uniSelector.ReturnColumnName = (UseDepartmentNameForSelection ? "DepartmentName" : "DepartmentID");

        if (ShowAllSites)
        {
            uniSelector.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
            uniSelector.SetValue("FilterMode", "department");
        }

        // Add special records
        if (AddAllMyRecord && AddWithoutDepartmentRecord)
        {
            uniSelector.SpecialFields = new string[,] { { GetString("product_list.allmydepartments"), "" }, { GetString("general.empty"), "-5" } };
        }
        else
        {
            if (AddAllMyRecord)
            {
                uniSelector.SpecialFields = new string[,] { { GetString("product_list.allmydepartments"), "" } };
            }

            if (AddWithoutDepartmentRecord)
            {
                uniSelector.SpecialFields = new string[,] { { GetString("general.empty"), "-5" } };
            }
        }

        SetupWhereCondition();

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (RequestHelper.IsPostBack() && DependsOnAnotherField)
        {
            SetupWhereCondition();
            pnlUpdate.Update();
        }

        uniSelector.Reload(true);

        base.OnPreRender(e);
    }

    #endregion


    #region "Initialization"

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

        // Select all
        string where = "(1=1)";

        if (!ShowAllSites)
        {
            // Get only departments of the given user if he is not authorized for all departments
            if ((UserID > 0) && !UserInfoProvider.IsAuthorizedPerResource("CMS.Ecommerce", "AccessAllDepartments", CMSContext.CurrentSiteName, UserInfoProvider.GetUserInfo(UserID)))
            {
                where = "DepartmentID IN (SELECT DepartmentID FROM COM_UserDepartment WHERE UserID = " + UserID + ")";
            }

            where += " AND ((1=0)";
            // Add global items
            if (DisplayGlobalItems)
            {
                where += " OR (DepartmentSiteID IS NULL) ";
            }
            // Add site specific items
            if (DisplaySiteItems)
            {
                where += " OR (DepartmentSiteID = " + SiteID + ")";
            }
            where += ")";

            where = "(" + where + ")";
            // Add items which have to be on the list (if any)
            string additionalList = SqlHelperClass.GetSafeQueryString(AdditionalItems, false);
            if ((!string.IsNullOrEmpty(where)) && (!string.IsNullOrEmpty(additionalList)))
            {
                where += " OR (DepartmentID IN (" + additionalList + "))";
            }

            // Selected item (if any) must be on the list
            if (EnsureSelectedItem && (!string.IsNullOrEmpty(where)) && (DepartmentID > 0))
            {
                where += " OR (DepartmentID = " + DepartmentID + ")";
            }
        }

        uniSelector.WhereCondition = where;

        if (UseDepartmentNameForSelection)
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
        if (DependsOnAnotherField
            && (Form != null)
            && Form.IsFieldAvailable("SiteName"))
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

        if (Form.AdditionalData.ContainsKey("DataClassID"))
        {
            var dataClassId = ValidationHelper.GetInteger(Form.AdditionalData["DataClassID"], 0);
            var dataClass = DataClassInfoProvider.GetDataClass(dataClassId);
            if (dataClass != null)
            {
                if (UseDepartmentNameForSelection)
                {
                    Value = dataClass.ClassSKUDefaultDepartmentName;
                }
                else
                {
                    DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo(dataClass.ClassSKUDefaultDepartmentName, SiteInfoProvider.GetSiteName(SiteID));
                    if (department != null)
                    {
                        Value = department.DepartmentID;
                    }
                }
            }
        }
    }

    #endregion
}