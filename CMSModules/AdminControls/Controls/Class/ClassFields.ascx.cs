using System;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSModules_AdminControls_Controls_Class_ClassFields : FormEngineUserControl
{
    #region "Variables"

    private FormFieldDataTypeEnum mFieldDataType = FormFieldDataTypeEnum.Unknown;
    private string mClassName;
    private string mDataTextField;
    private string mDataValueField;

    #endregion


    #region "Public properties"

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
            drpField.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpField.SelectedValue;
        }
        set
        {
            // Reload data
            ReloadData();
            if (drpField.Items.Count > 0)
            {
                // Try to select specified field
                ListItem li = drpField.Items.FindByValue(ValidationHelper.GetString(value, string.Empty));
                if (li != null)
                {
                    drpField.ClearSelection();
                    li.Selected = true;
                }
            }
        }
    }


    /// <summary>
    /// Gets the display name of the value item. Returns null if display name is not available.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            // Return the selected text
            if (drpField.SelectedIndex >= 0)
            {
                return drpField.Items[drpField.SelectedIndex].Text;
            }
            else
            {
                return drpField.Text;
            }
        }
    }


    /// <summary>
    /// Gets or sets name of the class which fields should be displayed.
    /// </summary>
    public string ClassName
    {
        get
        {
            if (string.IsNullOrEmpty(mClassName))
            {
                mClassName = ValidationHelper.GetString(GetValue("ClassName"), null);
            }
            return mClassName;
        }
        set
        {
            mClassName = value;
        }
    }


    /// <summary>
    /// Name of the field in the class data source used as a display name.
    /// </summary>
    public string DataTextField
    {
        get
        {
            if (String.IsNullOrEmpty(mDataTextField))
            {
                mDataTextField = ValidationHelper.GetString(GetValue("DataTextField"), null);
            }
            return mDataTextField;
        }
        set
        {
            mDataTextField = value;
        }
    }


    /// <summary>
    /// Name of the field in the class data source used as a value.
    /// </summary>
    public string DataValueField
    {
        get
        {
            if (String.IsNullOrEmpty(mDataValueField))
            {
                mDataValueField = ValidationHelper.GetString(GetValue("DataValueField"), null);
            }
            return mDataValueField;
        }
        set
        {
            mDataValueField = value;
        }
    }


    /// <summary>
    /// Gets or sets data type of fields that should be offered. Optional.
    /// </summary>
    public FormFieldDataTypeEnum FieldDataType
    {
        get
        {
            return mFieldDataType;
        }
        set
        {
            mFieldDataType = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the control.
    /// </summary>
    protected void ReloadData()
    {
        if (drpField.Items.Count == 0)
        {
            // Load dropdownlist with fields of specified class
            FormInfo fi = FormHelper.GetFormInfo(ClassName, false);
            if (fi != null)
            {
                if (CMSString.Equals(ClassName, "cms.user", true))
                {
                    // Combine user fields with those of user settings
                    FormInfo coupledInfo = FormHelper.GetFormInfo("cms.usersettings", false);
                    if (coupledInfo != null)
                    {
                        fi.CombineWithForm(coupledInfo, false, null, false);
                    }
                }

                if (FieldDataType == FormFieldDataTypeEnum.Unknown)
                {
                    // Get all form fields
                    drpField.DataSource = fi.GetFields(true, true);
                }
                else
                {
                    // Get form fields of specific data type
                    drpField.DataSource = fi.GetFields(FieldDataType);
                }

                drpField.DataTextField = DataTextField;
                drpField.DataValueField = DataValueField;
                drpField.DataBind();
            }
            // Add '(none)' item
            drpField.Items.Insert(0, new ListItem(GetString("general.selectnone"), string.Empty));
        }
    }

    #endregion
}