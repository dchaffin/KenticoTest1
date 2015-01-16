using System;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_AddressSettings : CloneSettingsControl
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
    /// Hide display name.
    /// </summary>
    public override bool HideDisplayName
    {
        get
        {
            return true;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            AddressInfo address = (AddressInfo)InfoToClone;
            txtAddress1.Text = address.AddressLine1;
            txtAddress2.Text = address.AddressLine2;
            txtPersonalName.Text = address.AddressPersonalName;
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ECommerceObjectType.ADDRESS + ".address1"] = txtAddress1.Text;
        result[ECommerceObjectType.ADDRESS + ".address2"] = txtAddress2.Text;
        result[ECommerceObjectType.ADDRESS + ".personalname"] = txtPersonalName.Text;
        return result;
    }

    #endregion
}