using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSFormControls_Basic_LabelControl : ReadOnlyFormEngineUserControl
{
    #region "Variables"

    private object mValue = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return label.Enabled;
        }
        set
        {
            label.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the transformation code to use to transform the value.
    /// </summary>
    public string Transformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Transformation"), "");
        }
        set
        {
            SetValue("Transformation", value);
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return mValue;
        }
        set
        {
            mValue = ValidationHelper.GetString(value, "");
            object val = value;

            // Try to find the transformation
            if (!string.IsNullOrEmpty(Transformation) && UniGridTransformations.Global.ExecuteTransformation(label, Transformation, ref val))
            {
                label.Text = ValidationHelper.GetString(val, null);
            }
            else if ((FieldInfo != null) && (FieldInfo.DataType == FormFieldDataTypeEnum.Decimal))
            {
                mValue = ValidationHelper.GetDouble(value, 0, "en-us");
                label.Text = ValidationHelper.GetString(mValue, null);
            }
            else if ((FieldInfo != null) && (FieldInfo.DataType == FormFieldDataTypeEnum.DateTime))
            {
                mValue = ValidationHelper.GetDateTime(value, DateTimeHelper.ZERO_TIME, "en-us");
                label.Text = ValidationHelper.GetString(mValue, null);
            }
            else
            {
                label.Text = ValidationHelper.GetString(value, null);
            }
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Apply styles to control
        if (!String.IsNullOrEmpty(CssClass))
        {
            label.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(label.CssClass))
        {
            label.CssClass = "LabelField";
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            label.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckFieldEmptiness = false;
    }

    #endregion
}