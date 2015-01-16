using System;

using CMS.FormControls;
using CMS.Ecommerce;
using CMS.GlobalHelper;

public partial class CMSModules_Ecommerce_FormControls_SelectBundleInventoryType : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the bundle inventory type as string.
    /// </summary>
    public override object Value
    {
        get
        {
            return SKUInfoProvider.GetBundleInventoryTypeString(BundleInventoryType);
        }
        set
        {
            BundleInventoryType = SKUInfoProvider.GetBundleInventoryTypeEnum(ValidationHelper.GetString(value, null));
        }
    }


    /// <summary>
    /// Gets or sets the bundle inventory type.
    /// </summary>
    public BundleInventoryTypeEnum BundleInventoryType
    {
        get
        {
            if (radRemoveBundle.Checked)
            {
                return BundleInventoryTypeEnum.RemoveBundle;
            }
            else if (radRemoveProducts.Checked)
            {
                return BundleInventoryTypeEnum.RemoveProducts;
            }
            else if (radRemoveBundleAndProducts.Checked)
            {
                return BundleInventoryTypeEnum.RemoveBundleAndProducts;
            }
            else
            {
                return BundleInventoryTypeEnum.RemoveBundle;
            }
        }
        set
        {
            radRemoveBundle.Checked = (value == BundleInventoryTypeEnum.RemoveBundle);
            radRemoveProducts.Checked = (value == BundleInventoryTypeEnum.RemoveProducts);
            radRemoveBundleAndProducts.Checked = (value == BundleInventoryTypeEnum.RemoveBundleAndProducts);
        }
    }

    #endregion


    #region "Lifecycle methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
    }

    #endregion
}
