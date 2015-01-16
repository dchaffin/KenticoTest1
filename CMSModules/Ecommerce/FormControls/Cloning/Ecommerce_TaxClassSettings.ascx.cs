using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.Ecommerce;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_TaxClassSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return ECommerceObjectType.DEPARTMENTTAXCLASS + ";" + ECommerceObjectType.SHIPPINGOPTIONTAXCLASS + ";" + ECommerceObjectType.SKUTAXCLASS;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblSKUAssignments.ToolTip = GetString("clonning.settings.taxclass.skuassignments.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ECommerceObjectType.TAXCLASS + ".skuassignments"] = chkSKUAssignments.Checked;
        return result;
    }

    #endregion
}