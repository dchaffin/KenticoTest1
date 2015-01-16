using System;

using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_ValidationSettings : CMSUserControl
{
    #region "Variables"

    private string mAttributeType;
    private string mFieldType;
    private FieldEditorModeEnum mMode = FieldEditorModeEnum.General;

    #endregion


    #region "Properties"

    /// <summary>
    /// Type of current attribute.
    /// </summary>
    public string AttributeType
    {
        get
        {
            return !String.IsNullOrEmpty(mAttributeType) ? mAttributeType.ToLowerCSafe() : "";
        }
        set
        {
            mAttributeType = value;
        }
    }


    /// <summary>
    /// Type of current field.
    /// </summary>
    public string FieldType
    {
        get
        {
            return !String.IsNullOrEmpty(mFieldType) ? mFieldType.ToLowerCSafe() : "";
        }
        set
        {
            mFieldType = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if current item is primary.
    /// </summary>
    public bool IsPrimary
    {
        get;
        set;
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get
        {
            return mMode;
        }
        set
        {
            mMode = value;
        }
    }


    /// <summary>
    /// FormFieldInfo of given field.
    /// </summary>
    public FormFieldInfo FieldInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets value set in Minimum length textbox.
    /// </summary>
    public string MinLengthText
    {
        get
        {
            return txtMinLength.Text.Trim();
        }
    }


    /// <summary>
    /// Gets value set in Maximum length textbox.
    /// </summary>
    public string MaxLengthText
    {
        get
        {
            return txtMaxLength.Text.Trim();
        }
    }


    /// <summary>
    /// Gets value set in Minimum value textbox.
    /// </summary>
    public string MinValueText
    {
        get
        {
            return txtMinValue.Text.Trim();
        }
    }


    /// <summary>
    /// Gets value set in Maximum value textbox.
    /// </summary>
    public string MaxValueText
    {
        get
        {
            return txtMaxValue.Text.Trim();
        }
    }


    /// <summary>
    /// Gets value indicating if Specll-check is checked.
    /// </summary>
    public bool SpellCheck
    {
        get
        {
            return chkSpellCheck.Checked;
        }
    }


    /// <summary>
    /// Gets value which is contained in Regular expression textbox.
    /// </summary>
    public string RegularExpression
    {
        get
        {
            return txtRegExpr.Text.Trim();
        }
    }


    /// <summary>
    /// Gets value which indicates which value is selected in 'Date from' control.
    /// </summary>
    public DateTime DateFrom
    {
        get
        {
            return dateFrom.SelectedDateTime;
        }
    }


    /// <summary>
    /// Gets value which indicates which value is selected in 'Date to' control .
    /// </summary>
    public DateTime DateTo
    {
        get
        {
            return dateTo.SelectedDateTime;
        }
    }


    /// <summary>
    /// Gets value which is contained in Error message textbox.
    /// </summary>
    public string ErrorMessage
    {
        get
        {
            return txtErrorMessage.Text.Trim();
        }
    }

    #endregion


    #region "Methods"

    public void Page_Load(object sender, EventArgs e)
    {
        pnlSectionValidation.GroupingText = GetString("TemplateDesigner.Section.Validation");
    }


    /// <summary>
    /// Show validation options according to selected attribute type.
    /// </summary>
    public void Reload()
    {
        if (FieldInfo != null)
        {
            chkSpellCheck.Checked = FieldInfo.SpellCheck;
            if (pnlSectionValidation.Visible)
            {
                // Validation section
                txtRegExpr.Text = FieldInfo.RegularExpression;
                switch (FieldInfo.DataType)
                {
                    case FormFieldDataTypeEnum.Integer:
                        txtMinValue.Text = ValidationHelper.IsInteger(FieldInfo.MinValue) ? FieldInfo.MinValue : "";
                        txtMaxValue.Text = ValidationHelper.IsInteger(FieldInfo.MaxValue) ? FieldInfo.MaxValue : "";
                        break;

                    case FormFieldDataTypeEnum.LongInteger:
                        txtMinValue.Text = ValidationHelper.IsLong(FieldInfo.MinValue) ? FieldInfo.MinValue : "";
                        txtMaxValue.Text = ValidationHelper.IsLong(FieldInfo.MaxValue) ? FieldInfo.MaxValue : "";
                        break;

                    case FormFieldDataTypeEnum.Decimal:
                        txtMinValue.Text = ValidationHelper.IsDouble(FieldInfo.MinValue) ? FieldInfo.MinValue : "";
                        txtMaxValue.Text = ValidationHelper.IsDouble(FieldInfo.MaxValue) ? FieldInfo.MaxValue : "";
                        break;
                }

                txtMinLength.Text = (FieldInfo.MinStringLength > -1) ? Convert.ToString(FieldInfo.MinStringLength) : "";
                txtMaxLength.Text = (FieldInfo.MaxStringLength > -1) ? Convert.ToString(FieldInfo.MaxStringLength) : "";

                dateFrom.SelectedDateTime = FieldInfo.MinDateTimeValue;
                dateTo.SelectedDateTime = FieldInfo.MaxDateTimeValue;
                txtErrorMessage.Text = FieldInfo.ValidationErrorMessage;
            }
        }
        else
        {
            chkSpellCheck.Checked = true;
            txtRegExpr.Text = null;
            txtMinLength.Text = null;
            txtMaxLength.Text = null;
            txtMinValue.Text = null;
            txtMaxValue.Text = null;
            dateFrom.SelectedDateTime = DateTimePicker.NOT_SELECTED;
            dateTo.SelectedDateTime = DateTimePicker.NOT_SELECTED;
            txtErrorMessage.Text = null;
        }
    }


    /// <summary>
    /// Displays controls according to current field settings.
    /// </summary>
    public void DisplayControls()
    {
        Visible = true;
        plcMinMaxLengthValidation.Visible = true;

        if (!IsPrimary)
        {
            switch (AttributeType)
            {
                case "text":
                case "longtext":
                    pnlSectionValidation.Visible = true;
                    plcTextValidation.Visible = true;
                    plcErrorMessageValidation.Visible = true;
                    if (Mode == FieldEditorModeEnum.ClassFormDefinition)
                    {
                        plcSpellCheck.Visible = true;
                    }
                    plcDateTimeValidation.Visible = false;
                    plcNumberValidation.Visible = false;
                    Visible = true;
                    break;

                case "datetime":
                    pnlSectionValidation.Visible = true;
                    plcDateTimeValidation.Visible = true;
                    plcErrorMessageValidation.Visible = true;
                    plcTextValidation.Visible = false;
                    plcNumberValidation.Visible = false;
                    plcSpellCheck.Visible = false;
                    Visible = true;
                    break;

                case "integer":
                case "longinteger":
                case "double":
                    pnlSectionValidation.Visible = true;
                    plcNumberValidation.Visible = true;
                    plcErrorMessageValidation.Visible = true;
                    plcTextValidation.Visible = false;
                    plcDateTimeValidation.Visible = false;
                    plcSpellCheck.Visible = false;
                    Visible = true;
                    break;
                case "guid":
                    pnlSectionValidation.Visible = true;
                    plcNumberValidation.Visible = false;
                    plcErrorMessageValidation.Visible = true;
                    plcTextValidation.Visible = false;
                    plcDateTimeValidation.Visible = false;
                    plcSpellCheck.Visible = false;
                    Visible = true;
                    break;
                default:
                    Visible = false;
                    break;
            }
        }
        else
        {
            Visible = false;
        }

        string loweredFieldType = FieldType.ToLowerCSafe();
        // Hide Min max length for selection controls
        if ((loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.DropDownListControl).ToLowerCSafe()) ||
            (loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.RadioButtonsControl).ToLowerCSafe()) ||
            (loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.MultipleChoiceControl).ToLowerCSafe()) ||
            (loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.ListBoxControl).ToLowerCSafe()))
        {
            plcMinMaxLengthValidation.Visible = false;
        }
        // Hide the whole validation section
        else if ((loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.ImageSelectionControl).ToLowerCSafe()) ||
                 (loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.FileSelectionControl).ToLowerCSafe()) ||
                 (loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.MediaSelectionControl).ToLowerCSafe()) ||
                 (loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.SelectColumns).ToLowerCSafe()) ||
                 (loweredFieldType == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.EncryptedPassword).ToLowerCSafe()))
        {
            pnlSectionValidation.Visible = false;
        }

        if (!plcTextValidation.Visible && !plcErrorMessageValidation.Visible && !plcDateTimeValidation.Visible && !plcNumberValidation.Visible && !plcSpellCheck.Visible)
        {
            Visible = false;
        }
    }

    #endregion
}