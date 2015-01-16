using System;
using System.Data;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_FormControls_SelectBundleItems : FormEngineUserControl
{
    #region "Backing fields"

    private int mSKUID = 0;
    private int mSiteID = -1;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the bundle ID.
    /// </summary>
    public int SKUID
    {
        get
        {
            if (mSKUID > 0)
            {
                return mSKUID;
            }

            // Try to get value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUID"))
            {
                return ValidationHelper.GetInteger(Form.Data.GetValue("SKUID"), 0);
            }

            return 0;
        }
        set
        {
            mSKUID = value;
        }
    }


    /// <summary>
    /// Gets or sets the site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID >= 0)
            {
                return mSiteID;
            }

            // Try to get value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUSiteID"))
            {
                int siteId = ValidationHelper.GetInteger(Form.Data.GetValue("SKUSiteID"), 0);
                if (siteId >= 0)
                {
                    return siteId;
                }
            }

            return CMSContext.CurrentSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets bundle items count value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            int count = 0;
            if (!string.IsNullOrEmpty(Items))
            {
                count = Items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
            return count;
        }
        set
        {
        }
    }


    /// <summary>
    /// Gets the selected bundle item IDs separated by semicolon.
    /// </summary>
    public string Items
    {
        get
        {
            return ValidationHelper.GetString(itemsUniSelector.Value, null);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        if (!RequestHelper.IsPostBack() && (SKUID > 0))
        {
            itemsUniSelector.Value = GetSavedItemIds();
        }

        itemsUniSelector.StopProcessing = StopProcessing;
        itemsUniSelector.WhereCondition = GetItemsWhereCondition();
        
        if (Form != null)
        {
            Form.OnUploadFile += (sender, args) =>
            {
                SaveSelectionChanges();
            };
        }
     }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Ensure correct current pager page and correct pager data
        itemsUniSelector.Reload(true);
    }

    #endregion


    #region "Initialization"

    private string GetItemsWhereCondition()
    {
        string where = null;

        // Exclude product options
        where = SqlHelperClass.AddWhereCondition(where, "SKUOptionCategoryID IS NULL");

        // Exclude bundle products
        where = SqlHelperClass.AddWhereCondition(where, String.Format("SKUProductType <> '{0}'", SKUInfoProvider.GetSKUProductTypeString(SKUProductTypeEnum.Bundle)));

        // Exclude donation products
        where = SqlHelperClass.AddWhereCondition(where, String.Format("SKUProductType <> '{0}'", SKUInfoProvider.GetSKUProductTypeString(SKUProductTypeEnum.Donation)));

        // Exclude edited product itself
        where = SqlHelperClass.AddWhereCondition(where, "SKUID <> " + SKUID);

        // If bundle is global
        if (SiteID == 0)
        {
            // Inlcude global products
            where = SqlHelperClass.AddWhereCondition(where, "SKUSiteID IS NULL");
        }
        else
        {
            // If global products are allowed on this site
            if (ECommerceSettings.AllowGlobalProducts(SiteInfoProvider.GetSiteName(SiteID)))
            {
                // Include global and site products
                where = SqlHelperClass.AddWhereCondition(where, String.Format("(SKUSiteID IS NULL) OR (SKUSiteID = {0})", SiteID));
            }
            else
            {
                // Include site products
                where = SqlHelperClass.AddWhereCondition(where, "SKUSiteID = " + SiteID);
            }
        }

        // Include only enabled products
        where = SqlHelperClass.AddWhereCondition(where, "SKUEnabled = 1");

        // Include only products from user's departments
        if (!ECommerceContext.IsUserAuthorizedForPermission("AccessAllDepartments"))
        {
            where = SqlHelperClass.AddWhereCondition(where, String.Format("(SKUDepartmentID IN (SELECT DepartmentID FROM COM_UserDepartment WHERE UserID = {0}) OR (SKUDepartmentID IS NULL))", CMSContext.CurrentUser.UserID));
        }

        // Include currently selected items
        if (!string.IsNullOrEmpty(Items))
        {
            // Perform validation of items IDs separated by ';'
            string[] idsAsString = Items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int[] idsAsInt = ValidationHelper.GetIntegers(idsAsString, 0);

            // Separates IDs with ',' for use in SQL query
            string idsQuery = TextHelper.Join(",", idsAsInt);

            where = SqlHelperClass.AddWhereCondition(where, String.Format("SKUID IN ({0})", idsQuery), "OR");
        }

        return where;
    }

    #endregion


    #region "Save"

    /// <summary>
    /// Saves the currently selected bundle items to the database.
    /// </summary>
    public void SaveSelectionChanges()
    {
        var savedItems = GetSavedItemIds();

        var addedItems = DiffItemIds(savedItems, Items);
        foreach (var id in addedItems)
        {
            BundleInfoProvider.AddSKUToBundle(SKUID, id);
        }

        var removedItems = DiffItemIds(Items, savedItems);
        foreach (var id in removedItems)
        {
            BundleInfoProvider.RemoveSKUFromBundle(SKUID, id);
        }
    }

    #endregion


    #region "Helper methods"

    private string GetSavedItemIds()
    {
        DataSet items = BundleInfoProvider.GetBundles("BundleID = " + SKUID, "SKUID");
        if (!DataHelper.DataSourceIsEmpty(items))
        {
            return TextHelper.Join(";", SystemDataHelper.GetStringValues(items.Tables[0], "SKUID"));
        }
        return null;
    }


    private int[] DiffItemIds(string ids1, string ids2)
    {
        string difference = DataHelper.GetNewItemsInList(ids1, ids2);
        return GetItemIdsArray(difference);
    }


    private int[] GetItemIdsArray(string ids)
    {
        if (string.IsNullOrEmpty(ids))
        {
            return new int[0];
        }
        return ValidationHelper.GetIntegers(ids.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries), 0);
    }

    #endregion
}
