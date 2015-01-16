using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSFormControls_Basic_CheckBoxControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return checkbox.Enabled;
        }
        set
        {
            checkbox.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return checkbox.Checked;
        }
        set
        {
            checkbox.Checked = ValidationHelper.GetBoolean(value, false);
        }
    }


    /// <summary>
    /// Gets or sets if control causes postback.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return checkbox.AutoPostBack;
        }
        set
        {
            checkbox.AutoPostBack = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Apply CSS styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            checkbox.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(checkbox.CssClass))
        {
            checkbox.CssClass = "CheckBoxField";
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            checkbox.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckFieldEmptiness = false;
    }

    #endregion
}