using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.Ecommerce;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_OptionCategorySettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Hide the control
    /// </summary>
    public override bool DisplayControl
    {
        get
        {
            return false;
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return ECommerceObjectType.SKUOPTIONCATEGORY;
        }
    }

    #endregion
}