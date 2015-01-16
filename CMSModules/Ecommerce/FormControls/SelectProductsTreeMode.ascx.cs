using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.ExtendedControls;

public partial class CMSModules_Ecommerce_FormControls_SelectProductsTreeMode : FormEngineUserControl
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
            chkTreeMode.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return hdnField.Value;
        }
        set
        {
            EnsureChildControls();

            string val = ValidationHelper.GetString(value, "None");
            hdnField.Value = val;

            chkTreeMode.Checked = (val == "Sections");
        }
    }


    /// <summary>
    /// Returns ClientID of the hidden field.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return hdnField.ClientID;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
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


    #region "Methods"

    protected void chkTreeMode_CheckedChanged(object sender, EventArgs e)
    {
        // Update hidden field value
        hdnField.Value = chkTreeMode.Checked ? "Sections" : "None";
    }
    

    /// <summary>
    /// Loads the child controls at run-time.
    /// </summary>
    private void ReloadData()
    {
        chkTreeMode.Checked = (Value.ToString() == "Sections");
    }

    #endregion
}
