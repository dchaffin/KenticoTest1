using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSModules_Ecommerce_FormControls_SelectTaxAddressType : FormEngineUserControl
{
    #region "Properties"

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
            rblAddresType.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return string.IsNullOrEmpty(rblAddresType.SelectedValue) ? "BillingAddress" : rblAddresType.SelectedValue;
        }
        set
        {
            EnsureChildControls();
            rblAddresType.SelectedValue = ValidationHelper.GetString(value, "BillingAddress");
        }
    }


    /// <summary>
    /// Returns ClientID of the RadioButtonList with tax options..
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return rblAddresType.ClientID;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if (!StopProcessing)
        {
            ReloadData();
        }
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Loads the child controls at run-time.
    /// </summary>
    private void ReloadData()
    {
        if ((rblAddresType.Items == null) || (rblAddresType.Items.Count <= 0))
        {
            rblAddresType.Items.Add(new ListItem(GetString("com.BillingAddress"), "BillingAddress"));
            rblAddresType.Items.Add(new ListItem(GetString("com.ShippingAddress"), "ShippingAddress"));
        }
        rblAddresType.SelectedValue = Value.ToString();
    }

    #endregion
}